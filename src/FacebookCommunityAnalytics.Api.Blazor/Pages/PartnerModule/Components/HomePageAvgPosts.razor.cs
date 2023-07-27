using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Statistics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule.Components
{
    public partial class HomePageAvgPosts
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }
        
        private bool _showLoading = true;

        private bool _unRendered = true;
        private GrowthCampaignChartDto _statistics = new();
        
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }
        private Guid? CurrentPartnerId { get; set; }

        private IEnumerable<Guid?> multipleCampaignIds { get; set; } = new List<Guid?>();

        private IReadOnlyList<LookupDto<Guid?>> PartnersLookups = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> CampaignLookups = new List<LookupDto<Guid?>>();
        private async Task ReloadDataAsync()
        {
            if (StartDate != null && EndDate != null)
            {
                var filter = new GetGrowthCampaignChartsInput();
                (filter.FromDateTime, filter.ToDateTime) = GetDateTimeForApi(StartDate, EndDate);

                if (multipleCampaignIds.IsNotNullOrEmpty())
                {
                    filter.CampaignIds = multipleCampaignIds.Where(x=>x.HasValue).Select(x=>x.Value).ToList();
                }
                else
                {
                    filter.CampaignIds.Clear();
                }

                if (CurrentPartnerId.HasValue && CurrentPartnerId.Value != Guid.Empty)
                {
                    filter.PartnerId = CurrentPartnerId;
                }

                if (filter.PartnerId.HasValue)
                {
                    _statistics = await _partnerModuleAppService.GetGrowthCampaignChart(filter);
                }
                else
                {
                    _statistics = new GrowthCampaignChartDto();
                }
            }

            _showLoading = false;
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetPartnerLookupAsync();
                //await GetCampaignLookupAsync();
                await InitDateTimeRangePicker();
                await ReloadDataAsync();
                
                await InvokeAsync(StateHasChanged);
                
                
                await JSRuntime.InvokeVoidAsync("HiddenMenuOnMobile");
            }
            _unRendered = false;
        }
        private async Task InitDateTimeRangePicker()
        {
            (StartDate, EndDate) = await GetDefaultMonthDate();
            _dateRanges = await GetDateRangePicker();
        }
        
        private async Task OnSelectedPartnerValueChanged(Guid? value)
        {
            CurrentPartnerId = value;
            multipleCampaignIds = new List<Guid?>();
            await GetCampaignLookupAsync();
            await ReloadDataAsync();
        }
        
        private async Task OnSelectedCampaignValueChanged(object value)
        {
            await ReloadDataAsync();
        }

        private async Task GetPartnerLookupAsync()
        {
            PartnersLookups = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto());

            if (PartnersLookups.IsNotNullOrEmpty())
            {
                CurrentPartnerId = PartnersLookups.FirstOrDefault()?.Id;
            }
        }

        private async Task GetCampaignLookupAsync()
        {
            CampaignLookups = await _partnerModuleAppService.GetCampaignLookup(new GetCampaignLookupDto { PartnerId = CurrentPartnerId });
            if (CampaignLookups.IsNotNullOrEmpty())
            {
                multipleCampaignIds = CampaignLookups.Select(x=>x.Id).ToList();
            }
        }
    }
}