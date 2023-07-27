using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Controllers.Integrations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Tiktok")]
    [Route("api/app/tiktok")]
    public class TiktokController: ApiController, ITiktokAppService
    {
        private readonly ITiktokAppService _tiktokAppService;

        public TiktokController(ITiktokAppService tiktokAppService)
        {
            _tiktokAppService = tiktokAppService;
        }

        [HttpPost]
        [Route("channel-stats")]
        public async Task<SaveChannelStatApiResponse> SaveChannelStats(SaveChannelStatApiRequest apiRequest)
        {
            return await _tiktokAppService.SaveChannelStats(apiRequest);
        }

        [HttpPost]
        [Route("channel-videos")]
        public async Task<SaveChannelVideoResponse> SaveVideos(SaveChannelVideoRequest apiRequest)
        {
            return await _tiktokAppService.SaveVideos(apiRequest);
        }

        [HttpPost]
        [Route("tiktok-stat")]
        public async Task<SaveTiktokStatApiResponse> SaveTiktokStat(SaveTiktokStatApiRequest apiRequest)
        {
            return await _tiktokAppService.SaveTiktokStat(apiRequest);
        }

        [HttpGet]
        [Route("hashtags")]
        public Task<GetTiktokHashTagsApiResponse> GetTiktokHashTags()
        {
            return _tiktokAppService.GetTiktokHashTags();
        }

        [HttpPost]
        [Route("update-channel-videos-state")]
        public Task UpdateTiktokVideosState(UpdateTiktokVideosStateRequest updateTiktokStateRequest)
        {
            return _tiktokAppService.UpdateTiktokVideosState(updateTiktokStateRequest);
        }

        [HttpPost("tiktok-videos-stats")]
        public Task SaveMCNChannelVideoStats(CrawlMCNVideoInput apiRequest)
        {
            return _tiktokAppService.SaveMCNChannelVideoStats(apiRequest);
        }
        
        [HttpPost("save-mcn-vietnam-channel")]
        public Task<List<MCNVietNamChannelDto>> SaveMCNVietNamChannel(MCNVietNamChannelApiRequest request)
        {
            return _tiktokAppService.SaveMCNVietNamChannel(request);
        }

        [HttpPost("save-trending-details")]
        public Task<List<TrendingDetailDto>> SaveTrendingDetails(TrendingDetailApiRequest request)
        {
            return _tiktokAppService.SaveTrendingDetails(request);
        }
    }
}