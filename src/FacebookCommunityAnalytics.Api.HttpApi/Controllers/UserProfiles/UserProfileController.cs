using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserProfiles;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.UserProfiles
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserProfile")]
    [Route("api/app/user-profile")]
    public class UserProfileController : AbpController, IUserProfileAppService
    {
        private readonly IUserProfileAppService _userProfileAppService;

        public UserProfileController(IUserProfileAppService userProfileAppService)
        {
            _userProfileAppService = userProfileAppService;
        }

        [HttpGet]
        [Route("get-contracts-page-result")]
        public Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetContractsPagedResult(GetContractsInput input)
        {
            return _userProfileAppService.GetContractsPagedResult(input);
        }

        [HttpGet]
        [Route("get-user-aff-nav")]
        public Task<PagedResultDto<UserAffiliateWithNavigationPropertiesDto>> GetUserAffiliateWithNavigationProperties(GetUserAffiliatesInputExtend input)
        {
            return _userProfileAppService.GetUserAffiliateWithNavigationProperties(input);
        }

        [HttpGet]
        [Route("get-posts-page-result")]
        public Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetPostsPagedResult(GetPostsInputExtend input)
        {
            return _userProfileAppService.GetPostsPagedResult(input);
        }

        [HttpGet]
        [Route("get-evaluation-post-detail-export-row")]
        public Task<List<PostDetailExportRow>> GetEvaluationPostDetailExportRow(GetStaffEvaluationsInput input)
        {
            return _userProfileAppService.GetEvaluationPostDetailExportRow(input);
        }

        [HttpGet]
        [Route("get-staff-evaluation")]
        public Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluation(Guid userId, int month, int year)
        {
            return _userProfileAppService.GetStaffEvaluation(userId, month, year);
        }
        
        [HttpGet]
        [Route("get-staff-evaluations")]
        public Task<List<StaffEvaluationWithNavigationPropertiesDto>> GetStaffEvaluations(Guid userId,int fromYear, int fromMonth,int toYear,int toMonth)
        {
            return _userProfileAppService.GetStaffEvaluations(userId, fromYear, fromMonth, toYear, toMonth);
        }

        [HttpGet]
        [Route("user-compensation-by-user")]
        public Task<UserCompensationNavigationPropertiesDto> GetUserCompensationByUser(Guid userId, int month, int year)
        {
            return _userProfileAppService.GetUserCompensationByUser(userId, month, year);
        }
        
        [HttpGet]
        [Route("get-aff-conversions")]
        public Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId)
        {
            return _userProfileAppService.GetAffiliateConversions(fromDate, toDate, userId);
        }
        [HttpGet]
        [Route("get-user-profile/{userId}")]
        public Task<UserProfileDto> GetUserProfileAsync(Guid userId)
        {
            return _userProfileAppService.GetUserProfileAsync(userId);
        }

        [HttpGet]
        [Route("get-user-lookup-async")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input)
        {
            return _userProfileAppService.GetUserLookupAsync(input);
        }

        [HttpGet]
        [Route("get-chart-stats")]
        public Task<UserProfileChartResponse> GetChartStats(UserProfileChartRequest request)
        {
            return _userProfileAppService.GetChartStats(request);
        }
    }
}