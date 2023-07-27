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
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.DashBoards;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ChartJsColor = System.Drawing.Color;

namespace FacebookCommunityAnalytics.Api.Blazor.Shared.Components.Dashboards
{
    public partial class TeamSale : BlazorComponentBase
    {
        private BarConfig _configSaleGrowth { get; set; }
        private Chart _saleGrowth { get; set; }
        private bool _showLoading;
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        
        private readonly string _primaryColor = "#037404";

        private IReadOnlyList<ContractWithNavigationPropertiesDto> Contracts { get; set; }
        
        private decimal Total { get; set; }
        private decimal PartialPayment { get; set; }
        private decimal RemainPayment { get; set; }
        private List<DashboardUserDto> SalePersons { get; set; }

        private SaleChartApiRequest Request { get; set; }

        public TeamSale()
        {
            _configSaleGrowth = new BarConfig();
            _saleGrowth = new Chart();
            Contracts = new List<ContractWithNavigationPropertiesDto>();
            SalePersons = new List<DashboardUserDto>();
            Request = new SaleChartApiRequest();
        }
        private async Task SetupCompletedCallback()
        {
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configSaleGrowth.CanvasId,
                false,
                false,
                true
            );
        }
        
        protected override async  Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            InitSaleGrowthChart();
            await InitDateTimeRangePicker();
            await InitUsers();
        }

        // SetupCompletedCallback run before OnAfterRenderAsync
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var (startDate, endDate) = await GetDefaultDate();
                StartDate = startDate.AddDays(-7);
                EndDate = endDate;
                await RenderCharts();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task InitDateTimeRangePicker()
        {
            _dateRanges = await GetDateRangePicker();
        }
        
        private void InitSaleGrowthChart()
        {
            _configSaleGrowth = new BarConfig()
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
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                return;
            }

            var request = Request.Clone();
            (request.FromDateTime, request.ToDateTime) = GetDateTimeForApi(StartDate, EndDate);
            var saleCharts = await DashboardAppService.GetSaleChart(request);

            Contracts = saleCharts.Contracts;
            Total = Contracts.Sum(_ => _.Contract.TotalValue);
            PartialPayment = Contracts.Sum(_ => _.Contract.PartialPaymentValue);
            RemainPayment = Contracts.Sum(_ => _.Contract.RemainingPaymentValue);
            _configSaleGrowth.Data.Labels.Clear();
            _configSaleGrowth.Data.Datasets.Clear();

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
            

            _configSaleGrowth.Data.Labels.AddRange(dateRanges.Select(x => x.ToString("dd-MM")));

            _configSaleGrowth.Data.Datasets.Add(totalLineChart);
            _configSaleGrowth.Data.Datasets.Add(partialPaymentLineChart);
            _configSaleGrowth.Data.Datasets.Add(remainingPaymentLineChart);

            _configSaleGrowth.Options.Scales = new BarScales
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
                            Max = maxGrowthNumber != null ? (double)maxGrowthNumber: 1,
                            BeginAtZero = true,
                        },
                        Position = Position.Left,
                        GridLines = new GridLines() {DrawOnChartArea = false}
                    }
                }
            };
            await _saleGrowth.Update();
        }

        private async Task InitUsers()
        {
            if (IsLeaderRole())
            {
                SalePersons = await DashboardAppService.GetSalePerson(); 
            }
            else
            {
                Request.SalePersonId = CurrentUser.Id;
            }
            
        }
        
        private void ViewDetailsAsync(ContractDto input)
        {
            NavigationManager.NavigateTo($"/contracts/contract-details/{input.Id.ToString()}");
        }
    }
}