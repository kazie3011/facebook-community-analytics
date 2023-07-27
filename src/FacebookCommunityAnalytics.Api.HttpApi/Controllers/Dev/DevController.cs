using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Dev;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Security.Encryption;

namespace FacebookCommunityAnalytics.Api.Controllers.Dev
{
    [AllowAnonymous]
    [Area("app")]
    [ControllerName("dev")]
    [Route("api/app/dev")]
    public class DevController : AbpController, IDevAppService
    {
        private readonly IDevAppService _devService;
        private readonly IApiNotificationAppService _apiNotificationAppService;
        private readonly IStringEncryptionService _stringEncryptionService;
        
        public DevController(IDevAppService devService,
            IApiNotificationAppService apiNotificationAppService, IStringEncryptionService stringEncryptionService)
        {
            _devService = devService;
            _apiNotificationAppService = apiNotificationAppService;
            _stringEncryptionService = stringEncryptionService;
        }

        [HttpGet("version")]
        public string Version()
        {
            return _devService.Version();
        }
        
        [HttpGet("TestSendNotify")]
        public async Task TestSendNotify()
        {
            await _apiNotificationAppService.SendMessageAsync(new SendMessageInput()
            {
                Message = "Test signalR"
            });
        }

        [HttpGet("encrypt/{input}")]
        public string Encrypt(string input)
        {
            return _stringEncryptionService.Encrypt(input);
        }

        #region CONFIG

        [HttpGet("Config_Global")]
        public Task<GlobalConfiguration> Config_Global()
        {
            return _devService.Config_Global();
        }
        
        [HttpGet("Config_Payroll")]
        public Task<PayrollConfiguration> Config_Payroll()
        {
            return _devService.Config_Payroll();
        }

        #endregion

        #region ACCOUNTS
        
        [HttpPost]
        [Route("Accounts_ChangeStatusAndType")]
        public Task<DevResponse<int>> Accounts_ChangeStatusAndType([FromBody] Accounts_ChangeStatusAndTypeRequest request)
        {
            return _devService.Accounts_ChangeStatusAndType(request);
        }

        #endregion

        #region AFFILIATE

        [HttpGet]
        [Route("Affiliates_Get")]
        public Task<DevAffiliateDetailModel> Aff_Get(string userCode)
        {
            return _devService.Aff_Get(userCode);
        }

        [HttpGet]
        [Route("Aff_SetPostUsersToAffUsers")]
        public Task Aff_SetPostUsersToAffUsers()
        {
            return _devService.Aff_SetPostUsersToAffUsers();
        }


        [HttpGet]
        [Route("Aff_GetAffiliatePayroll")]
        public Task<List<TeamAffiliatePayroll>> Aff_GetAffiliatePayroll()
        {
            return _devService.Aff_GetAffiliatePayroll();
        }

        #endregion
        
        #region CRAWL

        [HttpGet("Crawl_InitUncrawledPosts")]
        public Task<int> Crawl_InitUncrawledPosts(bool initNewPosts = false)
        {
            return _devService.Crawl_InitUncrawledPosts(initNewPosts);
        }

        [HttpGet("Crawl_InitCrawlForCampaign")]
        public Task<CampaignPostResponse> Crawl_InitCrawlForCampaign(CampaignPostRequest request)
        {
            return _devService.Crawl_InitCrawlForCampaign(request);
        }
        
        [HttpGet("Crawl_InitCrawlCampaignPosts")]
        public Task<int> Crawl_InitCrawlCampaignPosts()
        {
            return _devService.Crawl_InitCrawlCampaignPosts();
        }

        [HttpGet]
        [Route("Crawl_GetOldestUncrawledPosts")]
        public Task<OldestUncrawledPostsResponse> Crawl_GetOldestUncrawledPosts(OldestUncrawledPostsRequest request)
        {
            return _devService.Crawl_GetOldestUncrawledPosts(request);
        }

        #endregion

        #region USERS

