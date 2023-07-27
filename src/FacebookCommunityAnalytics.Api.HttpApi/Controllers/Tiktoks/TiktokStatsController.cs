using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Tiktoks
{
    [RemoteService]
    [Area("app")]
    [ControllerName("TiktokStats")]
    [Route("api/app/tiktok-stats")]
    public class TiktokStatsController : AbpController, ITiktokStatsAppService
    {
        private readonly ITiktokStatsAppService _tiktokStatsAppService;

        public TiktokStatsController(ITiktokStatsAppService tiktokStatsAppService)
        {
            _tiktokStatsAppService = tiktokStatsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<TiktokWithNavigationPropertiesDto>> GetListAsync(GetTiktoksInputExtend input)
        {
            return _tiktokStatsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("get-export-rows")]
        public Task<List<TiktokExportRow>> GetExportRows(GetTiktoksInputExtend input)
        {
            return _tiktokStatsAppService.GetExportRows(input);
        }

        [HttpGet]
        [Route("weekly-total-followers")]
        public Task<List<TiktokWeeklyTotalFollowers>> GetWeeklyTotalFollowersReport(GetTiktokWeeklyTotalFollowersRequest request)
        {
            return _tiktokStatsAppService.GetWeeklyTotalFollowersReport(request);
        }

        [HttpGet]
        [Route("weekly-total-views")]
        public Task<List<TiktokWeeklyTotalViews>> GetWeeklyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            return _tiktokStatsAppService.GetWeeklyTotalViewsReport(timeFrom, timeTo);
        }

        [HttpGet]
        [Route("monthly-total-followers")]
        public Task<List<TikTokMonthlyTotalFollowers>> GetMonthlyTotalFollowersReport(GetTiktokMonthlyTotalFollowersRequest request)
        {
            return _tiktokStatsAppService.GetMonthlyTotalFollowersReport(request);
        }

        [HttpGet]
        [Route("monthly-total-views")]
        public Task<List<TikTokMonthlyTotalViews>> GetMonthlyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            return _tiktokStatsAppService.GetMonthlyTotalViewsReport(timeFrom, timeTo);
        }

        [HttpGet]
        [Route("tiktok-mcn-monthly-followers")]
        public Task<List<TikTokMCNReport>> GetTiktokMCNMonthlyReport(DateTime? timeFrom = null, DateTime? timeTo = null, TikTokMCNType? tikTokMcnType = null)
        {
            return _tiktokStatsAppService.GetTiktokMCNMonthlyReport(timeFrom, timeTo, tikTokMcnType);
        }

        [HttpGet]
        [Route("tiktok-mcn-weekly-followers")]
        public Task<List<TikTokMCNReport>> GetTiktokMCNWeeklyReports(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            return _tiktokStatsAppService.GetTiktokMCNWeeklyReports(timeFrom, timeTo);
        }

        [HttpGet]
        [Route("get-list-tiktok-mcn")]
        public Task<List<TikTokMCNDto>> GetListTikTokMCN(TikTokMCNType tikTokMcnType)
        {
            return _tiktokStatsAppService.GetListTikTokMCN(tikTokMcnType);
        }

        [HttpGet("get-video-image-file-{videoId}")]
        public Task<FileResultDto> GetVideoImage(Guid videoId)
        {
            return _tiktokStatsAppService.GetVideoImage(videoId);
        }

        [Route("tiktok-video-image-{id}")]
        [HttpGet]
        public async Task<ActionResult> GetTikTokChannelImage(Guid id)
        {
            var result = await _tiktokStatsAppService.GetVideoImage(id);
            return File(result.FileData, contentType: result.ContentType);
        }

        [HttpGet("get-mcn-gdl-top-video-of-day")]
        public Task<TiktokDto> GetTopVideoOfDay(DateTime startDate, DateTime endDate)
        {
            return _tiktokStatsAppService.GetTopVideoOfDay(startDate, endDate);
        }

        [HttpGet("get-mcn-gdl-top-channel")]
        public Task<TopChannelDto> GetTopChannel(DateTime startDate, DateTime endDate)
        {
            return _tiktokStatsAppService.GetTopChannel(startDate, endDate);
        }
        
        [HttpGet("get-mcn-gdl-top-10-channel-of-week")]
        public async Task<List<TopChannelDto>> GetTopStatChannelsOfWeek()
        {
            return await _tiktokStatsAppService.GetTopStatChannelsOfWeek();
        }
        
        [HttpGet("get-mcn-gdl-top-10-channel-of-month")]
        public async Task<List<TopChannelDto>> GetTopStatChannelOfMonth()
        {
            return await _tiktokStatsAppService.GetTopStatChannelOfMonth();
        }

        [HttpGet("get-gdl-channels")]
        public Task<List<GroupDto>> GetGDLChannels()
        {
            return _tiktokStatsAppService.GetGDLChannels();
        }

        [HttpGet("get-trending-details")]
        public Task<List<TrendingDetailDto>> GetTrendingDetails(DateTime fromDate, DateTime toDate, int count)
        {
            return _tiktokStatsAppService.GetTrendingDetails(fromDate, toDate, count);
        }
        
        [HttpGet("Get-Top-Trending-Contents-By-Date-Range")]

        public Task<List<TrendingDetailDto>> GetTopTrendingContentsByDateRange(DateTime? @from, DateTime? to, int contentNumber, bool calculateGrowth)
        {
            return _tiktokStatsAppService.GetTopTrendingContentsByDateRange(from, to, contentNumber, calculateGrowth);
        }
    }
}