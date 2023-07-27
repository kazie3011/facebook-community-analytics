using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Statistics;

namespace FacebookCommunityAnalytics.Api.Controllers.AffiliateStats
{
    [RemoteService]
    [Area("app")]
    [ControllerName("AffiliateStat")]
    [Route("api/app/affiliate-stats")]

    public class AffiliateStatController : AbpController, IAffiliateStatsAppService
    {
        private readonly IAffiliateStatsAppService _affiliateStatsAppService;

        public AffiliateStatController(IAffiliateStatsAppService affiliateStatsAppService)
        {
            _affiliateStatsAppService = affiliateStatsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<AffiliateStatDto>> GetListAsync(GetAffiliateStatsInput input)
        {
            return _affiliateStatsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<AffiliateStatDto> GetAsync(Guid id)
        {
            return _affiliateStatsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<AffiliateStatDto> CreateAsync(AffiliateStatCreateDto input)
        {
            return _affiliateStatsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<AffiliateStatDto> UpdateAsync(Guid id, AffiliateStatUpdateDto input)
        {
            return _affiliateStatsAppService.UpdateAsync(id, input);
        }

        [HttpGet]
        [Route("aff-daily-summary")]
        public Task<AffDailySummaryApiResponse> GetDailyAffSummary(GeneralStatsApiRequest apiRequest)
        {
            return _affiliateStatsAppService.GetDailyAffSummary(apiRequest);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _affiliateStatsAppService.DeleteAsync(id);
        }
    }
}