        [HttpGet("Users_GetDuplicated")]
        public Task<List<string>> Users_GetDuplicated()
        {
            return _devService.Users_GetDuplicated();
        }

        [HttpGet("Users_GetNoRoles")]
        public Task<List<string>> Users_GetNoRoles()
        {
            return _devService.Users_GetNoRoles();
        }

        [HttpGet("Users_GetNoTeams")]
        public Task<List<string>> Users_GetNoTeams()
        {
            return _devService.Users_GetNoTeams();
        }

        [HttpGet("Users_GetNoPosts")]
        public Task<List<string>> Users_GetNoPosts()
        {
            return _devService.Users_GetNoPosts();
        }

        [HttpGet("Users_AddDefaultStaffRole")]
        public Task<List<AppUserDto>> Users_AddDefaultStaffRole()
        {
            return _devService.Users_AddDefaultStaffRole();
        }

        [HttpGet("Users_SetRoleForNoRole")]
        public Task<List<string>> Users_SetRoleForNoRole()
        {
            return _devService.Users_SetRoleForNoRole();
        }

        [HttpGet("Users_GetInMultipleTeams")]
        public Task<List<string>> Users_GetInMultipleTeams()
        {
            return _devService.Users_GetInMultipleTeams();
        }
        
        [HttpPost]
        [Route("User_DeactiveNoRole")]
        public Task<List<string>> User_DeactiveNoRole()
        {
            return _devService.User_DeactiveNoRole();
        }

        #endregion USERS

        #region USERINFOs

        
        [HttpGet]
        [Route("UserInfo_RemoveDuplicates")]
        public Task<DevResponse<string>> UserInfo_RemoveDuplicates()
        {
            return _devService.UserInfo_RemoveDuplicates();
        }

        [HttpGet]
        [Route("ClearAverageReactionUserInfo")]
        public Task<int> UserInfo_ClearAverageReaction()
        {
            return _devService.UserInfo_ClearAverageReaction();
        }

        [HttpGet]
        [Route("ExportAverageReactionUserInfo")]
        public Task<int> UserInfo_ExportAverageReaction()
        {
            return _devService.UserInfo_ExportAverageReaction();
        }

        [HttpGet]
        [Route("ImportAverageReactionUserInfo")]
        public Task<int> UserInfo_ImportAverageReaction()
        {
            return _devService.UserInfo_ImportAverageReaction();
        }

        [HttpGet]
        [Route("UserInfo_UpdateSalaryAndPosition")]
        public Task<int> UserInfo_UpdateSalaryAndPosition()
        {
            return _devService.UserInfo_UpdateSalaryAndPosition();
        }

        [HttpGet]
        [Route("UserInfo_GetDuplicateSeedingAccounts")]
        public Task<Dictionary<string, List<DuplicateSeedingAccountModel>>> UserInfo_GetDuplicateSeedingAccounts()
        {
            return _devService.UserInfo_GetDuplicateSeedingAccounts();
        }

        #endregion
        
        #region POSTS

        [HttpGet("Posts_CleanUp")]
        public Task Posts_CleanUp()
        {
            return _devService.Posts_CleanUp();
        }

        [HttpGet("Posts_CleanUpUrl")]
        public Task Posts_CleanUpUrl()
        {
            return _devService.Posts_CleanUpUrl();
        }

        [HttpGet("Posts_GetInvalidUrls")]
        public Task<List<PostWithNavigationPropertiesDto>> Posts_GetInvalidUrls()
        {
            return _devService.Posts_GetInvalidUrls();
        }

        [HttpGet("Posts_GetNotAvailable")]
        public async Task<DevResponse<string>>  Posts_GetNotAvailable()
        {
            return await _devService.Posts_GetNotAvailable();
        }

        [HttpGet("Posts_GetNoCreatedAt")]
        public async Task<DevResponse<string>>  Posts_GetNoCreatedAt()
        {
            return await _devService.Posts_GetNoCreatedAt();
        }

        [HttpGet("Posts_GetNoUsers")]
        public Task<DevResponse<string>>  Posts_GetNoUsers()
        {
            return _devService.Posts_GetNoUsers();
        }

