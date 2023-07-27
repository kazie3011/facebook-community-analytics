using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    public interface ITikTokMCNGdlAppService : IApplicationService
    {
        Task<MultipleDataSourceChart<int>> GetMonthlyChannelInOutChart();
        Task<MultipleDataSourceChart<int>> GetWeeklyChannelInOutChart();
        Task<MultipleDataSourceChart<long>> GetViewAndCreatorChart();
        
        Task<PieChartDataSource<double>> GetChannelContractStatusChart();
        Task<PieChartDataSource<double>> GetChannelCategoriesChart(List<GroupCategoryType> categoryTypes);
        Task<List<TiktokDto>> GetTopMCNGDLVideos(GetTikTokVideosRequest request);
        Task<List<MCNVietNamChannelDto>> GetTopMCNVietNamChannel(DateTime fromDate, DateTime toDate, int count);
    }
}