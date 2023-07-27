using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.UserSalaryConfigurations;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using UrlHelper = FacebookCommunityAnalytics.Api.Core.Helpers.UrlHelper;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public interface IUserCompensationDomainService : IDomainService
    {
        Task<CompensationResponse> CalculateCompensations(
            bool persist,
            bool isTempPayroll,
            bool isHappyDay,
            Guid? targetUserId,
            int month,
            int year);

        Task DeleteCompensation(Guid payrollId);
        Task ConfirmPayroll(Guid payrollId);
        Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId);
    }

    public class UserCompensationDomainService : BaseDomainService, IUserCompensationDomainService
    {
        /// <summary>
        /// ST3 Team bonus Affiliate Config: 5k/post under 60 affiliate posts and 500k bonus if over
        /// </summary>
        private const int SpecialCase_AffiliatePostAmount = 5000;

        private const int SpecialCase_MaxAffiliatePostCount = 60;
        private const int SpecialCase_MaxBonusAmount = 500000;

        private List<string> happyDayUsers = new List<string>()
        {
            "ST1_DuongNuHuynhLinh",
            "ST1_PhanHien",
            "ST1_PhungHyen",
            "ST1_TrThiBinh04",
            "ST1_VietHung",
            "ST1_PhuongThao",
            "ST1_NguyenHuyen",
            "ST1_ThuyTien",
            "ST1_KhangHoang",
            "ST1_NguyenThiThuy",
            "ST1_DoLeThuyTrang",
            "ST1_LuongYenKhanh",
            "ST1_KimNgoc"
        };

        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IPostRepository _postRepository;
        private readonly IPayrollRepository _payrollRepository;

        private readonly IUserDomainService _userDomainService;
        private readonly IGroupDomainService _groupDomainService;
        private readonly IStaffEvaluationDomainService _staffEvaluationDomain;


        private readonly IUserCompensationRepository _userCompensationRepository;
        private readonly IAffiliateConversionRepository _affiliateConversionRepository;
        private readonly IRepository<UserBonusConfig, Guid> _userBonusConfigRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IStaffEvaluationRepository _staffEvaluationRepository;

        public UserCompensationDomainService(
            IOrganizationDomainService organizationDomainService,
            IUserDomainService userDomainService,
            IGroupDomainService groupDomainService,
            IPostRepository postRepository,
            IPayrollRepository payrollRepository,
            IUserCompensationRepository userCompensationRepository,
            IAffiliateConversionRepository affiliateConversionRepository,
            IRepository<UserBonusConfig, Guid> userBonusConfigRepository,
            IUserAffiliateRepository userAffiliateRepository,
            IStaffEvaluationRepository staffEvaluationRepository,
            IStaffEvaluationDomainService staffEvaluationDomain)
        {
            _payrollRepository = payrollRepository;
            _userDomainService = userDomainService;
            _groupDomainService = groupDomainService;
            _userCompensationRepository = userCompensationRepository;
            _affiliateConversionRepository = affiliateConversionRepository;
            _userBonusConfigRepository = userBonusConfigRepository;
            _userAffiliateRepository = userAffiliateRepository;
            _staffEvaluationRepository = staffEvaluationRepository;
            _staffEvaluationDomain = staffEvaluationDomain;
            _postRepository = postRepository;
            _organizationDomainService = organizationDomainService;
        }

        public async Task DeleteCompensation(Guid payrollId)
        {
            var userPayrolls = await _userCompensationRepository.GetListAsync(x => x.PayrollId == payrollId);

            foreach (var userPayroll in userPayrolls)
            {
                await _userCompensationRepository.DeleteAsync(userPayroll);
            }

            await _payrollRepository.DeleteAsync(x => x.Id == payrollId);
        }


        private PayrollRequest GetDefaultPayrollRequest(
            bool autoSave,
            bool isTempPayroll,
            bool isHappyDay,
            int month,
            int year)
        {
            var (fromDateTime, toDateTime) = GetPayrollDateTime(year, month);

            var payrollRequest = new PayrollRequest
            {
                FromDateTime = fromDateTime,
                ToDateTime = toDateTime,
                AutoSave = autoSave,
                IsTempPayroll = isTempPayroll,
                IsHappyDay = isHappyDay
            };

            return payrollRequest;
        }

        public async Task<CompensationResponse> CalculateCompensations(
            bool persist,
            bool isTempPayroll,
            bool isHappyDay,
            Guid? targetUserId,
            int month,
            int year)
        {
            var request = GetDefaultPayrollRequest
            (
                persist,
                isTempPayroll,
                isHappyDay,
                month,
                year
            );
            var userDetails = await _userDomainService.GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetForPayrollCalculation = true,
                    GetTeamUsers = true,
                    GetSystemUsers = false,
                    GetActiveUsers = true,
                }
            );
            var result = await DoCalculateCompensations(request, userDetails);

            // if (targetUserId.HasValue)
            // {
            //     var user = await _userRepository.GetAsync(targetUserId.Value);
            //     await _distributedEventBus.PublishAsync(new ReceivedMessageEto(targetUserId.Value, user.UserName, L["Message.PayrollCalulationSuccess"]));
            // }

            return result;
        }

        private async Task<CompensationResponse> DoCalculateCompensations(PayrollRequest request, List<UserDetail> userDetails)
        {
            var response = new CompensationResponse
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
                payrollDesc = L["Payrolls.TempPayrollDescription", request.ToDateTime.Month, request.ToDateTime.Year] + $" ({DateTime.UtcNow} UTC)";
            }
            else { payrollDesc = L["Payrolls.OfficialPayrollDescription", request.ToDateTime.Month, request.ToDateTime.Year] + $" ({DateTime.UtcNow} UTC)"; }

            if (request.IsHappyDay)
            {
                division = PayrollConsts.HappyDay_;
                payrollDesc += $" - {PayrollConsts.HappyDay}";
            }

            var payroll = new Payroll
            {
                Title = $"{draftString}{division}Payroll_v2 {request.ToDateTime:yyyy.MM}",
                Code = $"{draftString}{division}PAYROLL_v2_{request.ToDateTime:yyyy.MM}", // e.g. DRAFT_PAYROLL_202105
                FromDateTime = request.FromDateTime,
                ToDateTime = request.ToDateTime,
                Description = payrollDesc,
                IsCompensation = true
            };

            var userIds = request.IsHappyDay ? userDetails.Where(_ => happyDayUsers.Contains(_.User.UserName)).Select(_ => _.User.Id).ToList() : userDetails.Select(_ => _.User.Id).ToList();
            await DeleteDraftPayrolls(request.IsHappyDay);
            await _payrollRepository.InsertAsync(payroll);

            Trace.WriteLine
            (
                $"===================GET POSTS FOR PAYROLL CALCULATION "
                + $"{payroll.Code} from {request.FromDateTime} to {request.ToDateTime} "
                + $"for {userIds.Count} user(s) at {request.ToDateTime}"
            );

            List<PostWithNavigationProperties> postNavs;
            var orgUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() { IsGDLNode = true });
            var affiliateTeams = orgUnits.Where(_ => _.DisplayName.Contains("GDL - AT", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var seedingTeams = orgUnits.Where(_ => _.DisplayName.Contains("GDL - ST", StringComparison.CurrentCultureIgnoreCase)).ToList();

            var happyDayGroups = await _groupDomainService.GetManyAsync(new GetGroupApiRequest { GroupOwnershipType = GroupOwnershipType.HappyDay });
            var happyDayGroupIds = happyDayGroups.Select(_ => _.Id).ToList();

            var affConversions = await _affiliateConversionRepository.GetListAsync
                (_ => _.ConversionTime >= request.FromDateTime.ConvertToUnixTimestamp() && _.ConversionTime < request.ToDateTime.ConvertToUnixTimestamp());

            var userAffiliates = await _userAffiliateRepository.GetListAsync(createdAtMin: DateTime.UtcNow.AddMonths(-6));

            var staffEvaluations = await _staffEvaluationRepository.GetListWithNavigationPropertiesAsync(year: request.ToDateTime.Year, month: request.ToDateTime.Month);

            if (request.IsHappyDay)
            {
                postNavs = (await _postRepository.GetListWithNavigationPropertiesExtendAsync
                    (
                        createdDateTimeMin: request.FromDateTime,
                        createdDateTimeMax: request.ToDateTime,
                        isNotAvailable: false,
                        //isValid: true,
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
                        isNotAvailable: false,
                        //isValid: true,
                        appUserIds: userIds
                    ))
                    .Where(_ => _.Post.CreatedDateTime != null)
                    .ToList();
            }

            var postUserIds = postNavs.Where(x => x.Post?.AppUserId != null).Select(_ => _.Post.AppUserId.Value).Distinct().ToList();
            userDetails = userDetails.Where(_ => _.User.Id.IsIn(postUserIds)).ToList();

            Trace.WriteLine
            (
                $"===================GET POSTS FOR PAYROLL CALCULATION - "
                + $"FOUND {postNavs.Count} posts at {request.ToDateTime} "
                + $"FOR {userDetails.Count} users"
            );

            var userBonusConfigs = await _userBonusConfigRepository.GetListAsync();


            var bag = new ConcurrentBag<UserCompensation>();


            foreach (var userDetail in userDetails)
            {
                var userCompensation = await DoGetUserCompensation
                (
                    postNavs.Select(_ => _.Post).ToList(),
                    userDetail,
                    payroll,
                    affiliateTeams,
                    seedingTeams,
                    affConversions,
                    userBonusConfigs,
                    userAffiliates
                );
                if (userCompensation == null) continue;

                var evaluationBonus = staffEvaluations.FirstOrDefault(_ => _.AppUser?.Id == userCompensation.UserId)?.StaffEvaluation;
                if (evaluationBonus is { BonusAmount: > 0 } or { FinesAmount: > 0 })
                {
                    userCompensation.Bonuses.Add
                    (
                        new UserCompensationBonus()
                        {
                            UserId = userCompensation.UserId,
                            BonusAmount = evaluationBonus.BonusAmount,
                            Description = evaluationBonus.BonusDescription,
                            BonusType = BonusType.Evaluation,
                            FinesAmount = evaluationBonus.FinesAmount,
                            FinesDescription = evaluationBonus.FinesDescription
                        }
                    );
                }

                var workDayRate = await _staffEvaluationDomain.GetWorkDayRate(userCompensation.UserId);
                if (workDayRate > 0)
                {
                    userCompensation.SalaryAmount *= workDayRate;
                }

                userCompensation.TotalAmount = userCompensation.SalaryAmount + userCompensation.Bonuses.Sum(x => x.BonusAmount) - userCompensation.Bonuses.Sum(x => x.FinesAmount);
                bag.Add(userCompensation);
                Trace.WriteLine($"===================DoGetUserPayroll - DONE - {userDetail.User.UserName} at {request.ToDateTime}");
            }

            response.UserCompensations = bag.ToList();

            foreach (var batch in response.UserCompensations.Partition(100))
            {
                await _userCompensationRepository.InsertManyAsync(batch);
            }

            Trace.WriteLine($"===================DONE CALCULATE COMPENSATION at {DateTime.UtcNow}");

            return response;
        }

        #region UserPayslip - User Bonus

        private async Task<UserCompensation> DoGetUserCompensation(
            List<Post> allPosts,
            UserDetail userDetail,
            Payroll payroll,
            List<OrganizationUnit> affiliateTeams,
            List<OrganizationUnit> seedingTeams,
            List<AffiliateConversion> affiliateConversions,
            List<UserBonusConfig> userBonusConfigs,
            List<UserAffiliate> userAffiliates)
        {
            if (!payroll.FromDateTime.HasValue || !payroll.ToDateTime.HasValue) return null;
            var salaryConfiguration = await _staffEvaluationDomain.GetSalaryConfiguration(userDetail.Team.Id, userDetail.User.Id);

            var userCompensation = new UserCompensation()
            {
                PayrollId = payroll.Id,
                UserId = userDetail.User.Id,
                Team = userDetail.Team.DisplayName,
                Month = payroll.ToDateTime.Value.Month,
                Year = payroll.ToDateTime.Value.Year,
                SalaryAmount = salaryConfiguration.Salary
            };
            var userPosts = allPosts.Where(_ => _.AppUserId == userDetail.User.Id).ToList();
            var affPosts = userPosts.Where(_ => _.PostContentType == PostContentType.Affiliate).ToList();
            var seedingPosts = userPosts.Where(_ => _.PostContentType == PostContentType.Seeding).ToList();

            var shortKeys = userAffiliates.Where(_ => _.AppUserId == userDetail.User.Id).Select(_ => UrlHelper.GetShortKey(_.AffiliateUrl)).ToList();
            var affConversionCount = affiliateConversions.Count(_ => _.ShortKey.IsIn(shortKeys));

            // user KPI config is required
            var userConfig = await _staffEvaluationDomain.GetKPIConfigs(userDetail.Team.Id, userDetail.User.Id);
            if (userConfig is null) return null;


            if (userDetail.Team.DisplayName.Contains("ST1"))
            {
                // var minSeedingPost = userConfig.Seeding.SeedingPostQuantity.GetValueOrDefault();
                // var minSeedingReaction = userConfig.Seeding.MinimumPostReactions.GetValueOrDefault();
                // var minAffiliatePost = userConfig.Affiliate.AffiliatePostQuantity.GetValueOrDefault();
                // var minAffiliateConversion = userConfig.Affiliate.MinConversionCount.GetValueOrDefault();
                //
                // userCompensation.Description += L[
                //     "UserCompensation.Desc.Community",
                //     seedingPosts.Count.ToCommaStyle(),
                //     minSeedingPost.ToCommaStyle(),
                //     seedingPosts.Sum(x => x.TotalCount).ToCommaStyle(),
                //     minSeedingReaction.ToCommaStyle(),
                //     affPosts.Count.ToCommaStyle(),
                //     minAffiliatePost.ToCommaStyle(),
                //     affConversionCount.ToCommaStyle(),
                //     minAffiliateConversion.ToCommaStyle()
                // ];
                //
                // var conversionBonus = userBonusConfigs.FirstOrDefault(x => x.BonusType == BonusType.SeedingDoAffiliateConversion);
                // if (conversionBonus is not null)
                // {
                //     var conversionBonusCount = (affConversionCount - minAffiliateConversion);
                //     if (conversionBonusCount > 0)
                //     {
                //         var conversionAmount = (affConversionCount - minAffiliateConversion) * conversionBonus.BonusAmount;
                //         userCompensation.Bonuses.Add
                //         (
                //             new UserCompensationBonus
                //             {
                //                 UserId = userDetail.User.Id,
                //                 BonusType = BonusType.SeedingDoAffiliateConversion,
                //                 BonusAmount = conversionAmount
                //             }
                //         );
                //     }
                // }
            }
            else if (userDetail.Team.DisplayName.Contains("ST3") || userDetail.Team.DisplayName.Contains("ST6"))
            {
                var minPost = userConfig.Seeding.SeedingPostQuantity.GetValueOrDefault();
                var minReaction = userConfig.Seeding.MinimumPostReactions.GetValueOrDefault();

                userCompensation.Description += L["UserCompensation.Desc.Seeding",
                    seedingPosts.Count.ToCommaStyle(),
                    minPost.ToCommaStyle(),
                    seedingPosts.Sum(x => x.TotalCount).ToCommaStyle(),
                    minReaction.ToCommaStyle()];

                var affBonusAmount = SpecialCase_AffiliatePostAmount;
                var affPostAmount = affPosts.Count * affBonusAmount;
                if (affPosts.Count >= SpecialCase_MaxAffiliatePostCount)
                {
                    affPostAmount = SpecialCase_MaxBonusAmount;
                }

                userCompensation.Bonuses.Add
                (
                    new UserCompensationBonus
                    {
                        UserId = userDetail.User.Id,
                        BonusType = BonusType.SeedingDoAffiliatePost,
                        BonusAmount = affPostAmount
                    }
                );


                var conversionBonus = userBonusConfigs.FirstOrDefault(x => x.BonusType == BonusType.SeedingDoAffiliateConversion);
                if (conversionBonus is not null)
                {
                    var conversionAmount = affConversionCount * conversionBonus.BonusAmount;
                    userCompensation.Bonuses.Add
                    (
                        new UserCompensationBonus
                        {
                            UserId = userDetail.User.Id,
                            BonusType = BonusType.SeedingDoAffiliateConversion,
                            BonusAmount = conversionAmount
                        }
                    );
                }
            }
            else if (userDetail.Team.Id.IsIn(seedingTeams.Select(_ => _.Id).ToList()))
            {
                var minPost = userConfig.Seeding.SeedingPostQuantity.GetValueOrDefault();
                var minReaction = userConfig.Seeding.MinimumPostReactions.GetValueOrDefault();

                userCompensation.Description += L["UserCompensation.Desc.Seeding",
                    seedingPosts.Count.ToCommaStyle(),
                    minPost.ToCommaStyle(),
                    seedingPosts.Sum(x => x.TotalCount).ToCommaStyle(),
                    minReaction.ToCommaStyle()];

                var affBonus = userBonusConfigs.FirstOrDefault(x => x.BonusType == BonusType.SeedingDoAffiliatePost);
                if (affBonus is not null)
                {
                    var affPostAmount = affPosts.Count * affBonus.BonusAmount;
                    userCompensation.Bonuses.Add
                    (
                        new UserCompensationBonus
                        {
                            UserId = userDetail.User.Id,
                            BonusType = BonusType.SeedingDoAffiliatePost,
                            BonusAmount = affPostAmount
                        }
                    );
                }

                var conversionBonus = userBonusConfigs.FirstOrDefault(x => x.BonusType == BonusType.SeedingDoAffiliateConversion);
                if (conversionBonus is not null)
                {
                    var conversionAmount = affConversionCount * conversionBonus.BonusAmount;
                    userCompensation.Bonuses.Add
                    (
                        new UserCompensationBonus
                        {
                            UserId = userDetail.User.Id,
                            BonusType = BonusType.SeedingDoAffiliateConversion,
                            BonusAmount = conversionAmount
                        }
                    );
                }
            }
            else if (userDetail.Team.Id.IsIn(affiliateTeams.Select(_ => _.Id).ToList()))
            {
                var affBonus = new UserCompensationBonus { UserId = userDetail.User.Id };

                var minPost = userConfig.Affiliate.AffiliatePostQuantity.GetValueOrDefault();
                var minConversionCount = userConfig.Affiliate.MinConversionCount.GetValueOrDefault();

                switch (userDetail.Info.UserPosition)
                {
                    case UserPosition.CommunityAffiliateStaff:
                    {
                        var bonusConfig = userBonusConfigs.FirstOrDefault(x => x.BonusType == BonusType.AffiliateStaff);
                        if (bonusConfig != null)
                        {
                            affBonus.BonusType = BonusType.AffiliateStaff;
                            affBonus.BonusAmount = (affConversionCount - minConversionCount) * bonusConfig.BonusAmount;
                        }

                        break;
                    }
                    case UserPosition.CommunityAffiliateGroupLeader:
                    case UserPosition.CommunityAffiliateLeader:
                    case UserPosition.CommunityLeader:
                    {
                        var bonusConfig = userBonusConfigs.FirstOrDefault(x => x.BonusType == BonusType.AffiliateGroupLeader);
                        if (bonusConfig != null)
                        {
                            affBonus.BonusType = BonusType.AffiliateGroupLeader;
                            affBonus.BonusAmount = (affConversionCount - minConversionCount) * bonusConfig.BonusAmount;
                        }

                        break;
                    }
                }

                affBonus.Description = L["UserCompensation.Desc.Affiliate",
                    affPosts.Count.ToCommaStyle(),
                    minPost.ToCommaStyle(),
                    affConversionCount.ToCommaStyle(),
                    minConversionCount.ToCommaStyle()];

                userCompensation.Bonuses.Add(affBonus);
                userCompensation.Description += affBonus.Description;

                var affDoSeedingBonusConfig = userBonusConfigs.FirstOrDefault(x => x.BonusType == BonusType.AffiliateDoSeeding);
                if (affDoSeedingBonusConfig != null)
                {
                    userCompensation.Bonuses.Add
                    (
                        new UserCompensationBonus
                        {
                            UserId = userDetail.User.Id,
                            BonusType = BonusType.AffiliateDoSeeding,
                            BonusAmount = seedingPosts.Count * affDoSeedingBonusConfig.BonusAmount
                        }
                    );
                }
            }


            userCompensation.Bonuses = userCompensation.Bonuses.Where(x => !x.BonusType.IsIn(BonusType.Unknown, BonusType.FilterNoSelect) && x.BonusAmount > 0).ToList();
            return userCompensation;
        }

        #endregion

        #region Base Method

        public async Task ConfirmPayroll(Guid payrollId)
        {
            var payroll = await _payrollRepository.GetAsync(payrollId);
            var isHappyDay = payroll.Description?.ToLower().Contains(PayrollConsts.HappyDay.ToLower()) ?? false;

            if (payroll.FromDateTime != null)
            {
                var month = payroll.FromDateTime.Value.Month;
                var year = payroll.FromDateTime.Value.Year;
                payroll.Description = L["Payrolls.OfficialPayrollDescription", month, year] + (isHappyDay ? $" {PayrollConsts.HappyDay}" : "");
                payroll.Code = L["Payrolls.OfficialPayrollTitle", year, month] + (isHappyDay ? $" {PayrollConsts.HappyDay}" : "");
                payroll.Title = L["Payrolls.OfficialPayrollCode", year, month] + (isHappyDay ? $" {PayrollConsts.HappyDay}" : "");
            }

            await _payrollRepository.UpdateAsync(payroll);

            await DeleteDraftPayrolls(isHappyDay);
        }

        private async Task DeleteDraftPayrolls(bool isHappyDay)
        {
            List<Payroll> draftPayrolls;

            if (isHappyDay)
            {
                draftPayrolls = _payrollRepository.Where
                    (
                        _ => _.Code.ToLower().Contains("draft")
                             && _.Code.ToLower().Contains(PayrollConsts.HappyDay.ToLower())
                    )
                    .ToList();
            }
            else
            {
                draftPayrolls = _payrollRepository.Where
                    (
                        _ => _.Code.ToLower().Contains("draft")
                             && !_.Code.ToLower().Contains(PayrollConsts.HappyDay.ToLower())
                    )
                    .ToList();
            }

            var draftPayrollIds = draftPayrolls.Select(_ => _.Id).ToList();
            var draftUserCompensations = _userCompensationRepository.Where(_ => draftPayrollIds.Contains(_.PayrollId)).ToList();

            foreach (var item in draftUserCompensations)
            {
                await _userCompensationRepository.DeleteAsync(item);
            }

            foreach (var payroll in draftPayrolls)
            {
                await _payrollRepository.DeleteAsync(payroll);
            }
        }

        #endregion

        public async Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId)
        {
            var compensationAffiliates = new List<CompensationAffiliateDto>();
            var userAffiliates = await _userAffiliateRepository.GetListAsync(appUserId: userId, createdAtMin: DateTime.UtcNow.AddMonths(-6));
            var shortKeys = userAffiliates.Select(_ => UrlHelper.GetShortKey(_.AffiliateUrl)).ToList();

            var conversions = await _affiliateConversionRepository.GetListExtendAsync
                (dateTimeMin: (long)fromDate.ConvertToUnixTimestamp(), dateTimeMax: (long)toDate.ConvertToUnixTimestamp(), shortKeys: shortKeys);

            foreach (var userAffiliate in userAffiliates)
            {
                var shortKey = UrlHelper.GetShortKey(userAffiliate.AffiliateUrl);
                if (conversions.Count(_ => string.Equals(_.ShortKey, shortKey, StringComparison.CurrentCultureIgnoreCase)) > 0)
                {
                    compensationAffiliates.Add
                    (
                        new CompensationAffiliateDto()
                        {
                            AppUserId = userAffiliate.AppUserId,
                            Click = userAffiliate.AffConversionModel.ClickCount,
                            Shortlink = userAffiliate.AffiliateUrl,
                            Conversions = conversions.Count(_ => string.Equals(_.ShortKey, shortKey, StringComparison.CurrentCultureIgnoreCase))
                        }
                    );
                }
            }

            return compensationAffiliates.OrderByDescending(_ => _.Conversions).ToList();
        }
    }
}