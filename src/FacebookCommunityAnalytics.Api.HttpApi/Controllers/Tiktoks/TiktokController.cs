using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Tiktoks
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Tiktok")]
    [Route("api/app/tiktoks")]
    public class TiktokController : AbpController, ITiktokAppService
    {
        private readonly ITiktokAppService _tiktokAppService;

        public TiktokController(ITiktokAppService tiktokAppService)
        {
            _tiktokAppService = tiktokAppService;
        }

        [HttpPost]
        [Route("save-channel-stats")]
        public Task<SaveChannelStatApiResponse> SaveChannelStats(SaveChannelStatApiRequest apiRequest)
        {
            return _tiktokAppService.SaveChannelStats(apiRequest);
        }

        [HttpPost]
        [Route("save-channel-video")]
        public Task<SaveChannelVideoResponse> SaveVideos(SaveChannelVideoRequest apiRequest)
        {
            return _tiktokAppService.SaveVideos(apiRequest);
        }

        [HttpPost]
        [Route("save-tiktok-stats")]
        public Task<SaveTiktokStatApiResponse> SaveTiktokStat(SaveTiktokStatApiRequest apiRequest)
        {
            return _tiktokAppService.SaveTiktokStat(apiRequest);
        }

        [HttpGet]
        [Route("get-hashtags")]
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

        [HttpPost("save-tiktok-videos-stats")]
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