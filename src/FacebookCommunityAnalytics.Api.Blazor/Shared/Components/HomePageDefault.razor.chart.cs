using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
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
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Utils;
using Microsoft.JSInterop;
using ChartJsColor = System.Drawing.Color;

namespace FacebookCommunityAnalytics.Api.Blazor.Shared.Components
{
    public partial class HomePageDefault
    {
        private static readonly string Red = ChartColor.FromRgba(214, 65, 53, 0.5f);
        private static readonly string Orange = ChartColor.FromRgba(255, 159, 64, 0.5f);
        private static readonly string Yellow = ChartColor.FromRgba(247, 200, 63, 0.5f);
        private static readonly string Green = ChartColor.FromRgba(16, 152, 84, 0.5f);
        private static readonly string Blue = ChartColor.FromRgba(65, 129, 238, 0.5f);
        private static readonly string Purple = ChartColor.FromRgba(153, 102, 255, 0.5f);
        private static readonly string Grey = ChartColor.FromRgba(201, 203, 207, 0.5f);
        private static readonly string GreenLight = ChartColor.FromRgba(116, 156, 247, 0.5f);
        private static readonly string RedBlood = ChartColor.FromRgba(233, 17, 14, 0.5f);
        private static readonly string Brown = ChartColor.FromRgba(100, 73, 14, 0.5f);
        private static readonly string Aqua = ChartColor.FromRgba(0, 255, 255, 0.5f);
        private static readonly string Fuchsia = ChartColor.FromRgba(255, 0, 255, 0.5f);
        private static readonly string Darkorchid = ChartColor.FromRgba(153, 50, 204, 0.5f);
        private static readonly string Teal = ChartColor.FromRgba(0,128,128, 0.5f);
        private static readonly string Deeppink = ChartColor.FromRgba(255,20,147, 0.5f);
        private static readonly string Mediumvioletred = ChartColor.FromRgba(199,21,133, 0.5f);
        
        private readonly string _primaryColor = "#037404";

        private List<string> backgroundColors = new List<string>()
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Grey,
            GreenLight,
            RedBlood,
            Brown,
            Aqua,
            Fuchsia,
            Darkorchid,
            Teal,
            Deeppink,
            Mediumvioletred
        };

