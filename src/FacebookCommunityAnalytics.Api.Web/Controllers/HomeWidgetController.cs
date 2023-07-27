using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Controllers
{
    [Route("HomeWidgets")]
    public class GrowthChartController : AbpController
    {
        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public GrowthChartController(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        [HttpGet]
        [Route("GrowthChart")]
        public IActionResult GrowthChart()
        {
            return ViewComponent("GrowthChart");
        }
        
        [HttpGet]
        [Route("GrowthChartData")]
        public async Task<IActionResult> GrowthChartData(GetGrowthCampaignChartsInput input)
        {
            if (input.PartnerId.HasValue)
            {
                var data = await _partnerModuleAppService.GetGrowthCampaignChartsAsync(input);
                return Json(GenerateGrowthChart(data).SerializeBody());
            }
            else
            {
                return Json(GenerateGrowthChart(new GrowthCampaignChartDto()).SerializeBody()); 
            }
        }

        private static Chart GenerateGrowthChart(GrowthCampaignChartDto model)
        {
            var homeChart = new Chart
            {
                Type = Enums.ChartType.Line
            };

            var data = new ChartJSCore.Models.Data
            {
                Labels = model.ChartLabels
            };
            
            var datasets = new List<Dataset>();
            var indexLine = 1;
            foreach (var g in model.TotalInteractionsLineCharts.GroupBy(x => x.Type))
            {
                datasets.Add(ChartJsHelper.GetLineDataset(g.Select(_ => (double?)_.Value).ToList(), indexLine, g.Key));
                indexLine++;
            }
            
            data.Datasets = new List<Dataset>();
            data.Datasets.AddRange(datasets);

            homeChart.Data = data;
            homeChart.Options.Scales = new Scales
            {
                YAxes = new List<Scale>
                {
                    new Linear()
                    {
                        Display = true,
                        Ticks = new CartesianLinearTick()
                        {
                            BeginAtZero = true,
                        },
                        GridLines = new GridLine() { DrawOnChartArea = false }
                    }
                }
            };
            return homeChart;
        }
        
       
        [HttpGet]
        [Route("Statistics")]
        public IActionResult Statistics(GetGrowthCampaignChartsInput input)
        {
            return ViewComponent("HomeStatistics", input);
        }
    }
}