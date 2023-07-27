using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiktok
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Tiktok.Default)]
    public class TiktokAppService: ApiAppService, ITiktokAppService
    {
        private readonly ITiktokCrawlDomainService _tiktokCrawlDomainService;

        public TiktokAppService(ITiktokCrawlDomainService tiktokCrawlDomainService)
        {
            _tiktokCrawlDomainService = tiktokCrawlDomainService;
        }

        public async Task<SaveChannelStatApiResponse> SaveChannelStats(SaveChannelStatApiRequest apiRequest)
        {
            return await _tiktokCrawlDomainService.DoSaveChannelStats(apiRequest);
        }

        public async Task<SaveChannelVideoResponse> SaveVideos(SaveChannelVideoRequest apiRequest)
        {
            var result = await _tiktokCrawlDomainService.DoSaveVideos(apiRequest);
            Hangfire.BackgroundJob.Enqueue<ITiktokCrawlDomainService>(sv => sv.DoSaveThumbnailVideos(apiRequest));
            return result;
        }

        public async Task<SaveTiktokStatApiResponse> SaveTiktokStat(SaveTiktokStatApiRequest apiRequest)
        {
            return await _tiktokCrawlDomainService.DoSaveTiktokStat(apiRequest);
        }

        public Task SaveMCNChannelVideoStats(CrawlMCNVideoInput apiRequest)
        {
            Hangfire.BackgroundJob.Enqueue<ITiktokCrawlDomainService>(sv => sv.DoSaveMCNChannelVideoStats(apiRequest));
            return Task.CompletedTask;
        }
        
        public async Task<GetTiktokHashTagsApiResponse> GetTiktokHashTags()
        {
            return await _tiktokCrawlDomainService.GetTiktokHashTags();
        }

        public Task UpdateTiktokVideosState(UpdateTiktokVideosStateRequest updateTiktokStateRequest)
        {
            return _tiktokCrawlDomainService.UpdateTiktokVideosState(updateTiktokStateRequest);
        }
        
        public Task<List<MCNVietNamChannelDto>> SaveMCNVietNamChannel(MCNVietNamChannelApiRequest request)
        {
            return _tiktokCrawlDomainService.SaveMCNVietNamChannel(request);
        }

        public Task<List<TrendingDetailDto>> SaveTrendingDetails(TrendingDetailApiRequest request)
        {
            return _tiktokCrawlDomainService.SaveTrendingDetails(request);
        }

    }
}