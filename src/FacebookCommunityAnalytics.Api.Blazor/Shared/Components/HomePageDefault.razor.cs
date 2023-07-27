using Blazorise.Charts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Statistics;
using Volo.Abp.Identity;
using ChartJsColor = System.Drawing.Color;


namespace FacebookCommunityAnalytics.Api.Blazor.Shared.Components
{
    public partial class HomePageDefault
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }
        private IList<GroupDto> _groupDtos = new List<GroupDto>();
        private IList<OrganizationUnitDto> _organizationUnitDtos = new List<OrganizationUnitDto>();
        private IReadOnlyList<LookupDto<Guid?>> GroupsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private Modal ChoosingGroupModal { get; set; }
        private Modal ExportPostModal { get; set; }
        private bool _unRendered = true;
        //private readonly List<Action> _actionsToRunOnceGroupModal = new();

        private GeneralStatsResponse _statistics { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }

        private IList<TiktokFollowerGrowth> TiktokFollowerGrowths { get; set; }
        private GeneralStatsApiRequest Filter { get; set; }
        private AffDailySummaryApiResponse ApiResponse { get; set; }
        private AffiliateSummaryType _affiliateSummaryType;
        private Dictionary<string, DateRange> _dateRanges { get; set; }

        private BarConfig _configGrowth = new();
        private BarConfig _configAvgPostChart = new();
        private BarConfig _configAvgCampaignPostChart = new();
        private BarConfig _configAffiliateChart = new();
        private BarConfig _configTiktokGrowth = new();

        private Chart _chartGrowth = new();
        private Chart _tiktokGrowth = new();
        private Chart _chart_AvgPostStats = new();
        private Chart _chart_AvgCampaignPostStats = new();
        private Chart _chart_AffiliateSummary = new();
        private LineChart<int> _chart_totalPostsByDate = new();

        private int _postRequiredQuantity;
        private int _affiliateRequiredQuantity;
        private bool _showLoading;

        private string DashBoardHeader { get; set; }

        public HomePageDefault()
        {
            _statistics = new GeneralStatsResponse();
            Filter = new GeneralStatsApiRequest();

            _postRequiredQuantity = 300;
            _affiliateRequiredQuantity = 300;
            _showLoading = true;
            _affiliateSummaryType = AffiliateSummaryType.Commission;
            TiktokFollowerGrowths = new List<TiktokFollowerGrowth>();
        }

        protected override async Task OnInitializedAsync()
        {
            if (CurrentUser is { IsAuthenticated: true } && IsManagerRole())
            {
                DashBoardHeader = L["Home.Dashboard.Title"];

                InitChartConfigs();

                _organizationUnitDtos = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest());
                _organizationUnitDtos = _organizationUnitDtos.OrderBy(_ => _.DisplayName).ToList();
                _groupDtos = await GroupExtendAppService.GetListAsync();
                UsersNullable = (await PostsAppService.GetAppUserLookupAsync(new LookupRequestDto { Filter = "" })).Items;
                GroupsNullable = (await PostsExtendAppService.GetGroupLookupAsync(new GroupLookupRequestDto { Filter = "" })).Items;
                
                //_actionsToRunOnceGroupModal.Add(async () => await PostValueToSelect());
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _unRendered = false;
            if (firstRender && CurrentUser is { IsAuthenticated: true } && IsManagerRole())
            {
                Filter = new GeneralStatsApiRequest() { ClientOffsetInMinutes = await GetOffsetInMinutes()};
                await InitDateTimeRangePicker();
                await RenderCharts();
                await InvokeAsync(StateHasChanged);
            }
            //await JSRuntime.InvokeVoidAsync("setTitle", $"GDL - {L["Index.PageTitle"].Value}");
            await JsRuntime.InvokeVoidAsync("HiddenMenuOnMobile");
        }

        private string GetPostUrl(string fid)
        {
            if (fid.IsNotNullOrEmpty()) { return fid.Length >= 5 ? $"Fid:{fid.Substring(fid.Length - 5)}" : fid; }

            return string.Empty;
        }

        private async Task PostValueToSelect()
        {
            await JSRuntime.InvokeVoidAsync("PostValueToSelect");
        }

        private async Task ExportTopReactionPosts()
        {
            var (fromDateTime, toDateTime) = GetDateTimeForApi(StartDate, EndDate);
            var excelBytes = await PostsExtendAppService.ExportExcelTopReactionPosts
            (
                new ExportExcelTopReactionPostInput()
                {
                    EndDateTime = toDateTime,
                    StartDateTime = fromDateTime,
                    RequiredQuantity = _postRequiredQuantity
                }
            );
            var fileName = "TopReactionPosts";
            await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(excelBytes));
        }

        private async Task ExportTopReactionAffiliatePosts()
        {
            var (fromDateTime, toDateTime) = GetDateTimeForApi(StartDate, EndDate);
            var excelBytes = await PostsExtendAppService.ExportExcelTopReactionPosts
            (
                new ExportExcelTopReactionPostInput()
                {
                    EndDateTime = toDateTime,
                    StartDateTime = fromDateTime,
                    PostContentType = PostContentType.Affiliate,
                    RequiredQuantity = _affiliateRequiredQuantity
                }
            );
            var fileName = "TopReactionAffPosts";
            await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(excelBytes));
        }
    }
}