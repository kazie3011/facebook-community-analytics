using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Reports
{
    public partial class TiktokWeeklyReports
    {
        private List<TiktokWeeklyTotalFollowers> TiktokWeeklyTotalFollowers = new();
        private List<TiktokWeeklyReportModelMCN> TiktokWeeklyTotalFollowerModelMCN = new();
        private List<TiktokWeeklyTotalViews> TiktokWeeklyTotalViews = new();

        // private DatePicker<DateTime> datePickerFrom { get; set; }
        // private DatePicker<DateTime> datePickerTo { get; set; }
        private DateTimeOffset? TimeFrom { get; set; }
        private DateTimeOffset? TimeTo { get; set; }
        private PageToolbar Toolbar { get; } = new();

        private bool ReportLoaded;

        private bool? _orderByFollower;
        private bool? _orderByView;
        private bool? _orderByVideo;
        private bool? _orderByAverage;

        private string _orderByWeekName;

        public TiktokWeeklyReports()
        {
            TimeTo = DateTimeOffset.Now.Date.EndOfDay();
            TimeFrom = TimeTo.Value.Date.AddDays(-21);
            _orderByFollower = null;
            _orderByView = null;
            _orderByVideo = null;
            _orderByAverage = null;
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await GetWeeklyTotalFollowers(TiktokPropertyType.NoSelect, null);
            await GetWeeklyTotalViews();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["TiktokWeeklyReports.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["TiktokReport.ExportBoD"], async () => { await ExportsReportTikTokBoD(); }, IconName.Save, requiredPolicyName: ApiPermissions.Tiktok.Default);
            Toolbar.AddButton(L["TiktokReport.ExportMCN"], async () => { await ExportsReportTikTokMCN(); }, IconName.Save, requiredPolicyName: ApiPermissions.Tiktok.Default);
            return ValueTask.CompletedTask;
        }

        private async Task GetWeeklyTotalFollowers(TiktokPropertyType tiktokPropertyType, string weekName)
        {
            _orderByWeekName = weekName;
            var tiktokOrderByType = TiktokOrderByType(tiktokPropertyType);
            if (TimeFrom.HasValue && TimeTo.HasValue)
            {
                TiktokWeeklyTotalFollowers = await TiktokStatsAppService.GetWeeklyTotalFollowersReport
                (
                    new GetTiktokWeeklyTotalFollowersRequest
                    {
                        TimeFrom = TimeFrom.Value.DateTime,
                        TimeTo = TimeTo.Value.DateTime,
                        TiktokOrderByType = tiktokOrderByType,
                        OrderByWeekName = weekName
                    }
                );
            }

            GetTiktokWeeklyTotalFollowerModels();
            GetWeeklyChanging();
            StateHasChanged();
        }

        private TiktokOrderByType TiktokOrderByType(TiktokPropertyType tiktokPropertyType)
        {
            var tiktokOrderByType = Core.Enums.TiktokOrderByType.NoSelect;
            switch (tiktokPropertyType)
            {
                case TiktokPropertyType.Followers:
                    if (_orderByFollower is null or false)
                    {
                        _orderByFollower = true;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.Followers;
                    }
                    else
                    {
                        _orderByFollower = false;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.DescendingFollowers;
                    }

                    _orderByView = null;
                    _orderByVideo = null;
                    _orderByAverage = null;
                    break;
                case TiktokPropertyType.Views:
                    if (_orderByView is null or false)
                    {
                        _orderByView = true;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.Views;
                    }
                    else
                    {
                        _orderByView = false;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.DescendingViews;
                    }

                    _orderByFollower = null;
                    _orderByVideo = null;
                    _orderByAverage = null;
                    break;
                case TiktokPropertyType.Videos:
                    if (_orderByVideo is null or false)
                    {
                        _orderByVideo = true;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.Videos;
                    }
                    else
                    {
                        _orderByVideo = false;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.DescendingVideos;
                    }

                    _orderByFollower = null;
                    _orderByView = null;
                    _orderByAverage = null;
                    break;
                case TiktokPropertyType.Average:
                    if (_orderByAverage is null or false)
                    {
                        _orderByAverage = true;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.Average;
                    }
                    else
                    {
                        _orderByAverage = false;
                        tiktokOrderByType = Core.Enums.TiktokOrderByType.DescendingAverage;
                    }

                    _orderByFollower = null;
                    _orderByView = null;
                    _orderByVideo = null;
                    break;
                case TiktokPropertyType.NoSelect:
                    break;
            }

            return tiktokOrderByType;
        }

        // private async Task RenderWeeklyFollowerReportGrid()
        // {
        //     await JSRuntime.InvokeVoidAsync("createPivotGrid", gridId, TiktokWeeklyTotalFollowers);
        // }

        private async Task GetWeeklyTotalViews()
        {
            if (TimeFrom.HasValue && TimeTo.HasValue)
            {
                TiktokWeeklyTotalViews = await TiktokStatsAppService.GetWeeklyTotalViewsReport(TimeFrom.Value.DateTime, TimeTo.Value.DateTime);
            }
        }

        private async Task ReloadAllData()
        {
            await GetWeeklyTotalFollowers(TiktokPropertyType.NoSelect, null);
            await GetWeeklyTotalViews();
            //await RenderWeeklyFollowerReportGrid();
        }

        private void GetTiktokWeeklyTotalFollowerModels()
        {
            TiktokWeeklyTotalFollowerModelMCN = new List<TiktokWeeklyReportModelMCN>();

            foreach (var item in TiktokWeeklyTotalFollowers)
            {
                TiktokWeeklyTotalFollowerModelMCN.Add
                (
                    new TiktokWeeklyReportModelMCN
                    {
                        TimeTitle = item.WeekName,
                        Followers = item.Followers,
                        Views = item.Views,
                        Videos = item.Videos,
                        ChannelName = item.ChannelName,
                        TiktokCategoryType = L[$"Enum:GroupCategoryType:{Convert.ToInt32(item.TiktokCategoryType)}"]
                    }
                );
            }
        }

        private void GetWeeklyChanging()
        {
            foreach (var items in TiktokWeeklyTotalFollowerModelMCN.GroupBy(_ => _.ChannelName))
            {
                var first = true;
                var preFollowers = 0;
                foreach (var item in items)
                {
                    if (first)
                    {
                        preFollowers = item.Followers;
                        first = false;
                    }
                    else
                    {
                        int changedValue = item.Followers - preFollowers;
                        if (changedValue > (preFollowers * 0.1))
                        {
                            item.IncreasedFollowers = true;
                        }
                        else if (changedValue < 0)
                        {
                            item.IncreasedFollowers = false;
                        }
                        else
                        {
                            item.IncreasedFollowers = null;
                        }

                        preFollowers = item.Followers;
                    }
                }
            }
        }

        private async Task ExportsReportTikTokMCN()
        {
            var excelBytes = ExportHelper.GenerateTiktokWeeklyReportExcelBytesMCN(L, TiktokWeeklyTotalFollowerModelMCN);
            if (excelBytes == null)
            {
                await Message.Info(L["TiktokExport.NoData"]);
                return;
            }

            await JsRuntime.InvokeVoidAsync("saveAsFile", $"TikTokWeeklyReport_MCN_{TimeFrom:yyyyMMdd}-{TimeTo:yyyyMMdd}.xlsx", Convert.ToBase64String(excelBytes));
        }

        private async Task ExportsReportTikTokBoD()
        {
            var bodModel = new List<TiktokWeeklyReportModelBoD>();

            foreach (var item in TiktokWeeklyTotalFollowerModelMCN)
            {
                bodModel.Add
                (
                    new TiktokWeeklyReportModelBoD
                    {
                        Followers = item.Followers,
                        IncreasedFollowers = item.IncreasedFollowers,
                        ChannelName = item.ChannelName,
                        TimeTitle = item.TimeTitle,
                        TiktokCategoryType = item.TiktokCategoryType
                    }
                );
            }

            var excelBytes = ExportHelper.GenerateTiktokWeeklyReportExcelBytesBoD(L, bodModel);
            if (excelBytes == null)
            {
                await Message.Info(L["TiktokExport.NoData"]);
                return;
            }

            await JsRuntime.InvokeVoidAsync("saveAsFile", $"TikTokWeeklyReport_BoD_{TimeFrom:yyyyMMdd}-{TimeTo:yyyyMMdd}.xlsx", Convert.ToBase64String(excelBytes));
        }
    }

    public class TiktokWeeklyReportModelMCN
    {
        public string TimeTitle { get; set; }
        public int Followers { get; set; }
        public int Views { get; set; }
        public double Videos { get; set; }
        public string ChannelName { get; set; }
        public bool? IncreasedFollowers { get; set; }
        public string TiktokCategoryType { get; set; }
    }


    public class TiktokWeeklyReportModelBoD
    {
        public string TimeTitle { get; set; }
        public int Followers { get; set; }
        public string ChannelName { get; set; }
        public bool? IncreasedFollowers { get; set; }
        public string TiktokCategoryType { get; set; }
    }
}