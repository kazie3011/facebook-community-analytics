
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Statistics;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Statistics
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Statistic")]
    [Route("api/app/statistics")]
    public class StatController : AbpController, IStatAppService
    {
        private readonly IStatAppService _statAppService;

        public StatController(IStatAppService statAppService)
        {
            _statAppService = statAppService;
        }

        [HttpGet]
        [Route("stat")]
        public Task<GeneralStatsResponse> GetGeneralStats(GeneralStatsApiRequest apiRequest)
        {
            return _statAppService.GetGeneralStats(apiRequest);
        }
        
        [HttpGet]
        [Route("growth-stats")]
        public  Task<GrowthChartDto> GetGrowthChartStats(GeneralStatsApiRequest input)
        {
            return _statAppService.GetGrowthChartStats(input);
        }

        [HttpGet]
        [Route("tiktok-stats")]
        public Task<TiktokGrowthChartDto> GetTiktokChartStats(GeneralStatsApiRequest input)
        {
            return _statAppService.GetTiktokChartStats(input);
        }
        
        [HttpGet]
        [Route("tiktok-mcn-gdl-tiktok-chart-stats")]
        public Task<MultipleDataSourceChart<double>> GetMCNGDLTikTokChartStats(GeneralStatsApiRequest input)
        {
            return _statAppService.GetMCNGDLTikTokChartStats(input);
        }

        [HttpPost]
        [Route("tiktok-mcn-vietnam-tiktok-chart-stats")]
        public Task<MultipleDataSourceChart<long>> GetMCNVietnamTikTokChartStats(GeneralStatsApiRequest input)
        {
            return _statAppService.GetMCNVietnamTikTokChartStats(input);
        }
    }
}