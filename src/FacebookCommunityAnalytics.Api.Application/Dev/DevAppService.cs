using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Notifications.Emails;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Services.Emails;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Dev
{
    [RemoteService(IsEnabled = false)]
    [AllowAnonymous]
    public class DevAppService : ApiAppService, IDevAppService
    {
        private readonly ISampleSendEmailService _sampleSendEmailService;

        private readonly IApiConfigurationDomainService _apiConfigurationDomainService;
        private readonly ICrawlDomainService _crawlDomainService;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IAffiliateStatsDomainService _affiliateStatsDomainService;
        private readonly IPayrollDomainService _payrollDomainService;
        private readonly ICampaignDomainService _campaignDomainService;

        private readonly IAccountRepository _accountRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPostRepository _postRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUncrawledPostRepository _uncrawledPostRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IUserDomainService _userDomainService;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IUserAffiliateDomainService _userAffiliateDomainService;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IRepository<TikiAffiliateStat, Guid> _tikiAffStatRepo;
        private readonly IShopinessDomainService _shopinessDomainService;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IUserPayrollRepository _userPayrollRepository;
        private readonly IRepository<Tiktok, Guid> _tiktokRepository;
        private readonly IAffiliateConversionDomainService _affiliateConversionDomainService;
        private readonly IContractRepository _contractRepository;
        private readonly IRepository<ContractTransaction, Guid> _contractTransactionRepository;
        private readonly IClearDataDomainService _clearDataDomainService;
        private readonly IAffiliateConversionRepository _affiliateConversionRepository;
        private readonly IStaffEvaluationDomainService _staffEvaluationDomainService;
        private readonly IRepository<GroupStatsHistory, Guid> _groupStatsHistoriesRepository;
        private readonly IUserEvaluationConfigurationRepository _userEvaluationConfigurationRepository;
        private readonly IStaffEvaluationRepository _staffEvaluationRepository;
        private readonly IRepository<TiktokStat, Guid> _tiktokStatRepository;

        public DevAppService(
            IdentityUserManager userManager,
            IOrganizationDomainService organizationDomainService,
            IRepository<AppUser, Guid> userRepository,
            IUserInfoRepository userInfoRepository,
            IUserDomainService userDomainService,
            IPostRepository postRepository,
            IGroupRepository groupRepository,
            ICrawlDomainService crawlDomainService,
            IApiConfigurationDomainService apiConfigurationDomainService,
            IUserAffiliateRepository userAffiliateRepository,
            ICampaignRepository campaignRepository,
            IUncrawledPostRepository uncrawledPostRepository,
            ISampleSendEmailService sampleSendEmailService,
            IAffiliateStatsDomainService affiliateStatsDomainService,
            IAccountRepository accountRepository,
            ICampaignDomainService campaignDomainService,
            IAuditLogRepository auditLogRepository,
            IRepository<TikiAffiliateStat, Guid> tikiAffStatRepo,
            IShopinessDomainService shopinessDomainService,
            IUserAffiliateDomainService userAffiliateDomainService,
            IPayrollDomainService payrollDomainService,
            IPayrollRepository payrollRepository,
            IUserPayrollRepository userPayrollRepository,
            IContractRepository contractRepository,
            IRepository<ContractTransaction, Guid> contractTransactionRepository,
            IRepository<Tiktok, Guid> tiktokRepository,
            IAffiliateConversionDomainService affiliateConversionDomainService,
            IClearDataDomainService clearDataDomainService,
            IAffiliateConversionRepository affiliateConversionRepository,
            IStaffEvaluationDomainService staffEvaluationDomainService,
            IRepository<GroupStatsHistory, Guid> groupStatsHistoriesRepository,
            IUserEvaluationConfigurationRepository userEvaluationConfigurationRepository,
            IStaffEvaluationRepository staffEvaluationRepository,
            IRepository<TiktokStat, Guid> tiktokStatRepository)
        {
            _postRepository = postRepository;
            _groupRepository = groupRepository;
            _crawlDomainService = crawlDomainService;
            _apiConfigurationDomainService = apiConfigurationDomainService;
            _userAffiliateRepository = userAffiliateRepository;
            _campaignRepository = campaignRepository;
            _uncrawledPostRepository = uncrawledPostRepository;
            _sampleSendEmailService = sampleSendEmailService;
            _affiliateStatsDomainService = affiliateStatsDomainService;
            _accountRepository = accountRepository;
            _campaignDomainService = campaignDomainService;
            _userRepository = userRepository;
            _userInfoRepository = userInfoRepository;
            _userManager = userManager;
            _organizationDomainService = organizationDomainService;
            _userDomainService = userDomainService;
            _auditLogRepository = auditLogRepository;
            _tikiAffStatRepo = tikiAffStatRepo;
            _shopinessDomainService = shopinessDomainService;
            _userAffiliateDomainService = userAffiliateDomainService;
            _payrollDomainService = payrollDomainService;
            _payrollRepository = payrollRepository;
            _userPayrollRepository = userPayrollRepository;
            _tiktokRepository = tiktokRepository;
            _affiliateConversionDomainService = affiliateConversionDomainService;
            _clearDataDomainService = clearDataDomainService;
            _affiliateConversionRepository = affiliateConversionRepository;
            _staffEvaluationDomainService = staffEvaluationDomainService;
            _groupStatsHistoriesRepository = groupStatsHistoriesRepository;
            _userEvaluationConfigurationRepository = userEvaluationConfigurationRepository;
            _staffEvaluationRepository = staffEvaluationRepository;
            _tiktokStatRepository = tiktokStatRepository;
            _contractRepository = contractRepository;
            _contractTransactionRepository = contractTransactionRepository;
        }

        private string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) { return ip.ToString(); }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public string Version()
        {
            return GetLocalIpAddress();
        }

        public async Task<GlobalConfiguration> Config_Global()
        {
            return GlobalConfiguration;
        }

        public async Task<PayrollConfiguration> Config_Payroll()
        {
            return _apiConfigurationDomainService.GetPayrollConfiguration();
        }

        #region Affiliate

        public async Task<DevAffiliateDetailModel> Aff_Get(string userCode)
        {
            var userinfo = await _userInfoRepository.FindAsync(_ => _.Code == userCode);
            if (userinfo != null)
            {
                var now = DateTime.UtcNow;
                var (fromDateTime, toDateTime) = GetPayrollDateTime(now.Year, now.Month);

                var posts = await _postRepository.GetListExtendAsync
                (
                    createdDateTimeMin: fromDateTime,
                    createdDateTimeMax: toDateTime,
                    appUserId: userinfo.AppUserId.Value,
                    postContentType: PostContentType.Affiliate
                );

                var affs = await _userAffiliateRepository.GetListAsync
                (
                    appUserId: userinfo.AppUserId.Value,
                    createdAtMin: fromDateTime,
                    createdAtMax: toDateTime
                );

                var shortUrlsFromPosts = posts.Where(_ => _.Shortlinks.IsNotNullOrEmpty()).SelectMany(_ => _.Shortlinks).Distinct().ToList();
                var shortUrlsFromShopiness = affs.Select(_ => _.AffiliateUrl).Distinct().ToList();
                return new DevAffiliateDetailModel
                {
                    UserCode = userinfo.Code,
                    PostCount = posts.Count,
                    Urls = posts.Select(_ => _.Url).ToList(),
                    ShortUrlsFromPosts = shortUrlsFromPosts,
                    ShortUrlsFromPostsCount = shortUrlsFromPosts.Count,
                    ShortUrlsFromShopiness = shortUrlsFromShopiness,
                    ShortUrlsFromShopinessCount = shortUrlsFromShopiness.Count,
                    ClickCount = affs.Sum(_ => _.AffConversionModel.ClickCount),
                    ConversionCount = affs.Sum(_ => _.AffConversionModel.ConversionCount)
                };
            }

            return new DevAffiliateDetailModel();
        }

        public async Task<AffTopLinkSummaryApiResponse> Aff_GetTopLinkAffSummary()
        {
            var affTopLinkSummaryApiResponse = await _affiliateStatsDomainService.GetTopLinkAffSummary(new GeneralStatsApiRequest());
            using (var writer = new StreamWriter("TopClicks.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<AffTopLinkSummaryApiItem>();
                await csv.NextRecordAsync();
                foreach (var record in affTopLinkSummaryApiResponse.TopClicks)
                {
                    csv.WriteRecord(record);
                    await csv.NextRecordAsync();
                }
            }

            using (var writer = new StreamWriter("TopAmount.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<AffTopLinkSummaryApiItem>();
                await csv.NextRecordAsync();
                foreach (var record in affTopLinkSummaryApiResponse.TopAmounts)
                {
                    csv.WriteRecord(record);
                    await csv.NextRecordAsync();
                }
            }

            await using (var writer = new StreamWriter("TopConversions.csv"))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<AffTopLinkSummaryApiItem>();
                await csv.NextRecordAsync();
                foreach (var record in affTopLinkSummaryApiResponse.TopConversions)
                {
                    csv.WriteRecord(record);
                    await csv.NextRecordAsync();
                }
            }

            return affTopLinkSummaryApiResponse;
        }

        public async Task<int> Aff_ExportTopLazada()
        {
            var userConversions = new List<UserConversion>();
            var now = DateTime.UtcNow;
            var (fromTime, toTime) = GetPayrollDateTime(now.Year, now.Month);
            var userAffiliates = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties
                (marketplaceType: MarketplaceType.Lazada, createdAtMin: fromTime, createdAtMax: toTime);
            var groupAffiliates = userAffiliates.Where(_ => _.UserAffiliate.AppUserId != null).GroupBy(_ => _.UserAffiliate.AppUserId).ToList();
            foreach (var group in groupAffiliates)
            {
                var userConversion = new UserConversion()
                {
                    UserName = group.First().AppUser.UserName,
                    Conversion = group.Sum(_ => _.UserAffiliate.AffConversionModel.ConversionCount)
                };
                userConversions.Add(userConversion);
                Debug.WriteLine($"--------------------{userConversion}--------------------------");
            }

            userConversions = userConversions.OrderByDescending(_ => _.Conversion).ToList();
            await using var writer = new StreamWriter(@"D:/TopLazadaAffiliate.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(userConversions);
            return userConversions.Count;
        }

        #endregion

        #region Crawl

        public async Task<int> Crawl_InitUncrawledPosts(bool initNewPosts = false)
        {
            var (fromDate, toDate) = GetPayrollDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            return await _crawlDomainService.InitUncrawledPosts(fromDate, toDate, initNewPosts);
        }

        public async Task<int> Crawl_InitCrawlCampaignPosts()
        {
            return await _crawlDomainService.InitCampaignPosts();
        }

        public async Task<CampaignPostResponse> Crawl_InitCrawlForCampaign(CampaignPostRequest request)
        {
            if (request.campaignCode.IsNullOrWhiteSpace()) { return new CampaignPostResponse(); }

            var campaign = _campaignRepository.FirstOrDefault(c => c.Code == request.campaignCode);
            if (campaign == null) { return new CampaignPostResponse(); }

            var posts = _postRepository.Where(_ => _.CampaignId == campaign.Id).ToList();

            var uncrawledPosts = posts.Select
                (
                    p => new UncrawledPost
                    {
                        Url = p.Url,
                        PostSourceType = p.PostSourceType,
                        UpdatedAt = DateTime.UtcNow.AddYears(-10)
                    }
                )
                .ToList();
            if (uncrawledPosts.IsNotNullOrEmpty()) { await _uncrawledPostRepository.InsertManyAsync(uncrawledPosts); }

            return new CampaignPostResponse()
            {
                Items = uncrawledPosts.Select
                    (
                        _ => new CampaignPostResponseItem
                        {
                            Url = _.Url,
                            UpdatedAt = _.UpdatedAt,
                            PostSourceType = _.PostSourceType
                        }
                    )
                    .ToList()
            };
        }

        public async Task<OldestUncrawledPostsResponse> Crawl_GetOldestUncrawledPosts(OldestUncrawledPostsRequest request)
        {
            var uncrawledPosts = (await _uncrawledPostRepository.GetListAsync()).OrderBy(_ => _.UpdatedAt).ToList();

            return new OldestUncrawledPostsResponse
            {
                Items = uncrawledPosts.Take(request.Count)
                    .Select
                    (
                        _ => new OldestUncrawledPostsResponseItem
                        {
                            Url = _.Url,
                            UpdatedAt = _.UpdatedAt,
                            PostSourceType = _.PostSourceType
                        }
                    )
                    .ToList()
            };
        }

        public async Task Crawl_InitGroupUserCrawlTeams(List<string> source)
        {
            await _crawlDomainService.InitGroupUserCrawlTeams(source);
        }

        #endregion

        #region User

        public async Task<List<string>> Users_GetDuplicated()
        {
            var users = await _userRepository.GetListAsync();
            var dupEmailUsers = users.GroupBy(_ => _.Email).Where(_ => _.Count() > 1).ToList();
            var dupUserNameUsers = users.GroupBy(_ => _.UserName).Where(_ => _.Count() > 1).ToList();

            var userInfos = await _userInfoRepository.GetListAsync();

            var noCodeUserInfos = userInfos.Where(_ => _.Code.IsNullOrWhiteSpace()).ToList();
            var noCodeUserIds = noCodeUserInfos.Select(_ => _.AppUserId).ToList();
            var noCodeUsers = users.Where(u => noCodeUserIds.Contains(u.Id)).ToList();

            var dupCodeUserInfos = userInfos.GroupBy(_ => _.Code).Where(_ => _.Count() > 1).ToList();

            var list = new List<string>();
            list.AddRange(dupEmailUsers.Select(_ => $"dupEmail:{_.Key}"));
            list.AddRange(dupUserNameUsers.Select(_ => $"dupUsername:{_.Key}"));
            list.AddRange(noCodeUsers.Select(_ => $"noCode:{_.Email}"));
            list.AddRange(dupCodeUserInfos.Select(_ => $"dupCode:{_.Key}"));

            return list;
        }

        public async Task<List<string>> Users_GetNoRoles()
        {
            var users = await _userRepository.GetListAsync();

            var usersNoRole = new List<AppUser>();
            foreach (var appUser in users)
            {
                var identityUser = await _userManager.GetByIdAsync(appUser.Id);
                if (await _userDomainService.IsUserNoRole(identityUser)) { usersNoRole.Add(appUser); }
            }

            return usersNoRole.Select(_ => _.UserName).ToList();
        }

        public async Task<List<string>> Users_GetNoTeams()
        {
            var users = await _userRepository.GetListAsync();
            var usersWithoutTeam = new List<AppUser>();
            foreach (var appUser in users)
            {
                var orgUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() {UserId = CurrentUser.Id});
                if (orgUnits.IsNullOrEmpty()) { usersWithoutTeam.Add(appUser); }
            }

            return usersWithoutTeam.Select(_ => _.Email).ToList();
        }

        public async Task<List<string>> Users_GetNoPosts()
        {
            var users = await _userRepository.GetListAsync();
            var posts = await _postRepository.GetListAsync();

            var usersNoPosts = new Dictionary<AppUser, UserInfo>();
            foreach (var appUser in users)
            {
                var p = posts.Where(_ => _.AppUserId == appUser.Id).ToList();
                if (p.IsNullOrEmpty())
                {
                    var userInfo = await _userInfoRepository.GetByUserIdAsync(appUser.Id);
                    usersNoPosts.Add(appUser, userInfo);
                }
            }

            return usersNoPosts.Select(_ => $"{_.Key.Email}").ToList();
        }

        public async Task<List<AppUserDto>> Users_AddDefaultStaffRole()
        {
            var users = await _userRepository.GetListAsync(user => user.UserName.Contains("admin"));

            var errors = new List<AppUser>();
            foreach (var appUser in users)
            {
                var identityUser = await _userManager.GetByIdAsync(appUser.Id);
                if (identityUser != null)
                {
                    try { await _userManager.SetRolesAsync(identityUser, new[] {RoleConsts.Staff, RoleConsts.Admin}); }
                    catch (Exception) { errors.Add(appUser); }
                }
            }

            return ObjectMapper.Map<List<AppUser>, List<AppUserDto>>(errors);
        }

        public async Task<List<string>> Users_SetRoleForNoRole()
        {
            List<string> setRoleUsers = new();
            var noRoleUsers = await GetUsers_NoRoleIds();
            foreach (var userId in noRoleUsers)
            {
                var user = await _userManager.GetByIdAsync(userId);
                //await _userManager.AddToRolesAsync(user, new[] {RoleConst.Staff});
                await _userManager.ResetAuthenticatorKeyAsync(user);
                setRoleUsers.Add(user.UserName);
            }

            return setRoleUsers;
        }

        public async Task<List<string>> Users_GetInMultipleTeams()
        {
            var users = await _userRepository.GetListAsync();
            var manyTeamUsers = new List<string>();
            foreach (var appUser in users)
            {
                var identityUser = await _userManager.GetByIdAsync(appUser.Id);
                if (identityUser != null)
                {
                    var org = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() {UserId = CurrentUser.Id});
                    if (org.Count > 1) { manyTeamUsers.Add(appUser.UserName); }
                }
            }

            return manyTeamUsers;
        }

        public async Task<List<string>> User_DeactiveNoRole()
        {
            return await _userDomainService.DeactiveUserNoRole();
        }

        private async Task<List<Guid>> GetUsers_NoRoleIds()
        {
            var users = await _userRepository.GetListAsync();

            var usersNoRole = new List<AppUser>();
            foreach (var appUser in users)
            {
                var identityUser = await _userManager.GetByIdAsync(appUser.Id);
                if (identityUser != null)
                {
                    try
                    {
                        var roles = await _userManager.GetRolesAsync(identityUser);
                        if (roles.IsNullOrEmpty()) { usersNoRole.Add(appUser); }
                    }
                    catch (ArgumentNullException)
                    {
                        usersNoRole.Add(appUser);
                        //await _userManager.AddToRoleAsync(identityUser, RoleConst.Staff);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        //throw;
                    }
                }
            }

            return usersNoRole.Select(_ => _.Id).ToList();
        }

        #endregion

        #region UserInfo

        public async Task<int> UserInfo_ClearAverageReaction()
        {
            var userInfos = await _userInfoRepository.GetListAsync();
            var updateUserInfos = userInfos.Select
                (
                    _ =>
                    {
                        _.AverageReactionTop100Post = 0;
                        return _;
                    }
                )
                .ToList();
            await _userInfoRepository.UpdateManyAsync(updateUserInfos);
            return updateUserInfos.Count;
        }

        public async Task<int> UserInfo_ExportAverageReaction()
        {
            var averageReactionUserInfos = new List<AverageReactionUserInfo>();
            var userInfos = await _userInfoRepository.GetListAsync();
            foreach (var userInfo in userInfos)
            {
                var averageReactionUserInfo = new AverageReactionUserInfo()
                {
                    UserCode = userInfo.Code,
                    AverageReaction = userInfo.AverageReactionTop100Post
                };
                averageReactionUserInfos.Add(averageReactionUserInfo);
            }

            await using var writer = new StreamWriter(@"D:/AverageReactionUserInfo.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(averageReactionUserInfos);
            return averageReactionUserInfos.Count;
        }

        public async Task<int> UserInfo_ImportAverageReaction()
        {
            var userInfos = await _userInfoRepository.GetListAsync();
            using var reader = new StreamReader(@"C:/AverageReactionUserInfo.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var averageReactionUserInfos = csv.GetRecords<AverageReactionUserInfo>().ToList();
            foreach (var averageReactionUserInfo in averageReactionUserInfos)
            {
                var num = int.Parse(averageReactionUserInfo.UserCode).ToString();
                var userInfo = userInfos.FirstOrDefault(_ => _.Code.TrimStart('0') == num.TrimStart('0'));
                if (userInfo != null) { userInfo.AverageReactionTop100Post = averageReactionUserInfo.AverageReaction; }
            }

            await _userInfoRepository.UpdateManyAsync(userInfos);
            return averageReactionUserInfos.Count;
        }

        public async Task<DevResponse<string>> UserInfo_RemoveDuplicates()
        {
            var userInfos = await _userInfoRepository.GetListAsync();
            var groups = userInfos.GroupBy(x => x.Code);

            var userinfoToDeletes = new List<UserInfo>();
            var codes = new List<string>();
            foreach (var @group in groups)
            {
                if (@group.Count() > 1)
                {
                    var first = group.FirstOrDefault();
                    if (first == null)
                    {
                        break;
                    }

                    foreach (var userInfo in group.Skip(1))
                    {
                        var posts = await _postRepository.GetListExtendAsync(appUserId: userInfo.AppUserId);
                        foreach (var post in posts)
                        {
                            post.AppUserId = first.AppUserId;
                            await _postRepository.UpdateAsync(post);
                        }

                        userinfoToDeletes.Add(userInfo);
                    }

                    codes.Add(group.Key);
                }
            }

            await _userInfoRepository.DeleteManyAsync(userinfoToDeletes);

            return new DevResponse<string> {Payload = codes};
        }

        public async Task<int> UserInfo_UpdateSalaryAndPosition()
        {
            return 0;
            // var ST1 = new List<string>() {"GDL - ST1"};
            // var st1UserInfos = await _organizationDomainService.GetUsersByOrgNames(ST1);
            // st1UserInfos = st1UserInfos.Select
            //     (
            //         _ =>
            //         {
            //             _.UserPosition = UserPosition.CommunitySeedingStaff_ST1;
            //             _.SalaryAmount = 2000000;
            //             return _;
            //         }
            //     )
            //     .ToList();
            // await _userInfoRepository.UpdateManyAsync(st1UserInfos);
            //
            // var seedingTeams = new List<string>()
            // {
            //     "GDL - ST2",
            //     "GDL - ST3",
            //     "GDL - ST5"
            // };
            // var seedingUserInfos = await _userDomainService.GetUsersByOrgNames(seedingTeams);
            // seedingUserInfos = seedingUserInfos.Select
            //     (
            //         _ =>
            //         {
            //             _.UserPosition = UserPosition.CommunitySeedingStaff;
            //             _.SalaryAmount = 1200000;
            //             return _;
            //         }
            //     )
            //     .ToList();
            // await _userInfoRepository.UpdateManyAsync(seedingUserInfos);
            //
            // var affiliateTeams = new List<string>()
            // {
            //     "GDL - AT1",
            //     "GDL - AT2"
            // };
            // var affiliateUserInfos = await _userDomainService.GetUsersByOrgNames(affiliateTeams);
            // affiliateUserInfos = affiliateUserInfos.Select
            //     (
            //         _ =>
            //         {
            //             _.UserPosition = UserPosition.CommunityAffiliateStaff;
            //             _.SalaryAmount = 1500000;
            //             return _;
            //         }
            //     )
            //     .ToList();
            // await _userInfoRepository.UpdateManyAsync(affiliateUserInfos);
            //
            // return seedingUserInfos.Count + affiliateUserInfos.Count;
        }

        public async Task<Dictionary<string, List<DuplicateSeedingAccountModel>>> UserInfo_GetDuplicateSeedingAccounts()
        {
            var result = new Dictionary<string, List<DuplicateSeedingAccountModel>>();
            var userDetails = await _userDomainService.GetUserDetails
            (
                new ApiUserDetailsRequest
                {
                    GetSystemUsers = false,
                    GetTeamUsers = true,
                    GetActiveUsers = true,
                }
            );

            foreach (var item in userDetails)
            {
                var info = item.Info;
                var dupItems = new List<DuplicateSeedingAccountModel>();

                var seedingAccounts = info?.Accounts;
                if (seedingAccounts.IsNotNullOrEmpty())
                {
                    foreach (var account in seedingAccounts)
                    {
                        var existing = userDetails.Where(x => x.Info.Code != info.Code && x.Info.Accounts.Any(_ => _.Fid.Contains(account.Fid))).ToList();
                        if (existing.IsNotNullOrEmpty())
                        {
                            dupItems.Add
                            (
                                new DuplicateSeedingAccountModel
                                {
                                    Fid = account.Fid,
                                    Usernames = existing.Select(_ => _.User.UserName).ToList()
                                }
                            );
                        }
                    }

                    if (seedingAccounts.Count <=1)
                    {
                        dupItems.Add(new DuplicateSeedingAccountModel
                        {
                            Fid = seedingAccounts.FirstOrDefault()?.Fid,
                            Usernames = new List<string>
                            {
                                $"ONLY {seedingAccounts.Count} SEEDING ACCOUNTS"
                            }

                        });
                    }
                }
                else
                {
                    if (!item.Team.DisplayName.IsIn(TeamMemberConsts.Sale
                            , TeamMemberConsts.CommunitySpecialist
                            , TeamMemberConsts.CommunitySupervisor
                            , TeamMemberConsts.TikTok
                            ))
                    {
                        dupItems.Add
                        (
                            new DuplicateSeedingAccountModel
                            {
                                Fid = "missing fid/fb link",
                            }
                        );
                    }
                }

                if (dupItems.IsNotNullOrEmpty())
                {
                    var user = await _userRepository.GetAsync(_ => _.Id == info.AppUserId);
                    result.Add(user.UserName, dupItems);
                }
            }

            return result;
        }

        #endregion

        #region Post

        public async Task Posts_CleanUp()
        {
        }

        public async Task Posts_CleanUpUrl()
        {
            var fanpagePosts = await _postRepository.GetListAsync
            (
                p =>
                    p.PostSourceType == PostSourceType.Page && p.Url.Contains("permalink")
            );

            if (fanpagePosts.Count != 0)
            {
                foreach (var post in fanpagePosts) { post.Url = post.Url.Replace("permalink", "posts"); }

                await _postRepository.UpdateManyAsync(fanpagePosts);
            }

            var groupPosts = await _postRepository.GetListAsync
            (
                p =>
                    p.PostSourceType == PostSourceType.Group && p.Url.Contains("posts")
            );

            if (groupPosts.Count != 0)
            {
                foreach (var post in groupPosts) { post.Url = post.Url.Replace("posts", "permalink"); }

                await _postRepository.UpdateManyAsync(groupPosts);
            }

            var posts = await _postRepository.GetListAsync();

            var check = posts.DistinctBy(_ => _.Url).ToList();

            var groupByPosts = posts.GroupBy(_ => _.Url).ToList();

            var deletePosts = new List<Post>();
            foreach (var item in groupByPosts)
            {
                if (item.Count() > 1)
                {
                    var temp = item.OrderByDescending(_ => _.TotalCount).ToList();

                    temp.Remove(temp.First());
                    deletePosts.AddRange(temp);
                }
            }

            foreach (var batch in deletePosts.Partition(100)) { await _postRepository.DeleteManyAsync(batch); }
        }

        public virtual async Task<List<PostWithNavigationPropertiesDto>> Posts_GetInvalidUrls()
        {
            var posts = await _postRepository.GetListWithNavigationPropertiesAsync();
            var dtos =
                ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostWithNavigationPropertiesDto>>(posts);

            var invalidPosts = new List<PostWithNavigationPropertiesDto>();
            foreach (var post in dtos)
            {
                if (!FacebookHelper.IsValidGroupPostUrl(post.Post.Url)) { invalidPosts.Add(post); }
            }

            return invalidPosts;
        }

        public async Task<DevResponse<string>> Posts_GetNoCreatedAt()
        {
            var posts = await _postRepository.GetListAsync();
            var noCreatedAtPosts = posts.Where(p => p.CreatedDateTime == null && p.IsNotAvailable == false).ToList();

            var postDtos = ObjectMapper.Map<List<Post>, List<PostDto>>(noCreatedAtPosts);

            return new DevResponse<string>
            {
                Count = postDtos.Count,
                Payload = postDtos.Select(_ => _.Url).ToList()
            };
        }

        public async Task<DevResponse<string>> Posts_GetNotAvailable()
        {
            var unavailablePosts = await _postRepository.GetListAsync(_ => _.IsNotAvailable);

            return new DevResponse<string>
            {
                Count = unavailablePosts.Count,
                Payload = unavailablePosts.Select(_ => _.Url).ToList()
            };
        }

        public async Task<DevResponse<string>> Posts_GetNoUsers()
        {
            var posts = await _postRepository.GetListAsync();
            var noUserPosts = new List<Post>();

            foreach (var post in posts)
            {
                if (post.AppUserId.HasValue)
                {
                    var existing = await _userRepository.FindAsync(u => u.Id == post.AppUserId);
                    if (existing == null) { noUserPosts.Add(post); }
                }
                else { noUserPosts.Add(post); }
            }

            return new DevResponse<string>
            {
                Count = noUserPosts.Count,
                Payload = noUserPosts.Select(_ => _.Url).ToList()
            };
        }

        public async Task<DevResponse<string>> Posts_GetUncrawled()
        {
            var uncrawledPosts = await _crawlDomainService.GetUncrawledPosts(new UncrawledPostsApiRequest());

            var response = new DevResponse<string>
            {
                Count = (int) uncrawledPosts.Count,
                Payload = uncrawledPosts.Items.Select(i => i.Url).ToList()
            };

            return response;
        }

        public async Task<PostConvertResponse> Posts_Convert(PostConvertRequest request)
        {
            // var fromDateTime = new DateTime(2021, 6, 14, 17, 0, 0);
            // var toDateTime = new DateTime(2021, 7, 14, 17, 0, 0);
            if (request.Usercode.IsNullOrEmpty()) return new PostConvertResponse();

            var userinfo = await _userInfoRepository.FindAsync(_ => _.Code == request.Usercode);
            if (userinfo == null) return new PostConvertResponse();

            var postNavs = await _postRepository
                .GetListWithNavigationPropertiesAsync
                (
                    createdDateTimeMin: request.FromDateTime,
                    createdDateTimeMax: request.ToDateTime,
                    postContentType: request.FromContentType,
                    postCopyrightType: request.FromCopyrightType,
                    appUserId: userinfo.AppUserId
                );

            var posts = postNavs
                .Select(_ => _.Post)
                .ToList();

            foreach (var post in posts)
            {
                if (request.ToContentType.HasValue) { post.PostContentType = request.ToContentType.Value; }

                if (request.ToCopyrightType.HasValue) { post.PostCopyrightType = request.ToCopyrightType.Value; }
            }

            if (posts.IsNotNullOrEmpty()) { await _postRepository.UpdateManyAsync(posts); }

            return new PostConvertResponse
            {
                Count = posts.Count,
                Urls = posts.Select(_ => _.Url).ToList()
            };
        }

        public async Task<DevResponse<string>> Posts_UpdateIsValid(PostUpdateIsValidRequest request)
        {
            var now = DateTime.UtcNow;
            var (fromDateTime, toDateTime) = GetPayrollDateTime(now.Year, now.Month);

            var userInfo = await _userInfoRepository.FindAsync(_ => _.Code == request.UserCode);
            if (userInfo == null) return new DevResponse<string>();

            var postNavs = await _postRepository.GetListWithNavigationPropertiesExtendAsync
            (
                createdDateTimeMax: toDateTime,
                createdDateTimeMin: fromDateTime,
                appUserId: userInfo.AppUserId
            );

            var posts = postNavs.Select(_ => _.Post).ToList();
            foreach (var post in posts) { post.IsValid = request.IsValid; }

            if (posts.IsNotNullOrEmpty()) await _postRepository.UpdateManyAsync(posts);

            return new DevResponse<string>
            {
                Count = posts.Count,
                Payload = posts.Select(_ => _.Url).ToList()
            };
        }

        #endregion

        public async Task<bool> Email_SendSampleEmail(string email)
        {
            try
            {
                await _sampleSendEmailService.Send(new SampleEmailModel());
                return true;
            }
            catch (Exception e) { throw new ApiException(JsonConvert.SerializeObject(e)); }
        }

        public async Task<DevResponse<int>> Accounts_ChangeStatusAndType(Accounts_ChangeStatusAndTypeRequest request)
        {
            var accs = await _accountRepository.GetListAsync(accountType: request.FromType, accountStatus: request.FromStatus);

            foreach (var item in accs.Take(request.Count))
            {
                item.AccountType = request.ToType;
                item.AccountStatus = request.ToStatus;
            }

            if (accs.IsNotNullOrEmpty())
            {
                await _accountRepository.UpdateManyAsync(accs);
            }

            return new DevResponse<int>
            {
                Count = accs.Count,
                Message = $"{JsonConvert.SerializeObject(request)}"
            };
        }

        public async Task<DevResponse<int>> Temp()
        {
            return await DoTemp();
        }

        private async Task<DevResponse<int>> DoTemp()
        {
            int count = 0;

            return new DevResponse<int> {Count = count};
        }

        public async Task<string> Temp(DateTime dateTime)
        {
            var s = $"Current TimeZone: {TimeZoneInfo.Local.ToString()}. CurrentTime: {DateTime.UtcNow}. Value = {dateTime}. ToUniversal = {dateTime.ToUniversalTime()}";
            return s;
        }

        public async Task<DevResponse<int>> Aff_CleanDupAndEmptyAff()
        {
            var emptyExternalAffs = (await _userAffiliateRepository.GetListAsync(_ => _.AffiliateUrl == null)).ToList();
            foreach (var aff in emptyExternalAffs) { await _userAffiliateRepository.HardDeleteAsync(aff); }

            var notEmptyExternalAffs = (await _userAffiliateRepository.GetListAsync(_ => _.AffiliateUrl != null)).ToList();
            var notEmptyGroups = notEmptyExternalAffs.GroupBy(_ => _.AffiliateUrl).ToList();
            var deleteAffs = new List<UserAffiliate>();

            foreach (var g in notEmptyGroups)
            {
                var list = g.OrderByDescending(_ => _.AffConversionModel.ConversionAmount).ThenByDescending(_ => _.AffConversionModel.ClickCount).Skip(1).ToList();
                if (list.IsNotNullOrEmpty()) { deleteAffs.AddRange(list); }
            }

            foreach (var aff in deleteAffs) { await _userAffiliateRepository.HardDeleteAsync(aff); }

            return new DevResponse<int> {Count = emptyExternalAffs.Count + deleteAffs.Count};
        }

        private static KeyValuePair<bool, string> CleanShortUrls(string url)
        {
            var shortKey = url.Split('/').Last();
            var baseAffUrl = url.Split('/').SkipLast(1).ToList();
            if (shortKey.Length <= 6) { return new KeyValuePair<bool, string>(false, url); }

            var cleanShortKey = shortKey.Remove(6);
            return new KeyValuePair<bool, string>(true, $"{string.Join("/", baseAffUrl)}/{cleanShortKey}|");
        }

        public async Task<int> ExportTopConversionAndReactionPosts()
        {
            var conversionAndReactionPosts = new List<ConversionAndReactionPost>();
            var userAffiliates = await _userAffiliateRepository.GetListAsync();
            userAffiliates = userAffiliates.Where
                (
                    _ => _.AffiliateUrl.IsNotNullOrEmpty()
                         && (_.AffConversionModel.ClickCount != 0
                             || _.AffConversionModel.ConversionCount != 0
                             || _.AffConversionModel.ConversionAmount != 0
                             || _.AffConversionModel.CommissionAmount != 0
                             || _.AffConversionModel.CommissionBonusAmount != 0)
                )
                .ToList();
            var fromDateTime = DateTime.SpecifyKind(new DateTime(2021, 7, 1), DateTimeKind.Utc);
            var posts = await _postRepository.GetListExtendAsync(createdDateTimeMin: fromDateTime, createdDateTimeMax: DateTime.UtcNow, postContentType: PostContentType.Affiliate);
            int i = 0;
            foreach (var navPost in posts)
            {
                i++;
                Debug.WriteLine($"-------------Post{i}: {navPost.Url}--------------");
                var clickCount = 0;
                var conversionCount = 0;
                if (navPost.Shortlinks.IsNotNullOrEmpty())
                {
                    var shortLinks = navPost.Shortlinks;
                    foreach (var shortLink in shortLinks)
                    {
                        if (shortLink.IsNotNullOrEmpty())
                            Debug.WriteLine($"-------------Shortlink: {shortLink}--------------");
                        var userAffiliate = userAffiliates.FirstOrDefault(_ => string.Equals(_.AffiliateUrl.Trim(), shortLink.Trim(), StringComparison.CurrentCultureIgnoreCase));
                        if (userAffiliate != null)
                        {
                            clickCount += userAffiliate.AffConversionModel.ClickCount;
                            conversionCount += userAffiliate.AffConversionModel.ConversionCount;
                        }
                    }
                }

                var rate = clickCount > 0 ? ((double) conversionCount / clickCount) * 100 : 0;
                var conversionAndReactionPost = new ConversionAndReactionPost()
                {
                    Url = navPost.Url,
                    Total = navPost.TotalCount,
                    Like = navPost.LikeCount,
                    Comment = navPost.CommentCount,
                    Share = navPost.ShareCount,
                    Click = clickCount,
                    Conversion = conversionCount,
                    Rate = rate.ToString("F")
                };
                conversionAndReactionPosts.Add(conversionAndReactionPost);
            }

            var topReactionPosts = conversionAndReactionPosts.OrderByDescending(_ => _.Total).Take(300).ToList();
            using (var writer = new StreamWriter("TopReactions.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<ConversionAndReactionPost>();
                await csv.NextRecordAsync();
                foreach (var record in topReactionPosts)
                {
                    csv.WriteRecord(record);
                    await csv.NextRecordAsync();
                }
            }

            var topConversionPosts = conversionAndReactionPosts.OrderByDescending(_ => _.Conversion).Take(300).ToList();
            using (var writer = new StreamWriter("TopConversions.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<ConversionAndReactionPost>();
                await csv.NextRecordAsync();
                foreach (var record in topConversionPosts)
                {
                    csv.WriteRecord(record);
                    await csv.NextRecordAsync();
                }
            }

            return conversionAndReactionPosts.Count;
        }

        public async Task<int> SyncUserAffiliate()
        {
            var affs = await _userAffiliateRepository.GetListAsync();
            var updateAffs = new List<UserAffiliate>();

            int i = 0;
            foreach (var aff in affs)
            {
                i++;
                Debug.WriteLine($"---------------------{i}----------------------");
                updateAffs.Add(_userAffiliateDomainService.InitUserAffiliateCreation(aff));
            }

            foreach (var partition in updateAffs.Partition(1000)) { await _userAffiliateRepository.UpdateManyAsync(partition); }

            return updateAffs.Count;
        }

        public async Task<int> ClearAverageReactionUserInfo()
        {
            var userInfos = await _userInfoRepository.GetListAsync();
            var updateUserInfos = userInfos.Select
                (
                    _ =>
                    {
                        _.AverageReactionTop100Post = 0;
                        return _;
                    }
                )
                .ToList();
            await _userInfoRepository.UpdateManyAsync(updateUserInfos);
            return updateUserInfos.Count;
        }

        public async Task<int> ExportAverageReactionUserInfo()
        {
            var averageReactionUserInfos = new List<AverageReactionUserInfo>();
            var userInfos = await _userInfoRepository.GetListAsync();
            foreach (var userInfo in userInfos)
            {
                var averageReactionUserInfo = new AverageReactionUserInfo()
                {
                    UserCode = userInfo.Code,
                    AverageReaction = userInfo.AverageReactionTop100Post
                };
                averageReactionUserInfos.Add(averageReactionUserInfo);
            }

            await using var writer = new StreamWriter(@"D:/AverageReactionUserInfo.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(averageReactionUserInfos);
            return averageReactionUserInfos.Count;
        }

        public async Task<int> ImportAverageReactionUserInfo()
        {
            var userInfos = await _userInfoRepository.GetListAsync();
            using var reader = new StreamReader(@"C:/AverageReactionUserInfo.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var averageReactionUserInfos = csv.GetRecords<AverageReactionUserInfo>().ToList();
            foreach (var averageReactionUserInfo in averageReactionUserInfos)
            {
                var num = int.Parse(averageReactionUserInfo.UserCode).ToString();
                var userInfo = userInfos.FirstOrDefault(_ => _.Code == num);
                if (userInfo != null) { userInfo.AverageReactionTop100Post = averageReactionUserInfo.AverageReaction; }
            }

            await _userInfoRepository.UpdateManyAsync(userInfos);
            return averageReactionUserInfos.Count;
        }

        public async Task CleanUpData()
       {
           var userConfigs = await _userEvaluationConfigurationRepository.GetListAsync();
           var invalidUserConfigs = userConfigs.Where(x => x.TeamId.IsNullOrEmpty() && x.AppUserId.IsNotNullOrEmpty()).ToList();

           var deleteConfig = new List<UserEvaluationConfiguration>();
           foreach (var config in invalidUserConfigs)
           {
               deleteConfig.Add(config);
           }

           await _userEvaluationConfigurationRepository.DeleteManyAsync(deleteConfig);
       }

        public async Task<List<TeamAffiliatePayroll>> Aff_GetAffiliatePayroll()
        {
            var teamAffiliatePayrolls = new List<TeamAffiliatePayroll>();
            var payrollDetail = await _payrollDomainService.GetPayrollDetail(new PayrollDetailRequest {PayrollId = "3dbdf70a-713c-dc4a-7261-39ff01f93202".ToNullableGuid()});
            var teamPayrolls = payrollDetail.UserPayrolls.GroupBy(_ => _.OrganizationName);
            foreach (var team in teamPayrolls)
            {
                teamAffiliatePayrolls.Add
                (
                    new TeamAffiliatePayroll()
                    {
                        Name = team.Key,
                        CountMember = team.Count(),
                        AffiliateWave = team.Sum(_ => _.AffiliateWaves.Sum(wave => wave.Amount)).ToCommaStyle(),
                        AffiliateBonus = team.Sum(_ => _.AffiliateBonuses.Sum(bonus => bonus.Amount)).ToCommaStyle(),
                        AffiliateTotal = (team.Sum(_ => _.AffiliateWaves.Sum(wave => wave.Amount)) + team.Sum(_ => _.AffiliateBonuses.Sum(bonus => bonus.Amount))).ToCommaStyle()
                    }
                );
            }

            teamAffiliatePayrolls.Add
            (
                new TeamAffiliatePayroll()
                {
                    Name = "Total",
                    CountMember = payrollDetail.UserPayrolls.Count(),
                    AffiliateWave = payrollDetail.UserPayrolls.Sum(_ => _.AffiliateWaves.Sum(wave => wave.Amount)).ToCommaStyle(),
                    AffiliateBonus = payrollDetail.UserPayrolls.Sum(_ => _.AffiliateBonuses.Sum(wave => wave.Amount)).ToCommaStyle(),
                    AffiliateTotal = (payrollDetail.UserPayrolls.Sum(_ => _.AffiliateWaves.Sum(wave => wave.Amount)) + payrollDetail.UserPayrolls.Sum(_ => _.AffiliateBonuses.Sum(wave => wave.Amount)))
                        .ToCommaStyle()
                }
            );

            return teamAffiliatePayrolls;
        }

        public async Task Payroll_CalculatePayroll()
        {
            await _payrollDomainService.CalculateUserPayrolls(true, true, false);
        }

        public async Task Payroll_CalculateHappyDayPayroll()
        {
            await _payrollDomainService.CalculateUserPayrolls(true, true, true);
        }

        public async Task Aff_SetPostUsersToAffUsers()
        {
            var dateTime = new DateTime(2021, 11, 1);
            await _userAffiliateDomainService.InitUserAffiliates(dateTime, DateTime.UtcNow);
        }

        public async Task Aff_SyncAffShopinessConversions()
        {
            var dateTime = new DateTime(2021, 12, 26);
            dateTime = dateTime.AddHours(-7);
            var now = DateTime.UtcNow;

            await _affiliateConversionDomainService.SyncAffConversions(dateTime, now, AffiliateOwnershipType.GDL);

            await Task.Delay(3000);

            await _affiliateConversionDomainService.SyncAffConversions(dateTime, now, AffiliateOwnershipType.HappyDay);
        }

        public async Task<int> Aff_Check()
        {
            var affs = await _userAffiliateRepository.GetListAsync();
            return affs.Sum(_ => _.AffConversionModel.ConversionCount);
        }

        public async Task CleanPayroll()
        {
            var payrolls = _payrollRepository.ToList();
            var userPayrolls = _userPayrollRepository.ToList();
            foreach (var payroll in payrolls)
            {
                var userPayroll = userPayrolls.Where(_ => _.PayrollId != null && _.PayrollId == payroll.Id);
                if (userPayroll.IsNullOrEmpty())
                {
                    Debug.WriteLine($"-----------------{payroll.Title}-----------------------");
                    await _payrollRepository.HardDeleteAsync(payroll);
                }
            }

            foreach (var userPayroll in userPayrolls)
            {
                var payroll = payrolls.Where(_ => _.Id == userPayroll.PayrollId);
                if (payroll.IsNullOrEmpty())
                {
                    Debug.WriteLine($"-----------------{userPayroll.Code}-----------------------");
                    await _userPayrollRepository.HardDeleteAsync(userPayroll);
                }
            }
        }

        public async Task Aff_InitUserAffOwners()
        {
            await _userAffiliateDomainService.InitUserAffiliates(null, null);
        }

        public async Task Aff_SyncCampaignAffStats()
        {
            var postNavs = await _postRepository
                .GetListWithNavigationPropertiesExtendAsync
                (
                    postContentType: PostContentType.Affiliate
                );
            postNavs = postNavs.Where(_ => _.Post.CampaignId != null).ToList();
            if (postNavs.IsNullOrEmpty()) return;

            // 1. get the SHORTLINK OWNER
            List<UserAffiliateModel> affiliateModels = new();
            foreach (var post in postNavs.Where(p => p.Post.Shortlinks.IsNotNullOrEmpty()))
            {
                if (post.Post.Shortlinks.IsNotNullOrEmpty())
                {
                    foreach (var shortUrl in post.Post.Shortlinks)
                    {
                        var item = new UserAffiliateModel
                        {
                            AppUserId = post.AppUser?.Id,
                            Shortlink = shortUrl,
                            CreatedAt = post.Post.SubmissionDateTime ?? DateTime.UtcNow,
                            GroupId = post.Group?.Id,
                            CampaignId = post.Campaign?.Id
                        };

                        if (affiliateModels.All(_ => _.Shortlink != shortUrl)) { affiliateModels.Add(item); }
                    }
                }
            }

            var userAffiliateModels = affiliateModels.DistinctBy(_ => _.Shortlink.Trim()).ToList();

            var initAffiliates = new List<UserAffiliate>();

            // 2. map the OWNER (APPUSER) to current affiliates
            foreach (var userAff in userAffiliateModels)
            {
                var initAff = new UserAffiliate()
                {
                    AppUserId = userAff.AppUserId,
                    MarketplaceType = MarketplaceType.Shopee,
                    AffiliateUrl = userAff.Shortlink,
                    CreatedAt = userAff.CreatedAt,
                    GroupId = userAff.GroupId,
                    CampaignId = userAff.CampaignId
                };
                var initAffCreation = _userAffiliateDomainService.InitUserAffiliateCreation(initAff);
                initAffiliates.Add(initAffCreation);

                Debug.WriteLine($"===========================================NEW USER AFFILIATE: {userAff.Shortlink}");
            }


            if (initAffiliates.IsNotNullOrEmpty())
            {
                foreach (var batch in initAffiliates.Partition(100)) { await _userAffiliateRepository.InsertManyAsync(batch); }
            }

            var affiliates = await _userAffiliateRepository.GetListAsync();

            var gdlShortLinks = affiliates.Where(x => x.AffiliateUrl.Contains(GlobalConsts.GDLDomain)).Select(x => x.AffiliateUrl).ToList();
            var happyShortLinks = affiliates.Where(x => x.AffiliateUrl.Contains(GlobalConsts.HPDDomain)).Select(x => x.AffiliateUrl).ToList();

            var partitions = gdlShortLinks.Partition(ShopinessApiConfig.MaxApiBatchCount).ToList();
            var stack = new Stack<IEnumerable<string>>(partitions);

            while (true)
            {
                if (stack.TryPop(out var list))
                {
                    Debug.WriteLine($"SyncAffs: current stack {stack.Count}");
                    await DoSyncAffs(affiliates, list.ToList(), AffiliateOwnershipType.GDL, 1);
                }
                else
                {
                    break;
                }
            }

            var happyPartitions = happyShortLinks.Partition(ShopinessApiConfig.MaxApiBatchCount).ToList();
            var happyStack = new Stack<IEnumerable<string>>(happyPartitions);
            while (true)
            {
                if (happyStack.TryPop(out var list))
                {
                    Debug.WriteLine($"SyncAffs: current stack {happyStack.Count}");
                    await DoSyncAffs(affiliates, list.ToList(), AffiliateOwnershipType.HappyDay, 1);
                }
                else
                {
                    break;
                }
            }
        }

        private async Task DoSyncAffs(List<UserAffiliate> outdatedAffs, List<string> shortLinks, AffiliateOwnershipType affiliateOwner, int months)
        {
            if (shortLinks.IsNullOrEmpty()) { return; }

            var stopWatch = Stopwatch.StartNew();

            var gdlAffiliateResponse = await _shopinessDomainService.GetStat
            (
                new ShopinessAffiliateStatRequest
                {
                    StartDate = DateTime.UtcNow.Date.AddMonths(-months),
                    EndDate = DateTime.UtcNow,
                    Shortlinks = shortLinks,
                    IsGDL = affiliateOwner == AffiliateOwnershipType.GDL
                }
            );

            stopWatch.Stop();
            // await Task.Delay(ShopinessApiConfig.ApiDelayInMs);

            if (gdlAffiliateResponse != null && gdlAffiliateResponse.ListData.IsNotNullOrEmpty())
            {
                foreach (var item in gdlAffiliateResponse.ListData)
                {
                    var existing = outdatedAffs.FirstOrDefault(x => x.AffiliateUrl.Contains(item.Key));
                    if (existing == null) { continue; }

                    existing.UpdatedAt = DateTime.UtcNow;

                    existing.AffConversionModel.ClickCount = item.Click;
                    existing.AffConversionModel.ConversionCount = item.Conversion;
                    existing.AffConversionModel.ConversionAmount = item.Value;
                    existing.AffConversionModel.CommissionAmount = item.Commission;
                    existing.AffConversionModel.CommissionBonusAmount = item.Commission;

                    await _userAffiliateRepository.UpdateAsync(existing);
                }
            }

            var timeTakenInMs = stopWatch.ElapsedMilliseconds;
            if (timeTakenInMs < ShopinessApiConfig.ApiDelayInMs + 100)
            {
                var apiDelayInMs = ShopinessApiConfig.ApiDelayInMs + 100 - timeTakenInMs;
                await Task.Delay((int) apiDelayInMs);
            }
        }

        #region Contract

        public async Task GenerateContracts()
        {
            var userDetails = await _userDomainService.GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetSystemUsers = false,
                    GetActiveUsers = true,
                }
            );
            var salePersons = userDetails.Where(x => x.Team.DisplayName.Contains("Sale")).ToList();
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            foreach (var user in salePersons)
            {
                Random gen = new Random();
                bool isCreate = gen.Next(100) < 50;
                if (isCreate)
                {
                    var totalValue = gen.Next(100000000);
                    var date = DateTimeHelper.RandomDay(firstDayOfMonth, lastDayOfMonth);
                    var contract = new Contract
                    {
                        LastModificationTime = null,
                        LastModifierId = null,
                        ContractCode = date.Date.ToString(),
                        Content = "test",
                        ContractStatus = ContractStatus.Unknown,
                        ContractPaymentStatus = ContractPaymentStatus.Unknown,
                        SignedAt = date.Date,
                        CreatedAt = date.Date,
                        TotalValue = totalValue,
                        PartialPaymentValue = 0,
                        RemainingPaymentValue = totalValue,
                        PaymentDueDate = date.Date,
                        SalePersonId = user.User.Id
                    };
                    await _contractRepository.InsertAsync(contract);
                }
            }
        }

        public async Task GenerateTransactions()
        {
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            Random gen = new Random();
            var contracts = await _contractRepository.GetListExtendAsync
            (
                createdAtMin: firstDayOfMonth,
                createdAtMax: lastDayOfMonth,
                contractStatus: ContractStatus.ContractSigned
            );

            foreach (var contract in contracts)
            {
                var partialPaymentValue = gen.Next(10000000);
                var transaction = new ContractTransaction
                {
                    ContractId = contract.Id,
                    SalePersonId = contract.SalePersonId ?? Guid.Empty,
                    Description = "Auto Generate",
                    CreatedAt = now,
                    PaymentDueDate = contract.PaymentDueDate,
                    PartialPaymentValue = partialPaymentValue
                };
                await _contractTransactionRepository.InsertAsync(transaction);

                contract.PartialPaymentValue = partialPaymentValue;
                contract.RemainingPaymentValue = contract.TotalValue - partialPaymentValue > 0 ? contract.TotalValue - partialPaymentValue : 0;
                await _contractRepository.UpdateAsync(contract);
            }
        }

        #endregion

        public async Task Group_UpdateSeedingTeams()
        {
            var groups = await _groupRepository.GetListAsync();
            foreach (var group in groups.Where(_ => _.CrawlInfo.SeedingTeams.IsNotNullOrEmpty()))
            {
                var seedingTeams = new List<string>();
                foreach (var seedingTeam in group.CrawlInfo.SeedingTeams)
                {
                    if (seedingTeam.Length > 3)
                    {
                        seedingTeams.Add(seedingTeam);
                        continue;
                    }

                    seedingTeams.Add($"GDL - {seedingTeam}");
                }

                group.CrawlInfo.SeedingTeams = seedingTeams;
                await _groupRepository.UpdateAsync(group);
            }
        }

        public async Task CleanData()
        {
            await _clearDataDomainService.CleanUpData(300);
        }

        public async Task CleanUserInfos()
        {
            await _clearDataDomainService.CleanUserInfos();
        }

        public async Task<int> ResyncUserAffiliate()
        {
            var updatedUserAffiliates = new List<UserAffiliate>();
            var conversions = await _affiliateConversionRepository.GetListAsync();
            var groupByConversion = conversions.GroupBy(_ => _.ShortKey);
            var userAffiliates = await _userAffiliateRepository.GetListAsync();
            foreach (var item in groupByConversion)
            {
                Debug.WriteLine($"-----------{item.Key}---------------------");
                var existingUserAff = userAffiliates.FirstOrDefault(_ => UrlHelper.GetShortKey(_.AffiliateUrl) == item.Key);

                if (existingUserAff == null)
                {
                    continue;
                }

                existingUserAff.AffConversionModel.ConversionCount = item.Count();
                existingUserAff.AffConversionModel.ConversionAmount = item.Sum(_ => _.SaleAmount);
                existingUserAff.AffConversionModel.CommissionAmount = item.Sum(_ => _.Payout);
                existingUserAff.AffConversionModel.CommissionBonusAmount = item.Sum(_ => _.PayoutBonus);

                var firstConversion = item.FirstOrDefault();
                var marketplaceType = firstConversion.Campaign.IsNotNullOrEmpty() && firstConversion.Campaign.Contains("lazada") ? MarketplaceType.Lazada
                    : firstConversion.Campaign.IsNotNullOrEmpty() && firstConversion.Campaign.Contains("tiki") ? MarketplaceType.Tiki
                    : MarketplaceType.Shopee;
                existingUserAff.MarketplaceType = marketplaceType;

                updatedUserAffiliates.Add(existingUserAff);
            }

            var partitions = updatedUserAffiliates.Partition(ShopinessApiConfig.MaxApiBatchCount).ToList();
            var stack = new Stack<IEnumerable<UserAffiliate>>(partitions);
            while (true)
            {
                if (stack.TryPop(out var list))
                {
                    Debug.WriteLine($"SyncAffs: current stack {stack.Count}");
                    await _userAffiliateRepository.UpdateManyAsync(list);
                }
                else
                {
                    break;
                }
            }

            return updatedUserAffiliates.Sum(_ => _.AffConversionModel.ConversionCount);
        }

        public async Task<int> AffCheck()
        {
            var userAffiliates = await _userAffiliateRepository.GetListAsync();
            return userAffiliates.Sum(_ => _.AffConversionModel.ConversionCount);
        }

        public async Task<int> Post_Clean()
        {
            var deletePosts = new List<Post>();
            var posts = await _postRepository.GetListAsync();
            try
            {
                foreach (var post in posts)
                {
                    Debug.WriteLine(post.Url);
                    post.Url = FacebookHelper.GetCleanUrl(post.Url);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var groupPosts = posts.GroupBy(_ => _.Url);
            foreach (var item in groupPosts)
            {
                var invalidPosts = item.OrderByDescending(_ => _.TotalCount).ToList();
                invalidPosts.Remove(invalidPosts.Last());
                deletePosts.AddRange(invalidPosts);
            }

            var partitions = deletePosts.Partition(ShopinessApiConfig.MaxApiBatchCount).ToList();
            var stack = new Stack<IEnumerable<Post>>(partitions);
            while (true)
            {
                if (stack.TryPop(out var list))
                {
                    Debug.WriteLine($"Delete Posts: current stack {stack.Count}");
                    await _postRepository.DeleteManyAsync(list);
                }
                else
                {
                    break;
                }
            }

            return deletePosts.Count;
        }

        public async Task<int> Eval_GenerateEval(int year, int month)
        {
            await _staffEvaluationDomainService.GenerateStaffEvaluations(year, month);

            return 0;
        }

        public async Task<int> Tiktok_CloneGroupHistory()
        {
            var groupStatsHistories = new List<GroupStatsHistory>();
            var toTime = DateTime.UtcNow;
            var fromTime = toTime.AddDays(-10).Date;
            var fixTime = new DateTime
            (
                2021,
                12,
                9,
                0,
                0,
                0,
                DateTimeKind.Utc
            );
            var tiktokHistories = await _groupStatsHistoriesRepository.GetListAsync(_ => _.CreatedAt.HasValue && _.CreatedAt >= fromTime && _.GroupSourceType == GroupSourceType.Tiktok);
            var groupTiktokHistories = tiktokHistories.OrderBy(_ => _.CreatedAt).GroupBy(_ => _.GroupFid).ToList();
            var numberList = new List<int>()
            {
                1,
                -1
            };
            var random = new Random();
            foreach (var groupTiktok in groupTiktokHistories)
            {
                if (groupTiktok.IsNullOrEmpty()) continue;
                var groupStatsHistoryChanged = groupTiktok.First();
                var lastGroupStats = groupTiktok.Last();
                foreach (var dateTime in fixTime.To(toTime))
                {
                    var i = random.Next(numberList.Count);
                    var num = numberList[i];
                    if (tiktokHistories.FirstOrDefault(_ => _.CreatedAt.Value.Date == dateTime.Date) == null)
                    {
                        var groupStatsHistory = new GroupStatsHistory()
                        {
                            GroupFid = groupTiktok.Key,
                            GroupSourceType = GroupSourceType.Tiktok,
                            CreatedAt = dateTime.Date,
                            TotalInteractions = (int) groupTiktok.Average(_ => _.TotalInteractions) + (lastGroupStats.TotalInteractions - groupStatsHistoryChanged.TotalInteractions) / 2 * num,
                            AvgPosts = groupTiktok.Average(_ => _.AvgPosts) + (lastGroupStats.AvgPosts - groupStatsHistoryChanged.AvgPosts) / 2 * num,
                            GroupMembers = (int) groupTiktok.Average(_ => _.GroupMembers) + (lastGroupStats.GroupMembers - groupStatsHistoryChanged.GroupMembers) / 2 * num,
                            Reactions = (int) groupTiktok.Average(_ => _.Reactions) + (lastGroupStats.Reactions - groupStatsHistoryChanged.Reactions) / 2 * num,
                            GrowthPercent = groupTiktok.Average(_ => _.GrowthPercent) + (lastGroupStats.GrowthPercent - groupStatsHistoryChanged.GrowthPercent) / 2 * num,
                            GrowthNumber = (int) groupTiktok.Average(_ => _.GrowthNumber) + (lastGroupStats.GrowthNumber - groupStatsHistoryChanged.GrowthNumber) / 2 * num,
                        };
                        groupStatsHistories.Add(groupStatsHistory);
                        groupStatsHistoryChanged = groupStatsHistory;
                    }
                }
            }

            if (groupStatsHistories.IsNotNullOrEmpty()) await _groupStatsHistoriesRepository.InsertManyAsync(groupStatsHistories);
            return groupStatsHistories.Count;
        }

        public async Task<int> Clone_GroupStatsHistory()
        {
            var newGroupStatsHistories = new List<GroupStatsHistory>();
            var toTime = DateTime.UtcNow.AddDays(-1);
            var fromTime = toTime.AddDays(-10).Date;
            var pageStatsHistories = await _groupStatsHistoriesRepository.GetListAsync(_ => _.CreatedAt.HasValue && _.CreatedAt >= fromTime && _.GroupSourceType == GroupSourceType.Page);
            var groupStatHistories = await _groupStatsHistoriesRepository.GetListAsync(_ => _.CreatedAt.HasValue && _.CreatedAt >= fromTime && _.GroupSourceType == GroupSourceType.Group);

            var pageHistories = pageStatsHistories.OrderBy(_ => _.CreatedAt).GroupBy(_ => _.GroupFid).ToList();
            var groupHistories = groupStatHistories.OrderBy(_ => _.CreatedAt).GroupBy(_ => _.GroupFid).ToList();
            var numberList = new List<int>()
            {
                1,
                -1
            };
            var random = new Random();

            foreach (var page in pageHistories)
            {
                if (page.IsNullOrEmpty()) continue;
                var groupStatsHistoryChanged = page.First();
                var lastGroupStats = page.Last();
                foreach (var dateTime in fromTime.To(toTime))
                {
                    var i = random.Next(numberList.Count);
                    var num = numberList[i];
                    if (pageStatsHistories.FirstOrDefault(_ => _.CreatedAt.Value.Date == dateTime.Date) == null)
                    {
                        var groupStatsHistory = new GroupStatsHistory()
                        {
                            GroupFid = page.Key,
                            GroupSourceType = GroupSourceType.Page,
                            CreatedAt = dateTime.Date,
                            TotalInteractions = (int) page.Average(_ => _.TotalInteractions) + (lastGroupStats.TotalInteractions - groupStatsHistoryChanged.TotalInteractions) / 2 * num,
                            AvgPosts = page.Average(_ => _.AvgPosts) + (lastGroupStats.AvgPosts - groupStatsHistoryChanged.AvgPosts) / 2 * num,
                            GroupMembers = (int) page.Average(_ => _.GroupMembers) + (lastGroupStats.GroupMembers - groupStatsHistoryChanged.GroupMembers) / 2 * num,
                            Reactions = (int) page.Average(_ => _.Reactions) + (lastGroupStats.Reactions - groupStatsHistoryChanged.Reactions) / 2 * num,
                            GrowthPercent = page.Average(_ => _.GrowthPercent) + (lastGroupStats.GrowthPercent - groupStatsHistoryChanged.GrowthPercent) / 2 * num,
                            GrowthNumber = (int) page.Average(_ => _.GrowthNumber) + (lastGroupStats.GrowthNumber - groupStatsHistoryChanged.GrowthNumber) / 2 * num,
                        };
                        newGroupStatsHistories.Add(groupStatsHistory);
                        groupStatsHistoryChanged = groupStatsHistory;
                    }
                }
            }

            foreach (var group in groupHistories)
            {
                if (group.IsNullOrEmpty()) continue;
                var groupStatsHistoryChanged = group.First();
                var lastGroupStats = group.Last();
                foreach (var dateTime in fromTime.To(toTime))
                {
                    var i = random.Next(numberList.Count);
                    var num = numberList[i];
                    if (groupStatHistories.FirstOrDefault(_ => _.CreatedAt.Value.Date == dateTime.Date) == null)
                    {
                        var groupStatsHistory = new GroupStatsHistory()
                        {
                            GroupFid = group.Key,
                            GroupSourceType = GroupSourceType.Group,
                            CreatedAt = dateTime.Date,
                            TotalInteractions = (int) group.Average(_ => _.TotalInteractions) + (lastGroupStats.TotalInteractions - groupStatsHistoryChanged.TotalInteractions) / 2 * num,
                            AvgPosts = group.Average(_ => _.AvgPosts) + (lastGroupStats.AvgPosts - groupStatsHistoryChanged.AvgPosts) / 2 * num,
                            GroupMembers = (int) group.Average(_ => _.GroupMembers) + (lastGroupStats.GroupMembers - groupStatsHistoryChanged.GroupMembers) / 2 * num,
                            Reactions = (int) group.Average(_ => _.Reactions) + (lastGroupStats.Reactions - groupStatsHistoryChanged.Reactions) / 2 * num,
                            GrowthPercent = group.Average(_ => _.GrowthPercent) + (lastGroupStats.GrowthPercent - groupStatsHistoryChanged.GrowthPercent) / 2 * num,
                            GrowthNumber = (int) group.Average(_ => _.GrowthNumber) + (lastGroupStats.GrowthNumber - groupStatsHistoryChanged.GrowthNumber) / 2 * num,
                        };
                        newGroupStatsHistories.Add(groupStatsHistory);
                        groupStatsHistoryChanged = groupStatsHistory;
                    }
                }
            }

            if (newGroupStatsHistories.IsNotNullOrEmpty()) await _groupStatsHistoriesRepository.InsertManyAsync(newGroupStatsHistories);
            return newGroupStatsHistories.Count;
        }

        public async Task<int> EvalConfig_CleanUp()
        {
            var userEvaluationConfigs = await _userEvaluationConfigurationRepository.GetListAsync();
            var userDetails = await _userDomainService.GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetSystemUsers = false,
                    GetActiveUsers = true,
                }
            );
            var staffUser = userDetails.Where
                (
                    x => x.Info.UserPosition.IsIn
                    (
                        UserPosition.Community,
                        UserPosition.CommunityIntern,
                        UserPosition.CommunityStaff,
                        UserPosition.CommunitySeedingStaff,
                        UserPosition.CommunityAffiliateStaff,
                        UserPosition.ContentStaff,
                        UserPosition.ContentExecutive,
                        UserPosition.ContentSeniorExecutive,
                        UserPosition.Content
                    )
                )
                .Select(x => x.User.Id)
                .ToList();
            if (staffUser.IsNullOrEmpty()) return 0;

            var invalidConfigs = userEvaluationConfigs.FindAll(x => x.AppUserId.HasValue && staffUser.Contains(x.AppUserId.Value));
            if (invalidConfigs.Count > 0)
            {
                await _userEvaluationConfigurationRepository.DeleteManyAsync(invalidConfigs);
            }

            return invalidConfigs?.Count ?? 0;
        }

        public async Task<List<ShortLinkPost>> GetDuplicateShortLinkPosts()
        {
            var results = new List<ShortLinkPost>();
            var toDateTime = new DateTime(2021, 12, 26).AddHours(-7);
            var fromDateTime = toDateTime.AddMonths(-1);
            var navPosts = (await _postRepository.GetListWithNavigationPropertiesExtendAsync
                (
                    createdDateTimeMin: fromDateTime,
                    createdDateTimeMax: toDateTime
                ))
                .ToList();
            var posts = navPosts.Where(_ => _.Post.CreatedDateTime != null && _.Post.PostContentType == PostContentType.Affiliate && _.Post.Shortlinks.IsNotNullOrEmpty()).ToList();

            var shortLinkPosts = new List<ShortLinkPost>();
            try
            {
                foreach (var post in posts)
                {
                    if (post.Post.Shortlinks != null)
                    {
                        foreach (var shortLink in post.Post.Shortlinks)
                        {
                            shortLinkPosts.Add
                            (
                                new ShortLinkPost()
                                {
                                    Username = post.AppUser.UserName,
                                    Url = post.Post.Url,
                                    ShortLink = shortLink
                                }
                            );
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var groupByShortLink = shortLinkPosts.Where(_ => _.ShortLink.IsNotNullOrEmpty()).GroupBy(_ => _.ShortLink);
            foreach (var item in groupByShortLink)
            {
                if (item.Count() > 1)
                {
                    if (item.GroupBy(_ => _.Username).Count() > 1)
                        results.AddRange(item);
                }
            }

            return results;
        }

        public async Task StaffEvalChangingUser(string changeUserCode = "185", string toUserCode = "018")
        {
            var changeUser = await _userDomainService.GetByCode(changeUserCode);
            var toUser = await _userDomainService.GetByCode(toUserCode);

            var staffEvals = await _staffEvaluationRepository.GetListAsync(_ => _.AppUserId == changeUser.AppUser.Id);
            staffEvals = staffEvals.Select
                (
                    _ =>
                    {
                        _.AppUserId = toUser.AppUser.Id;
                        return _;
                    }
                )
                .ToList();
            await _staffEvaluationRepository.UpdateManyAsync(staffEvals);
        }

        public async Task GetDuplicateFidPosts()
        {
            var listPost = await _postRepository.ToListAsync();
            ;
            var postGroup = listPost.GroupBy(_ => _.Fid);
            List<string> contentList = new List<string>();

            foreach (var items in postGroup)
            {
                if (items.Count() > 1)
                {
                    foreach (var i in items)
                    {
                        contentList.Add($"{i.Url}|{i.Fid}|{i.TotalCount}");
                    }
                }
            }

            await File.WriteAllLinesAsync(@"D:\List_Duplicate_Posts.txt", contentList);
        }

        public async Task GetDuplicateVideos()
        {
            var tiktokList = await _tiktokRepository.ToListAsync();
            var tiktokGroup = tiktokList.GroupBy(_ => _.VideoId);
            List<string> contentList = new List<string>();
            foreach (var items in tiktokGroup)
            {
                if (items.Count() > 1)
                {
                    foreach (var i in items)
                    {
                        contentList.Add($"{i.Url}|{i.VideoId}");
                    }
                }
            }

            await File.WriteAllLinesAsync(@"D:\List_Duplicate_Videos.txt", contentList);
        }

        public async Task GetEmptyAppUserID()
        {
            var staffEvalList = await _staffEvaluationRepository.ToListAsync();
            var staffEvalGroup = staffEvalList.Where(_ => _.AppUserId.IsNullOrEmpty()).ToList();

            await using var writer = new StreamWriter(@"D:\List_Empty_AppUserID.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(staffEvalGroup);
        }

        public async Task GetStaffEvalWithoutTeam()
        {
            var listEvalConfig = await _userEvaluationConfigurationRepository.ToListAsync();
            var staffEvalWithoutTeam = listEvalConfig.Where(_ => _.TeamId.IsNullOrEmpty());

            await using var writer = new StreamWriter(@"D:\Staff_Evaluate_Without_Team.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(staffEvalWithoutTeam);
        }

        public async Task<List<string>> GetUserInfoWithoutPosition()
        {
            var userInfoList = await _userInfoRepository.GetListWithNavigationPropertiesAsync(isActive: true, isSystemUser: false);
            var userInfoWithoutPosition = userInfoList.Where(_ => _.UserInfo.UserPosition == UserPosition.FilterNoSelect || _.UserInfo.UserPosition == UserPosition.Unknown).ToList();
            return userInfoWithoutPosition.Select(_ => $"{_.AppUser.UserName}({_.UserInfo.Code})").ToList();
        }

        public async Task<int> UpdateContract()
        {
            var contracts = await _contractRepository.GetListExtendAsync();
            contracts = contracts.Select
                (
                    _ =>
                    {
                        _.RemainingPaymentValue = _.TotalValue - _.PartialPaymentValue;
                        return _;
                    }
                )
                .ToList();
            await _contractRepository.UpdateManyAsync(contracts);
            return contracts.Count;
        }

        public async Task InitTiktokStatHistory()
        {
            var fromDateTime = DateTime.SpecifyKind(new DateTime(2022, 01, 21), DateTimeKind.Utc);
            var toDateTime = fromDateTime.AddDays(2);
            var tikTokStatHistories = _tiktokStatRepository
                .Where(x => x.Date >= fromDateTime)
                .Where(x => x.Date <= toDateTime)
                .ToList();
            var tikTokStat = new TiktokStat()
            {
                Count = (int)tikTokStatHistories.Average(_ => _.Count),
                Date = fromDateTime.AddDays(1),
                Hashtag = tikTokStatHistories.FirstOrDefault()?.Hashtag,
            };
            await _tiktokStatRepository.InsertAsync(tikTokStat);
            
            var groupStatsHistories = _groupStatsHistoriesRepository
                .Where(x => x.GroupSourceType == GroupSourceType.Tiktok)
                .Where(x => x.CreatedAt >= fromDateTime)
                .Where(x => x.CreatedAt <= toDateTime)
                .ToList();
            
            var groupStatsGroupBy = groupStatsHistories.GroupBy(_ => _.GroupFid);
            foreach (var group in groupStatsGroupBy)
            {
                await _groupStatsHistoriesRepository.InsertAsync
                (
                    new GroupStatsHistory()
                    {
                        GroupFid = group.Key,
                        GroupSourceType = @group.FirstOrDefault()!.GroupSourceType,
                        CreatedAt = fromDateTime.AddDays(1),
                        TotalInteractions = (int)group.Average(_ => _.TotalInteractions),
                        InteractionRate = group.FirstOrDefault()?.InteractionRate,
                        AvgPosts = group.Average(_ => _.AvgPosts),
                        GroupMembers = (int)group.Average(_ => _.GroupMembers),
                        Reactions = (int)group.Average(_ => _.Reactions),
                        GrowthPercent = group.Average(_ => _.GrowthPercent),
                        GrowthNumber = (int)group.Average(_ => _.GrowthNumber)
                    }
                );
            }
        }
    }
}