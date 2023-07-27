using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public interface IStatAppService : IApplicationService
    {
        Task<GeneralStatsResponse> GetGeneralStats(GeneralStatsApiRequest apiRequest);
        Task<GrowthChartDto> GetGrowthChartStats(GeneralStatsApiRequest input);
        Task<TiktokGrowthChartDto> GetTiktokChartStats(GeneralStatsApiRequest input);

        Task<MultipleDataSourceChart<double>> GetMCNGDLTikTokChartStats(GeneralStatsApiRequest input);
        Task<MultipleDataSourceChart<long>> GetMCNVietnamTikTokChartStats(GeneralStatsApiRequest input);
    }
}