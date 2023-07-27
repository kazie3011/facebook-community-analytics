using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public interface IUserAffiliateAppService : IApplicationService
    {
        Task<UserAffDetailDto> GetUserAffDetails(UserAffDetailApiRequest apiRequest);
        
        Task<PagedResultDto<UserAffiliateDto>> GetListAsync(GetUserAffiliatesInput input);

        Task<UserAffiliateDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<UserAffiliateDto> CreateAsync(UserAffiliateCreateDto input);

        Task<UserAffiliateDto> UpdateAsync(Guid id, UserAffiliateUpdateDto input);
        
        Task<UserAffiliateWithNavigationPropertiesDto> GetUserAffiliateWithNavigationProperties(Guid id);
        
        Task<PagedResultDto<UserAffiliateWithNavigationPropertiesDto>> GetUserAffiliateWithNavigationProperties(GetUserAffiliatesInputExtend input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input);
        Task<ShortlinkDto> GenerateAffiliate(GenerateShortlinkApiRequest generateShortlinkApiRequest);
        Task<List<UserAffiliateDto>> GetListUserAffiliate(GetUserAffiliatesInputExtend input);
        Task<PagedResultDto<CampaignAffiliateDto>> GetUserAffiliatePageByCampaignAsync(Guid campaignId,
            GetUserAffiliatesInputExtend input);
        Task<UserAffSummaryApiResponse> GetUserAffiliateSummary(UserAffSummaryApiRequest request);
        Task<byte[]> GetUserAffiliateExcelAsync(GetUserAffiliatesInputExtend input);
    }
}