        [HttpGet("Posts_GetUncrawled")]
        public async Task<DevResponse<string>>  Posts_GetUncrawled()
        {
            return await _devService.Posts_GetUncrawled();
        }

        [HttpPost]
        [Route("Posts_Convert")]
        public Task<PostConvertResponse> Posts_Convert(PostConvertRequest request)
        {
            return _devService.Posts_Convert(request);
        }

        [HttpPost]
        [Route("Posts_UpdateIsValid")]
        public Task<DevResponse<string>>  Posts_UpdateIsValid(PostUpdateIsValidRequest request)
        {
            return _devService.Posts_UpdateIsValid(request);
        }
        
        [HttpGet]
        [Route("Email_SendSampleEmail")]
        public Task<bool> Email_SendSampleEmail(string email)
        {
            return _devService.Email_SendSampleEmail(email);
        }
        #endregion

        #region OTHERS

        [HttpPost]
        [Route("Crawl_InitGroupUserCrawlTeams")]
        public Task Crawl_InitGroupUserCrawlTeams(List<string> source)
        {
            return  _devService.Crawl_InitGroupUserCrawlTeams(source);
        }

        

        [HttpGet]
        [Route("Aff_ExportTopLazada")]
        public Task<int> Aff_ExportTopLazada()
        {
            return _devService.Aff_ExportTopLazada();
        }

        [HttpGet]
        [Route("GetTopLinkAffSummary")]
        public Task<AffTopLinkSummaryApiResponse> Aff_GetTopLinkAffSummary()
        {
            return _devService.Aff_GetTopLinkAffSummary();
        }

        #endregion
        
        [HttpGet]
        [Route("Temp")]
        public Task<DevResponse<int>> Temp()
        {
            return _devService.Temp();
        }
        [HttpGet]
        [Route("TempDate")]
        public Task<string> Temp(DateTime dateTime)
        {
            return _devService.Temp(dateTime);
        }
        
        [HttpGet]
        [Route("ExportTopConversionAndReactionPosts")]
        public Task<int> ExportTopConversionAndReactionPosts()
        {
            return _devService.ExportTopConversionAndReactionPosts();
        }
        
        [HttpPost]
        [Route("CleanUpData")]
        public Task CleanUpData()
        {
            return _devService.CleanUpData();
        }
        
        [HttpGet]
        [Route("CleanData")]
        public Task CleanData()
        {
            return _devService.CleanData();
        }

        [HttpGet]
        [Route("CleanUserInfos")]
        public Task CleanUserInfos()
        {
            return _devService.CleanUserInfos();
        }

        [HttpGet]
        [Route("Payroll_CalculateHappyDayPayroll")]
        public Task Payroll_CalculateHappyDayPayroll()
        {
            return _devService.Payroll_CalculateHappyDayPayroll();
        }

        [HttpGet]
        [Route("CleanPayroll")]
        public Task CleanPayroll()
        {
            return _devService.CleanPayroll();
        }

        [HttpGet]
        [Route("Aff_InitUserAffOwners")]
        public Task Aff_InitUserAffOwners()
        {
            return _devService.Aff_InitUserAffOwners();
        }

        [HttpGet]
        [Route("Aff_Check")]
        public Task<int> Aff_Check()
        {
            return _devService.Aff_Check();
        }

        [HttpGet]
        [Route("Aff_SyncCampaignAffStats")]
        public Task Aff_SyncCampaignAffStats()
        {
            return _devService.Aff_SyncCampaignAffStats();
        }
        
        [HttpGet]
        [Route("Aff_SyncAffShopinessConversions")]
        public Task Aff_SyncAffShopinessConversions()
        {
            return _devService.Aff_SyncAffShopinessConversions();
        }

        [HttpGet]
        [Route("Payroll_CalculatePayroll")]
        public Task Payroll_CalculatePayroll()
        {
            return _devService.Payroll_CalculatePayroll();
        }

        #region SYNC