        private List<string> borderColors = new List<string>()
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Grey,
            GreenLight,
            RedBlood,
            Brown,
            Aqua,
            Fuchsia,
            Darkorchid,
            Teal,
            Deeppink,
            Mediumvioletred
        };

        private List<string> avgPostsChartColors = new List<string>()
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Grey,
            GreenLight,
            RedBlood,
            Brown,
            Aqua,
            Fuchsia,
            Darkorchid,
            Teal,
            Deeppink,
            Mediumvioletred
        };

        private void InitChartConfigs()
        {
            InitGrowthChart();
            InitTiktokGrowthChart();
            InitAvgPostsChart();
            InitAvgCampaignPostsChart();
            InitAffiliateChart();
        }

        private void InitGrowthChart()
        {
            _configGrowth = new BarConfig()
            {
                Options = new BarOptions()
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.GrowthChart"].ToString(),
                        FontColor = _primaryColor,
                        FontSize = 20
                        //Text = "Community Growth Stats"
                    },
                    Tooltips = new ChartJs.Blazor.Common.Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
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
                    }
                }
            };
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
                        Text = L["Index.Chart.TiktokGrowthChart"].ToString(),
                        FontColor = _primaryColor,
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
                    }
                }
            };
        }

        private void InitAvgPostsChart()
        {
            _configAvgPostChart = new BarConfig()
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.AvgPostsChart"].ToString(),
                        FontColor = _primaryColor,
                        FontSize = 20
                    },
                    Tooltips = new ChartJs.Blazor.Common.Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis> {new BarCategoryAxis {Stacked = true}},
                        YAxes = new List<CartesianAxis> {new BarLinearCartesianAxis {Stacked = true}}
                    }
                }
            };
        }

        private void InitAvgCampaignPostsChart()
        {
            _configAvgCampaignPostChart = new BarConfig()
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.AvgCampaignPostsChart"].ToString(),
                        FontColor = _primaryColor,
                        FontSize = 20
                    },
                    Tooltips = new ChartJs.Blazor.Common.Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis> {new BarCategoryAxis {Stacked = true}},
                        YAxes = new List<CartesianAxis> {new BarLinearCartesianAxis {Stacked = true}}
                    }
                }
            };
        }

        private void InitAffiliateChart()
        {
            _configAffiliateChart = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.AffiliateSummaryChart"].ToString(),
                        FontColor = _primaryColor,
                        FontSize = 20
                    },
                    Tooltips = new ChartJs.Blazor.Common.Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis> {new BarCategoryAxis {Stacked = true}},
                        YAxes = new List<CartesianAxis>
                        {
                            new BarLinearCartesianAxis
                            {
                                Display = AxisDisplay.True,
                                ScaleLabel = new ScaleLabel
                                {
                                    Display = true,
                                    LabelString = "",
                                },
                                Ticks = new LinearCartesianTicks {BeginAtZero = true},
                            },
                        }
                    }
                }
            };
        }

        private async Task InitDateTimeRangePicker()
        {
            (StartDate, EndDate) = await GetDefaultMonthDate();
            _dateRanges = await GetDateRangePicker();
        }

        #region RENDER CHARTS

        private async Task RenderCharts()
        {
            _showLoading = true;
            var request = new GeneralStatsApiRequest();

            if (StartDate.HasValue && EndDate.HasValue)
            {
                (request.FromDateTime, request.ToDateTime) = (StartDate.Value.Date, EndDate.Value.Date.AddDays(1).AddTicks(-1));
                _statistics = await _statAppService.GetGeneralStats(request);
                await RenderChart_GrowthChart();
                await RenderChart_TiktokChart();
                await RenderChart_AvgPostsChart();
                await RenderChart_PostsByDateChart();
                await RenderChart_AvgCampaignPostsChart();
                await RenderChart_AffiliateSummaryChart(_affiliateSummaryType);
            }
            _showLoading = false;
        }


        private async Task RenderChart_GrowthChart()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                _configGrowth.Data.Labels.Clear();
                _configGrowth.Data.Datasets.Clear();
                var dateRanges = StartDate.Value.Date.To(EndDate.Value.Date).ToList();
                var (fromDateTime, toDateTime) = GetDateTimeForApi(StartDate, EndDate);
                var _growthLabel = dateRanges.Select(x => x.ToString("dd-MM"));
                var growthDataCharts = await _statAppService.GetGrowthChartStats
                (
                    new GeneralStatsApiRequest
                    {
                        FromDateTime = fromDateTime,
                        ToDateTime = toDateTime
                    }
                );

                IDataset<double?> datasetLineChart = new LineDataset<double?>(growthDataCharts.TotalInteractionsLineCharts.Where(x => x.Display.IsIn(_growthLabel)).Select(x => x.Value))
                {
                    Label = L["TotalInteractions"],
                    BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Red)),
                    BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Red),
                    BorderWidth = 2,
                    LineTension = 0.15,
                    BorderJoinStyle = BorderJoinStyle.Round,
                    PointRadius = 1,
                    PointHoverRadius = 2,
                    SpanGaps = false,
                    Fill = FillingMode.Disabled,
                    YAxisId = "TotalInteractions"
                };

                var datasetsGroupGrowthNumber = new List<IDataset<double>>();
                var maxGrowthNumber = growthDataCharts.GrowthNumberBarCharts.Max(x => x.Value);
                IDataset<double> datasetGrowthNumbers = new BarDataset<double>(growthDataCharts.GrowthNumberBarCharts.Where(x => x.Display.IsIn(_growthLabel)).Select(x => x.Value))
                {
                    Label = L["GrowthNumberFacebook"],
                    BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(100, ChartJsUtils.ChartColors.Blue)),
                    BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Blue),
                    BorderWidth = 1,
                    YAxisId = "GrowthNumber"
                };

                datasetsGroupGrowthNumber.Add(datasetGrowthNumbers);

                _configGrowth.Data.Labels.AddRange(_growthLabel);

                _configGrowth.Data.Datasets.Add(datasetLineChart);
                _configGrowth.Data.Datasets.AddRange<IDataset>(datasetsGroupGrowthNumber);

                _configGrowth.Options.Scales = new BarScales
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
                        new BarLinearCartesianAxis()
                        {
                            ScaleLabel = new ScaleLabel
                            {
                                Display = true,
                                LabelString = L["GrowthNumber"],
                            },
                            Position = Position.Left,
                            ID = "GrowthNumber",
                            Ticks = new LinearCartesianTicks() { Max = (int)maxGrowthNumber * 3 },
                            Stacked = true
                        },
                        new LinearCartesianAxis
                        {
                            Display = AxisDisplay.True,
                            ScaleLabel = new ScaleLabel
                            {
                                Display = true,
                                LabelString = L["TotalInteractions"],
                            },
                            ID = "TotalInteractions",
                            Ticks = new LinearCartesianTicks()
                            {
                                BeginAtZero = true,
                            },
                            Position = Position.Right,
                            GridLines = new GridLines() { DrawOnChartArea = false }
                        }
                    },
                };
                await _chartGrowth.Update();
                await JsRuntime.InvokeVoidAsync
                (
                    "generalInterop.datalabelsConfig",
                    _configGrowth.CanvasId,
                    false,
                    false,
                    true
                );
            }
        }

        private async Task RenderChart_TiktokChart()
        {
            _configTiktokGrowth.Data.Labels.Clear();
            _configTiktokGrowth.Data.Datasets.Clear();

            if (StartDate.HasValue && EndDate.HasValue)
            {
                var growthDataCharts = await _statAppService.GetTiktokChartStats
            (
                new GeneralStatsApiRequest
                {
                    FromDateTime = StartDate.Value.Date,
                    ToDateTime = EndDate.Value.Date,
                    GroupSourceType = GroupSourceType.Tiktok
                }
            );

            TiktokFollowerGrowths = growthDataCharts.TiktokFollowerGrowths;
            
            IDataset<double?> datasetLineChart = new LineDataset<double?>(growthDataCharts.TotalViewLineCharts.Select(x => x.Value))
            {
                Label = L["TiktokTotalView"],
                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Red)),
                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Red),
                BorderWidth = 2,
                LineTension = 0,
                SpanGaps = false,
                Fill = FillingMode.Disabled,
                YAxisId = "TiktokTotalView"
            };

            IListExtensions.AddRange<string>(_configTiktokGrowth.Data.Labels, growthDataCharts.ChartLabels.Select(x => x.ToString()));
            _configTiktokGrowth.Data.Datasets.Add(datasetLineChart);

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
                            LabelString = L["TiktokTotalView"],
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
            await _tiktokGrowth.Update();
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configTiktokGrowth.CanvasId,
                false,
                true,
                true
            );
            }
        }

        #region SECTION 2 - POST RELATED CHARTS

        private async Task RenderChart_AvgPostsChart()
        {
            _configAvgPostChart.Data.Labels.Clear();
            _configAvgPostChart.Data.Datasets.Clear();

            var datasets = new List<IDataset<int>>();
            var indexLine = 0;
            foreach (var g in Enumerable.GroupBy<DataChartItemDto<int, PostContentType>, PostContentType>(_statistics.DataChartCountPostsByType, p => p.Type))
            {
                IDataset<int> datasetAvgPost = new BarDataset<int>(g.Select(_ => _.Value).ToList())
                {
                    Label = g.Key.ToString(),
                    BackgroundColor = avgPostsChartColors[GetIndexColor(indexLine, borderColors.Count)],
                    BorderWidth = 1
                };
                datasets.Add(datasetAvgPost);
                indexLine++;
            }

            var labels = Enumerable.Select<DataChartItemDto<int, PostContentType>, string>(_statistics.DataChartCountPostsByType, p => p.Display).Distinct().ToArray();

            IListExtensions.AddRange(_configAvgPostChart.Data.Labels, labels);

            IListExtensions.AddRange<IDataset>(_configAvgPostChart.Data.Datasets, datasets);

            await _chart_AvgPostStats.Update();
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configAvgPostChart.CanvasId,
                false,
                false,
                true
            );
        }

        private async Task RenderChart_PostsByDateChart()
        {
            var datasets = new List<LineChartDataset<int>>();
            var indexLine = 0;
            foreach (var g in _statistics.DataChartCountPosts.GroupBy(p => p.Type))
            {
                datasets.Add(GetLineDataset(g.Select(_ => _.Value).ToList(), indexLine, g.Key));
                indexLine++;
            }

            datasets = datasets.OrderBy(d => d.Label).ToList();

            var labels = _statistics.DataChartCountPosts.Select(p => p.Display).Distinct().ToArray();

            await _chart_totalPostsByDate.Clear();
            await _chart_totalPostsByDate.AddLabelsDatasetsAndUpdate(labels, datasets.ToArray());
        }

        #endregion

        private async Task RenderChart_AvgCampaignPostsChart()
        {
            _configAvgCampaignPostChart.Data.Labels.Clear();
            _configAvgCampaignPostChart.Data.Datasets.Clear();

            var datasets = new List<IDataset<int>>();
            var indexLine = 0;
            foreach (var g in Enumerable.GroupBy<DataChartItemDto<int, CampaignType>, CampaignType>(_statistics.DataChartCountPostsByCampaignType, p => p.Type))
            {
                IDataset<int> datasetAvgPost = new BarDataset<int>(g.Select(_ => _.Value).ToList())
                {
                    Label = g.Key.ToString(),
                    BackgroundColor = backgroundColors[GetIndexColor(indexLine, backgroundColors.Count)],
                    BorderWidth = 1
                };
                datasets.Add(datasetAvgPost);
                indexLine++;
            }

            var labels = Enumerable.Select<DataChartItemDto<int, CampaignType>, string>(_statistics.DataChartCountPostsByCampaignType, p => p.Display).Distinct().ToArray();

            IListExtensions.AddRange(_configAvgCampaignPostChart.Data.Labels, labels);

            IListExtensions.AddRange<IDataset>(_configAvgCampaignPostChart.Data.Datasets, datasets);

            await _chart_AvgCampaignPostStats.Update();
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configAvgCampaignPostChart.CanvasId,
                false,
                false,
                true
            );
        }

        private async Task RenderChart_AffiliateSummaryChart(AffiliateSummaryType value)
        {
            _affiliateSummaryType = value;

            if (StartDate.HasValue && EndDate.HasValue)
            {
                var filter = Filter.Clone();
                (filter.FromDateTime, filter.ToDateTime) = GetDateTimeForApi(StartDate, EndDate);

                ApiResponse = await AffiliateStatsAppService.GetDailyAffSummary(filter);

                _configAffiliateChart.Data.Labels.Clear();
                _configAffiliateChart.Data.Datasets.Clear();
                var barDataset = new List<IDataset<decimal>>();
                IDataset<decimal> dataset;
                var divideValue = 1;
                string axisYLabel = string.Empty;
                switch (_affiliateSummaryType)
                {
                    case AffiliateSummaryType.Commission:
                    case AffiliateSummaryType.Amount:
                    case AffiliateSummaryType.Conversion:
                        axisYLabel = L["Million"];
                        divideValue = 1000000;
                        switch (_affiliateSummaryType)
                        {
                            case AffiliateSummaryType.Amount:
                                dataset = new BarDataset<decimal>(Enumerable.Select<AffDailySummaryItem, decimal>(ApiResponse.Items, x => x.Amount / divideValue))
                                {
                                    Label = L["Amount"],
                                    BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(100, ChartJsUtils.ChartColors.Green)),
                                    BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Green),
                                    BorderWidth = 1
                                };
                                barDataset.Add(dataset);

                                break;
                            case AffiliateSummaryType.Commission:
                                var commistionDataset = new BarDataset<decimal>(Enumerable.Select<AffDailySummaryItem, decimal>(ApiResponse.Items, x => Math.Round((decimal)(x.Commission / divideValue), 2, MidpointRounding.ToEven)))
                                {
                                    Label = L["Commission"],
                                    BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Pink)),
                                    BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Pink),
                                    BorderWidth = 1
                                };
                                var commistionBonusDataSet = new BarDataset<decimal>(Enumerable.Select<AffDailySummaryItem, decimal>(ApiResponse.Items, x => Math.Round((decimal)(x.CommissionBonus / divideValue), 2, MidpointRounding.ToEven)))
                                {
                                    Label = L["CommissionBonus"],
                                    BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(50, ChartJsUtils.ChartColors.Green)),
                                    BorderColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(80, ChartJsUtils.ChartColors.Green)),
                                    BorderWidth = 1
                                };
                                barDataset.Add(commistionDataset);
                                barDataset.Add(commistionBonusDataSet);

                                break;
                            case AffiliateSummaryType.Conversion:

                                axisYLabel = L["ConversionCount"];
                                dataset = new BarDataset<decimal>(Enumerable.Select<AffDailySummaryItem, decimal>(ApiResponse.Items, x => (decimal) x.Conversion))
                                {
                                    Label = L["Aff.Summary"],
                                    BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(100, ChartJsUtils.ChartColors.Yellow)),
                                    BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Yellow),
                                    BorderWidth = 1
                                };
                                barDataset.Add(dataset);

                                break;
                        }

                        foreach (var dataChart in barDataset) { _configAffiliateChart.Data.Datasets.Add(dataChart); }

                        break;
                    case AffiliateSummaryType.Click:
                        axisYLabel = L["Click"];
                        var barDatasetClick = new BarDataset<int>(Enumerable.Select<AffDailySummaryItem, int>(ApiResponse.Items, x => x.Click))
                        {
                            Label = L["Aff.Summary"],
                            BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(100, ChartJsUtils.ChartColors.Blue)),
                            BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Blue),
                            BorderWidth = 1
                        };

                        _configAffiliateChart.Data.Datasets.Add(barDatasetClick);
                        break;
                }

                IListExtensions.AddRange(_configAffiliateChart.Data.Labels, Enumerable.Select<AffDailySummaryItem, string>(ApiResponse.Items, x => x.Display));
                _configAffiliateChart.Options.Scales.XAxes = new List<CartesianAxis>
                {
                    new BarCategoryAxis
                    {
                        Stacked = true,
                    }
                };

                _configAffiliateChart.Options.Scales.YAxes = new List<CartesianAxis>
                {
                    new BarLinearCartesianAxis
                    {
                        Stacked = true,
                        ScaleLabel = new ScaleLabel
                        {
                            Display = true,
                            LabelString = axisYLabel,
                        },
                        Ticks = new LinearCartesianTicks {BeginAtZero = true}
                    },
                };
                await _chart_AffiliateSummary.Update();
                await JsRuntime.InvokeVoidAsync
                (
                    "generalInterop.datalabelsConfig",
                    _configAffiliateChart.CanvasId,
                    false,
                    true,
                    true
                );
            }
            else { await Message.Warn(L["SelectDateTimeRange"]); }

            await InvokeAsync(StateHasChanged);
        }

        #endregion

        LineChartDataset<int> GetLineDataset(List<int> postsData, int number, string teamName)
        {
            return new()
            {
                Label = teamName,
                Data = postsData,
                BackgroundColor = backgroundColors[GetIndexColor(number, backgroundColors.Count)],
                BorderColor = borderColors[GetIndexColor(number, borderColors.Count)],
                Fill = false,
                CubicInterpolationMode = "monotone",
            };
        }
        
        private int GetIndexColor(int index, int colorCount)
        {
            return index % colorCount;
        }
    }
}    