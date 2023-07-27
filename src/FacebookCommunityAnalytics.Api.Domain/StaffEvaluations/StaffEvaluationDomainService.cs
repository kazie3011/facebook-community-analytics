using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exports;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.UserSalaryConfigurations;
using IdentityServer4.Extensions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public interface IStaffEvaluationDomainService : IDomainService
    {
        Task<List<OrganizationUnitDto>> GetEvaluationTeams(IdentityUser currentUser);
        Task GenerateStaffEvaluations(int year, int month);
        Task EvaluateStaffs(int year, int month, TeamType? teamType, bool isEvaluateNoModChannel);

        Task<CreateUpdateStaffEvaluationDto> UpdateSaleKPIAmount(CreateUpdateStaffEvaluationDto input);

        // Task<List<StaffEvaluation>> EvaluateTikTokChannel(StaffEvaluationWithNavigationPropertiesDto input);
        Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluationByUser(Guid userId, int month, int year);
        Task<UserEvaluationConfiguration> GetKPIConfigs(Guid teamId, Guid userId);
        Task<UserSalaryConfiguration> GetSalaryConfiguration(Guid teamId, Guid userId);


        Task<List<StaffEvaluationWithNavigationPropertiesDto>> GetStaffEvaluationsByUser(
            Guid userId,
            int fromYear,
            int fromMonth,
            int toYear,
            int toMonth);

        Task DailyEvaluateStaffs();

        TeamType GetTeamType(string teamName, TeamTypeMapping globalConfigurationTeamTypeMapping);
        Task<List<PostDetailExportRow>> GetEvaluationSeedingPostExport(GetPostEvaluationRequest request);
        Task<byte[]> GetEvaluationAffiliatesExport(GetAffiliateEvaluationRequest request);
        Task<List<ContractEvaluationExport>> GetEvaluationContractExport(GetContractEvaluationRequest request);
        Task<decimal> GetWorkDayRate(Guid userId);
    }

    public partial class StaffEvaluationDomainService : BaseDomainService, IStaffEvaluationDomainService
    {
        private readonly IRepository<OrganizationUnit, Guid> _organizationUnitsRepository;
        private readonly IUserDomainService _userDomainService;
        private readonly IUserEvaluationConfigurationRepository _userEvaluationConfigurationRepository;
        private readonly IUserSalaryConfigurationRepository _userSalaryConfigurationRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IPostRepository _postRepository;
        private readonly IAffiliateConversionRepository _affiliateConversionRepository;
        private readonly IRepository<GroupStatsHistory, Guid> _groupStatsHistoryRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ITiktokRepository _tiktokRepository;
        private readonly IRepository<ContractTransaction, Guid> _contractTransactionRepository;
        private readonly IStaffEvaluationRepository _staffEvaluationRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IdentityUserManager _userManager;
        private readonly IExportDomainService _exportDomainService;

        public StaffEvaluationDomainService(
            IRepository<OrganizationUnit, Guid> organizationUnitsRepository,
            IUserDomainService userDomainService,
            IUserEvaluationConfigurationRepository userEvaluationConfigurationRepository,
            IContractRepository contractRepository,
            IPostRepository postRepository,
            IAffiliateConversionRepository affiliateConversionRepository,
            IRepository<GroupStatsHistory, Guid> groupStatsHistoryRepository,
            IGroupRepository groupRepository,
            ITiktokRepository tiktokRepository,
            IRepository<ContractTransaction, Guid> contractTransactionRepository,
            IStaffEvaluationRepository staffEvaluationRepository,
            IUserAffiliateRepository userAffiliateRepository,
            IOrganizationDomainService organizationDomainService,
            IdentityUserManager userManager,
            IExportDomainService exportDomainService,
            IUserSalaryConfigurationRepository userSalaryConfigurationRepository)
        {
            _organizationUnitsRepository = organizationUnitsRepository;
            _userDomainService = userDomainService;
            _userEvaluationConfigurationRepository = userEvaluationConfigurationRepository;
            _contractRepository = contractRepository;
            _postRepository = postRepository;
            _affiliateConversionRepository = affiliateConversionRepository;
            _groupStatsHistoryRepository = groupStatsHistoryRepository;
            _groupRepository = groupRepository;
            _tiktokRepository = tiktokRepository;
            _contractTransactionRepository = contractTransactionRepository;
            _staffEvaluationRepository = staffEvaluationRepository;
            _userAffiliateRepository = userAffiliateRepository;
            _organizationDomainService = organizationDomainService;
            _userManager = userManager;
            _exportDomainService = exportDomainService;
            _userSalaryConfigurationRepository = userSalaryConfigurationRepository;
        }

        public async Task<List<OrganizationUnitDto>> GetEvaluationTeams(IdentityUser currentUser)
        {
            var evaluationTeams = GlobalConfiguration.EvaluationTeams;
            var teams = new List<OrganizationUnit>();
            if (await _userManager.IsInRoleAsync(currentUser, RoleConsts.Admin)
                || await _userManager.IsInRoleAsync(currentUser, RoleConsts.Director))
            {
                teams = await _organizationUnitsRepository.GetListAsync();
            }
            else if (await _userManager.IsInRoleAsync(currentUser, RoleConsts.Manager)
                     || await _userManager.IsInRoleAsync(currentUser, RoleConsts.Supervisor)
                     || await _userManager.IsInRoleAsync(currentUser, RoleConsts.Leader))
            {
                teams = await _organizationDomainService.GetTeams
                (
                    new GetChildOrganizationUnitRequest()
                    {
                        IsGDLNode = !(await _userManager.IsInRoleAsync(currentUser, RoleConsts.Manager)),
                        UserId = currentUser.Id
                    }
                );
            }

            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(teams.Where(_ => evaluationTeams.Contains(_.DisplayName)).ToList());
        }

        public async Task GenerateStaffEvaluations(int year, int month)
        {
            var existingEvaluations = _staffEvaluationRepository.Where(x => x.Month == month && x.Year == year).ToList();
            var userDetails = await _userDomainService.GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetTeamUsers = true,
                    GetSystemUsers = false,
                    GetActiveUsers = true,
                }
            );

            var newEvals = new List<StaffEvaluation>();
            var updateEvals = new List<StaffEvaluation>();
            foreach (var u in userDetails)
            {
                if (!u.Team.DisplayName.IsIn(GlobalConfiguration.EvaluationTeams)) continue;
                var currentEval = existingEvaluations.FirstOrDefault(x => x.AppUserId == u.User.Id && x.CommunityId == null);
                if (currentEval == null)
                {
                    var newEval = new StaffEvaluation(u.Team.Id, u.User.Id, year, month)
                    {
                        StaffEvaluationStatus = StaffEvaluationStatus.Pending
                    };
                    if (u.Info.IsGDLStaff)
                    {
                        newEval.ReviewPoint = 20;
                    }
                    newEval.TotalPoint = newEval.QualityKPI + newEval.QuantityKPI + newEval.ReviewPoint;
                    newEvals.Add(newEval);
                }
                else
                {
                    if (currentEval.TeamId != u.Team.Id)
                    {
                        currentEval.TeamId = u.Team.Id;
                        currentEval.TotalPoint = currentEval.QualityKPI + currentEval.QuantityKPI + currentEval.ReviewPoint;
                        updateEvals.Add(currentEval);
                    }
                }
            }

            if (newEvals.IsNotNullOrEmpty())
            {
                await _staffEvaluationRepository.InsertManyAsync(newEvals);
            }

            if (updateEvals.IsNotNullOrEmpty())
            {
                await _staffEvaluationRepository.UpdateManyAsync(updateEvals);
            }
        }

        public async Task EvaluateStaffs(int year, int month, TeamType? userTeamType, bool isEvaluateNoModChannel)
        {
            await GenerateStaffEvaluations(year, month);
            await DoEvaluateStaffs(year, month, userTeamType);
        }

        public async Task DailyEvaluateStaffs()
        {
            var endDay = GlobalConfiguration.GlobalPayrollConfiguration.PayrollEndDay;
            var date = DateTime.UtcNow;
            if (date.Day >= endDay) date = date.AddMonths(1);
            var year = date.Year;
            var month = date.Month;
            var count = await _staffEvaluationRepository.GetCountAsync
            (
                month: month,
                year: year
            );
            if (count == 0) await GenerateStaffEvaluations(year, month);

            await DoEvaluateStaffs(year, month, null);
        }

        public TeamType GetTeamType(string teamName, TeamTypeMapping globalConfigurationTeamTypeMapping)
        {
            if (globalConfigurationTeamTypeMapping.Sale.Contains(teamName)) return TeamType.Sale;
            if (globalConfigurationTeamTypeMapping.Affiliate.Contains(teamName)) return TeamType.Affiliate;
            if (globalConfigurationTeamTypeMapping.Seeding.Contains(teamName)) return TeamType.Seeding;
            if (globalConfigurationTeamTypeMapping.Content.Contains(teamName)) return TeamType.Content;
            if (globalConfigurationTeamTypeMapping.Tiktok.Contains(teamName)) return TeamType.Tiktok;

            return TeamType.Unknown;
        }
        
        public async Task<List<PostDetailExportRow>> GetEvaluationSeedingPostExport(GetPostEvaluationRequest request)
        {
            var posts = await _postRepository.GetListWithNavigationPropertiesExtendAsync(appUserId: request.AppUserId, createdDateTimeMin: request.FromDateTime, createdDateTimeMax: request.ToDateTime, isNotAvailable: false);
            return ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostDetailExportRow>>(posts);
        }

        public async Task<byte[]> GetEvaluationAffiliatesExport(GetAffiliateEvaluationRequest request)
        {
            var posts = await _postRepository.GetListWithNavigationPropertiesExtendAsync(appUserId: request.UserId, createdDateTimeMin: request.FromDateTime, createdDateTimeMax: request.ToDateTime, isNotAvailable: false);
            var affiliates = await _userAffiliateRepository.GetListAsync(appUserId: request.UserId, createdAtMin: DateTime.UtcNow.AddMonths(-6));
            var shortKeys = affiliates.Select(_ => UrlHelper.GetShortKey(_.AffiliateUrl)).ToList();
            var affConversions = await _affiliateConversionRepository.GetListExtendAsync((long)request.FromDateTime.ConvertToUnixTimestamp()
                , (long)request.ToDateTime.ConvertToUnixTimestamp()
                , shortKeys);
            var compensationAffiliates = affiliates.Select(_ => new CompensationAffiliateExport()
            {   
                Shortlink = _.AffiliateUrl,
                Click = _.AffConversionModel.ClickCount,
                Conversions = affConversions.Count(c => c.ShortKey == UrlHelper.GetShortKey(_.AffiliateUrl)),
            }).Where(_ => _.Conversions > 0).OrderByDescending(_ => _.Conversions).ToList();
            return _exportDomainService.GenerateAffiliateEvaluationExcelBytes(posts, compensationAffiliates);
        }

        public async Task<List<ContractEvaluationExport>> GetEvaluationContractExport(GetContractEvaluationRequest request)
        {
            var contracts = await _contractRepository.GetListAsync();
            var contractTransactions = await _contractTransactionRepository.GetListAsync
            (
                x => x.SalePersonId == request.SalePersonId
                     && x.CreatedAt >= request.FromDateTime
                     && x.CreatedAt < request.ToDateTime
            );
            return contractTransactions.Select( _ => new ContractEvaluationExport()
            {
                ContractCode = contracts.FirstOrDefault(c => c.Id == _.ContractId)?.ContractCode,
                CreatedAt = _.CreatedAt.ToShortDateString(),
                Description = _.Description,
                PartialPaymentValue = _.PartialPaymentValue
            }).ToList();
        }

        public async Task<decimal> GetWorkDayRate(Guid userId)
        {
            var userInfo = await _userDomainService.GetUserInfo(userId);
            if (userInfo is not null)
            {
                var joinedDateTime = userInfo.JoinedDateTime;
                var (startPayrollDateTime, endPayrollDateTime) = GetPayrollDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
                if (joinedDateTime.HasValue && joinedDateTime > startPayrollDateTime)
                {
                    var daysOfWork = (endPayrollDateTime - joinedDateTime).Value.Days + 1; //endPayrollDateTime is last day payroll - 1 tick example: 25/03/2022 23:59:59 - 10/03/2022 = 15;
                    var payrollDays = (endPayrollDateTime - startPayrollDateTime).Days + 1;
                    return  (decimal)daysOfWork / payrollDays;
                }
            }
            return 0;
        }

        public CommunityKPIType GetCommunityKPIType(UserEvaluationConfiguration userEvalConfig)
        {
            if (userEvalConfig == null) return CommunityKPIType.Unknown;

            if (userEvalConfig.Seeding != null
                && userEvalConfig.Seeding.MinimumPostReactions.GetValueOrDefault() > 0
                && userEvalConfig.Seeding.SeedingPostQuantity.GetValueOrDefault() > 0
                && userEvalConfig.Affiliate != null
                && userEvalConfig.Affiliate.AffiliatePostQuantity.GetValueOrDefault() > 0
                && userEvalConfig.Affiliate.MinConversionCount.GetValueOrDefault() > 0) return CommunityKPIType.Community;

            if (userEvalConfig.Seeding != null
                && userEvalConfig.Seeding.MinimumPostReactions.GetValueOrDefault() > 0
                && userEvalConfig.Seeding.SeedingPostQuantity.GetValueOrDefault() > 0) return CommunityKPIType.Seeding;

            if (userEvalConfig.Affiliate != null
                && userEvalConfig.Affiliate.AffiliatePostQuantity.GetValueOrDefault() > 0
                && userEvalConfig.Affiliate.MinConversionCount.GetValueOrDefault() > 0) return CommunityKPIType.Affiliate;

            return CommunityKPIType.Unknown;
        }

        private async Task<UserEvaluationConfiguration> GetKPIDefaultConfig()
        {
            var KPIConfigs = await _userEvaluationConfigurationRepository.GetListAsync();
            var defaultConfig = KPIConfigs.FirstOrDefault
            (
                evalConfig => evalConfig.OrganizationId == null && evalConfig.AppUserId == null && evalConfig.TeamId == null && evalConfig.UserPosition == null
            );

            return defaultConfig;
        }

        public async Task<UserEvaluationConfiguration> GetKPIConfigs(Guid teamId, Guid userId)
        {
            var KPIConfigs = await _userEvaluationConfigurationRepository.GetListAsync(x => x.TeamId == teamId);
            var userInfo = await _userDomainService.GetUserInfo(userId);
            var defaultConfig = _userEvaluationConfigurationRepository.FirstOrDefault
            (
                evalConfig => evalConfig.OrganizationId == null && evalConfig.AppUserId == null && evalConfig.TeamId == null && evalConfig.UserPosition == null
            );

            UserEvaluationConfiguration config = null;

            config = (KPIConfigs.FirstOrDefault(x => x.AppUserId == userId)
                      ?? (userInfo.UserPosition is not UserPosition.Unknown and not UserPosition.FilterNoSelect
                          ? KPIConfigs.FirstOrDefault(x => x.UserPosition == userInfo.UserPosition)
                          : KPIConfigs.FirstOrDefault(x => x.UserPosition is UserPosition.Unknown or UserPosition.FilterNoSelect)))
                     ?? defaultConfig;
            if (config == null) return null;
            
            // TODOO LONG: Need calculate exact KPI before decreasing
            var currentKPIRate = GlobalConfiguration.KPIConfig.CurrentRate;
            config.Sale.ContractAmountKPI = Math.Round(config.Sale.ContractAmountKPI.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero);
            config.Sale.PaidContractAmountKPI = Math.Round(config.Sale.PaidContractAmountKPI.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero);
            config.Tiktok.TiktokAverageVideoView = Convert.ToInt32(Math.Round(config.Tiktok.TiktokAverageVideoView.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));
            config.Tiktok.TiktokVideoPerMonth = Convert.ToInt32(Math.Round(config.Tiktok.TiktokVideoPerMonth.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));
            config.Content.ContentPostQuantity = Convert.ToInt32(Math.Round(config.Content.ContentPostQuantity.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));
            config.Content.MinimumPostReactions = Convert.ToInt32(Math.Round(config.Content.MinimumPostReactions.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));
            config.Affiliate.AffiliatePostQuantity = Convert.ToInt32(Math.Round(config.Affiliate.AffiliatePostQuantity.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));
            config.Affiliate.MinConversionCount = Convert.ToInt32(Math.Round(config.Affiliate.MinConversionCount.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));
            config.Seeding.SeedingPostQuantity = Convert.ToInt32(Math.Round(config.Seeding.SeedingPostQuantity.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));
            config.Seeding.MinimumPostReactions = Convert.ToInt32(Math.Round(config.Seeding.MinimumPostReactions.GetValueOrDefault() * currentKPIRate, MidpointRounding.AwayFromZero));

            return config;
        }
        
        public async Task<UserSalaryConfiguration> GetSalaryConfiguration(Guid teamId, Guid userId)
        {
            var teamSalaryConfigurations = await _userSalaryConfigurationRepository.GetListAsync(x => x.TeamId == teamId);
            if (teamSalaryConfigurations.IsNullOrEmpty()) return new UserSalaryConfiguration();
            
            var userInfo = await _userDomainService.GetUserInfo(userId);
            var salaryConfig = teamSalaryConfigurations.FirstOrDefault(x => x.UserId == userId) ?? teamSalaryConfigurations.FirstOrDefault(x => x.UserPosition == userInfo.UserPosition);
            
            return salaryConfig ?? teamSalaryConfigurations.First();
        }

        private async Task DoEvaluateStaffs(int year, int month, TeamType? userTeamType)
        {
            var (fromDateTime, toDateTime) = GetPayrollDateTime(year, month);

            var postNavs = (await _postRepository.GetListWithNavigationPropertiesExtendAsync
                (
                    createdDateTimeMin: fromDateTime,
                    createdDateTimeMax: toDateTime
                ))
                .ToList();

            if (userTeamType != null)
            {
                Debug.WriteLine($"=====================START EVALUATE {userTeamType}=====================");
                switch (userTeamType)
                {
                    case TeamType.Sale:
                        await DoEval_Sale(year, month);
                        break;
                    case TeamType.Content:
                        await DoEval_Content(year, month, postNavs);
                        break;
                    case TeamType.Affiliate:
                    case TeamType.Seeding:
                        await DoEval_Community(year, month, postNavs);
                        break;
                    case TeamType.Tiktok:
                        await DoEval_TikTok(year, month);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(userTeamType), userTeamType, null);
                }
            }
            else
            {
                Debug.WriteLine($"=====================START EVALUATE ALL TEAMS=====================");
                await DoEval_Sale(year, month);
                await DoEval_TikTok(year, month);
                await DoEval_Content(year, month, postNavs);
                await DoEval_Community(year, month, postNavs);
            }

            Debug.WriteLine($"=====================EVALUATION DONE =====================");
        }

        public async Task<CreateUpdateStaffEvaluationDto> UpdateSaleKPIAmount(CreateUpdateStaffEvaluationDto input)
        {
            var saleTeam = await _organizationDomainService.GetTeam(TeamMemberConsts.Sale);
            var userKPI = await GetKPIConfigs(saleTeam.Id, input.AppUserId);

            var (fromDateTime, toDateTime) = GetPayrollDateTime(input.Year, input.Month);
            var contractTransactions = await _contractTransactionRepository.GetListAsync
            (
                x => x.SalePersonId == input.AppUserId
                     && x.CreatedAt >= fromDateTime
                     && x.CreatedAt < toDateTime
            );

            var totalTransactionAmount = input.SaleKPIAmount;
            var requiredContractAmount = userKPI.Sale.PaidContractAmountKPI.GetValueOrDefault() * 1000000;
            var quantityPoint = Math.Min(50, Math.Round(totalTransactionAmount / requiredContractAmount * 50, 0));

            string quantityKPIDescription = L["StaffEvaluation.Desc.Sale.QuantityKPI",
                totalTransactionAmount.ToCommaStyle(),
                requiredContractAmount.ToCommaStyle(),
                contractTransactions.Count
            ];

            input.QuantityKPI = quantityPoint;
            input.QuantityKPIDescription = quantityKPIDescription;
            input.StaffEvaluationStatus = StaffEvaluationStatus.Automated;
            input.TotalPoint = input.QuantityKPI + input.QualityKPI + input.ReviewPoint;

            return input;
        }

        public async Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluationByUser(Guid userId, int month, int year)
        {
            var item = (await _staffEvaluationRepository.GetListWithNavigationPropertiesAsync
            (
                appUserId: userId,
                month: month,
                year: year
            )).FirstOrDefault();

            return ObjectMapper.Map<StaffEvaluationWithNavigationProperties, StaffEvaluationWithNavigationPropertiesDto>(item);
        }

        public async Task<List<StaffEvaluationWithNavigationPropertiesDto>> GetStaffEvaluationsByUser(
            Guid userId,
            int fromYear,
            int fromMonth,
            int toYear,
            int toMonth)
        {
            var staffEvaluations = new List<StaffEvaluationWithNavigationProperties>();
            var fromDate = DateTime.SpecifyKind(new DateTime(fromYear, fromMonth, 1), DateTimeKind.Utc);
            var toDate = DateTime.SpecifyKind(new DateTime(toYear, toMonth, 1), DateTimeKind.Utc);
            if (fromDate > toDate) return new List<StaffEvaluationWithNavigationPropertiesDto>();
            var dateTimes = GetDate(fromDate, toDate);
            foreach (var (month, year) in dateTimes)
            {
                var staffEval = await _staffEvaluationRepository.GetWithNavigationPropertiesByUserAsync(userId, year, month);
                if (staffEval == null) continue;

                staffEvaluations.Add(staffEval);
            }

            return ObjectMapper.Map<List<StaffEvaluationWithNavigationProperties>, List<StaffEvaluationWithNavigationPropertiesDto>>(staffEvaluations);
        }

        private List<KeyValuePair<int, int>> GetDate(DateTime fromDate, DateTime toDate)
        {
            var dateTimes = new List<KeyValuePair<int, int>>();

            var date = fromDate;
            while (true)
            {
                if (date <= toDate)
                {
                    dateTimes.Add(new KeyValuePair<int, int>(date.Month, date.Year));
                    date = date.AddMonths(1);
                }
                else
                {
                    break;
                }
            }

            return dateTimes;
        }

    }
}