        [HttpGet]
        [Route("SyncUserAffiliate")]
        public Task<int> SyncUserAffiliate()
        {
            return _devService.SyncUserAffiliate();
        }

        #endregion
        
        [HttpGet]
        [Route("Create_Contract")]
        public Task GenerateContracts()
        {
            return _devService.GenerateContracts();
        }
        
        [HttpGet]
        [Route("Create_Transaction")]
        public Task GenerateTransactions()
        {
            return _devService.GenerateTransactions();
        }

        [HttpGet]
        [Route("Group_UpdateSeedingTeams")]
        public Task Group_UpdateSeedingTeams()
        {
            return _devService.Group_UpdateSeedingTeams();
        }

        [HttpGet]
        [Route("ResyncUserAffiliate")]
        public Task<int> ResyncUserAffiliate()
        {
            return _devService.ResyncUserAffiliate();
        }

        [HttpGet]
        [Route("AffCheck")]
        public Task<int> AffCheck()
        {
            return _devService.Aff_Check();
        }

        [HttpGet]
        [Route("Post_Clean")]
        public Task<int> Post_Clean()
        {
            return _devService.Post_Clean();
        }
        
        [HttpPost]
        [Route("Eval_GenerateEval")]
        public Task<int> Eval_GenerateEval(int year, int month)
        {
            return _devService.Eval_GenerateEval(year, month);
        }

        [HttpPost]
        [Route("Tiktok_CloneGroupHistory")]
        public Task<int> Tiktok_CloneGroupHistory()
        {
            return _devService.Tiktok_CloneGroupHistory();
        }
        
        [HttpPost]
        [Route("Clone_GroupStatsHistory")]
        public Task<int> Clone_GroupStatsHistory()
        {
            return _devService.Clone_GroupStatsHistory();
        }
        
        [HttpPost]
        [Route("EvalConfig_CleanUp")]
        public Task<int> EvalConfig_CleanUp()
        {
            return _devService.EvalConfig_CleanUp();
        }

        [HttpGet]
        [Route("Aff_CleanDupAndEmptyAff")]
        public Task<DevResponse<int>> Aff_CleanDupAndEmptyAff()
        {
            return _devService.Aff_CleanDupAndEmptyAff();
        }

        [HttpGet]
        [Route("GetDuplicateShortLinkPosts")]
        public Task<List<ShortLinkPost>> GetDuplicateShortLinkPosts()
        {
            return _devService.GetDuplicateShortLinkPosts();
        }

        [HttpGet]
        [Route("StaffEvalChangingUser")]
        public Task StaffEvalChangingUser(string changeUserCode = "185", string toUserCode = "018")
        {
            return _devService.StaffEvalChangingUser(changeUserCode, toUserCode);
        }
        
        [HttpGet]
        [Route("GetDuplicateFidPosts")]
        public Task GetDuplicateFidPosts()
        {
            return _devService.GetDuplicateFidPosts();
        }
        
        [HttpGet]
        [Route("GetDuplicateVideos")]
        public Task GetDuplicateVideos()
        {
            return _devService.GetDuplicateVideos();
        }
         
        [HttpGet]
        [Route("GetEmptyAppUserID")]
        public Task GetEmptyAppUserID()
        {
            return _devService.GetEmptyAppUserID();
        }
        
        [HttpGet]
        [Route("GetStaffEvalWithoutTeam")]
        public Task GetStaffEvalWithoutTeam()
        {
            return _devService.GetStaffEvalWithoutTeam();
        }
        
        [HttpGet]
        [Route("GetUserInfoWithoutPosition")]
        public Task<List<string>> GetUserInfoWithoutPosition()
        {
            return _devService.GetUserInfoWithoutPosition();
        }

        [HttpGet]
        [Route("UpdateContract")]
        public Task<int> UpdateContract()
        {
            return _devService.UpdateContract();
        }

        [HttpGet]
        [Route("InitTiktokStatHistory")]
        public Task InitTiktokStatHistory()
        {
            return _devService.InitTiktokStatHistory();
        }
    }
}