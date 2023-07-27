using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public interface ITiktokStatsAppService : IApplicationService
    {
        Task<PagedResultDto<TiktokWithNavigationPropertiesDto>> GetListAsync(GetTiktoksInputExtend input);
        Task<List<TiktokExportRow>> GetExportRows(GetTiktoksInputExtend input);
        Task<List<TiktokWeeklyTotalFollowers>> GetWeeklyTotalFollowersReport(GetTiktokWeeklyTotalFollowersRequest request);
        Task<List<TiktokWeeklyTotalViews>> GetWeeklyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null);
        Task<List<TikTokMonthlyTotalFollowers>> GetMonthlyTotalFollowersReport(GetTiktokMonthlyTotalFollowersRequest request);
        Task<List<TikTokMonthlyTotalViews>> GetMonthlyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null);
        Task<List<TikTokMCNReport>> GetTiktokMCNMonthlyReport(DateTime? timeFrom = null, DateTime? timeTo = null, TikTokMCNType? tikTokMcnType = null);
        Task<List<TikTokMCNReport>> GetTiktokMCNWeeklyReports(DateTime? timeFrom = null, DateTime? timeTo = null);
        Task<List<TikTokMCNDto>> GetListTikTokMCN(TikTokMCNType tikTokMcnType);
        Task<FileResultDto> GetVideoImage(Guid videoId);
        Task<TiktokDto> GetTopVideoOfDay(DateTime startDate, DateTime endDate);
        Task<TopChannelDto> GetTopChannel(DateTime startDate, DateTime endDate);
        Task<List<TopChannelDto>> GetTopStatChannelsOfWeek();
        Task<List<TopChannelDto>> GetTopStatChannelOfMonth();
        Task<List<GroupDto>> GetGDLChannels();
        Task<List<TrendingDetailDto>> GetTrendingDetails(DateTime fromDate, DateTime toDate, int count);
        Task<List<TrendingDetailDto>> GetTopTrendingContentsByDateRange(DateTime? from, DateTime? to ,int contentNumber,bool calculateGrowth);

    }
}