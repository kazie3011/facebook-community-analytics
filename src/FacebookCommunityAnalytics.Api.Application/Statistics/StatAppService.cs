using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    [Authorize]
    public class StatAppService : ApplicationService, IStatAppService
    {
        private readonly IStatDomainService _statDomainService;
        private readonly IApiConfigurationDomainService _apiConfigurationDomainService;
        private readonly IPartnerModuleAppService _partnerModuleAppService;
        public StatAppService(IStatDomainService statDomainService, IApiConfigurationDomainService apiConfigurationDomainService,IPartnerModuleAppService partnerModuleAppService)
        {
            _statDomainService = statDomainService;
            _apiConfigurationDomainService = apiConfigurationDomainService;
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task<GeneralStatsResponse> GetGeneralStats(GeneralStatsApiRequest apiRequest)
        {
            return await _statDomainService.GetGeneralStats(apiRequest);
        }

        public async  Task<GrowthChartDto> GetGrowthChartStats(GeneralStatsApiRequest input)
        {
            return await _statDomainService.GetGrowthChartStats(input);
        }

        public async Task<TiktokGrowthChartDto> GetTiktokChartStats(GeneralStatsApiRequest input)
        {
            return await _statDomainService.GetTiktokChartStats(input);
        }

        public async Task<MultipleDataSourceChart<double>> GetMCNGDLTikTokChartStats(GeneralStatsApiRequest input)
        {
            return await _statDomainService.GetMCNGDLTikTokChartStats(input);
        }

        public async Task<MultipleDataSourceChart<long>> GetMCNVietnamTikTokChartStats(GeneralStatsApiRequest input)
        {
            return await _statDomainService.GetMCNVietNamTikTokChartStats(input);
        }
    }
}