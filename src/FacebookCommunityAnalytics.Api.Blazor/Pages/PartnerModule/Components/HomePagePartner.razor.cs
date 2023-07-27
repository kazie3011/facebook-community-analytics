using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.PieChart;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.PartnerModule;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule.Components
{
    public partial class HomePagePartner
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; set; } = new();
        [Inject] public IJSRuntime JSRuntime { get; set; }
        private bool _unRendered = true;
        private bool _showLoading = true;

        private BarConfig _chartConfig = new();
        private Guid? CurrentPartnerId { get; set; }
        private  IEnumerable<Guid?> multipleCampaignIds { get; set; }

        private PieConfig _piePartnerCampaignChartConfig = new();
        private PieConfig _piePartnerPostsContentTypeChartConfig = new();
        private List<string> _campaignChartCustomLabels = new List<string>();
        private List<string> _postsContentTypeChartCustomLabels = new List<string>();
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; } = DateTimeOffset.Now.Date.AddMonths(-1);
        private DateTimeOffset? EndDate { get; set; } = DateTimeOffset.Now.Date.AddDays(1).AddTicks(-1);
        public HomePagePartner()
        {
            multipleCampaignIds = new List<Guid?>();
        }
        protected override async Task OnInitializedAsync()
        {
            //InitGrowthChart();
            InitPieChart();
            await SetBreadcrumbItemsAsync();
        }
        
        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            return ValueTask.CompletedTask;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                BrowserDateTime = await GetBrowserDateTime();
                await ReloadChartData();
                
                await InvokeAsync(StateHasChanged);
                await JSRuntime.InvokeVoidAsync("HiddenMenuOnMobile");
            }
            _unRendered = false;
        }

        private async Task ReloadChartData()
        {
            await LoadPartnerCampaignsChartDataAsync();
            await LoadPartnerPostsContentTypeChartData();
        }
        
        private void InitPieChart()
        {
            _piePartnerCampaignChartConfig = new PieConfig(useDoughnutType:true)
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Partner.CampaignChart"].Value
                    },
                    Tooltips = new ChartJs.Blazor.Common.Tooltips
                    {
                        Mode = InteractionMode.Dataset,
                        Intersect = true
                    },
                    // Hover = new Hover
                    // {
                    //     Mode = InteractionMode.Dataset,
                    //     Intersect = false
                    // },
                    Events = new []{ BrowserEvent.MouseMove, BrowserEvent.TouchStart, BrowserEvent.TouchMove, BrowserEvent.Click, }
                }
            };
            
            _piePartnerPostsContentTypeChartConfig = new PieConfig(useDoughnutType:true)
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Partner.PostsContentTypeChart"].Value
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Dataset,
                        Intersect = true
                    },
                    // Hover = new Hover
                    // {
                    //     Mode = InteractionMode.Dataset,
                    //     Intersect = false
                    // },
                    Events = new []{ BrowserEvent.MouseMove, BrowserEvent.TouchStart, BrowserEvent.TouchMove, BrowserEvent.Click, }
                }
            };
        }

        private void InitGrowthChart()
        {
            _chartConfig = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.GrowthCampaignChart"].ToString(),
                        FontColor = ChartColorHelper.PrimaryColor,
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
                                ScaleLabel = new ScaleLabel { LabelString = "Date" },
                                Stacked = true
                            }
                        },
                    }
                }
            };
        }

        private async Task LoadPartnerCampaignsChartDataAsync()
        {
            _piePartnerCampaignChartConfig.Data.Labels.Clear();
            _piePartnerCampaignChartConfig.Data.Datasets.Clear();

            var chartData = await _partnerModuleAppService.GetPartnerCampaignsChart();
            _piePartnerCampaignChartConfig.Data.Labels.AddRange(chartData.PartnerCampaignChartItems.Select(x=>x.Label).ToList());
            
            foreach (var item in chartData.PartnerCampaignChartItems)
            {
                _campaignChartCustomLabels.Add($"{item.Label}:{item.TotalCampaign}");
            }
            
            var dataItems = chartData.PartnerCampaignChartItems.Select(x => x.TotalCampaign).ToList();
            var dataSet = new PieDataset<int>(dataItems,useDoughnutDefaults: true);
            
            var colors = new List<string>();
            for (int i = 0; i < chartData.PartnerCampaignChartItems.Count; i++)
            {
                colors.Add(ChartColorHelper.GetColor(i));
            }
            dataSet.BackgroundColor = colors.ToArray();
            _piePartnerCampaignChartConfig.Data.Datasets.Add(dataSet);
            
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _piePartnerCampaignChartConfig.CanvasId,
                false,
                false,
                false,
                _campaignChartCustomLabels
            );
        }
        
        private async Task LoadPartnerPostsContentTypeChartData()
        {
            _piePartnerPostsContentTypeChartConfig.Data.Labels.Clear();
            _piePartnerPostsContentTypeChartConfig.Data.Datasets.Clear();

            if (StartDate.HasValue && EndDate.HasValue)
            {
                var chartData = await _partnerModuleAppService.GetPartnerPostContentTypeChart(StartDate.Value.DateTime,EndDate.Value.DateTime);
                _piePartnerPostsContentTypeChartConfig.Data.Labels.AddRange(chartData.PartnerPostTypeChartItems.Select(x=>x.Label).ToList());
                foreach (var item in chartData.PartnerPostTypeChartItems)
                {
                    _postsContentTypeChartCustomLabels.Add($"{item.Label}:{item.TotalPost}");
                }
                
                var dataItems = chartData.PartnerPostTypeChartItems.Select(x => x.TotalPost).ToList();
                
                var dataSet = new PieDataset<int>(dataItems,useDoughnutDefaults: true);
            
                var colors = new List<string>();
                for (var i = 0; i < chartData.PartnerPostTypeChartItems.Count; i++)
                {
                    colors.Add(ChartColorHelper.GetColor(i));
                }
                dataSet.BackgroundColor = colors.ToArray();
                _piePartnerPostsContentTypeChartConfig.Data.Datasets.Add(dataSet);
            }
            
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _piePartnerPostsContentTypeChartConfig.CanvasId,
                false,
                false,
                false,
                _postsContentTypeChartCustomLabels
            );
        }
        
        // private async Task RenderGrowthCampaignChartAsync()
        // {
        //     _chartConfig.Data.Labels.Clear();
        //     _chartConfig.Data.Datasets.Clear();
        //
        //     var datasets = new List<IDataset<int>>();
        //     var indexLine = 1;
        //     foreach (var g in _statistics.TotalInteractionsLineCharts.GroupBy(x => x.Type))
        //     {
        //         datasets.Add(GetLineDataset(g.Select(_ => _.Value).ToList(), indexLine, g.Key));
        //         indexLine++;
        //     }
        //
        //     _chartConfig.Data.Labels.AddRange(_statistics.ChartLabels);
        //
        //     _chartConfig.Data.Datasets.AddRange(datasets);
        //
        //     _chartConfig.Options.Scales = new BarScales
        //     {
        //         XAxes = new List<CartesianAxis>
        //         {
        //             new BarCategoryAxis()
        //             {
        //                 ScaleLabel = new ScaleLabel { LabelString = "Date" },
        //                 Stacked = true
        //             }
        //         },
        //         YAxes = new List<CartesianAxis>
        //         {
        //             new LinearCartesianAxis
        //             {
        //                 Display = AxisDisplay.True,
        //                 ScaleLabel = new ScaleLabel
        //                 {
        //                     Display = true,
        //                     LabelString = L["Chart.GrowthReaction"],
        //                 },
        //                 ID = "GrowthReaction",
        //                 Ticks = new LinearCartesianTicks()
        //                 {
        //                     BeginAtZero = true,
        //                 },
        //                 Position = Position.Left,
        //                 GridLines = new GridLines() { DrawOnChartArea = false }
        //             }
        //         },
        //     };
        // }

        private IDataset<int> GetLineDataset(List<int> postsData, int number, string teamName)
        {
            return new LineDataset<int>(postsData)
            {
                Label = teamName,
                BackgroundColor = ChartColorHelper.GetColor(number),
                BorderColor = ChartColorHelper.GetBorderColor(number),
                Fill = false,
                CubicInterpolationMode = CubicInterpolationMode.Monotone,
            };
        }

        private int GetIndexColor(int index, int colorCount)
        {
            return index + 1 % colorCount;
        }
    }
}