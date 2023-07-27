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
using ChartJs.Blazor.Util;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.DashBoards;
using FacebookCommunityAnalytics.Api.Utils;
using Microsoft.JSInterop;
using ChartJsColor = System.Drawing.Color;

namespace FacebookCommunityAnalytics.Api.Blazor.Shared.Components.Dashboards
{
    public partial class TeamContent : BlazorComponentBase
    {
        private BarConfig _configContentGrowth = new();
        private Chart _contentGrowth = new();
        private bool _showLoading;
        
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }
        
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        
        private readonly string _primaryColor = "#037404";

        private IReadOnlyList<ContractWithNavigationPropertiesDto> Contracts = new List<ContractWithNavigationPropertiesDto>();
        
        private decimal Total { get; set; }
        private decimal PartialPayment { get; set; }
        private decimal RemainPayment { get; set; }

        private async Task SetupCompletedCallback()
        {
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configContentGrowth.CanvasId,
                false,
                false,
                true
            );
        }
        
        private async Task InitDateTimeRangePicker()
        {
            var (startDate, endDate) = await GetDefaultDate();
            StartDate = startDate.AddDays(-7);
            EndDate = endDate;
            _dateRanges = await GetDateRangePicker();
        }

        protected override async  Task OnInitializedAsync()
        {
            InitSaleGrowthChart();
            await InitDateTimeRangePicker();
            await RenderCharts();
        }
        
        private void InitSaleGrowthChart()
        {
            _configContentGrowth = new BarConfig()
            {
                Options = new BarOptions()
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["SaleChart.ChartTitle"].ToString(),
                        FontColor = _primaryColor,
                        FontSize = 20
                        //Text = "Community Growth Stats"
                    },
                    Tooltips = new Tooltips
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
        
        private async Task RenderCharts()
        {
            _showLoading = true;
            await RenderChart_SaleChart();
            _showLoading = false;
        }

        private async Task RenderChart_SaleChart()
        {
            var request = new SaleChartApiRequest()
            {
                SalePersonId = CurrentUser.Id
            };
            
            var saleCharts = await DashboardAppService.GetSaleChart(request);

            Contracts = saleCharts.Contracts;
            Total = Contracts.Sum(_ => _.Contract.TotalValue);
            PartialPayment = Contracts.Sum(_ => _.Contract.PartialPaymentValue);
            RemainPayment = Contracts.Sum(_ => _.Contract.RemainingPaymentValue);
            _configContentGrowth.Data.Labels.Clear();
            _configContentGrowth.Data.Datasets.Clear();

            var dateRanges = StartDate.Value.Date.To(EndDate.Value.Date).ToList();

            IDataset<decimal?> totalLineChart = new LineDataset<decimal?>(saleCharts.TotalBarCharts.Select(x => x.Value / 1000000))
            {
                Label = L["TotalChart"],
                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Blue)),
                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Blue),
                BorderWidth = 2,
                LineTension = 0,
                BorderJoinStyle = BorderJoinStyle.Round,
                PointRadius = 1,
                PointHoverRadius = 2,
                SpanGaps = false,
                Fill = FillingMode.Disabled,
                YAxisId = "TotalChart"
            };
            
            var maxGrowthNumber = saleCharts.TotalBarCharts.Max(x => x.Value / 1000000);
            IDataset<decimal?> remainingPaymentLineChart = new LineDataset<decimal?>(saleCharts.RemainingPaymentBarCharts.Select(x => x.Value / 1000000))
            {
                Label = L["RemainingPayment"],
                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(100, ChartJsUtils.ChartColors.Red)),
                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Red),
                BorderWidth = 1,
                LineTension = 0,
                BorderJoinStyle = BorderJoinStyle.Round,
                PointRadius = 1,
                PointHoverRadius = 2,
                SpanGaps = false,
                Fill = FillingMode.Disabled,
                YAxisId = "TotalChart"
            };
            
            IDataset<decimal?> partialPaymentLineChart = new LineDataset<decimal?>(saleCharts.PartialPaymentBarCharts.Select(x => x.Value / 1000000))
            {
                Label = L["PartialPayment"],
                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(100, ChartJsUtils.ChartColors.Green)),
                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Green),
                BorderWidth = 1,
                LineTension = 0,
                BorderJoinStyle = BorderJoinStyle.Round,
                PointRadius = 1,
                PointHoverRadius = 2,
                SpanGaps = false,
                Fill = FillingMode.Disabled,
                YAxisId = "TotalChart"
            };
            

            _configContentGrowth.Data.Labels.AddRange(dateRanges.Select(x => x.ToString("dd-MM")));

            _configContentGrowth.Data.Datasets.Add(totalLineChart);
            _configContentGrowth.Data.Datasets.Add(partialPaymentLineChart);
            _configContentGrowth.Data.Datasets.Add(remainingPaymentLineChart);

            _configContentGrowth.Options.Scales = new BarScales
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
                            LabelString = L["Currency"],
                        },
                        ID = "TotalChart",
                        Ticks = new LinearCartesianTicks()
                        {
                            Max = (double)maxGrowthNumber,
                            BeginAtZero = true,
                        },
                        Position = Position.Left,
                        GridLines = new GridLines() {DrawOnChartArea = false}
                    }
                }
            };
            await _contentGrowth.Update();
        }
        
        private void ViewDetailsAsync(ContractDto input)
        {
            NavigationManager.NavigateTo($"/contracts/contract-details/{input.Id.ToString()}");
        }
    }
}