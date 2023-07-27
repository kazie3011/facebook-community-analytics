using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.UserAffiliates
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserAffiliate")]
    [Route("api/app/user-affiliate")]
    public class UserAffiliateController : AbpController, IUserAffiliateAppService
    {
        private readonly IUserAffiliateAppService _userAffiliateAppService;

        public UserAffiliateController(IUserAffiliateAppService userAffiliateAppService)
        {
            _userAffiliateAppService = userAffiliateAppService;
        }

        [HttpPost]
        public Task<UserAffiliateDto> CreateAsync(UserAffiliateCreateDto input)
        {
            return _userAffiliateAppService.CreateAsync(input);
        }

        [HttpDelete]
        public Task DeleteAsync(Guid id)
        {
            return _userAffiliateAppService.DeleteAsync(id);
        }

        [HttpGet]
        public Task<UserAffiliateDto> GetAsync(Guid id)
        {
            return _userAffiliateAppService.GetAsync(id);
        }

        [HttpGet("user-aff-details")]
        public Task<UserAffDetailDto> GetUserAffDetails(UserAffDetailApiRequest apiRequest)
        {
            return _userAffiliateAppService.GetUserAffDetails(apiRequest);
        }

        [HttpGet("get-list")]
        public Task<PagedResultDto<UserAffiliateDto>> GetListAsync(GetUserAffiliatesInput input)
        {
            return _userAffiliateAppService.GetListAsync(input);
        }

        [HttpPut]
        public Task<UserAffiliateDto> UpdateAsync(Guid id, UserAffiliateUpdateDto input)
        {
            return _userAffiliateAppService.UpdateAsync(id, input);
        }

        [HttpGet("get-list-navigation")]
        public Task<PagedResultDto<UserAffiliateWithNavigationPropertiesDto>> GetUserAffiliateWithNavigationProperties(GetUserAffiliatesInputExtend input)
        {
            return _userAffiliateAppService.GetUserAffiliateWithNavigationProperties(input);
        }
        
        [HttpGet("get-navigation")]
        public Task<UserAffiliateWithNavigationPropertiesDto> GetUserAffiliateWithNavigationProperties(Guid id)
        {
            return _userAffiliateAppService.GetUserAffiliateWithNavigationProperties(id);
        }

        [HttpGet("get-affiliate-url")]
        public Task<ShortlinkDto> GenerateAffiliate(GenerateShortlinkApiRequest generateShortlinkApiRequest)
        {
            return _userAffiliateAppService.GenerateAffiliate(generateShortlinkApiRequest);
        }

        [HttpGet("get-list-user-affiliate")]
        public Task<List<UserAffiliateDto>> GetListUserAffiliate(GetUserAffiliatesInputExtend input)
        {
            return _userAffiliateAppService.GetListUserAffiliate(input);
        }

        [HttpGet("get-affiliate-page-by-campaign")]
        public Task<PagedResultDto<CampaignAffiliateDto>> GetUserAffiliatePageByCampaignAsync(Guid campaignId, GetUserAffiliatesInputExtend input)
        {
            return _userAffiliateAppService.GetUserAffiliatePageByCampaignAsync(campaignId, input);
        }

        [HttpGet]
        [Route("export-user-affiliate")]
        public Task<byte[]> GetUserAffiliateExcelAsync(GetUserAffiliatesInputExtend input)
        {
            return _userAffiliateAppService.GetUserAffiliateExcelAsync(input);
        }

        [HttpGet]
        [Route("group-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input)
        {
            return _userAffiliateAppService.GetGroupLookupAsync(input);
        }

        [HttpGet]
        [Route("category-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            return _userAffiliateAppService.GetCategoryLookupAsync(input);
        }

        [HttpGet]
        [Route("partner-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input)
        {
            return _userAffiliateAppService.GetPartnerLookupAsync(input);
        }

        [HttpGet]
        [Route("campaign-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input)
        {
            return _userAffiliateAppService.GetCampaignLookupAsync(input);
        }

        [HttpGet]
        [Route("user-affiliate-summary")]
        public Task<UserAffSummaryApiResponse> GetUserAffiliateSummary(UserAffSummaryApiRequest request)
        {
            return _userAffiliateAppService.GetUserAffiliateSummary(request);
        }
    }
}
