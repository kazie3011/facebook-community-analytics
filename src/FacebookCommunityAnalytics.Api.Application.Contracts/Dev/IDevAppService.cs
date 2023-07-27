using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.Users;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Volo.Abp.Application.Services;


namespace FacebookCommunityAnalytics.Api.Dev
{
    public interface IDevAppService : IApplicationService
    {

        string Version();
        
        Task<GlobalConfiguration> Config_Global();
        Task<PayrollConfiguration> Config_Payroll();
        
        Task<DevResponse<int>> Accounts_ChangeStatusAndType(Accounts_ChangeStatusAndTypeRequest request);

        Task<int> Crawl_InitUncrawledPosts(bool initNewPosts = false);
        Task<CampaignPostResponse> Crawl_InitCrawlForCampaign(CampaignPostRequest request);
        Task<int> Crawl_InitCrawlCampaignPosts();
        Task<OldestUncrawledPostsResponse> Crawl_GetOldestUncrawledPosts(OldestUncrawledPostsRequest request);
        Task Crawl_InitGroupUserCrawlTeams(List<string> source);

        Task<List<string>> Users_GetDuplicated();
        Task<List<string>> Users_GetNoRoles();
        Task<List<string>> Users_GetNoTeams();
        Task<List<string>> Users_GetNoPosts();
        Task<List<AppUserDto>> Users_AddDefaultStaffRole();
        Task<List<string>> Users_SetRoleForNoRole();
        Task<List<string>> Users_GetInMultipleTeams();
        Task<List<string>> User_DeactiveNoRole();

        Task Posts_CleanUp();
        Task Posts_CleanUpUrl();
        Task<PostConvertResponse> Posts_Convert(PostConvertRequest request);
        Task<List<PostWithNavigationPropertiesDto>> Posts_GetInvalidUrls();
        Task<DevResponse<string>> Posts_GetNotAvailable();
        Task<DevResponse<string>> Posts_GetNoCreatedAt();
        Task<DevResponse<string>> Posts_GetNoUsers();
        Task<DevResponse<string>> Posts_GetUncrawled();
        Task<DevResponse<string>> Posts_UpdateIsValid(PostUpdateIsValidRequest request);

        Task<bool> Email_SendSampleEmail(string email);

        Task Aff_InitUserAffOwners();
        Task<AffTopLinkSummaryApiResponse> Aff_GetTopLinkAffSummary();
        Task<int> Aff_ExportTopLazada();
        Task<DevAffiliateDetailModel> Aff_Get(string userCode);
        Task Aff_SetPostUsersToAffUsers();
        Task<List<TeamAffiliatePayroll>> Aff_GetAffiliatePayroll();
        Task Aff_SyncCampaignAffStats();
        Task Aff_SyncAffShopinessConversions();
        Task<int> Aff_Check();

        Task<DevResponse<string>> UserInfo_RemoveDuplicates();
        Task<int> UserInfo_ClearAverageReaction();
        Task<int> UserInfo_ExportAverageReaction();
        Task<int> UserInfo_ImportAverageReaction();
        Task<int> UserInfo_UpdateSalaryAndPosition();
        Task<Dictionary<string, List<DuplicateSeedingAccountModel>>> UserInfo_GetDuplicateSeedingAccounts();
        
        Task<DevResponse<int>> Temp();
        Task<string> Temp(DateTime dateTime);
        Task<int> ExportTopConversionAndReactionPosts();

        Task<int> SyncUserAffiliate();
        Task CleanUpData();
        Task CleanData();
        Task CleanUserInfos();

        Task Payroll_CalculatePayroll();
        Task Payroll_CalculateHappyDayPayroll();
        Task CleanPayroll();
        Task GenerateContracts();
        Task GenerateTransactions();
        Task Group_UpdateSeedingTeams();
        Task<int> ResyncUserAffiliate();
        Task<int> AffCheck();
        Task<int> Post_Clean();

        Task<int> Eval_GenerateEval(int year, int month);
        Task<int> Tiktok_CloneGroupHistory();
        Task<int> Clone_GroupStatsHistory();
        Task<int> EvalConfig_CleanUp();
        Task<DevResponse<int>> Aff_CleanDupAndEmptyAff();
        Task<List<ShortLinkPost>> GetDuplicateShortLinkPosts();
        Task StaffEvalChangingUser(string changeUserCode = "185", string toUserCode = "018");
        Task GetDuplicateFidPosts();
        Task GetDuplicateVideos();
        Task GetEmptyAppUserID();
        Task GetStaffEvalWithoutTeam();
        Task<List<string>> GetUserInfoWithoutPosition();
        Task<int> UpdateContract();
        Task InitTiktokStatHistory();
    }
}