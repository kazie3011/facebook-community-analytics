using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Statistics;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public interface IAffiliateStatsAppService : IApplicationService
    {
        Task<PagedResultDto<AffiliateStatDto>> GetListAsync(GetAffiliateStatsInput input);

        Task<AffiliateStatDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<AffiliateStatDto> CreateAsync(AffiliateStatCreateDto input);

        Task<AffiliateStatDto> UpdateAsync(Guid id, AffiliateStatUpdateDto input);
        Task<AffDailySummaryApiResponse> GetDailyAffSummary(GeneralStatsApiRequest apiRequest);
    }
}