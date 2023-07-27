using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Statistics;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.AffiliateStats.Default)]
    public class AffiliateStatsAppService : ApplicationService, IAffiliateStatsAppService
    {
        private readonly IAffiliateStatRepository _affiliateStatRepository;
        private readonly IAffiliateStatsDomainService _affiliateStatsDomainService;

        public AffiliateStatsAppService(IAffiliateStatRepository affiliateStatRepository
            , IAffiliateStatsDomainService affiliateStatsDomainService)
        {
            _affiliateStatRepository = affiliateStatRepository;
            _affiliateStatsDomainService = affiliateStatsDomainService;
        }

        public virtual async Task<PagedResultDto<AffiliateStatDto>> GetListAsync(GetAffiliateStatsInput input)
        {
            var totalCount = await _affiliateStatRepository.GetCountAsync(input.FilterText, input.AffiliateOwnershipType, input.ClickMin, input.ClickMax, input.ConversionMin, input.ConversionMax, input.AmountMin, input.AmountMax, input.CommissionMin, input.CommissionMax, input.CommissionBonusMin, input.CommissionBonusMax, input.ClientOffsetInMinutes, input.CreatedAtMin, input.CreatedAtMax ); 
            var items = await _affiliateStatRepository.GetListAsync(input.FilterText, input.AffiliateOwnershipType, input.ClickMin, input.ClickMax, input.ConversionMin, input.ConversionMax, input.AmountMin, input.AmountMax, input.CommissionMin, input.CommissionMax, input.CommissionBonusMin, input.CommissionBonusMax, input.ClientOffsetInMinutes, input.CreatedAtMin, input.CreatedAtMax, input.Sorting, input.MaxResultCount, input.SkipCount); 
 
            return new PagedResultDto<AffiliateStatDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AffiliateStat>, List<AffiliateStatDto>>(items)
            };
        }

        public virtual async Task<AffiliateStatDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<AffiliateStat, AffiliateStatDto>(await _affiliateStatRepository.GetAsync(id));
        }

        [Authorize(ApiPermissions.AffiliateStats.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _affiliateStatRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.AffiliateStats.Create)]
        public virtual async Task<AffiliateStatDto> CreateAsync(AffiliateStatCreateDto input)
        {

            var affiliateStat = ObjectMapper.Map<AffiliateStatCreateDto, AffiliateStat>(input);
            affiliateStat.TenantId = CurrentTenant.Id;
            affiliateStat = await _affiliateStatRepository.InsertAsync(affiliateStat, autoSave: true);
            return ObjectMapper.Map<AffiliateStat, AffiliateStatDto>(affiliateStat);
        }

        [Authorize(ApiPermissions.AffiliateStats.Edit)]
        public virtual async Task<AffiliateStatDto> UpdateAsync(Guid id, AffiliateStatUpdateDto input)
        {

            var affiliateStat = await _affiliateStatRepository.GetAsync(id);
            ObjectMapper.Map(input, affiliateStat);
            affiliateStat = await _affiliateStatRepository.UpdateAsync(affiliateStat);
            return ObjectMapper.Map<AffiliateStat, AffiliateStatDto>(affiliateStat);
        }
        
        public async Task<AffDailySummaryApiResponse> GetDailyAffSummary(GeneralStatsApiRequest apiRequest)
        {
            return await _affiliateStatsDomainService.GetDailyAffSummary(apiRequest);
        }
    }
}