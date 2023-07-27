using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.JSInterop;
using OfficeOpenXml;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Accounts
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<AccountDto> AccountList { get; set; }
        private int PageSize { get; set; } = 100;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateAccount { get; set; }
        private bool CanEditAccount { get; set; }
        private bool CanDeleteAccount { get; set; }
        private AccountCreateDto NewAccount { get; set; }
        private Validations NewAccountValidations { get; set; }
        private AccountUpdateDto EditingAccount { get; set; }
        private Validations EditingAccountValidations { get; set; }
        private Guid EditingAccountId { get; set; }
        private Modal CreateAccountModal { get; set; }
        private Modal EditAccountModal { get; set; }
        private GetAccountsInput Filter { get; set; }
        private DataGridEntityActionsColumn<AccountDto> EntityActionsColumn { get; set; }


        private Modal ImportAccountModal { get; set; }
        private List<AccountImportDto> _accImportDtos { get; set; }

        private AccountTypeFilter _typeFilterInput { get; set; } = AccountTypeFilter.NoSelect;
        private AccountStatusFilter _statusFilterInput { get; set; } = AccountStatusFilter.NoSelect;
        private AccountCountryFilter _countryFilterInput { get; set; } = AccountCountryFilter.NoSelect;

        public Accounts()
        {
            NewAccount = new AccountCreateDto();
            EditingAccount = new AccountUpdateDto();
            Filter = new GetAccountsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["Accounts.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Accounts"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewAccount"],
                () =>
                {
                    OpenCreateAccountModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Accounts.Create
            );

            Toolbar.AddButton
            (
                L["ImportAccount"],
                () =>
                {
                    OpenImportAccountModal();
                    return Task.CompletedTask;
                },
                IconName.FileUpload,
                requiredPolicyName: ApiPermissions.Accounts.Create
            );

            Toolbar.AddButton
            (
                L["ExportAccount"],
                async () =>
                {
                    await ExportPostsAsync();
                },
                IconName.Download,
                requiredPolicyName: ApiPermissions.Accounts.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateAccount = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Accounts.Create);
            CanEditAccount = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Accounts.Edit);
            CanDeleteAccount = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Accounts.Delete);
        }

        private async Task GetAccountsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            OnAccountTypeFilterSelectedValueChanged(_typeFilterInput);
            OnAccountStatusFilterSelectedValueChanged(_statusFilterInput);
            OnCountrFilter_Changed(_countryFilterInput);

            var result = await AccountsAppService.GetListAsync(Filter);
            AccountList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetAccountsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AccountDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;

            await GetAccountsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateAccountModal()
        {
            NewAccount = new AccountCreateDto { };
            NewAccountValidations.ClearAll();
            CreateAccountModal.Show();
        }

        private void CloseCreateAccountModal()
        {
            CreateAccountModal.Hide();
        }

        private void OpenEditAccountModal(AccountDto input)
        {
            EditingAccountId = input.Id;
            EditingAccount = ObjectMapper.Map<AccountDto, AccountUpdateDto>(input);
            EditingAccountValidations.ClearAll();
            EditAccountModal.Show();
        }

        private async Task DeleteAccountAsync(AccountDto input)
        {
            await AccountsAppService.DeleteAsync(input.Id);
            await GetAccountsAsync();
        }

        private async Task CreateAccountAsync()
        {
            if (!NewAccountValidations.ValidateAll()) return;
            await AccountsAppService.CreateAsync(NewAccount);
            await GetAccountsAsync();
            CreateAccountModal.Hide();
        }

        private void CloseEditAccountModal()
        {
            EditAccountModal.Hide();
        }

        private async Task UpdateAccountAsync()
        {
            if (!EditingAccountValidations.ValidateAll()) return;
            await AccountsAppService.UpdateAsync(EditingAccountId, EditingAccount);
            await GetAccountsAsync();
            EditAccountModal.Hide();
        }


        private void OpenImportAccountModal()
        {
            ImportAccountModal.Show();
        }

        private void CloseImportAccountModal()
        {
            ImportAccountModal.Hide();
        }

        private async Task OnFileImportAccountsChanged(FileChangedEventArgs e)
        {
            _accImportDtos = new List<AccountImportDto>();
            foreach (var file in e.Files)
            {
                await using var stream = new MemoryStream();

                await file.WriteToStreamAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var excel = new ExcelPackage(stream);
                var workSheet = excel.Workbook.Worksheets.FirstOrDefault();

                var postImportItems = workSheet.ConvertSheetToObjects<AccountImportDto>().ToList();

                if (postImportItems.Any())
                {
                    _accImportDtos.AddRange(postImportItems);
                }
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task ImportAccountsAsync()
        {
            var accounts = _accImportDtos.Where(x => x.Username.IsNotNullOrWhiteSpace()).ToList();
            await AccountExtendAppService.ImportAccountsAsync(new AccountImportInput {Items = accounts});
            await GetAccountsAsync();
            ImportAccountModal.Hide();
            _accImportDtos = new List<AccountImportDto>();
        }

        private void OnAccountTypeFilterSelectedValueChanged(AccountTypeFilter value)
        {
            if (value == AccountTypeFilter.NoSelect)
            {
                Filter.AccountType = null;
            }
            else
            {
                Filter.AccountType = Enum.Parse<AccountType>(value.ToString());
            }
        }

        private void OnAccountStatusFilterSelectedValueChanged(AccountStatusFilter value)
        {
            if (value == AccountStatusFilter.NoSelect)
            {
                Filter.AccountStatus = null;
            }
            else
            {
                Filter.AccountStatus = Enum.Parse<AccountStatus>(value.ToString());
            }
        }
        
        private void OnCountrFilter_Changed(AccountCountryFilter value)
        {
            if (value == AccountCountryFilter.NoSelect)
            {
                Filter.AccountCountry = null;
            }
            else
            {
                Filter.AccountCountry = Enum.Parse<AccountCountry>(value.ToString());
            }
        }

        private async Task ExportPostsAsync()
        {
            OnAccountTypeFilterSelectedValueChanged(_typeFilterInput);
            OnAccountStatusFilterSelectedValueChanged(_statusFilterInput);
            OnCountrFilter_Changed(_countryFilterInput);

            var filter = Filter.Clone();

            var exportAccounts = await AccountsAppService.GetExportAccounts(filter);

            var fileName = $"Account_{DateTime.UtcNow.Date:yyyy-MM-dd}.xlsx";
            var excelBytes = ExportHelper.GenerateAccountExcelBytes(L, exportAccounts, fileName);

            await JSRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
        }
    }
}