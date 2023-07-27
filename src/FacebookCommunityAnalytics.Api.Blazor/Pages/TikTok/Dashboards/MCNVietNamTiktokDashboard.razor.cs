using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Charts;
using Blazorise.Utilities;
using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.Common.Handlers;
using ChartJs.Blazor.Interop;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.Utils;
using Flurl;
using Microsoft.JSInterop;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Radzen.Blazor.Rendering;
using Tooltips = ChartJs.Blazor.Common.Tooltips;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Dashboards
{
    public partial class MCNVietNamTiktokDashboard : IDisposable
    {
        private List<TikTokMCNReport> TiktokMonthlyTotalFollowers = new List<TikTokMCNReport>();
        private List<TikTokMCNReport> TiktokWeeklyTotalFollowers = new List<TikTokMCNReport>();
        private List<MCNVietNamChannelDto> TopVietNamChannels = new();
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
        private int SelectedTopValue = 10;

        private DateTime fromDate;
        private DateTime toDate;
        private DateTimeOffset? TimeFromWeekly { get; set; }
        private DateTimeOffset? TimeToWeekly { get; set; }
        private BarConfig _configTiktokGrowth = new();
        private Chart _tiktokGrowth = new();
        private List<TikTokMCNDto> TikTokMCN { get; set; } = new();

        private List<KeyValuePair<string, int>> _topMcnSelectedItems = new()
        {
            new KeyValuePair<string, int>("Top 10", 10),
            new KeyValuePair<string, int>("Top 20", 20)
        };

        private IEnumerable<Guid> SelectedTikTokMCNIds { get; set; } = new List<Guid>();
        private List<GroupStatsViewDto> TopViewChannelsTwoWeek = new List<GroupStatsViewDto>();

        public MCNVietNamTiktokDashboard()
        {
            TimeToWeekly = DateTimeOffset.Now.Date.EndOfDay();
            TimeFromWeekly = TimeToWeekly.Value.Date.AddDays(-21);
            var now = DateTime.UtcNow;
            Years = Enumerable.Range(year, (DateTime.Now.Year - year) + 1).ToList();
            fromMonth = now.AddMonths(-3).Month;
            fromYear = now.AddMonths(-3).Year;
            toMonth = now.Month;
            toYear = now.Year;
        }

        protected override async Task OnInitializedAsync()
        {
            InitTiktokGrowthChart();
            BrowserDateTime = await GetBrowserDateTime();
            await GetTiktokMonthlyReport();
            await GetTikTokWeeklyReportData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                TikTokMCN = await TiktokStatsAppService.GetListTikTokMCN(TikTokMCNType.MCNVietNam);
                // SelectedTikTokMCNIds = TikTokMCN.Select(_ => _.Id);
                await GetTopChannelMCN();
                await RenderChart_TiktokChart();
                var ksd = L["abc"];
            }
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


        private async Task GetMonthlyTotalFollowers(TiktokPropertyType tiktokPropertyType, string timeTitle)
        {
            TiktokMonthlyTotalFollowers = await TiktokStatsAppService.GetTiktokMCNMonthlyReport(fromDate, toDate);
            StateHasChanged();
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
        }

        private async Task GetTikTokWeeklyReportData()
        {
            TiktokWeeklyTotalFollowers = await TiktokStatsAppService.GetTiktokMCNWeeklyReports(TimeFromWeekly.Value.DateTime, TimeToWeekly.Value.DateTime);
        }

        private async Task RenderChart_TiktokChart()
        {
            _configTiktokGrowth.Data.Labels.Clear();
            _configTiktokGrowth.Data.Datasets.Clear();

            var now = DateTime.UtcNow;
            var lastYear = now.AddMonths(-11);
            fromDate = DateTime.SpecifyKind(new DateTime(lastYear.Year, lastYear.Month, 1), DateTimeKind.Utc);
            toDate = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1).EndOfMonth().Add(new TimeSpan(23, 59, 59)), DateTimeKind.Utc);

            if (fromDate > toDate)
            {
                await Message.Info(L["TiktokReports.CheckMonthFilter"]);
                return;
            }

            if (SelectedTikTokMCNIds.IsNullOrEmpty() && SelectedTopValue < 1)
            {
                await _tiktokGrowth.Update();
                return;
            }

            var chartTikTokData = await StatAppService.GetMCNVietnamTikTokChartStats
            (
                new GeneralStatsApiRequest()
                {
                    FromDateTime = fromDate,
                    ToDateTime = toDate,
                    TikTokMcnIds = SelectedTikTokMCNIds.ToList(),
                    Count = SelectedTopValue
                }
            );

            var index = 0;
            foreach (var dataChart in chartTikTokData.MultipleDataCharts)
            {
                IDataset<long> datasetLineChart = new LineDataset<long>(dataChart.Select(x => x.Value))
                {
                    Label = dataChart.FirstOrDefault()?.Type,
                    BackgroundColor = ChartColorHelper.GetColor(index),
                    BorderColor = ChartColorHelper.GetBorderColor(index),
                    BorderWidth = 2,
                    LineTension = 0,
                    SpanGaps = false,
                    Fill = FillingMode.Disabled,
                    YAxisId = "TiktokTotalView"
                };

                _configTiktokGrowth.Data.Datasets.Add(datasetLineChart);
                index++;
            }

            _configTiktokGrowth.Data.Labels.AddRange(chartTikTokData.ChartLabels);
            _configTiktokGrowth.Options.Scales = new BarScales
            {
                XAxes = new List<CartesianAxis>
                {
                    new BarCategoryAxis()
                    {
                        ScaleLabel = new ScaleLabel {LabelString = "Date"},
                        Stacked = true
                    }
                },
                YAxes = new List<CartesianAxis>
                {
                    new LinearCartesianAxis
                    {
                        Display = AxisDisplay.True,
                        ScaleLabel = new ScaleLabel
                        {
                            Display = true,
                            LabelString = L["TikTokDashBoard.MCNTikTokChart"],
                        },
                        Ticks = new LinearCartesianTicks()
                        {
                            BeginAtZero = true,
                            Callback = new JavaScriptHandler<AxisTickCallback>("generalInterop.customAxisLabel")
                        },
                        ID = "TiktokTotalView",
                        Position = Position.Right,
                        GridLines = new GridLines() {DrawOnChartArea = false}
                    }
                }
            };

            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configTiktokGrowth.CanvasId,
                false,
                true,
                true,
                new List<string>(), // Don't custom labels
                new[] {0}, // Hide label for dataset position
                true
            );
            await _tiktokGrowth.Update();
        }

        private void InitTiktokGrowthChart()
        {
            _configTiktokGrowth = new BarConfig()
            {
                Options = new BarOptions()
                {
                    MaintainAspectRatio = false,
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        FontSize = 20
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new BarCategoryAxis()
                            {
                                ScaleLabel = new ScaleLabel {LabelString = "Date"},
                                Stacked = true
                            }
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new LinearCartesianAxis
                            {
                                Position = Position.Right,
                            }
                        }
                    }
                }
            };
        }

        private async void OnSelectedTikTokMCNChanged(object value)
        {
            SelectedTikTokMCNIds = value is IEnumerable<Guid> guids ? guids : new List<Guid>();
            SelectedTopValue = 0;
            await RenderChart_TiktokChart();
        }

        private async void OnSelectedTopMCNChanged(object value)
        {
            SelectedTopValue = (int) value;
            SelectedTikTokMCNIds = new List<Guid>();
            await RenderChart_TiktokChart();
        }

        async void IDisposable.Dispose()
        {
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.clearTooltips"
            );
        }

        private async Task GetTopChannelMCN()
        {
            var (fromDate, toDate) = await GetYesterday();
            var (from, to) = GetDateTimeForApi(fromDate, toDate);
            TopVietNamChannels = await TikTokMCNGdlAppService.GetTopMCNVietNamChannel(from.Value, to.Value, 20);
        }
    }
}