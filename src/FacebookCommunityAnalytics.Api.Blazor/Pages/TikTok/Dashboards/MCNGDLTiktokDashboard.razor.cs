using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Util;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Models;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.Utils;
using Microsoft.JSInterop;
using Microsoft.OpenApi.Extensions;
using ChartJsColor = System.Drawing.Color;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Dashboards
{
    public partial class MCNGDLTiktokDashboard
    {
        private BarConfig _configChartInOut;
        private Chart _chartInOut;

        private BarConfig _configChartViewAndCreator;
        private Chart _chartViewAndCreator;

        private PieConfig _configContractStatusChart;
        private Chart _chartContractStatus;

        private PieConfig _configCategoriesChart;
        private Chart _chartCategories;

        private List<string> contractStatusChartLabels = new();
        private List<string> channelCategoriesLabels = new();
        
        private TikTokTimeFrame ChannelTimeFrame = TikTokTimeFrame.Weekly;
        private TikTokDateTimeFrame _tikTokDateTimeFrame = TikTokDateTimeFrame.LastDay;
        private string RankTitleByTime { get; set; }

        private List<EnumSelectItem<GroupCategoryType>> Categories = new();

        private string ThumbnailVideoOfDayUrl { get; set; }
        private string ThumbnailChannelOfWeekUrl { get; set; }
        private TiktokDto TopVideoOfDay { get; set; }
        private TopChannelDto TopChannelOfWeek { get; set; }
        private TikTokTimeFrame tikTokTimeFrame = TikTokTimeFrame.Weekly;
        private string _chartFontTitleColor = "#037404";
        private string _inoutChartTitle;
        
        private IReadOnlyList<TopChannelDto> TopStatChannels { get; set; } = new List<TopChannelDto>();
        private IList<TiktokDto> TopTikTokVideos { get; set; } = new List<TiktokDto>();
        private IList<GroupCategoryType> selectedCategories = new List<GroupCategoryType>()
        {
            GroupCategoryType.Beauty,
            GroupCategoryType.FnB,
            GroupCategoryType.Travel,
            GroupCategoryType.Sport,
            GroupCategoryType.Pets,
            GroupCategoryType.Daily,
            GroupCategoryType.Fashion,
            GroupCategoryType.Education,
            GroupCategoryType.Entertainment,
            GroupCategoryType.Talent
        };

        public MCNGDLTiktokDashboard()
        {
        }

        protected override Task OnInitializedAsync()
        {
            _inoutChartTitle = L["TikTok.Dashboard.WeeklyInOutChannelChart"].Value;
            RankTitleByTime = L["TikTok.MCNGDL.Top10.Week"].Value;
            ConfigInOutChart();
            ConfigViewAndCreatorChart();
            ConfigContractStatusChart();
            ConfigChannelCategoriesChart();
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                BrowserDateTime = await GetBrowserDateTime();
                await GetTopVideoOfDay();
                await GetTopChannelOfWeek();
                await OnTopVideoTimeFrame_Changed(TikTokDateTimeFrame.LastWeek);

                if (TopVideoOfDay != null)
                {
                    var  item = await TiktokStatsAppService.GetVideoImage(TopVideoOfDay.Id);
                    if (item is not null)
                    {
                        ThumbnailVideoOfDayUrl = $"data:image/gif;base64,{Convert.ToBase64String(item.FileData, 0, item.FileData.Length)}";
                    }
                }

                if (TopChannelOfWeek != null)
                {
                    var item = await GroupExtendAppService.GetChannelImage(TopChannelOfWeek.Group.Id);
                    if (item is not null)
                    {
                        ThumbnailChannelOfWeekUrl = $"data:image/gif;base64,{Convert.ToBase64String(item.FileData, 0, item.FileData.Length)}";
                    }
                }
                
                await LoadDataChartInOutChannels();
                await LoadViewAndCreatorChart();
                await LoadContractStatusChart();
                await LoadChannelCategoriesChart();
                StateHasChanged();
            }
        }

        private async Task GetTopVideoOfDay()
        {
            var (startDate, endDate) = await GetYesterday();
            var (from, end) = GetDateTimeForApi(startDate, endDate);
            TopVideoOfDay = await TiktokStatsAppService.GetTopVideoOfDay(from.Value, end.Value);
        }

        private async Task GetTopChannelOfWeek()
        {
            var (startDate, endDate) = await GetDefaultWeekDate();
            var (from, end) = GetDateTimeForApi(startDate, endDate);
            TopChannelOfWeek = await TiktokStatsAppService.GetTopChannel(from.Value, end.Value);

        }

        private async Task LoadDataChartInOutChannels()
        {
            _configChartInOut.Data.Labels.Clear();
            _configChartInOut.Data.Datasets.Clear();

            var dataCharts = new MultipleDataSourceChart<int>();
            switch (tikTokTimeFrame)
            {
                case TikTokTimeFrame.Monthly:
                { 
                    dataCharts = await TikTokMcnGdlAppService.GetMonthlyChannelInOutChart();
                    break;
                }
                case TikTokTimeFrame.Weekly:
                {
                    dataCharts = await TikTokMcnGdlAppService.GetWeeklyChannelInOutChart();
                    break;
                }
                    
            }
            
            var datasetCharts = new List<IDataset<int>>();
            for (int i = 0; i < dataCharts.MultipleDataCharts.Count; i++)
            {
                var dataChart = dataCharts.MultipleDataCharts[i];
                IDataset<int> dataset = new BarDataset<int>(dataChart.Where(x => x.Display.IsIn(dataCharts.ChartLabels)).Select(x => x.Value))
                {
                    Label = i == 0 ? L["InOutChart.InChannels"] : L["InOutChart.OutChannels"],
                    BackgroundColor = i == 0 ? ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Blue)) : ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Red)),
                    BorderColor = i == 0 ? ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Blue) : ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Red),
                    BorderWidth = 1,
                    YAxisId = "TotalChannel"
                };

                datasetCharts.Add(dataset);
            }

            _configChartInOut.Data.Labels.AddRange(dataCharts.ChartLabels);
            _configChartInOut.Data.Datasets.AddRange(datasetCharts);

            _configChartInOut.Options.Scales = new BarScales
            {
                XAxes = new List<CartesianAxis>
                {
                    new BarCategoryAxis()
                    {
                        ScaleLabel = new ScaleLabel { LabelString = "Date" },
                        Stacked = false
                    }
                },
                YAxes = new List<CartesianAxis>
                {
                    new BarLinearCartesianAxis()
                    {
                        ScaleLabel = new ScaleLabel
                        {
                            Display = true,
                            LabelString = L["InOutChart.TotalChannel"],
                        },
                        Position = Position.Left,
                        ID = "TotalChannel",
                        Stacked = false
                    }
                },
            };

            await _chartInOut.Update();
            await JSRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configChartInOut.CanvasId,
                false, //horizontal
                false, //setpostion
                false //isHideLabel
            );
        }

        private async Task LoadViewAndCreatorChart()
        {
            _configChartViewAndCreator.Data.Labels.Clear();
            _configChartViewAndCreator.Data.Datasets.Clear();

            var dataCharts = await TikTokMcnGdlAppService.GetViewAndCreatorChart();

            IDataset<long> datasetLineChart = new LineDataset<long>(dataCharts.MultipleDataCharts.First().Where(x => x.Display.IsIn(dataCharts.ChartLabels)).Select(x => x.Value))
            {
                Label = L["ViewAndCreatorChart.TotalViews"],
                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Red)),
                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Red),
                BorderWidth = 2,
                LineTension = 0.15,
                BorderJoinStyle = BorderJoinStyle.Round,
                PointRadius = 1,
                PointHoverRadius = 2,
                SpanGaps = false,
                Fill = FillingMode.Disabled,
                YAxisId = "TotalViews"
            };

            IDataset<long> datasetCreator = new BarDataset<long>(dataCharts.MultipleDataCharts.Last().Where(x => x.Display.IsIn(dataCharts.ChartLabels)).Select(x => x.Value))
            {
                Label = L["ViewAndCreatorChart.TotalCreators"],
                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(100, ChartJsUtils.ChartColors.Blue)),
                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Blue),
                BorderWidth = 1,
                YAxisId = "TotalCreators"
            };

            _configChartViewAndCreator.Data.Labels.AddRange(dataCharts.ChartLabels);

            _configChartViewAndCreator.Data.Datasets.Add(datasetLineChart);
            _configChartViewAndCreator.Data.Datasets.Add(datasetCreator);

            var maxTotalCreators = dataCharts.MultipleDataCharts.Last().Max(x => x.Value);
            _configChartViewAndCreator.Options.Scales = new BarScales
            {
                XAxes = new List<CartesianAxis>
                {
                    new BarCategoryAxis()
                    {
                        ScaleLabel = new ScaleLabel { LabelString = "Date" },
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
                            Display = false,
                            LabelString = L["ViewAndCreatorChart.Views"],
                        },
                        ID = "TotalViews",
                        Ticks = new LinearCartesianTicks()
                        {
                            BeginAtZero = false,
                            Callback = new JavaScriptHandler<AxisTickCallback>("generalInterop.customAxisLabel")
                        },
                        Position = Position.Right,
                        GridLines = new GridLines() { DrawOnChartArea = false }
                    },
                    new BarLinearCartesianAxis()
                    {
                        ScaleLabel = new ScaleLabel
                        {
                            Display = false,
                            LabelString = L["ViewAndCreatorChart.Creators"],
                        },
                        Position = Position.Left,
                        ID = "TotalCreators",
                        Ticks = new LinearCartesianTicks() { Max = (int)maxTotalCreators * 10 },
                        Stacked = true
                    }
                },
            };
            await _chartViewAndCreator.Update();

            await JSRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configChartViewAndCreator.CanvasId,
                false, //horizontal
                false, //setpostion
                true, //isHideLabel
                new List<string>(), // Don't custom labels
                new[] { 0 }, // Hide label for dataset position
                false
            );
        }

        private void ConfigInOutChart()
        {
            _configChartInOut = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        FontSize = 20,
                        FontColor = _chartFontTitleColor
                    },
                    // Tooltips = new ChartJs.Blazor.Common.Tooltips
                    // {
                    //     Mode = InteractionMode.Index,
                    //     Intersect = false
                    // },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis> { new BarCategoryAxis { Stacked = false } },
                        YAxes = new List<CartesianAxis> { new BarLinearCartesianAxis { Stacked = false } }
                    }
                }
            };
        }

        private void ConfigViewAndCreatorChart()
        {
            _configChartViewAndCreator = new BarConfig()
            {
                Options = new BarOptions()
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["TikTok.Dashboard.ViewAndCreatorChart"].Value,
                        FontSize = 20,
                        FontColor = _chartFontTitleColor
                    },
                    // Tooltips = new ChartJs.Blazor.Common.Tooltips
                    // {
                    //     Mode = InteractionMode.Index,
                    //     Intersect = false
                    // },
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
                                ScaleLabel = new ScaleLabel { LabelString = "Date" },
                                Stacked = true
                            }
                        },
                    }
                }
            };
        }

        private void ConfigContractStatusChart()
        {
            _configContractStatusChart = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["ContractStatusChart"].Value,
                        FontSize = 20,
                        FontColor = _chartFontTitleColor
                    },
                    Tooltips = new ChartJs.Blazor.Common.Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = true
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                    Events = new []{ BrowserEvent.MouseMove, BrowserEvent.TouchStart, BrowserEvent.TouchMove, BrowserEvent.Click, }
                }
            };
        }

        private async Task LoadContractStatusChart()
        {
            _configContractStatusChart.Data.Labels.Clear();
            _configContractStatusChart.Data.Datasets.Clear();

            var chartData = await TikTokMcnGdlAppService.GetChannelContractStatusChart();
            _configContractStatusChart.Data.Labels.AddRange(chartData.Items.Select(x => x.Label).ToList());

            foreach (var item in chartData.Items)
            {
                var label = string.Empty;
                if (item.ValuePercent != 0) label = $"{item.ValuePercent}%";
                contractStatusChartLabels.Add(label);
            }

            var dataItems = chartData.Items.Select(x => x.Value).ToList();
            var dataSet = new PieDataset<double>(dataItems, useDoughnutDefaults: false);

            var colors = new List<string>();
            for (int i = 0; i < chartData.Items.Count; i++)
            {
                colors.Add(ChartColorHelper.GetColor(i));
            }

            dataSet.BackgroundColor = colors.ToArray();
            _configContractStatusChart.Data.Datasets.Add(dataSet);

            await _chartContractStatus.Update();

            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configContractStatusChart.CanvasId,
                false,
                false,
                false,
                contractStatusChartLabels
            );
        }


        private void ConfigChannelCategoriesChart()
        {
            _configCategoriesChart = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["ChannelCategoriesChart"].Value,
                        FontSize = 20,
                        FontColor = _chartFontTitleColor
                    },
                    Tooltips = new ChartJs.Blazor.Common.Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = true
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                  Events = new []{ BrowserEvent.MouseMove, BrowserEvent.TouchStart, BrowserEvent.TouchMove, BrowserEvent.Click, }
                }
            };
        }

        private async Task LoadChannelCategoriesChart()
        {
            _configCategoriesChart.Data.Labels.Clear();
            _configCategoriesChart.Data.Datasets.Clear();

            if (selectedCategories.IsNullOrEmpty())
            {
                return;
            }

            var chartData = await TikTokMcnGdlAppService.GetChannelCategoriesChart(selectedCategories.ToList());
            _configCategoriesChart.Data.Labels.AddRange(chartData.Items.Select(x => x.Label).ToList());

            foreach (var item in chartData.Items)
            {
                var label = string.Empty;
                if (item.ValuePercent != 0) label = $"{item.ValuePercent}%";
                channelCategoriesLabels.Add(label);
            }

            var dataItems = chartData.Items.Select(x => x.Value).ToList();
            var dataSet = new PieDataset<double>(dataItems, useDoughnutDefaults: false);

            var colors = new List<string>();
            for (int i = 0; i < chartData.Items.Count; i++)
            {
                colors.Add(ChartColorHelper.GetColor(i));
            }

            dataSet.BackgroundColor = colors.ToArray();
            _configCategoriesChart.Data.Datasets.Add(dataSet);

            await _chartCategories.Update();

            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configCategoriesChart.CanvasId,
                false,
                false,
                false,
                channelCategoriesLabels
            );
        }

        private async Task GetTopTikTokVideos(DateTime fromDate, DateTime toDate)
        {
            TopTikTokVideos = await TikTokMcnGdlAppService.GetTopMCNGDLVideos
            (
                new GetTikTokVideosRequest()
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Count = 10
                }
            );
            StateHasChanged();
        }
        // private async void OnSelectedCategoriesChanged(object value)
        // {
        //     selectedCategories = value is IEnumerable<GroupCategoryType> ? (IEnumerable<GroupCategoryType>)value : new List<GroupCategoryType>();
        //
        //     await LoadChannelCategoriesChart();
        // }
        private async Task OnDataGridReadAsync()
        {
             TopStatChannels = await TiktokStatsAppService.GetTopStatChannelsOfWeek();
        }

        private async Task OnChannelTimeFrame_Changed(TikTokTimeFrame value)
        {
            ChannelTimeFrame = value;
            switch (value)
            {
                case TikTokTimeFrame.Weekly:
                {
                    RankTitleByTime = L["TikTok.MCNGDL.Top10.Week"].Value;
                    TopStatChannels = await TiktokStatsAppService.GetTopStatChannelsOfWeek();
                    break;
                }
                case TikTokTimeFrame.Monthly:
                {
                    RankTitleByTime = L["TikTok.MCNGDL.Top10.Month"].Value;
                    TopStatChannels = await TiktokStatsAppService.GetTopStatChannelOfMonth();
                    break;
                }
            }
        }

        private async Task OnTimeStatus_Changed(TikTokTimeFrame value)
        {
            switch (value)
            {
                case TikTokTimeFrame.Weekly:
                {
                    _inoutChartTitle = L["TikTok.Dashboard.WeeklyInOutChannelChart"];
                    break;
                }
                case TikTokTimeFrame.Monthly:
                {
                    _inoutChartTitle = L["TikTok.Dashboard.MonthlyInOutChannelChart"];
                    break;
                }    
            }
            tikTokTimeFrame = value;
            await LoadDataChartInOutChannels();
        }
        
        private async Task OnTopVideoTimeFrame_Changed(TikTokDateTimeFrame value)
        {
            _tikTokDateTimeFrame = value;
            var (fromDate, toDate) = (new DateTimeOffset(), new DateTimeOffset());
            switch (value)
            {
                case TikTokDateTimeFrame.LastDay:
                    (fromDate, toDate) = await GetYesterday();
                    break;
                case TikTokDateTimeFrame.LastWeek:
                    (fromDate, toDate) = await GetDefaultWeekDate();
                    break;
                case TikTokDateTimeFrame.LastMonth:
                    (fromDate, toDate) = await GetDefaultMonthDate();
                    break;
            }
            await GetTopTikTokVideos(fromDate.DateTime, toDate.DateTime);
        }
        
        private string GetVideoUrl(TiktokDto tiktokVideo)
        {
            return tiktokVideo.VideoId.IsNullOrWhiteSpace() ? L["PostUrl"] : $"VideoId: {tiktokVideo.VideoId.MaybeSubstring(7, true)}";
        }
    }
}