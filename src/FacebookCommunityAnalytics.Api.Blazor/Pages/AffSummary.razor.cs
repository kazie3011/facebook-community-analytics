using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.Util;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Utils;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Legend = ChartJs.Blazor.Common.Legend;
using ChartJsColor = System.Drawing.Color;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class AffSummary : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        private AffDailySummaryApiResponse ApiResponse { get; set; }
        private GeneralStatsApiRequest Filter { get; set; }
        private DateTimeOffset? StartDate;
        private DateTimeOffset? EndDate;
        private AffiliateSummaryType AffiliateSummaryType { get; set; }
        private Dictionary<string, DateRange> DateRanges { get; set; }

        private string _labelChart = string.Empty;
        private long _divideValue = 1;
        private BarConfig _config = new();
        private ChartJs.Blazor.Chart _chart = new();
        private bool _showChart;

        public AffSummary()
        {
            AffiliateSummaryType = AffiliateSummaryType.Commission;
            ApiResponse = new AffDailySummaryApiResponse();
            Filter = new GeneralStatsApiRequest();
        }

        protected override async Task OnInitializedAsync()
        {
            InitChart();
            BrowserDateTime = await GetBrowserDateTime();
            await SetBreadcrumbItemsAsync();
            DateRanges = await GetDateRangePicker();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                (StartDate, EndDate) = await GetDefaultDate();
                Filter.ClientOffsetInMinutes = await GetOffsetInMinutes();
                await GetData();
                await InitPage($"GDL - {L["Aff.Summary.PageTitle"].Value}");
            }
        }

        private async Task GetData()
        {
            _showChart = false;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var filter = Filter.Clone();

                (filter.FromDateTime, filter.ToDateTime) = GetDateTimeForApi(StartDate, EndDate);
                
                ApiResponse = await AffiliateStatsAppService.GetDailyAffSummary(filter);

                UpdateDataChart();
                _showChart = true;
            }
            else
            {
                await Message.Warn(L["SelectDateTimeRange"]);
            }

            await InvokeAsync(StateHasChanged);
        }

        private void UpdateTypeChart(AffiliateSummaryType value = AffiliateSummaryType.Commission)
        {
            AffiliateSummaryType = value;
        }

        private void UpdateDataChart()
        {
            _config.Data.Labels.Clear();
            _config.Data.Datasets.Clear();

            IDataset<decimal> barDataset = null;
            _divideValue = 1;
            switch (AffiliateSummaryType)
            {
                case AffiliateSummaryType.Commission:
                case AffiliateSummaryType.Amount:
                case AffiliateSummaryType.Conversion:
                    _labelChart = L["Million"];
                    _divideValue = 1000000;
                    switch (AffiliateSummaryType)
                    {
                        case AffiliateSummaryType.Amount:
                            barDataset = new BarDataset<decimal>(ApiResponse.Items.Select(x => x.Amount / _divideValue))
                            {
                                Label = L["Aff.Summary"],
                                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Green)),
                                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Green),
                                BorderWidth = 1
                            };
                            break;
                        case AffiliateSummaryType.Commission:
                            barDataset = new BarDataset<decimal>(ApiResponse.Items.Select(x => (x.Commission + x.CommissionBonus) / _divideValue))
                            {
                                Label = L["Aff.Summary"],
                                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Red)),
                                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Red),
                                BorderWidth = 1
                            };
                            break;
                        case AffiliateSummaryType.Conversion:

                            _labelChart = L["ConversionCount"];
                            barDataset = new BarDataset<decimal>(ApiResponse.Items.Select(x => (decimal) x.Conversion))
                            {
                                Label = L["Aff.Summary"],
                                BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Yellow)),
                                BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Yellow),
                                BorderWidth = 1
                            };

                            break;
                    }

                    _config.Data.Datasets.Add(barDataset);
                    break;
                case AffiliateSummaryType.Click:
                    _labelChart = L["Click"];
                    var barDatasetClick = new BarDataset<int>(ApiResponse.Items.Select(x => x.Click))
                    {
                        Label = L["Aff.Summary"],
                        BackgroundColor = ColorUtil.FromDrawingColor(ChartJsColor.FromArgb(128, ChartJsUtils.ChartColors.Blue)),
                        BorderColor = ColorUtil.FromDrawingColor(ChartJsUtils.ChartColors.Blue),
                        BorderWidth = 1
                    };

                    _config.Data.Datasets.Add(barDatasetClick);
                    break;
            }

            _config.Data.Labels.AddRange(ApiResponse.Items.Select(x => x.Display));
            _config.Options.Scales.YAxes = new List<CartesianAxis>
            {
                new BarLinearCartesianAxis
                {
                    Display = AxisDisplay.True,
                    ScaleLabel = new ScaleLabel
                    {
                        Display = true,
                        LabelString = _labelChart,
                    },
                    Ticks = new LinearCartesianTicks {BeginAtZero = true}
                },
            };
            _chart.Update();
        }

        private void InitChart()
        {
            _config = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Legend = new Legend
                    {
                        Position = Position.Top,
                        Display = false
                    },
                    Title = new OptionsTitle
                    {
                        Display = false,
                    },
                    Scales = new BarScales
                    {
                        YAxes = new List<CartesianAxis>
                        {
                            new BarLinearCartesianAxis
                            {
                                Display = AxisDisplay.True,
                                ScaleLabel = new ScaleLabel
                                {
                                    Display = true,
                                    LabelString = _labelChart,
                                },
                                Ticks = new LinearCartesianTicks {BeginAtZero = true}
                            },
                        }
                    }
                }
            };
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Aff.Summary.Title"], "/affsummary"));
            return ValueTask.CompletedTask;
        }

        private async Task SetupCompletedCallback() => await JsRuntime.InvokeVoidAsync
        (
            "generalInterop.datalabelsConfig",
            _config.CanvasId,
            false,
            false,
            true
        );

        private void StartDateChanged(DateTimeOffset? offset)
        {
            StartDate = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            EndDate = offset;
        }
    }
}