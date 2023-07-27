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
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using Microsoft.JSInterop;
using OfficeOpenXml;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class TikTokMCNs
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<TikTokMCNDto> TikTokMcnDtos { get; set; }
        private int PageSize { get; set; } = 100;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateAccount { get; set; }
        private bool CanEditAccount { get; set; }
        private bool CanDeleteAccount { get; set; }
        private CreateUpdateTikTokMCNDto CreateTikTokMcnDto { get; set; }
        private Validations NewValidations { get; set; }
        private CreateUpdateTikTokMCNDto UpdateTikTokMcnDto { get; set; }
        private Validations UpdateValidations { get; set; }
        private Guid EditingAccountId { get; set; }
        private Modal CreateTikTokMCNModal { get; set; }
        private Modal UpdateTikTokMCNModal { get; set; }
        private GetTikTokMCNsInput Filter { get; set; }
        private DataGridEntityActionsColumn<TikTokMCNDto> EntityActionsColumn { get; set; }


        private Modal ImportAccountModal { get; set; }
        private List<AccountImportDto> _accImportDtos { get; set; }

        private AccountTypeFilter _typeFilterInput { get; set; } = AccountTypeFilter.NoSelect;
        private AccountStatusFilter _statusFilterInput { get; set; } = AccountStatusFilter.NoSelect;
        private AccountCountryFilter _countryFilterInput { get; set; } = AccountCountryFilter.NoSelect;

        public TikTokMCNs()
        {
            CreateTikTokMcnDto = new CreateUpdateTikTokMCNDto();
            UpdateTikTokMcnDto = new CreateUpdateTikTokMCNDto();
            Filter = new GetTikTokMCNsInput()
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
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
                L["TikTokMCN.AddNew"],
                () =>
                {
                    OpenCreateAccountModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
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

        private async Task GetMCNsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            var result = await TikTokMcnAppService.GetListAsync(Filter);
            TikTokMcnDtos = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMCNsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TikTokMCNDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;

            await GetMCNsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateAccountModal()
        {
            CreateTikTokMcnDto = new CreateUpdateTikTokMCNDto();
            NewValidations.ClearAll();
            CreateTikTokMCNModal.Show();
        }

        private void CloseCreateModal()
        {
            CreateTikTokMCNModal.Hide();
        }

        private void OpenEditModal(TikTokMCNDto input)
        {
            EditingAccountId = input.Id;
            UpdateTikTokMcnDto = ObjectMapper.Map<TikTokMCNDto, CreateUpdateTikTokMCNDto>(input);
            UpdateValidations.ClearAll();
            UpdateTikTokMCNModal.Show();
        }

        private async Task DeleteAsync(TikTokMCNDto input)
        {
            var confirmResult = await UiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                await TikTokMcnAppService.DeleteAsync(input.Id);
                await GetMCNsAsync();
            }
        }

        private async Task CreateAsync()
        {
            if (!NewValidations.ValidateAll()) return;
            await TikTokMcnAppService.CreateAsync(CreateTikTokMcnDto);
            await GetMCNsAsync();
            CreateTikTokMCNModal.Hide();
        }

        private void CloseEditModal()
        {
            UpdateTikTokMCNModal.Hide();
        }

        private async Task UpdateAsync()
        {
            if (!UpdateValidations.ValidateAll()) return;
            await TikTokMcnAppService.UpdateAsync(EditingAccountId, UpdateTikTokMcnDto);
            await GetMCNsAsync();
            UpdateTikTokMCNModal.Hide();
        }

    }
}