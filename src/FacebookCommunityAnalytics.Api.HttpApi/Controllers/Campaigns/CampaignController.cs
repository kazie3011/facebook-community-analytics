using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;

namespace FacebookCommunityAnalytics.Api.Controllers.Campaigns
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Campaign")]
    [Route("api/app/campaigns")]
    public class CampaignController : AbpController, ICampaignsAppService
    {
        private readonly ICampaignsAppService _campaignsAppService;

        public CampaignController(ICampaignsAppService campaignsAppService)
        {
            _campaignsAppService = campaignsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetListAsync(GetCampaignsInput input)
        {
            return _campaignsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<CampaignWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _campaignsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("/get-by-idorcode/{idOrCode}")]
        public virtual Task<CampaignDto> GetByIdOrCode(string idOrCode)
        {
            return _campaignsAppService.GetByIdOrCode(idOrCode);
        }

        [HttpGet]
        [Route("partner-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input)
        {
            return _campaignsAppService.GetPartnerLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<CampaignDto> CreateAsync(CampaignCreateDto input)
        {
            return _campaignsAppService.CreateAsync(input);
        }

        [HttpPost]
        [Route("update-campaign-prizes")]
        public Task UpdateCampaignPrizes(Guid id, CampaignUpdateDto input)
        {
            return _campaignsAppService.UpdateCampaignPrizes(id, input);
        }

        [HttpPost]
        [Route("create-campaign-posts")]
        public Task CreateCampaignPosts(PostCreateDto input)
        {
            return _campaignsAppService.CreateCampaignPosts(input);
        }
        
        [HttpPut]
        [Route("{id}")]
        public virtual Task<CampaignDto> UpdateAsync(Guid id, CampaignUpdateDto input)
        {
            return _campaignsAppService.UpdateAsync(id, input);
        }

        [HttpPut]
        [Route("remove-campaign-post/{postId}")]
        public Task RemoveCampaignPost(Guid postId)
        {
            return _campaignsAppService.RemoveCampaignPost(postId);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _campaignsAppService.DeleteAsync(id);
        }
        
        [HttpGet]
        [Route("/campaign-posts/{campaignId}")]
        public Task<List<CampaignPostDto>> GetPosts(Guid campaignId)
        {
            return _campaignsAppService.GetPosts(campaignId);
        }

        [HttpGet]
        [Route("export-campaign")]
        public Task<byte[]> ExportCampaign(Guid campaignId)
        {
            return _campaignsAppService.ExportCampaign(campaignId);
        }

        [HttpGet]
        [Route("get-tiktoks")]
        public Task<List<TiktokWithNavigationPropertiesDto>> GetTikToks(GetTiktoksInputExtend input)
        {
            return _campaignsAppService.GetTikToks(input);
        }

        [HttpPut]
        [Route("update-tiktok")]
        public Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id)
        {
            return _campaignsAppService.UpdateCampaignTiktok(input, id);
        }
        
        [HttpGet]
        [Route("get-camps-by-time")]
        public async Task<List<CampaignDto>> GetCampsByTime(DateTime from, DateTime to)
        {
            return await _campaignsAppService.GetCampsByTime(from,to);
        }

        [HttpGet]
        [Route("daily-chart-stats")]
        public Task<CampaignDailyChartResponse> GetCampaignDailyChartStats(Guid campaignId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            return _campaignsAppService.GetCampaignDailyChartStats(campaignId, fromDate, toDate);
        }

        [HttpGet]
        [Route("get-post-count-groups-chart")]
        public async Task<PieChartDataSource<double>> GetPostCountGroupsChart(DateTime fromDate, DateTime toDate)
        {
            return await _campaignsAppService.GetPostCountGroupsChart(fromDate,toDate);
        }

        [HttpGet]
        [Route("get-reaction-groups-chart")]
        public async Task<PieChartDataSource<double>> GetReactionGroupsChart(DateTime fromDate, DateTime toDate)
        {
            return await _campaignsAppService.GetReactionGroupsChart(fromDate,toDate);
        }

        [HttpGet]
        [Route("get-author-statistic")]
        public Task<List<AuthorStatistic>> GetAuthorStatistic()
        {
            return _campaignsAppService.GetAuthorStatistic();
        }

        [HttpGet]
        [Route("get-campaign-daily-chart-stats")]
        public Task<CampaignDailyChartResponse> GetCampaignDailyChartStats(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            return _campaignsAppService.GetCampaignDailyChartStats(campaignId, fromDate, toDate);
        }

        [HttpGet]
        [Route("get-affiliates-async")]
        public Task<List<UserAffiliateWithNavigationPropertiesDto>> GetAffiliatesAsync(List<string> shortLinks)
        {
            return _campaignsAppService.GetAffiliatesAsync(shortLinks);
        }

        [HttpGet]
        [Route("send-campaign-email")]
        public Task SendCampaignEmail(Guid campaignId)
        {
            return _campaignsAppService.SendCampaignEmail(campaignId);
        }
    }
}