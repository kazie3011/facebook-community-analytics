using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiktok
{
    public interface ITiktokAppService : IApplicationService
    {
        Task<SaveChannelStatApiResponse> SaveChannelStats(SaveChannelStatApiRequest apiRequest);
        Task<SaveChannelVideoResponse> SaveVideos(SaveChannelVideoRequest apiRequest);
        Task<SaveTiktokStatApiResponse> SaveTiktokStat(SaveTiktokStatApiRequest apiRequest);
        Task<GetTiktokHashTagsApiResponse> GetTiktokHashTags();
        Task UpdateTiktokVideosState(UpdateTiktokVideosStateRequest updateTiktokStateRequest);

        Task SaveMCNChannelVideoStats(CrawlMCNVideoInput apiRequest);
        Task<List<MCNVietNamChannelDto>> SaveMCNVietNamChannel(MCNVietNamChannelApiRequest request);
        Task<List<TrendingDetailDto>> SaveTrendingDetails(TrendingDetailApiRequest request);
    }
}