using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.UserWaves;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IPayrollDomainService : IDomainService
    {
        Task<PayrollResponse> GetPayrollDetail(PayrollDetailRequest payrollDetailRequest);
        Task<PayrollResponse> CalculateUserPayrolls(bool persist, bool isTempPayroll, bool isHappyDay, Guid? targetUserId = null);
        Task<UserPayroll> GetUserPayslip(UserPayrollRequest userPayrollRequest);
        Task ConfirmPayroll(Guid payrollId);
        Task RecalculateWaveMultipliers();

        Task<List<Post>> GetCurrentPayrollPosts();

        Task DeletePayroll(Guid payrollId);
    }

    public class PayrollDomainService : BaseDomainService, IPayrollDomainService
    {
        private readonly PayrollConfiguration _payrollConfiguration;
        private readonly IdentityUserManager _userManager;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserRoleFinder _userRoleFinder;

        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IOrganizationUnitRepository _organizationUnitRepository;
        private readonly IPostRepository _postRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IUserPayrollRepository _userPayrollRepository;

        private readonly IUserDomainService _userDomainService;
        private readonly IGroupDomainService _groupDomainService;

        private readonly IDistributedEventBus _distributedEventBus;
        private readonly IApiConfigurationDomainService _apiConfigurationDomainService;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IUserCompensationRepository _userCompensationRepository;

        public PayrollDomainService(
            IApiConfigurationDomainService apiConfigurationDomainService,
            IUnitOfWorkManager unitOfWorkManager,
            IdentityUserManager userManager,
            IRepository<AppUser, Guid> userRepository,
            IUserInfoRepository userInfoRepository,
            IOrganizationUnitRepository organizationUnitRepository,
            IOrganizationDomainService organizationDomainService,
            IPostRepository postRepository,
            IGroupRepository groupRepository,
            IPayrollRepository payrollRepository,
            IUserPayrollRepository userPayrollRepository,
            IUserRoleFinder userRoleFinder,
            IUserDomainService userDomainService,
            IDistributedEventBus distributedEventBus,
            IUserAffiliateRepository userAffiliateRepository,
            IGroupDomainService groupDomainService,
            IUserAffiliateDomainService userAffiliateDomainService,
            IUserCompensationRepository userCompensationRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _payrollRepository = payrollRepository;
            _userPayrollRepository = userPayrollRepository;
            _userDomainService = userDomainService;
            _organizationUnitRepository = organizationUnitRepository;
            _distributedEventBus = distributedEventBus;
            _userAffiliateRepository = userAffiliateRepository;
            _groupDomainService = groupDomainService;
            _userCompensationRepository = userCompensationRepository;
            _userManager = userManager;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _userInfoRepository = userInfoRepository;
            _organizationDomainService = organizationDomainService;
            _payrollConfiguration = apiConfigurationDomainService.GetPayrollConfiguration();
            _apiConfigurationDomainService = apiConfigurationDomainService;
            _userRoleFinder = userRoleFinder;
        }

        public async Task DeletePayroll(Guid payrollId)
        {
            var userPayrolls = await _userPayrollRepository.GetListAsync(x => x.PayrollId == payrollId);

            foreach (var userPayroll in userPayrolls)
            {
                await _userPayrollRepository.HardDeleteAsync(userPayroll);
            }

            var userCompensations = await _userCompensationRepository.GetListAsync(x => x.PayrollId == payrollId);
            foreach (var userCompensation in userCompensations)
            {
                await _userCompensationRepository.HardDeleteAsync(userCompensation);
            }
            await _payrollRepository.HardDeleteAsync(x=>x.Id == payrollId);
        }
        
        public async Task<List<Post>> GetCurrentPayrollPosts()
        {
            var now = DateTime.UtcNow;
            var (fromTime, toTime) = GetPayrollDateTime(now.Year, now.Month);

            return await _postRepository.GetListAsync(createdDateTimeMin: fromTime, createdDateTimeMax: toTime);
        }


        private PayrollRequest GetDefaultPayrollRequest(bool autoSave, bool isTempPayroll, bool isHappyDay)
        {
            var now = DateTime.UtcNow;
            var (fromTime, toTime) = GetPayrollDateTime(now.Year, now.Month);

            var payrollRequest = new PayrollRequest
            {
                FromDateTime = fromTime,
                ToDateTime = toTime,
                AutoSave = autoSave,
                CalculateCommunityBonus = true,
                IsTempPayroll = isTempPayroll,
                IsHappyDay = isHappyDay
            };

            return payrollRequest;
        }

        public async Task<UserPayroll> GetUserPayslip(UserPayrollRequest userPayrollRequest)
        {
            var userInfo = await _userInfoRepository.FindAsync(_ => _.Code == userPayrollRequest.UserCode);
            if (userInfo?.AppUserId == null) return new UserPayroll();

            var identityUser = await _userManager.GetByIdAsync(userInfo.AppUserId.Value);

            var managedUserIds = new List<Guid>();
            if (await _userManager.IsInRoleAsync(identityUser, RoleConsts.Leader)) { managedUserIds = await _userDomainService.GetManagedUserIds(userInfo.AppUserId.Value); }
            else if (await _userManager.IsInRoleAsync(identityUser, RoleConsts.Staff)) { managedUserIds.Add(identityUser.Id); }

            var payrollRequest = new PayrollRequest
            {
                AutoSave = false,
                CalculateCommunityBonus = false,
                FromDateTime = userPayrollRequest.FromDateTime,
                ToDateTime = userPayrollRequest.ToDateTime,
                UserIds = managedUserIds,
            };
            
            var userDetails = await _userDomainService.GetUserDetails(new ApiUserDetailsRequest()
            {
                UserIds = payrollRequest.UserIds,
                GetForPayrollCalculation = true,
                GetTeamUsers = true,
                GetSystemUsers = false,
                GetActiveUsers = true,
            });
            var response = await Do(payrollRequest, userDetails);

            return response.UserPayrolls.FirstOrDefault(_ => _.AppUserId == userInfo.AppUserId.Value);
        }

        public async Task<PayrollResponse> GetPayrollDetail(PayrollDetailRequest payrollDetailRequest)
        {
            var payrollResponse = new PayrollResponse();
            if (!payrollDetailRequest.PayrollId.HasValue) return null;

            var payroll = await _payrollRepository.FindAsync(payrollDetailRequest.PayrollId.Value);
            if (payroll == null) return null;

            var userPayrolls = await _userPayrollRepository.GetListAsync(_ => _.PayrollId == payroll.Id);
            if (userPayrolls.IsNullOrEmpty()) return null;

            payrollResponse.UserPayrolls = userPayrolls.ToList();
            payrollResponse.CommunityBonuses = userPayrolls.SelectMany(_ => _.CommunityBonuses).Distinct().ToList();
            payrollResponse.TeamBonuses = payroll.TeamBonuses;
            payrollResponse.Commissions = userPayrolls.SelectMany(_ => _.Commissions).Distinct().ToList();
            payrollResponse.FromDateTime = payroll.FromDateTime.GetValueOrDefault();
            payrollResponse.ToDateTime = payroll.ToDateTime.GetValueOrDefault();

            if (payroll.Code.IsNullOrWhiteSpace()) payrollResponse.IsHappyDay = false;

            payrollResponse.IsHappyDay = payroll.Code?.ToLower().Contains(PayrollConsts.HappyDay.ToLower()) ?? false;

            return payrollResponse;
        }

        public async Task<PayrollResponse> CalculateUserPayrolls(bool persist, bool isTempPayroll, bool isHappyDay, Guid? targetUserId)
        {
            var request = GetDefaultPayrollRequest(persist, isTempPayroll, isHappyDay);
            var userDetails = await _userDomainService.GetUserDetails(new ApiUserDetailsRequest()
            {
                GetForPayrollCalculation = true,
                GetTeamUsers = true,
                GetSystemUsers = false,
                GetActiveUsers = true,
            });
            var result = await Do(request, userDetails);

            if (targetUserId.HasValue)
            {
                var user = await _userRepository.GetAsync(targetUserId.Value);
                await _distributedEventBus.PublishAsync(new ReceivedMessageEto(targetUserId.Value, user.UserName, L["Message.PayrollCalculationSuccess"]));
            }

            return result;
        }

        private async Task<PayrollResponse> Do(PayrollRequest request, List<UserDetail> userDetails)
        {
            var response = new PayrollResponse
            {
                FromDateTime = request.FromDateTime,
                ToDateTime = request.ToDateTime,
            };
            if (userDetails.IsNullOrEmpty()) return response;

            var draftString = string.Empty;
            var division = string.Empty;
            string payrollDesc;
            if (request.IsTempPayroll)
            {
                draftString = PayrollConsts.Draft_;
                payrollDesc = L["Payrolls.TempPayrollDescription", request.FromDateTime.Month, request.FromDateTime.Year] + $" ({DateTime.UtcNow} UTC)";
            }
            else { payrollDesc = L["Payrolls.OfficialPayrollDescription", request.FromDateTime.Month, request.FromDateTime.Year] + $" ({DateTime.UtcNow} UTC)"; }

            if (request.IsHappyDay)
            {
                division = PayrollConsts.HappyDay_;
                payrollDesc += $" - {PayrollConsts.HappyDay}";
            }

            var payroll = new Payroll
            {
                Title = $"{draftString}{division}Payroll {request.FromDateTime.AddHours(_payrollConfiguration.PayrollTimeZone):yyyyMM}",
                Code = $"{draftString}{division}PAYROLL_{request.FromDateTime.AddHours(_payrollConfiguration.PayrollTimeZone):yyyyMM}", // DRAFT_PAYROLL_202105
                FromDateTime = request.FromDateTime,
                ToDateTime = request.ToDateTime,
                Description = payrollDesc
            };

            var userIds = userDetails.Select(_ => _.User.Id).ToList();
            using var uow = _unitOfWorkManager.Begin();
            {
                try
                {
                    if (request.AutoSave)
                    {
                        await DeleteDraftPayrolls(request.IsHappyDay);
                        await _payrollRepository.InsertAsync(payroll);
                    }

                    Trace.WriteLine
                    (
                        $"===================GET POSTS FOR PAYROLL CALCULATION "
                        + $"{payroll.Code} from {request.FromDateTime} to {request.ToDateTime} "
                        + $"for {userIds.Count} user(s) at {DateTime.Now}"
                    );

                    List<PostWithNavigationProperties> postNavs = new();
                    List<UserPayrollBonus> communityBonuses = new();
                    var orgUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() {IsGDLNode = true});
                    var affiliateTeams = orgUnits.Where(_ => _.DisplayName.Contains("AT", StringComparison.CurrentCultureIgnoreCase)).ToList();
                    var seedingTeams = orgUnits.Where(_ => _.DisplayName.Contains("ST", StringComparison.CurrentCultureIgnoreCase)).ToList();
                    
                    var GDLGroups = await _groupDomainService.GetManyAsync(new GetGroupApiRequest {GroupOwnershipType = GroupOwnershipType.GDLInternal});
                    var happyDayGroups = await _groupDomainService.GetManyAsync(new GetGroupApiRequest {GroupOwnershipType = GroupOwnershipType.HappyDay});
                    var happyDayGroupIds = happyDayGroups.Select(_ => _.Id).ToList();

                    if (request.IsHappyDay)
                    {
                        postNavs = (await _postRepository.GetListWithNavigationPropertiesExtendAsync
                            (
                                createdDateTimeMin: request.FromDateTime,
                                createdDateTimeMax: request.ToDateTime,
                                isValid: true,
                                appUserIds: userIds,
                                groupIds: happyDayGroupIds
                            ))
                            .Where(_ => _.Post.CreatedDateTime != null && _.Post.CampaignId == null)
                            .OrderByDescending(_ => _.Post.CreatedDateTime)
                            .ToList();
                    }
                    else
                    {
                        postNavs =
                            (await _postRepository.GetListWithNavigationPropertiesExtendAsync
                            (
                                createdDateTimeMin: request.FromDateTime,
                                createdDateTimeMax: request.ToDateTime,
                                isValid: true,
                                appUserIds: userIds,
                                groupIds: GDLGroups.Union(happyDayGroups).Select(_ => _.Id).ToList()
                            ))
                            .Where(_ => _.Post.CreatedDateTime != null && _.Post.CampaignId == null)
                            .OrderByDescending(_ => _.Post.CreatedDateTime)
                            .ToList();

                        var communityPostNavs = postNavs.Where(_ => _.AppUser.Id.IsIn(userIds)).ToList();

                        // 1. calculate community bonus - For Manager and Director
                        if (request.CalculateCommunityBonus)
                        {
                            //Only Seeding Users in CalculateCommunitySeedingBonus And CalculateSeedingTeamBonuses
                            if (seedingTeams.IsNotNullOrEmpty())
                            {
                                // NO community bonus calculation for HAPPY DAY groups
                                var seedingUsers = await _userDomainService.GetTeamMembers(seedingTeams.Select(_ => _.Id).ToList());
                                var postWithoutHappyDayGroupCodes = communityPostNavs.Where(_ => !happyDayGroupIds.Contains(_.Group.Id) && _.AppUser.Id.IsIn(seedingUsers.Select(u => u.Id))).ToList();
                                
                                communityBonuses = await CalculateCommunitySeedingBonus(payroll, postWithoutHappyDayGroupCodes);
                                response.CommunityBonuses = communityBonuses;
                                Trace.WriteLine($"===================COMMUNITY SEEDING BONUS - DONE at {DateTime.Now}");
                                
                                var teamBonuses = await CalculateSeedingTeamBonuses(postNavs, seedingTeams);
                                payroll.TeamBonuses.AddRange(teamBonuses);
                                await _payrollRepository.UpdateAsync(payroll);
                            }
                            Trace.WriteLine($"===================CalculateTeamBonuses - DONE at {DateTime.Now}");
                        }
                    }

                    var postUserIds = postNavs.Select(_ => _.Post.AppUserId.Value).Distinct().ToList();
                    userDetails = userDetails.Where(_ => _.User.Id.IsIn(postUserIds)).ToList();

                    Trace.WriteLine
                    (
                        $"===================GET POSTS FOR PAYROLL CALCULATION - "
                        + $"FOUND {postNavs.Count} posts at {DateTime.Now} "
                        + $"FOR {userDetails.Count} users"
                    );

                    var leaderUsers = await _userManager.GetUsersInRoleAsync(RoleConsts.Leader);
                    var leaderUserIds = leaderUsers.Select(l => l.Id);
                    userDetails = userDetails.Where
                        (
                            _ => postNavs.Any
                            (
                                postNav =>
                                    postNav.AppUser.Id == _.User.Id
                                    || leaderUserIds.Contains(_.Identity.Id)
                            )
                        )
                        .ToList();

                    // 2. calculate user payslips
                    foreach (var userDetail in userDetails)
                    {
                        var userPayroll = await DoGetUserPayroll
                        (
                            postNavs,
                            userDetail,
                            payroll,
                            request.FromDateTime,
                            request.ToDateTime,
                            happyDayGroupIds,
                            request.IsHappyDay
                        );
                        if (userPayroll == null) continue;

                        if (request.CalculateCommunityBonus && !request.IsHappyDay)
                        {
                            var bonuses = communityBonuses.Where(_ => _.AppUserId != null && _.AppUserId == userPayroll.AppUserId).ToList();
                            if (bonuses.IsNotNullOrEmpty())
                            {
                                userPayroll.CommunityBonuses = bonuses;

                                var userCommunityBonusAmount = bonuses.Sum(_ => _.Amount);
                                userPayroll.BonusAmount += userCommunityBonusAmount;
                            }
                        }

                        response.UserPayrolls.Add(userPayroll);

                        Trace.WriteLine($"===================DoGetUserPayroll - DONE - {userDetail.User.UserName} at {DateTime.Now}");
                    }

                    // LEADER COMMISSION
                    if (!request.IsHappyDay)
                    {
                        // 3. add commission for leader user payslips
                        var leaderIdentityUsers = await _userManager.GetUsersInRoleAsync(RoleConsts.Leader);
                        
                        foreach (var leaderIdentityUser in leaderIdentityUsers.Where(_ => userIds.Contains(_.Id)))
                        {
                            var leaderPayroll = response.UserPayrolls.FirstOrDefault(_ => _.User.Id == leaderIdentityUser.Id);
                            if (leaderPayroll == null) continue;

                            var managedUserIds = await _userDomainService.GetManagedUserIds(leaderIdentityUser.Id);
                            managedUserIds = managedUserIds.Where(_ => _ != leaderIdentityUser.Id).ToList();
                            var managedPayslips = response.UserPayrolls.Where(_ => _.User.Id.IsIn(managedUserIds)).ToList();

                            var commissions = await GetLeaderCommissions(leaderIdentityUser, payroll, managedPayslips, affiliateTeams, seedingTeams);
                            var commissionsAmount = commissions.Sum(_ => _.Amount);

                            leaderPayroll.Commissions.AddRange(commissions);
                            leaderPayroll.BonusAmount += commissionsAmount;

                            Trace.WriteLine($"===================GET LEADER COMMISSIONS - DONE - {leaderIdentityUser.UserName} at {DateTime.Now}");
                        }
                    }

                    foreach (var item in response.UserPayrolls)
                    {
                        item.TotalAmount = item.WaveAmount + item.BonusAmount;
                    }

                    if (!request.IsHappyDay)
                    {
                        // 4. TopTeamPerformance
                        var userPayrollGroups = response.UserPayrolls
                            .Where(_ => _.OrganizationId.IsNotNullOrWhiteSpace())
                            .GroupBy(_ => _.OrganizationName)
                            .ToList();
                        var bestTeamPerformances = new List<BestTeamPerformance>();
                        foreach (var userPayrollGroup in userPayrollGroups)
                        {
                            if(!userPayrollGroup.Key.IsIn(seedingTeams.Select(_ => _.DisplayName))) continue;
                            var leaderPayrolls = userPayrollGroup.Where(_ => _.Commissions.IsNotNullOrEmpty()).ToList();
                            var staffPayrolls = userPayrollGroup.Where(_ => _.Commissions.IsNullOrEmpty()).ToList();

                            if (leaderPayrolls.IsNullOrEmpty()
                                || staffPayrolls.IsNullOrEmpty()) continue;

                            bestTeamPerformances.Add
                            (
                                new BestTeamPerformance
                                {
                                    OrganizationName = userPayrollGroup.Key,
                                    LeaderUserId = leaderPayrolls.FirstOrDefault().User.Id,
                                    AverageTotalAmount = staffPayrolls.Average(userPayroll => userPayroll.TotalAmount),
                                    SumTotalAmount = staffPayrolls.Sum(userPayroll => userPayroll.TotalAmount),
                                    StaffCount = staffPayrolls.Count
                                }
                            );
                        }

                        var bestTeamPerformance = bestTeamPerformances.OrderByDescending(_ => _.AverageTotalAmount).FirstOrDefault();
                        if (bestTeamPerformance != null)
                        {
                            var organizations = await _organizationUnitRepository.GetListAsync();
                            var description = L[
                                "TopTeamPerformanceDescription",
                                _payrollConfiguration.Seeding.Bonus.WavePerformance.ToVND(),
                                bestTeamPerformance.OrganizationName,
                                bestTeamPerformance.AverageTotalAmount.ToVND(),
                                bestTeamPerformance.StaffCount.ToString()];

                            var bestTeamPerformanceBonus = new UserPayrollBonus
                            {
                                AppUserId = bestTeamPerformance.LeaderUserId,
                                PayrollId = payroll.Id,
                                Amount = _payrollConfiguration.Seeding.Bonus.WavePerformance,
                                PayrollBonusType = PayrollBonusType.PayrollPerformance,
                                Description = description
                            };

                            var leaderUserPayroll = response.UserPayrolls.FirstOrDefault(_ => _.User.Id == bestTeamPerformance.LeaderUserId);
                            if (leaderUserPayroll != null)
                            {
                                leaderUserPayroll.CommunityBonuses.Add(bestTeamPerformanceBonus);
                                leaderUserPayroll.BonusAmount += bestTeamPerformanceBonus.Amount;
                            }
                        }
                    }

                    var savedUserPayrolls = new List<UserPayroll>();
                    foreach (var item in response.UserPayrolls)
                    {
                        item.TotalAmount = item.WaveAmount + item.BonusAmount;
                        if (request.AutoSave) { savedUserPayrolls.Add(await _userPayrollRepository.InsertAsync(item)); }
                    }

                    await uow.CompleteAsync();

                    if (request.AutoSave) response.UserPayrolls = savedUserPayrolls;
                    return response;
                }
                catch (Exception e)
                {
                    await uow.RollbackAsync();
                    Trace.WriteLine("=========================== Error ===============");
                    Trace.WriteLine(e.Message);
                }
            }

            return null;
        }

        private static bool IsInTeam(List<OrganizationUnit> teams, IdentityUser leaderIdentityUser)
        {
            var isCalculateWaveCommission = true;
            foreach (var team in teams.Where(_ => isCalculateWaveCommission))
            {
                isCalculateWaveCommission = !leaderIdentityUser.IsInOrganizationUnit(team.Id);
            }

            return !isCalculateWaveCommission;
        }

        #region Calculate Leader Commission

        private async Task<List<UserPayrollCommission>> GetLeaderCommissions(IdentityUser leaderIdentityUser, Payroll payroll, List<UserPayroll> managedUserPayslips, List<OrganizationUnit> affiliateTeams, List<OrganizationUnit> seedingTeams)
        {
            var commissions = new List<UserPayrollCommission>();

            var leaderUserId = leaderIdentityUser.Id;
            var organizationUnit = (await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest()
            {
                IsGDLNode = true,
                UserId = leaderUserId
            })).FirstOrDefault();

            {
                if (seedingTeams.IsNotNullOrEmpty() && IsInTeam(seedingTeams, leaderIdentityUser))
                {
                    var seedingAmount = managedUserPayslips.SelectMany(_ => _.SeedingWaves).Sum(_ => _.Amount);
                    var seedingPercentage = _payrollConfiguration.Seeding.Bonus.LeaderCommission_Percentage;
                    var seedingCommissionAmount = seedingAmount * seedingPercentage;
                    if (seedingCommissionAmount > _payrollConfiguration.Seeding.Bonus.LeaderCommission_Max) { seedingCommissionAmount = _payrollConfiguration.Seeding.Bonus.LeaderCommission_Max; }

                    var seedingUserPayrollCommission = new UserPayrollCommission
                    {
                        AppUserId = leaderUserId,
                        PayrollId = payroll.Id,
                        OrganizationId = organizationUnit?.Id.ToString() ?? string.Empty,
                        Amount = seedingCommissionAmount,
                        PostContentType = PostContentType.Seeding,
                        PayrollCommissionType = PayrollCommissionType.SeedingSeeder,
                        PayrollCommission = (double) _payrollConfiguration.Seeding.Bonus.LeaderCommission_Percentage,
                        Description = L["PayrollCommissionType.SeedingSeeder", seedingPercentage],
                    };
                    commissions.Add(seedingUserPayrollCommission);
                }


                //True if Affiliate Bonus just have only conversion affiliate
                if (affiliateTeams.IsNotNullOrEmpty() && IsInTeam(affiliateTeams, leaderIdentityUser))
                {
                    var affiliateAmount = managedUserPayslips.SelectMany(_ => _.AffiliateBonuses).Sum(_ => _.Amount);
                    var affiliatePercentage = _payrollConfiguration.Affiliate.Bonus.LeaderCommission_Percentage;
                    var affiliateCommissionAmount = affiliateAmount * affiliatePercentage;
                    if (affiliateCommissionAmount > _payrollConfiguration.Affiliate.Bonus.LeaderCommission_Max) { affiliateCommissionAmount = _payrollConfiguration.Affiliate.Bonus.LeaderCommission_Max; }

                    var affiliateUserPayrollCommission = new UserPayrollCommission
                    {
                        AppUserId = leaderUserId,
                        PayrollId = payroll.Id,
                        OrganizationId = organizationUnit?.Id.ToString() ?? string.Empty,
                        Amount = affiliateCommissionAmount,
                        PostContentType = PostContentType.Affiliate,
                        PayrollCommissionType = PayrollCommissionType.AffiliateSeeder,
                        PayrollCommission = (double) _payrollConfiguration.Affiliate.Bonus.LeaderCommission_Percentage,
                        Description = L["PayrollCommissionType.AffiliateSeeder", affiliatePercentage],
                    };
                    commissions.Add(affiliateUserPayrollCommission);
                }
            }

            return commissions;
        }

        #endregion

        #region Payroll Bonus for the whole system

        private async Task<List<UserPayrollBonus>> CalculateCommunitySeedingBonus(Payroll payroll, List<PostWithNavigationProperties> allPosts)
        {
            var bonuses = new List<UserPayrollBonus>();

            var seedingConfig = _payrollConfiguration.Seeding;
            var postContentType = PostContentType.Seeding;

            var postNavs = allPosts
                .Where(_ => _.Post.PostContentType == postContentType)
                .Where(_ => _.Post.CreatedDateTime != null)
                .OrderByDescending(_ => _.Post.CreatedDateTime)
                .ToList();

            var postNavs_ByGroups = postNavs.Where(_ => _.Post.PostSourceType == PostSourceType.Group).GroupBy(_ => _.Group.Name).Where(_ => _.Key != null).ToList();

            // Seeding - 10 bài có tổng lượng tương tác cao nhất group(level 1)
            var seedingTopGroupReactionCount_Level1_GroupNames = new List<string>
            {
                // ONVTB, NTCM, CEMD, VNO (level 1)
                "onhavuithayba",
                "chiemmandep",
                "YAN.VietNamOi"
            };
            var level1GroupPosts = postNavs_ByGroups.Where(_ => _.Key.IsIn(seedingTopGroupReactionCount_Level1_GroupNames));
            foreach (var level1GroupPost in level1GroupPosts)
            {
                var top10BestReactionPosts_Level1 = level1GroupPost.OrderByDescending(p => p.Post.TotalCount).Take(10).ToList();
                bonuses.AddRange
                (
                    top10BestReactionPosts_Level1.Select
                    (
                        p =>
                        {
                            var bonus = new UserPayrollBonus
                            {
                                AppUserId = p.Post.AppUserId,
                                PayrollId = payroll.Id,
                                PayrollBonusType = PayrollBonusType.SeedingTopLevel1GroupReactionCount,
                                Amount = seedingConfig.Bonus.SeedingTopGroupReactionCount_Level1,
                                Description = $"{p.Group.Title} {p.Post.TotalCount.ToCommaStyle()}{L["UserPayrollDetails.Post.Reaction"]} - {FacebookHelper.GetCleanUrl(p.Post.Url)}",
                            };
                            bonus.SetProperty("PostId", p.Post.Id.ToString());

                            return bonus;
                        }
                    )
                );
            }

            // Seeding - 10 bài có tổng lượng tương tác cao nhất group (level 2)
            var seedingTopGroupReactionCount_Level2_GroupNames = new List<string>
            {
                // LDKK, LDDC, DLVTB, TKP, TDDL, CEĐD (level 2)
                "lamdepkhonghekho",
                "dilamvuithayba",
                "chiemdangchuan",
                "nhungtamchieumoi"
            };
            var level2GroupPosts = postNavs_ByGroups.Where(_ => _.Key.IsIn(seedingTopGroupReactionCount_Level2_GroupNames));
            foreach (var level2GroupPost in level2GroupPosts)
            {
                var top10BestReactionPosts_Level2 = level2GroupPost.OrderByDescending(p => p.Post.TotalCount).Take(10).ToList();
                bonuses.AddRange
                (
                    top10BestReactionPosts_Level2.Select
                    (
                        p =>
                        {
                            var bonus = new UserPayrollBonus
                            {
                                AppUserId = p.Post.AppUserId,
                                PayrollId = payroll.Id,
                                PayrollBonusType = PayrollBonusType.SeedingTopLevel2GroupReactionCount,
                                Amount = seedingConfig.Bonus.SeedingTopGroupReactionCount_Level2,
                                Description = $"{p.Group.Title} {p.Post.TotalCount.ToCommaStyle()}{L["UserPayrollDetails.Post.Reaction"]} - {FacebookHelper.GetCleanUrl(p.Post.Url)}",
                            };
                            bonus.SetProperty("PostId", p.Post.Id.ToString());

                            return bonus;
                        }
                    )
                );
            }


            return bonuses.Where(_ => _ != null).ToList();
        }

        #endregion

        #region UserPayslip - User Waves and User Bonus

        private async Task<UserPayroll> DoGetUserPayroll(
            List<PostWithNavigationProperties> allPosts,
            UserDetail userDetail,
            Payroll payroll,
            DateTime startDateTime,
            DateTime endDateTime,
            List<Guid> happyDayGroupIds,
            bool isHappyDay)
        {
            var organizationUnit = (await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest()
            {
                IsGDLNode = true,
                UserId = userDetail.User.Id
            })).FirstOrDefault();

            var userPayroll = new UserPayroll
            {
                PayrollId = payroll.Id,
                AppUserId = userDetail.User.Id,
                Code = $"{payroll.Code}_{userDetail.User.UserName}",
                OrganizationId = organizationUnit?.Id.ToString(),
                OrganizationName = organizationUnit?.DisplayName,
                ContentRoleType = userDetail.Info.ContentRoleType,
                SeedingMultiplier = userDetail.Info.SeedingMultiplier,
                WaveAmount = 0,
                User = userDetail.User,
                UserInfo = userDetail.Info,
            };

            var organizationUnitName = organizationUnit != null ? organizationUnit.DisplayName : string.Empty;
            var leader = await _userDomainService.GetUsersByRole(userDetail.User.Id, RoleConsts.Leader);
            var leaderName = leader != null ? leader.FirstOrDefault()?.Name ?? string.Empty : string.Empty;
            var descParts = new List<string>
                {
                    organizationUnitName,
                    leaderName,
                    userDetail.Info.ContentRoleType.ToString()
                }.Where(_ => _.IsNotNullOrEmpty())
                .ToList();
            userPayroll.Description = string.Join(" - ", descParts);

            // Only calculate waves for user is not gdl staff
            // seeding waves and bonuses
            {
                var seedingWaves = CalculateUserSeedingWave
                (
                    allPosts,
                    userPayroll.Id,
                    userDetail,
                    startDateTime,
                    endDateTime,
                    isHappyDay
                );
                if (seedingWaves != null)
                {
                    var waveAmount = seedingWaves.Where(_ => _ != null).Sum(_ => _.Amount);

                    userPayroll.WaveAmount += waveAmount;
                    userPayroll.SeedingWaves = seedingWaves;
                }

                var seedingBonuses = await CalculateUserSeedingBonus
                (
                    allPosts,
                    userPayroll.Id,
                    userDetail,
                    startDateTime,
                    endDateTime,
                    isHappyDay
                );
                userPayroll.BonusAmount += seedingBonuses.Sum(_ => _.Amount);
                userPayroll.SeedingBonuses = seedingBonuses;
            }

            // affiliate waves and bonuses
            {
                var affiliateWaves = CalculateUserAffiliateWave
                (
                    allPosts,
                    userPayroll.Id,
                    userDetail,
                    happyDayGroupIds
                );
                if (affiliateWaves != null)
                {
                    userPayroll.WaveAmount += affiliateWaves.Where(_ => _ != null).Sum(_ => _.Amount);
                    userPayroll.AffiliateWaves = affiliateWaves;
                }

                //Bonus
                var affiliateBonus = await CalculateUserAffiliateBonus
                (
                    userPayroll.Id,
                    userDetail,
                    startDateTime,
                    endDateTime,
                    isHappyDay
                );
                userPayroll.BonusAmount += affiliateBonus.Where(_ => _ != null).Sum(_ => _.Amount);
                userPayroll.AffiliateBonuses = affiliateBonus;
            }

            return userPayroll;
        }

        private List<UserWave> CalculateUserSeedingWave(
            List<PostWithNavigationProperties> allPosts,
            Guid payrollId,
            UserDetail userDetail,
            DateTime startDateTime,
            DateTime endDateTime,
            bool isHappyDay)
        {
            var waves = new List<UserWave>();

            var postContentType = PostContentType.Seeding;
            var seedingConfig = _payrollConfiguration.Seeding;

            var postWithNavigationProperties =
                allPosts
                    .Where
                    (
                        _ => _.Post.AppUserId == userDetail.User.Id
                             && _.Post.PostContentType == postContentType
                             && _.Post.CreatedDateTime != null
                    )
                    .OrderByDescending(_ => _.Post.CreatedDateTime)
                    .ToList();

            if (postWithNavigationProperties.IsNullOrEmpty()) return waves;

            postWithNavigationProperties = postWithNavigationProperties.Select
                (
                    p =>
                    {
                        if (p.Post.PostCopyrightType == PostCopyrightType.Unknown) { p.Post.PostCopyrightType = PostCopyrightType.Copy; }

                        if (p.Post.PostCopyrightType == PostCopyrightType.Remake) { p.Post.PostCopyrightType = PostCopyrightType.Exclusive; }

                        return p;
                    }
                )
                .ToList();
            var copyrightTypeGroups = postWithNavigationProperties.GroupBy(p => p.Post.PostCopyrightType).ToList();
            foreach (var group in copyrightTypeGroups)
            {
                var posts = group.ToList();
                if (group.Key.IsIn
                (
                    PostCopyrightType.Copy,
                    PostCopyrightType.Exclusive,
                    PostCopyrightType.VIA
                ))
                {
                    waves.Add
                    (
                        GetSeedingWave
                        (
                            postContentType,
                            @group.Key,
                            payrollId,
                            userDetail,
                            posts
                        )
                    );
                    if (group.Key == PostCopyrightType.Exclusive)
                        waves.Add
                        (
                            GetSeedingWave
                            (
                                postContentType,
                                @group.Key,
                                payrollId,
                                userDetail,
                                posts,
                                false
                            )
                        );
                }
            }

            //Calculate KPI
            var count = postWithNavigationProperties.Count;
            if (count >= seedingConfig.SeedingKPI_Count)
            {
                var amount = seedingConfig.SeedingKPI_Wave;
                var wave = new UserWave
                {
                    PayrollId = payrollId,
                    AppUserId = userDetail.User.Id,
                    TotalPostCount = count,
                    PostContentType = postContentType,
                    WaveType = WaveType.SeedingKPI,
                    Amount = amount,
                    TotalReactionCount = 0,
                    LikeCount = 0,
                    ShareCount = 0,
                    CommentCount = 0,
                    Description = L["Payroll.Seeding.Wave.SeedingKPI", seedingConfig.SeedingKPI_Count]
                };
                waves.Add(wave);
            }

            //Calculate Group Mod
            if (!isHappyDay)
            {
                if (userDetail.Info.ContentRoleType == ContentRoleType.Mod)
                {
                    var waveGroupMod =
                        _payrollConfiguration.GroupModerators.FirstOrDefault(_ => _.UserCode == userDetail.Info.Code);

                    if (waveGroupMod != null)
                    {
                        var amount = waveGroupMod.GroupWave.Sum(_ => _.Value);
                        var wave = new UserWave
                        {
                            PayrollId = payrollId,
                            AppUserId = userDetail.User.Id,
                            TotalPostCount = 0,
                            PostContentType = postContentType,
                            WaveType = WaveType.Mod,
                            Amount = amount,
                            TotalReactionCount = 0,
                            LikeCount = 0,
                            ShareCount = 0,
                            CommentCount = 0,
                            Description = L["Payroll.Seeding.Wave.Mod"]
                        };
                        wave.Description += $" \n{L["Group"]}: {string.Join(", ", waveGroupMod.GroupWave.Keys)}";

                        waves.Add(wave);
                    }
                }
            }

            return waves.Where(_ => _ != null).ToList();
        }

        private UserWave GetSeedingWave(
            PostContentType postContentType,
            PostCopyrightType postCopyrightType,
            Guid payrollId,
            UserDetail userDetail,
            List<PostWithNavigationProperties> posts,
            bool isGroupExclusive = true)
        {
            var seedingConfig = _payrollConfiguration.Seeding;

            List<PostWithNavigationProperties> postsToCalculate = posts
                .Where(p => p.Post.TotalCount >= seedingConfig.MinReactionCount)
                .Where(p => p.Post.PostCopyrightType == postCopyrightType)
                .OrderByDescending(_ => _.Post.TotalCount)
                .ToList();

            // var nonGroupPosts = postsToCalculate.Where(p => p.Post.PostSourceType != PostSourceType.Group).ToList();
            // var groupPosts = postsToCalculate.Where(p => p.Post.PostSourceType == PostSourceType.Group).OrderByDescending(_ => _.Post.TotalCount).ToList();
            int threshold_Max;

            WaveType waveType;
            decimal seedingWaveAmount;
            switch (postCopyrightType)
            {
                //todo: Add List Fanpage Group and threshold max
                case PostCopyrightType.Exclusive:
                    waveType = isGroupExclusive ? WaveType.SeedingGroupExclusive : WaveType.SeedingFanPageExclusive;
                    seedingWaveAmount = isGroupExclusive ? seedingConfig.Wave.SeedingGroupExclusive : seedingConfig.Wave.SeedingFanPageExclusive;
                    // threshold_Max = isGroupExclusive ? seedingConfig.SeedingGroupExclusive_Threshold_Max : Int32.MaxValue;
                    threshold_Max = Int32.MaxValue;
                    postsToCalculate = isGroupExclusive
                        ? postsToCalculate.Where(p => p.Post.PostSourceType == PostSourceType.Group).ToList()
                        : postsToCalculate.Where(p => p.Post.PostSourceType == PostSourceType.Page).ToList();
                    break;
                case PostCopyrightType.Copy:
                    waveType = WaveType.SeedingCopy;
                    seedingWaveAmount = seedingConfig.Wave.SeedingCopy;
                    threshold_Max = seedingConfig.SeedingCopy_Threshold_Max;
                    break;
                case PostCopyrightType.VIA:
                    waveType = WaveType.SeedingVIA;
                    seedingWaveAmount = seedingConfig.Wave.SeedingVIA;
                    threshold_Max = seedingConfig.SeedingVIA_Threshold_Max;
                    postsToCalculate = postsToCalculate.Where(_ => _.Post.TotalCount > seedingConfig.MinVIAReactionCount).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(postCopyrightType), postCopyrightType, null);
            }

            postsToCalculate = postsToCalculate.Take(threshold_Max).ToList();
            // groupPosts = groupPosts.Take(groupPostsCount).ToList();
            // postsToCalculate = groupPosts.Union(nonGroupPosts).ToList();

            var postCount = postsToCalculate.Count;

            var amount = postCount * seedingWaveAmount * (decimal) userDetail.Info.SeedingMultiplier;

            var like = postsToCalculate.Sum(p => p.Post.LikeCount);
            var share = postsToCalculate.Sum(p => p.Post.ShareCount);
            var comment = postsToCalculate.Sum(p => p.Post.CommentCount);

            var wave = new UserWave
            {
                PayrollId = payrollId,
                AppUserId = userDetail.User.Id,
                TotalPostCount = postCount,
                PostContentType = postContentType,
                WaveType = waveType,
                Amount = amount,
                TotalReactionCount = like + share + comment,
                LikeCount = like,
                ShareCount = share,
                CommentCount = comment,
                Description = waveType.ToString()
            };

            return wave;
        }

        private async Task<List<UserPayrollBonus>> CalculateUserSeedingBonus(
            List<PostWithNavigationProperties> allPosts,
            Guid payrollId,
            UserDetail userDetail,
            DateTime startDateTime,
            DateTime endDateTime,
            bool isHappyDay)
        {
            var bonuses = new List<UserPayrollBonus>();

            var postContentType = PostContentType.Seeding;
            var seedingConfig = _payrollConfiguration.Seeding;

            var postNavs = allPosts
                .Where
                (
                    _ => _.Post.AppUserId == userDetail.User.Id
                         && _.Post.PostContentType == postContentType
                         && _.Post.CreatedDateTime != null
                )
                .OrderByDescending(_ => _.Post.TotalCount)
                .ToList();
            if (postNavs.IsNullOrEmpty()) return bonuses;

            if (!isHappyDay)
            {
                if (postNavs.Count >= 100)
                {
                    var averageReactionTop100Post = postNavs.Take(100).Average(_ => _.Post.TotalCount);
                    var oldAverageReactionTop100Post = userDetail.Info.AverageReactionTop100Post;
                    if (averageReactionTop100Post > oldAverageReactionTop100Post)
                    {
                        if (oldAverageReactionTop100Post > 0)
                        {
                            var bonusViaKpi1 = new UserPayrollBonus
                            {
                                PayrollId = payrollId,
                                AppUserId = userDetail.User.Id,
                                PayrollBonusType = PayrollBonusType.AverageReactionTop100Post,
                                Amount = seedingConfig.Bonus.SeedingAverageReactionTop100Post,
                                Description = L["Payroll.Seeding.Bonus.SeedingAverageReactionTop100Post", averageReactionTop100Post, oldAverageReactionTop100Post]
                            };
                            bonuses.Add(bonusViaKpi1);
                        }

                        userDetail.Info.AverageReactionTop100Post = averageReactionTop100Post;
                        await _userInfoRepository.UpdateAsync(userDetail.Info);
                    }
                }
            }

            return bonuses;
        }

        private async Task<List<UserPayrollBonus>> CalculateUserAffiliateBonus(
            Guid payrollId,
            UserDetail userDetail,
            DateTime startDateTime,
            DateTime endDateTime,
            bool isHappyDay)
        {
            var bonus = new List<UserPayrollBonus>();

            var userAffiliates = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties
            (
                appUserId: userDetail.User.Id
                // createdAtMin: startDateTime,
                // createdAtMax: endDateTime
            );
            userAffiliates = userAffiliates.Where(_ => _.UserAffiliate.AffConversionModel.ClickCount == -1 && _.UserAffiliate.AffConversionModel.CommissionBonusAmount == -1).ToList();
            userAffiliates = isHappyDay 
                ? userAffiliates.Where(_ => _.UserAffiliate.AffiliateUrl.Contains(GlobalConsts.HPDDomain)).ToList() 
                : userAffiliates.Where(_ => _.UserAffiliate.AffiliateUrl.Contains(GlobalConsts.GDLDomain) || _.UserAffiliate.AffiliateUrl.Contains(GlobalConsts.BaseAffiliateDomain)).ToList();
            
            var shopeeAffiliates = userAffiliates.Where(_ => _.UserAffiliate.MarketplaceType == MarketplaceType.Shopee).ToList();
            var lazadaAffiliates = userAffiliates.Where(_ => _.UserAffiliate.MarketplaceType == MarketplaceType.Lazada).ToList();
            var shopeeConversionCount = shopeeAffiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ConversionCount);
            var lazadaConversionCount = lazadaAffiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ConversionCount);
            
            var bonusType = PayrollBonusType.AffiliateConversion;
            var shopeeConversionUnitAmount = _payrollConfiguration.Affiliate.Bonus.ShopeeAmountPerConversion;
            var lazadaConversionUnitAmount = _payrollConfiguration.Affiliate.Bonus.LazadaAmountPerConversion;
            
            var amount = (shopeeConversionCount * shopeeConversionUnitAmount + lazadaConversionCount * lazadaConversionUnitAmount) * (decimal) userDetail.Info.AffiliateMultiplier;
            bonus.Add
            (
                new UserPayrollBonus
                {
                    PayrollId = payrollId,
                    AppUserId = userDetail.User.Id,
                    PayrollBonusType = bonusType,
                    Amount = amount,
                    Description = L[$"Payroll.Affiliate.Bonus.{bonusType}"]
                }
            );

            return bonus.Where(_ => _ != null).ToList();
        }
        
        private List<UserWave> CalculateUserAffiliateWave(
            List<PostWithNavigationProperties> allPosts,
            Guid payrollId,
            UserDetail userDetail,
            List<Guid> happyDayGroupIds)
        {
            var waves = new List<UserWave>();
            var affiliateConfig = _payrollConfiguration.Affiliate;
            var postContentType = PostContentType.Affiliate;

            var postNavs = allPosts.Where
            (
                _ => _.Post.AppUserId == userDetail.User.Id
                     && _.Post.PostContentType == postContentType
                     && _.Post.CreatedDateTime != null
                     && _.Post.GroupId.IsNotNullOrEmpty()).ToList();
            var gdlPostNavs = postNavs
                .Where(_ => (!happyDayGroupIds.Contains((Guid) _.Post.GroupId))&& (_.Post.PostCopyrightType is PostCopyrightType.Remake or PostCopyrightType.Exclusive))
                .OrderByDescending(_ => _.Post.CreatedDateTime)
                .ToList();
            
            var happyDayPostNavs = postNavs
                .Where(_ => (happyDayGroupIds.Contains((Guid) _.Post.GroupId)))
                .OrderByDescending(_ => _.Post.CreatedDateTime)
                .ToList();

            if (gdlPostNavs.IsNullOrEmpty() && happyDayPostNavs.IsNullOrEmpty()) return waves;

            // limit post for some groups
            // {
            //     var groupFids_Limit180Posts = new List<string>
            //     {
            //         "329930094679983", // một chút đáng yêu mỗi ngày 
            //         "131140171935405" // chiemdepchanhsa
            //     };
            //
            //     var groupFids_Limit120Posts = new List<string>
            //     {
            //         "289584888717078", // dilamvuithayba
            //         "1809810929300686", // group yan pets
            //         "1454785734540259" // khoe con
            //     };
            //
            //     var limitedPosts = postNavs.Where(_ => _.Group.Fid == "329930094679983").Take(180)
            //         .Union(postNavs.Where(_ => _.Group.Fid == "131140171935405").Take(180))
            //         .Union(postNavs.Where(_ => _.Group.Fid == "289584888717078").Take(120))
            //         .Union(postNavs.Where(_ => _.Group.Fid == "1809810929300686").Take(120))
            //         .Union(postNavs.Where(_ => _.Group.Fid == "1454785734540259").Take(120));
            //     postNavs = postNavs
            //         .Where(_ => !_.Group.Fid.IsIn(groupFids_Limit180Posts))
            //         .Where(_ => !_.Group.Fid.IsIn(groupFids_Limit120Posts))
            //         .ToList();
            //
            //     postNavs = postNavs.Union(limitedPosts).ToList();
            // }

            var postNav_groupLevelA = gdlPostNavs.Where(_ => _.Post.TotalCount >= affiliateConfig.MinReactionCount).ToList();
            var postNav_groupBelowA = gdlPostNavs.Where(_ => _.Group.Point < 100 && _.Post.TotalCount < affiliateConfig.MinReactionCount).ToList();

            var single = postNav_groupLevelA.Where(p => p.Post.Shortlinks.Count == 1).ToList();
            var multiple = postNav_groupLevelA.Where(p => p.Post.Shortlinks.Count > 1).ToList();
            
            var contentRoleType = userDetail.Info.ContentRoleType;
            switch (contentRoleType)
            {
                case ContentRoleType.Admin:
                case ContentRoleType.Mod:
                case ContentRoleType.Seeder:
                {
                    var singleExclusiveWave = GetWave
                    (
                        userDetail,
                        payrollId,
                        single,
                        postContentType,
                        WaveType.AffiliateSingleExclusive,
                        affiliateConfig.Wave.AffiliateSingleExclusive,
                        (decimal) userDetail.Info.AffiliateMultiplier
                    );
                    waves.Add(singleExclusiveWave);

                    var multipleExclusiveWave = GetWave
                    (
                        userDetail,
                        payrollId,
                        multiple,
                        postContentType,
                        WaveType.AffiliateMultipleExclusive,
                        affiliateConfig.Wave.AffiliateMultipleExclusive,
                        (decimal) userDetail.Info.AffiliateMultiplier
                    );
                    waves.Add(multipleExclusiveWave);

                    var notAchievedWave = GetWave
                    (
                        userDetail,
                        payrollId,
                        postNav_groupBelowA,
                        postContentType,
                        WaveType.AffiliateNotAchieved,
                        affiliateConfig.Wave.AffiliateNotAchieved,
                        (decimal) userDetail.Info.AffiliateMultiplier
                    );
                    waves.Add(notAchievedWave);
                    break;
                }
                // {
                //     var singleWave = GetWave(userDetail,
                //         payrollId,
                //         singleCopy,
                //         postContentType,
                //         WaveType.AffiliateSingleEditor,
                //         affiliateConfig.WaveEditor.AffiliateSingle,
                //         (decimal) userDetail.Info.AffiliateMultiplier);
                //     waves.Add(singleWave);
                //
                //     var multipleWave = GetWave(userDetail,
                //         payrollId,
                //         multipleCopy,
                //         postContentType,
                //         WaveType.AffiliateMultipleEditor,
                //         affiliateConfig.WaveEditor.AffiliateMultiple,
                //         (decimal) userDetail.Info.AffiliateMultiplier);
                //     waves.Add(multipleWave);
                //     break;
                // }
            }
            
            var happyDayWave = GetWave
            (
                userDetail,
                payrollId,
                happyDayPostNavs,
                postContentType,
                WaveType.AffiliateHappyDay,
                affiliateConfig.Wave.AffiliateHappyDayExclusive,
                1
            );
            waves.Add(happyDayWave);

            return waves.Where(_ => _ != null).ToList();
        }

        private UserPayrollBonus GetBonus(
            bool isGDLStaff,
            Guid payrollId,
            Guid? userId,
            List<PostWithNavigationProperties> posts,
            PostContentType postContentType,
            PayrollBonusType bonusType,
            decimal bonusAmount,
            decimal multiplier)
        {
            if (posts.IsNullOrEmpty()) return null;

            var amount = isGDLStaff ? bonusAmount : (bonusAmount * multiplier);
            return new UserPayrollBonus
            {
                PayrollId = payrollId,
                AppUserId = userId,
                PayrollBonusType = bonusType,
                Amount = amount,
                Description = bonusType.ToString()
            };
        }

        private UserWave GetWave(
            UserDetail userDetail,
            Guid payrollId,
            List<PostWithNavigationProperties> posts,
            PostContentType postContentType,
            WaveType waveType,
            decimal unitWaveAmount,
            decimal multiplier)
        {
            if (posts.IsNullOrEmpty()) return null;

            var amount = posts.Count * unitWaveAmount * multiplier;
            return new()
            {
                PayrollId = payrollId,
                AppUserId = userDetail.User.Id,
                TotalPostCount = posts.Count,
                PostContentType = postContentType,
                WaveType = waveType,
                Amount = amount,
                TotalReactionCount = posts.Sum(_ => _.Post.TotalCount),
                LikeCount = posts.Sum(p => p.Post.LikeCount),
                ShareCount = posts.Sum(p => p.Post.ShareCount),
                CommentCount = posts.Sum(p => p.Post.CommentCount),
                Description = waveType.ToString()
            };
        }

        #endregion

        #region Base Method

        public async Task ConfirmPayroll(Guid payrollId)
        {
            // todo Huy: use UoW, please note that we maybe fall into UoW IN UoW => need to merge 2 UoW
            var mainPayroll = await _payrollRepository.GetAsync(payrollId);
            var isHappyDay = mainPayroll.Description?.ToLower().Contains(PayrollConsts.HappyDay.ToLower()) ?? false;

            var month = mainPayroll.ToDateTime.Value.Month;
            var year = mainPayroll.ToDateTime.Value.Year;
            mainPayroll.Description = L["Payrolls.OfficialPayrollDescription", month, year] + (isHappyDay ? $" {PayrollConsts.HappyDay}" : "");
            mainPayroll.Code = L["Payrolls.OfficialPayrollTitle", year, month] + (isHappyDay ? $" {PayrollConsts.HappyDay}" : "");
            mainPayroll.Title = L["Payrolls.OfficialPayrollCode", year, month] + (isHappyDay ? $" {PayrollConsts.HappyDay}" : "");
            await _payrollRepository.UpdateAsync(mainPayroll);

            await DeleteDraftPayrolls(isHappyDay);
        }

        private async Task DeleteDraftPayrolls(bool isHappyDay)
        {
            List<Payroll> draftPayrolls = new();

            if (isHappyDay) { draftPayrolls = _payrollRepository.Where(_ => _.Code.ToLower().Contains("draft") && _.Code.ToLower().Contains(PayrollConsts.HappyDay.ToLower())).ToList(); }
            else { draftPayrolls = _payrollRepository.Where(_ => _.Code.ToLower().Contains("draft") && !_.Code.ToLower().Contains(PayrollConsts.HappyDay.ToLower())).ToList(); }

            var draftPayrollIds = draftPayrolls.Select(_ => _.Id).ToList();
            var draftUserPayrolls = _userPayrollRepository.Where(_ => draftPayrollIds.Contains(_.PayrollId.Value)).ToList();

            await _userPayrollRepository.DeleteManyAsync(draftUserPayrolls);
            await _payrollRepository.DeleteManyAsync(draftPayrolls);
        }

        #endregion

        #region Calculate Team Bonuses

        public async Task<List<TeamBonuses>> CalculateSeedingTeamBonuses(List<PostWithNavigationProperties> allPosts, List<OrganizationUnit> seedingTeams)
        {
            // Team Bonus - Top 3 - Total 5 bonus: ReactionAvg, ReactionCount...
            var teamPayrollBonuses = new List<TeamBonuses>();
            var payrollTeamStats = new List<PayrollTeamStat>();
            var seedingConfig = _payrollConfiguration.Seeding;
            foreach (var org in seedingTeams)
            {
                var users = await _userDomainService.GetTeamMembers(org.Id);
                var posts = allPosts.Where(_ => _.Post.PostContentType == PostContentType.Seeding && _.AppUser.Id.IsIn(users.Select(u => u.Id)))
                    .Select(_ => _.Post)
                    .ToList();

                if (posts.IsNotNullOrEmpty())
                {
                    var payrollTeamStat = new PayrollTeamStat()
                    {
                        Team = org.DisplayName,
                        Reaction_Avg = posts.OrderByDescending(_ => _.TotalCount).Take(100).Average(_ => _.TotalCount),
                        Reaction_Count_Via = posts.Where(_ => _.PostCopyrightType == PostCopyrightType.VIA).Sum(_ => _.TotalCount),
                        Reaction_Count_Seeding = posts.Sum(_ => _.TotalCount),
                        Reaction_Count_Share = posts.Sum(_ => _.ShareCount),
                        Reaction_Count_Comment = posts.Sum(_ => _.ShareCount)
                    };
                    payrollTeamStats.Add(payrollTeamStat);
                }
            }

            //Make sure at least 3 team
            var payrollTopTeamAverage = payrollTeamStats.OrderByDescending(_ => _.Reaction_Avg).Take(3).ToList();
            if (payrollTopTeamAverage.IsNotNullOrEmpty())
            {
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamAverage.First().Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop1TeamPostAverageReaction,
                        Amount = seedingConfig.Bonus.SeedingTop1Bonus,
                        Description = $"{payrollTopTeamAverage.First().Team} - {L["UserPayrollDetails.Post.SeedingTop1TeamPostAverageReaction"]}"
                    }
                );
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamAverage[1].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop2TeamPostAverageReaction,
                        Amount = seedingConfig.Bonus.SeedingTop2Bonus,
                        Description = $"{payrollTopTeamAverage[1].Team} - {L["UserPayrollDetails.Post.SeedingTop2TeamPostAverageReaction"]}"
                    }
                );

                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamAverage[2].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop3TeamPostAverageReaction,
                        Amount = seedingConfig.Bonus.SeedingTop3Bonus,
                        Description = $"{payrollTopTeamAverage[2].Team} - {L["UserPayrollDetails.Post.SeedingTop3TeamPostAverageReaction"]}"
                    }
                );
            }

            var payrollTopTeamPostViaReaction = payrollTeamStats.OrderByDescending(_ => _.Reaction_Count_Via).Take(3).ToList();
            if (payrollTopTeamPostViaReaction.IsNotNullOrEmpty())
            {
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamPostViaReaction.First().Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop1TeamPostViaReaction,
                        Amount = seedingConfig.Bonus.SeedingTop1Bonus,
                        Description = $"{payrollTopTeamPostViaReaction.First().Team} - {L["UserPayrollDetails.Post.SeedingTop1TeamPostViaReaction"]}"
                    }
                );
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamPostViaReaction[1].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop2TeamPostViaReaction,
                        Amount = seedingConfig.Bonus.SeedingTop2Bonus,
                        Description = $"{payrollTopTeamPostViaReaction[1].Team} - {L["UserPayrollDetails.Post.SeedingTop2TeamPostViaReaction"]}"
                    }
                );

                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamPostViaReaction[2].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop3TeamPostViaReaction,
                        Amount = seedingConfig.Bonus.SeedingTop3Bonus,
                        Description = $"{payrollTopTeamPostViaReaction[2].Team} - {L["UserPayrollDetails.Post.SeedingTop3TeamPostViaReaction"]}"
                    }
                );
            }

            var payrollTopTeamReaction = payrollTeamStats.OrderByDescending(_ => _.Reaction_Count_Seeding).Take(3).ToList();
            if (payrollTopTeamReaction.IsNotNullOrEmpty())
            {
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamReaction.First().Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop1TeamReaction,
                        Amount = seedingConfig.Bonus.SeedingTop1Bonus,
                        Description = $"{payrollTopTeamReaction.First().Team} - {L["UserPayrollDetails.Post.SeedingTop1TeamReaction"]}"
                    }
                );
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamReaction[1].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop2TeamReaction,
                        Amount = seedingConfig.Bonus.SeedingTop2Bonus,
                        Description = $"{payrollTopTeamReaction[1].Team} - {L["UserPayrollDetails.Post.SeedingTop2TeamReaction"]}"
                    }
                );

                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamReaction[2].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop3TeamReaction,
                        Amount = seedingConfig.Bonus.SeedingTop3Bonus,
                        Description = $"{payrollTopTeamReaction[2].Team} - {L["UserPayrollDetails.Post.SeedingTop3TeamReaction"]}"
                    }
                );
            }

            var payrollTopTeamShareCount = payrollTeamStats.OrderByDescending(_ => _.Reaction_Count_Share).Take(3).ToList();
            if (payrollTopTeamShareCount.IsNotNullOrEmpty())
            {
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamShareCount.First().Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop1TeamShareCount,
                        Amount = seedingConfig.Bonus.SeedingTop1Bonus,
                        Description = $"{payrollTopTeamShareCount.First().Team} - {L["UserPayrollDetails.Post.SeedingTop1TeamShareCount"]}"
                    }
                );
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamShareCount[1].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop2TeamShareCount,
                        Amount = seedingConfig.Bonus.SeedingTop2Bonus,
                        Description = $"{payrollTopTeamShareCount[1].Team} - {L["UserPayrollDetails.Post.SeedingTop2TeamShareCount"]}"
                    }
                );

                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamShareCount[2].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop3TeamShareCount,
                        Amount = seedingConfig.Bonus.SeedingTop3Bonus,
                        Description = $"{payrollTopTeamShareCount[2].Team} - {L["UserPayrollDetails.Post.SeedingTop3TeamShareCount"]}"
                    }
                );
            }

            var payrollTopTeamCommentCount = payrollTeamStats.OrderByDescending(_ => _.Reaction_Count_Comment).Take(3).ToList();
            if (payrollTopTeamCommentCount.IsNotNullOrEmpty())
            {
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamCommentCount.First().Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop1TeamCommentCount,
                        Amount = seedingConfig.Bonus.SeedingTop1Bonus,
                        Description = $"{payrollTopTeamCommentCount.First().Team} - {L["UserPayrollDetails.Post.SeedingTop1TeamCommentCount"]}"
                    }
                );
                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamCommentCount[1].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop2TeamCommentCount,
                        Amount = seedingConfig.Bonus.SeedingTop2Bonus,
                        Description = $"{payrollTopTeamCommentCount[1].Team} - {L["UserPayrollDetails.Post.SeedingTop2TeamCommentCount"]}"
                    }
                );

                teamPayrollBonuses.Add
                (
                    new TeamBonuses()
                    {
                        TeamName = payrollTopTeamCommentCount[2].Team,
                        PayrollBonusType = PayrollBonusType.SeedingTop3TeamCommentCount,
                        Amount = seedingConfig.Bonus.SeedingTop3Bonus,
                        Description = $"{payrollTopTeamCommentCount[2].Team} - {L["UserPayrollDetails.Post.SeedingTop3TeamCommentCount"]}"
                    }
                );
            }

            return teamPayrollBonuses;
        }

        #endregion

        #region CalculateWaveMultipliers

        public async Task RecalculateWaveMultipliers()
        {
            var appUsers = await _userRepository.GetListAsync();
            var userInfos = await _userInfoRepository.Get(appUsers.Select(u => u.Id).ToArray());

            foreach (var userInfo in userInfos)
            {
                var joinTime = userInfo.UserInfo.JoinedDateTime;
                if (!joinTime.HasValue) continue;

                var userRoles = await _userRoleFinder.GetRolesAsync(userInfo.AppUser.Id);
                var userInfoMultiplier = userRoles.Contains(RoleConsts.Leader)
                    ? CalculateLeaderMultiplier(joinTime.Value)
                    : CalculateStaffMultiplier(joinTime.Value);

                userInfo.UserInfo.JoinedDateTime = joinTime;
                userInfo.UserInfo.SeedingMultiplier = (double) userInfoMultiplier.SeedingMultiplier;
                userInfo.UserInfo.AffiliateMultiplier = (double) userInfoMultiplier.AffiliateMultiplier;
                userInfo.UserInfo.PromotedDateTime = joinTime.Value.AddDays(userInfoMultiplier.PromotionDay);

                await _userInfoRepository.UpdateAsync(userInfo.UserInfo);
            }
        }

        private UserInfoMultiplier CalculateStaffMultiplier(DateTime dateTime)
        {
            var userInfoMultiplier = new UserInfoMultiplier();

            var payrollConfiguration = _payrollConfiguration.WaveMultiplier;

            var days = (DateTime.UtcNow - dateTime).Days;
            var rate = (int) Math.Floor((double) days / payrollConfiguration.Seeding_Staff_PromotionDayCountBase);
            switch (days)
            {
                case <= 365:
                    userInfoMultiplier.SeedingMultiplier = payrollConfiguration.Seeding_Staff_MultiplierBase + (rate * payrollConfiguration.BasePromotionRate);
                    break;
                default:
                    userInfoMultiplier.SeedingMultiplier = payrollConfiguration.Seeding_Staff_MultiplierMax;
                    break;
            }

            if (days > 182)
            {
                var affRate = 1 + (int) Math.Floor((double) (days - 182) / payrollConfiguration.Affiliate_PromotionDayCountBase);
                userInfoMultiplier.AffiliateMultiplier
                    = days > 365 ? payrollConfiguration.AffiliateMultiplierMax : payrollConfiguration.AffiliateMultiplierBase + (payrollConfiguration.BasePromotionRate * affRate);
            }
            else { userInfoMultiplier.AffiliateMultiplier = payrollConfiguration.AffiliateMultiplierBase; }

            userInfoMultiplier.PromotionDay = days > payrollConfiguration.Seeding_Staff_PromotionDayCountMax
                ? payrollConfiguration.Seeding_Staff_PromotionDayCountMax
                : rate * payrollConfiguration.Seeding_Staff_PromotionDayCountBase;
            return userInfoMultiplier;
        }

        private UserInfoMultiplier CalculateLeaderMultiplier(DateTime dateTime)
        {
            var userInfoMultiplier = new UserInfoMultiplier();

            var payrollConfiguration = _payrollConfiguration.WaveMultiplier;

            var days = (DateTime.UtcNow - dateTime).Days;
            var rate = ((int) (days / payrollConfiguration.Seeding_Leader_PromotionDayCountBase));

            userInfoMultiplier.SeedingMultiplier =
                days <= 730 ? payrollConfiguration.Seeding_Leader_MultiplierBase + (rate * payrollConfiguration.BasePromotionRate) : payrollConfiguration.Seeding_Leader_MultiplierMax;

            if (days > 182)
            {
                var affRate = 1 + (int) Math.Floor((double) (days - 182) / payrollConfiguration.Affiliate_PromotionDayCountBase);
                userInfoMultiplier.AffiliateMultiplier
                    = days > 365 ? payrollConfiguration.AffiliateMultiplierMax : payrollConfiguration.AffiliateMultiplierBase + (payrollConfiguration.BasePromotionRate * affRate);
            }
            else { userInfoMultiplier.AffiliateMultiplier = payrollConfiguration.AffiliateMultiplierBase; }

            userInfoMultiplier.PromotionDay = days > payrollConfiguration.Seeding_Leader_PromotionDayCountMax
                ? payrollConfiguration.Seeding_Leader_PromotionDayCountMax
                : rate * payrollConfiguration.Seeding_Leader_PromotionDayCountBase;

            return userInfoMultiplier;
        }

        #endregion

        // private List<UserPayrollBonus> CalculateTopAffiliateConversionBonus(List<UserAffiliate> userAffiliates, Payroll payroll)
        // {
        //     List<UserPayrollBonus> bonus = new();
        //     List<UserAffiliateConversionModel> affiliateConversionTotalModels = userAffiliates.Where(_ => _.AppUserId != null)
        //         .GroupBy(_ => _.AppUserId).Select(model => new UserAffiliateConversionModel
        //         {
        //             AppUserId = model.Key,
        //             ConversionCount = model.Sum(_ => _.UserAffiliateConversion.ConversionCount),
        //             ConversionAmount = model.Sum(_ => _.UserAffiliateConversion.ConversionAmount),
        //             AffiliateCount = model.Count()
        //         }).OrderByDescending(_ => _.ConversionAmount).ToList();
        //
        //     if (affiliateConversionTotalModels.IsNotNullOrEmpty())
        //     {
        //         bonus.Add(new UserPayrollBonus
        //         {
        //             AppUserId = affiliateConversionTotalModels.FirstOrDefault()?.AppUserId,
        //             PayrollId = payroll.Id,
        //             PayrollBonusType = PayrollBonusType.AffiliateConversion_Top1,
        //             Amount = _payrollConfiguration.Affiliate.Bonus.AffiliateConversion_Top1,
        //             Description = L["UserPayrollDetails.AffiliateConversion_Top1"],
        //         });
        //         bonus.Add(new UserPayrollBonus
        //         {
        //             AppUserId = affiliateConversionTotalModels.Skip(1).FirstOrDefault()?.AppUserId,
        //             PayrollId = payroll.Id,
        //             PayrollBonusType = PayrollBonusType.AffiliateConversion_Top2,
        //             Amount = _payrollConfiguration.Affiliate.Bonus.AffiliateConversion_Top2,
        //             Description = L["UserPayrollDetails.AffiliateConversion_Top2"],
        //         });
        //         bonus.Add(new UserPayrollBonus
        //         {
        //             AppUserId = affiliateConversionTotalModels.Skip(2).FirstOrDefault()?.AppUserId,
        //             PayrollId = payroll.Id,
        //             PayrollBonusType = PayrollBonusType.AffiliateConversion_Top3,
        //             Amount = _payrollConfiguration.Affiliate.Bonus.AffiliateConversion_Top3,
        //             Description = L["UserPayrollDetails.AffiliateConversion_Top3"],
        //         });
        //         for (var i = 0; i < 7; i++)
        //         {
        //             bonus.Add(new UserPayrollBonus
        //             {
        //                 AppUserId = affiliateConversionTotalModels.Skip(i + 3).FirstOrDefault()?.AppUserId,
        //                 PayrollId = payroll.Id,
        //                 PayrollBonusType = PayrollBonusType.AffiliateConversion_Top4_10,
        //                 Amount = _payrollConfiguration.Affiliate.Bonus.AffiliateConversion_Top4_10,
        //                 Description = L["UserPayrollDetails.AffiliateConversion_Top4_10", (i + 3).ToString()],
        //             });
        //         }
        //     }
        //
        //     return bonus;
        // }
    }
}