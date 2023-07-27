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
using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class AffiliateStats
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<AffiliateStatDto> AffiliateStatList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateAffiliateStat { get; set; }
        private bool CanEditAffiliateStat { get; set; }
        private bool CanDeleteAffiliateStat { get; set; }
        private AffiliateStatCreateDto NewAffiliateStat { get; set; }
        private Validations NewAffiliateStatValidations { get; set; }
        private AffiliateStatUpdateDto EditingAffiliateStat { get; set; }
        private Validations EditingAffiliateStatValidations { get; set; }
        private Guid EditingAffiliateStatId { get; set; }
        private Modal CreateAffiliateStatModal { get; set; }
        private Modal EditAffiliateStatModal { get; set; }
        private GetAffiliateStatsInput Filter { get; set; }
        private DataGridEntityActionsColumn<AffiliateStatDto> EntityActionsColumn { get; set; }

        public AffiliateStats()
        {
            NewAffiliateStat = new AffiliateStatCreateDto();
            EditingAffiliateStat = new AffiliateStatUpdateDto();
            Filter = new GetAffiliateStatsInput
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
            await InitPage($"GDL - {L["AffiliateStats.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:AffiliateStats"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewAffiliateStat"],
                () =>
                {
                    OpenCreateAffiliateStatModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.AffiliateStats.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateAffiliateStat = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.AffiliateStats.Create);
            CanEditAffiliateStat = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.AffiliateStats.Edit);
            CanDeleteAffiliateStat = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.AffiliateStats.Delete);
        }

        private async Task GetAffiliateStatsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await AffiliateStatsAppService.GetListAsync(Filter);
            AffiliateStatList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetAffiliateStatsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AffiliateStatDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetAffiliateStatsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateAffiliateStatModal()
        {
            NewAffiliateStat = new AffiliateStatCreateDto
            {
                CreatedAt = DateTime.Now,
            };
            NewAffiliateStatValidations.ClearAll();
            CreateAffiliateStatModal.Show();
        }

        private void CloseCreateAffiliateStatModal()
        {
            CreateAffiliateStatModal.Hide();
        }

        private void OpenEditAffiliateStatModal(AffiliateStatDto input)
        {
            EditingAffiliateStatId = input.Id;
            EditingAffiliateStat = ObjectMapper.Map<AffiliateStatDto, AffiliateStatUpdateDto>(input);
            EditingAffiliateStatValidations.ClearAll();
            EditAffiliateStatModal.Show();
        }

        private async Task DeleteAffiliateStatAsync(AffiliateStatDto input)
        {
            await AffiliateStatsAppService.DeleteAsync(input.Id);
            await GetAffiliateStatsAsync();
        }

        private async Task CreateAffiliateStatAsync()
        {
            await AffiliateStatsAppService.CreateAsync(NewAffiliateStat);
            await GetAffiliateStatsAsync();
            CreateAffiliateStatModal.Hide();
        }

        private void CloseEditAffiliateStatModal()
        {
            EditAffiliateStatModal.Hide();
        }

        private async Task UpdateAffiliateStatAsync()
        {
            await AffiliateStatsAppService.UpdateAsync(EditingAffiliateStatId, EditingAffiliateStat);
            await GetAffiliateStatsAsync();
            EditAffiliateStatModal.Hide();
        }
    }
}