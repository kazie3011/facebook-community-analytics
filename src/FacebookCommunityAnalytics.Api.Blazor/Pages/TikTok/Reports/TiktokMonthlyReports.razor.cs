using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Microsoft.JSInterop;
using Radzen.Blazor.Rendering;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Reports
{
    public partial class TiktokMonthlyReports
    {
        private List<TikTokMonthlyTotalFollowers> TiktokMonthlyTotalFollowers = new List<TikTokMonthlyTotalFollowers>();
        private List<TiktokMonthlyReportModel> TiktokMonthlyTotalFollowerModels = new List<TiktokMonthlyReportModel>();
        private List<TikTokMonthlyTotalViews> TiktokMonthlyTotalViews = new List<TikTokMonthlyTotalViews>();
        private PageToolbar Toolbar { get; } = new();

        private bool ReportLoaded;

        private bool? _orderByFollower;
        private bool? _orderByView;
        private bool? _orderByVideo;
        private bool? _orderByAverage;

        private string _orderByTimeTitle;

        private int fromMonth;
        private int fromYear;

        private int toMonth;
        private int toYear;

        private IReadOnlyList<int> Months = new List<int>()
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12
        };

        private IReadOnlyList<int> Years = new List<int>();
        private int year = 2021;

        private DateTime fromDate;
        private DateTime toDate;

        public TiktokMonthlyReports()
        {
            var now = DateTime.UtcNow;
            Years = Enumerable.Range(year, (DateTime.Now.Year - year) + 1).ToList();
            fromMonth = now.AddMonths(-1).Month;
            fromYear = now.AddMonths(-1).Year;
            toMonth = now.Month;
            toYear = now.Year;
            _orderByFollower = null;
            _orderByView = null;
            _orderByVideo = null;
            _orderByAverage = null;
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await GetTiktokMonthlyReport();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage(L["TiktokMonthlyReports.PageTitle"].Value);
            }
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["TiktokReport.Export"], async () => { await ExportTikTokReport(); }, IconName.FilePdf, requiredPolicyName: ApiPermissions.Tiktok.Default);
            return ValueTask.CompletedTask;
        }

        private async Task GetMonthlyTotalFollowers(TiktokPropertyType tiktokPropertyType, string timeTitle)
        {
            _orderByTimeTitle = timeTitle;
            var tiktokOrderByType = TiktokOrderByType(tiktokPropertyType);
            TiktokMonthlyTotalFollowerModels = new List<TiktokMonthlyReportModel>();
            TiktokMonthlyTotalFollowers = await TiktokStatsAppService.GetMonthlyTotalFollowersReport
            (
                new GetTiktokMonthlyTotalFollowersRequest
                {
                    TimeFrom = fromDate,
                    TimeTo = toDate,
                    TiktokOrderByType = tiktokOrderByType,
                    OrderByTimeTitle = timeTitle
                }
            );
            GetTiktokMonthlyTotalFollowerModels();
            GetMonthlyChanging();
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

        private async Task GetMonthlyTotalViews()
        {
            TiktokMonthlyTotalViews = await TiktokStatsAppService.GetMonthlyTotalViewsReport(fromDate, toDate);
        }

        private async Task ChangedDropDownFromMonth(int input)
        {
            fromMonth = input;
            await GetTiktokMonthlyReport();
        }
        private async Task ChangedDropDownFromYear(int input)
        {
            fromYear = input;
            await GetTiktokMonthlyReport();
        }
        private async Task ChangedDropDownToMonth(int input)
        {
            toMonth = input;
            await GetTiktokMonthlyReport();
        }
        private async Task ChangedDropDownToYear(int input)
        {
            fromYear = input;
            await GetTiktokMonthlyReport();
        }

        private async Task GetTiktokMonthlyReport()
        {
            fromDate = new DateTime(fromYear, fromMonth, 1);
            toDate = new DateTime(toYear, toMonth, 1).EndOfMonth().Add(new TimeSpan(23, 59, 59));

            if (fromDate > toDate)
            {
                await Message.Info(L["TiktokReports.CheckMonthFilter"]);
                return;
            }

            await GetMonthlyTotalFollowers(TiktokPropertyType.NoSelect, null);
            await GetMonthlyTotalViews();
        }

        private void GetTiktokMonthlyTotalFollowerModels()
        {
            TiktokMonthlyTotalFollowerModels = new List<TiktokMonthlyReportModel>();
            foreach (var item in TiktokMonthlyTotalFollowers)
            {
                TiktokMonthlyTotalFollowerModels.Add
                (
                    new TiktokMonthlyReportModel
                    {
                        TimeTitle = item.TimeTitle,
                        Followers = item.Followers,
                        Views = item.Views,
                        Videos = item.Videos,
                        Average = item.Average,
                        ChannelName = item.ChannelName,
                        TiktokCategoryType = L[$"Enum:GroupCategoryType:{Convert.ToInt32(item.TiktokCategoryType)}"]
                    }
                );
            }
        }

        private void GetMonthlyChanging()
        {
            foreach (var items in TiktokMonthlyTotalFollowerModels.GroupBy(_ => _.ChannelName))
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
                        var changedValue = item.Followers - preFollowers;

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

        private async Task ExportTikTokReport()
        {
            var excelBytes = ExportHelper.GenerateTiktokMonthlyReportExcelBytes(L, TiktokMonthlyTotalFollowerModels);
            if (excelBytes == null)
            {
                await Message.Info(L["TiktokReports.CheckMonthFilter"]);
                return;
            }

            await JsRuntime.InvokeVoidAsync("saveAsFile", $"TiktokMonthlyReport_{fromMonth}/{fromYear}-{toMonth}/{toYear}.xlsx", Convert.ToBase64String(excelBytes));
        }
    }

    public class TiktokMonthlyReportModel
    {
        public string TimeTitle { get; set; }
        public int Followers { get; set; }
        public int Views { get; set; }
        public double Videos { get; set; }
        public double Average { get; set; }
        public string ChannelName { get; set; }
        public bool? IncreasedFollowers { get; set; }
        public string TiktokCategoryType { get; set; }
    }
}