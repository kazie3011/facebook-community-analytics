using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class AccountProxies
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<AccountProxyWithNavigationPropertiesDto> AccountProxyList { get; set; }
        private int PageSize { get; set; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateAccountProxy { get; set; }
        private bool CanEditAccountProxy { get; set; }
        private bool CanDeleteAccountProxy { get; set; }
        private AccountProxyCreateDto NewAccountProxy { get; set; }
        private Validations NewAccountProxyValidations { get; set; }
        private AccountProxyUpdateDto EditingAccountProxy { get; set; }
        private Validations EditingAccountProxyValidations { get; set; }
        private Guid EditingAccountProxyId { get; set; }
        private Modal CreateAccountProxyModal { get; set; }
        private Modal EditAccountProxyModal { get; set; }
        private GetAccountProxiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<AccountProxyWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> AccountsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> ProxiesNullable { get; set; } = new List<LookupDto<Guid?>>();

        public AccountProxies()
        {
            NewAccountProxy = new AccountProxyCreateDto();
            EditingAccountProxy = new AccountProxyUpdateDto();
            Filter = new GetAccountProxiesInput
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
            await Init();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["AccountProxies.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:AccountProxies"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewAccountProxy"],
                () =>
                {
                    OpenCreateAccountProxyModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.AccountProxies.Create
            );

            SetToolbarItemsExtendAsync();
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateAccountProxy = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.AccountProxies.Create);
            CanEditAccountProxy = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.AccountProxies.Edit);
            CanDeleteAccountProxy = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.AccountProxies.Delete);
        }

        private async Task GetAccountProxiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await AccountProxiesAppService.GetListAsync(Filter);
            AccountProxyList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetAccountProxiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AccountProxyWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await GetAccountProxiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateAccountProxyModal()
        {
            NewAccountProxy = new AccountProxyCreateDto { };
            NewAccountProxyValidations.ClearAll();
            CreateAccountProxyModal.Show();
        }

        private void CloseCreateAccountProxyModal()
        {
            CreateAccountProxyModal.Hide();
        }

        private void OpenEditAccountProxyModal(AccountProxyWithNavigationPropertiesDto input)
        {
            EditingAccountProxyId = input.AccountProxy.Id;
            EditingAccountProxy = ObjectMapper.Map<AccountProxyDto, AccountProxyUpdateDto>(input.AccountProxy);
            EditingAccountProxyValidations.ClearAll();
            EditAccountProxyModal.Show();
        }

        private async Task DeleteAccountProxyAsync(AccountProxyWithNavigationPropertiesDto input)
        {
            await AccountProxiesAppService.DeleteAsync(input.AccountProxy.Id);
            await GetAccountProxiesAsync();
        }

        private async Task CreateAccountProxyAsync()
        {
            if (!NewAccountProxyValidations.ValidateAll()) return;

            await AccountProxiesAppService.CreateAsync(NewAccountProxy);
            await GetAccountProxiesAsync();
            CreateAccountProxyModal.Hide();
        }

        private void CloseEditAccountProxyModal()
        {
            EditAccountProxyModal.Hide();
        }

        private async Task UpdateAccountProxyAsync()
        {
            if (!EditingAccountProxyValidations.ValidateAll()) return;

            await AccountProxiesAppService.UpdateAsync(EditingAccountProxyId, EditingAccountProxy);
            await GetAccountProxiesAsync();
            EditAccountProxyModal.Hide();
        }

        private async Task GetNullableAccountLookupAsync(string newValue)
        {
            AccountsNullable = (await AccountProxiesAppService.GetAccountLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }


        private async Task GetNullableProxyLookupAsync(string newValue)
        {
            ProxiesNullable = (await AccountProxiesAppService.GetProxyLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }
    }
}