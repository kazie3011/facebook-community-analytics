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
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserProfiles
{
    public interface IUserProfileAppService : IApplicationService
    {
        Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetContractsPagedResult(GetContractsInput input);
        Task<PagedResultDto<UserAffiliateWithNavigationPropertiesDto>> GetUserAffiliateWithNavigationProperties(GetUserAffiliatesInputExtend input);
        Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetPostsPagedResult(GetPostsInputExtend input);
        Task<List<PostDetailExportRow>> GetEvaluationPostDetailExportRow(GetStaffEvaluationsInput input);
        Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluation(Guid userId, int month, int year);
        Task<List<StaffEvaluationWithNavigationPropertiesDto>> GetStaffEvaluations(Guid userId,int fromYear, int fromMonth,int toYear,int toMonth );
        Task<UserCompensationNavigationPropertiesDto> GetUserCompensationByUser(Guid userId, int month, int year);
        Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId);
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
        Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input);
        Task<UserProfileChartResponse> GetChartStats(UserProfileChartRequest request);
    }
}