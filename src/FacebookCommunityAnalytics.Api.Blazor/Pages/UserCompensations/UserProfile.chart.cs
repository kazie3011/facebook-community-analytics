using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Statistics;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.UserCompensations
{
    public partial class UserProfile
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
        private BarConfig _config_PostChart = new();
        private BarConfig _config_SaleChart = new();
        private BarConfig _config_TikTokChart = new();
        private Chart _chart_PostStats;
        private Chart _chart_SaleStats;
        private Chart _chart_TikTokStats;
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

        private void InitPostChart()
        {
            _config_PostChart = new BarConfig()
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.PostsChart"].ToString(),
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
        
        private void InitSaleChart()
        {
            _config_SaleChart = new BarConfig()
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.SaleChart"].ToString(),
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
        
        private void InitTikTokChart()
        {
            _config_TikTokChart = new BarConfig()
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.TikTokChart"].ToString(),
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
        
        private async Task RenderChart_PostChart()
        {
            _config_PostChart.Data.Labels.Clear();
            _config_PostChart.Data.Datasets.Clear();

            var datasets = new List<IDataset<int>>();
            var indexLine = 0;
            foreach (var g in _chartStat.CountPostsByTypeChartData.GroupBy(p => p.Type))
            {
                IDataset<int> datasetPost = new BarDataset<int>(g.Select(_ => _.Value).ToList())
                {
                    Label = g.Key.ToString(),
                    BackgroundColor = avgPostsChartColors[GetIndexColor(indexLine, avgPostsChartColors.Count)],
                    BorderWidth = 1
                };
                datasets.Add(datasetPost);
                indexLine++;
            }

            var labels = _chartStat.CountPostsByTypeChartData.Select(p => p.Display).Distinct().ToArray();

            _config_PostChart.Data.Labels.AddRange(labels);

            _config_PostChart.Data.Datasets.AddRange(datasets);

            await _chart_PostStats.Update();
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _config_PostChart.CanvasId,
                false,
                false,
                true
            );
        }
        
        private async Task RenderChart_SaleChart()
        {
            _config_SaleChart.Data.Labels.Clear();
            _config_SaleChart.Data.Datasets.Clear();
            
            var data = _chartStat.SaleChartData.Select(_ => _.Value).ToList();
            IDataset<decimal> datasetSale = new BarDataset<decimal>(data)
                {
                    Label = L["Payment"],
                    BackgroundColor = Teal,
                    BorderWidth = 1
                };
            var labels = _chartStat.SaleChartData.Select(p => p.Display).Distinct().ToArray();

            _config_SaleChart.Data.Labels.AddRange(labels);

            _config_SaleChart.Data.Datasets.Add(datasetSale);

            await _chart_SaleStats.Update();
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _config_SaleChart.CanvasId,
                false,
                false,
                true
            );
        }
        
        private async Task RenderChart_TikTokChart()
        {
            _config_TikTokChart.Data.Labels.Clear();
            _config_TikTokChart.Data.Datasets.Clear();

            var data = _chartStat.TikTokChartData.Select(_ => _.Value).ToList();
            var maxGrowthNumber = data.Max();
            IDataset<int> datasetTikTok = new BarDataset<int>(data)
            {
                Label = L["TikTokVideos"],
                BackgroundColor = Teal,
                BorderWidth = 1
            };
            var labels = _chartStat.TikTokChartData.Select(p => p.Display).Distinct().ToArray();

            _config_TikTokChart.Data.Labels.AddRange(labels);

            _config_TikTokChart.Data.Datasets.Add(datasetTikTok);

            await _chart_TikTokStats.Update();
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _config_TikTokChart.CanvasId,
                false,
                false,
                true
            );
        }
        
        private int GetIndexColor(int index, int colorCount)
        {
            return index % colorCount;
        }
        
        private void InitChart()
        {
            InitPostChart();
            InitSaleChart();
            InitTikTokChart();
        }
    }
}