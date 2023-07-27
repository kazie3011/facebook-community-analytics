using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FluentDateTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule.Components
{
    public partial class HomePageCampaigns
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        private IReadOnlyList<CampaignWithNavigationPropertiesDto> CampaignList { get; set; }
        private int PageSize { get; set; } = 10;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = "Campaign.StartDateTime DESC";
        private int TotalCount { get; set; }
        private Guid? CurrentPartnerId { get; set; }
        private GetCampaignsInput Filter { get; set; }
        private DataGridEntityActionsColumn<CampaignWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();

        public HomePageCampaigns()
        {
            Filter = new GetCampaignsInput
            {
                Status = CampaignStatus.Started,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await GetNullablePartnerLookupAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                BrowserDateTime = await BrowserDateTimeProvider.GetInstance();
                await InitPage($"GDL - {L["Campaigns.PageTitle"].Value}");
                await DoSearch();
            }
        }

        private async Task DoSearch()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            Filter.PartnerId = CurrentPartnerId == Guid.Empty ? null : CurrentPartnerId;
            
            var result = await _partnerModuleAppService.GetCampaignNavs(Filter);
            CampaignList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CampaignWithNavigationPropertiesDto> e)
        {
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }


        private async Task OnPartnerSelectChanged(Guid? value)
        {
            CurrentPartnerId = value;
            await DoSearch();
        }
        private async Task GetNullablePartnerLookupAsync()
        {
            PartnersNullable = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto());
        }

    }
}