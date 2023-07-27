using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    [RemoteService(false)]
    [Authorize(ApiPermissions.StaffEvaluations.Default)]
    public class StaffEvaluationAppService
        : BaseCrudApiAppService<StaffEvaluation, StaffEvaluationDto, Guid, GetStaffEvaluationsInput, CreateUpdateStaffEvaluationDto>, IStaffEvaluationAppService
    {
        private readonly IStaffEvaluationDomainService _staffEvaluationDomainService;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IStaffEvaluationRepository _staffEvaluationRepository;
        private readonly IStaffEvaluationCriteriaRepository _staffEvaluationCriteriaRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IPostDomainService _postDomainService;
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly GlobalConfiguration _globalConfiguration;

        public StaffEvaluationAppService(
            IRepository<StaffEvaluation, Guid> repository,
            IStaffEvaluationDomainService staffEvaluationDomainService,
            IOrganizationDomainService organizationDomainService,
            IStaffEvaluationRepository staffEvaluationRepository,
            IStaffEvaluationCriteriaRepository staffEvaluationCriteriaRepository,
            IdentityUserManager identityUserManager,
            IPostDomainService postDomainService,
            IDistributedEventBus distributedEventBus,
            GlobalConfiguration globalConfiguration) : base(repository)
        {
            _staffEvaluationDomainService = staffEvaluationDomainService;
            _organizationDomainService = organizationDomainService;
            _staffEvaluationRepository = staffEvaluationRepository;
            _staffEvaluationCriteriaRepository = staffEvaluationCriteriaRepository;
            _identityUserManager = identityUserManager;
            _postDomainService = postDomainService;
            _distributedEventBus = distributedEventBus;
            _globalConfiguration = globalConfiguration;
        }

        public async Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluationByUser(Guid userId, int month, int year)
        {
            return await _staffEvaluationDomainService.GetStaffEvaluationByUser(userId, month, year);
        }

        public async Task<List<StaffEvaluationCriteriaDto>> GetStaffEvaluationCriteria()
        {
            var staffEvaluationCriteria = await _staffEvaluationCriteriaRepository.GetListAsync();
            return ObjectMapper.Map<List<StaffEvaluationCriteria>, List<StaffEvaluationCriteriaDto>>(staffEvaluationCriteria);
        }

        public async Task<PagedResultDto<StaffEvaluationWithNavigationPropertiesDto>> GetListExtendAsync(GetStaffEvaluationsInput input)
        {
            if (IsDirectorRole())
            {
                
            }
            else if (IsSupervisorRole())
            {
                var organizationUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() {UserId = CurrentUser.Id});
                input.TeamIds = organizationUnits.IsNotNullOrEmpty() ? organizationUnits.Select(_ => _.Id).ToList() : null;
            }
            else if(IsLeaderRole())
            {
                var organizationUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() {UserId = CurrentUser.Id});
                input.TeamId = organizationUnits.IsNotNullOrEmpty() ? organizationUnits.FirstOrDefault()?.Id : Guid.Empty;
            }
            else
            {
                input.AppUserId = CurrentUser.Id;
            }

            var total = await _staffEvaluationRepository.GetCountAsync
            (
                input.FilterText,
                input.TeamId,
                input.TeamIds,
                input.AppUserId,
                input.Month,
                input.Year,
                input.StaffEvaluationStatus,
                input.IsTikTokEvaluation
            );

            var items = await _staffEvaluationRepository.GetListWithNavigationPropertiesAsync
            (
                input.FilterText,
                input.TeamId,
                input.TeamIds,
                input.AppUserId,
                month: input.Month,
                year: input.Year,
                staffEvaluationStatus: input.StaffEvaluationStatus,
                IsTikTokEvaluation: input.IsTikTokEvaluation,
                sorting: input.Sorting,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount
            );

            return new PagedResultDto<StaffEvaluationWithNavigationPropertiesDto>()
            {
                TotalCount = total,
                Items = ObjectMapper.Map<List<StaffEvaluationWithNavigationProperties>, List<StaffEvaluationWithNavigationPropertiesDto>>(items)
            };
        }

        public async Task<List<OrganizationUnitDto>> GetEvaluationTeams()
        {
            var currentUser = await _identityUserManager.GetByIdAsync(CurrentUser.GetId());
            var teams = await _staffEvaluationDomainService.GetEvaluationTeams(currentUser);

            return teams.OrderBy(x => x.DisplayName).ToList();
        }

        public Task GenerateStaffEvaluations()
        {
            return _staffEvaluationDomainService.GenerateStaffEvaluations(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
        }

        public async Task EvaluateStaffs(StaffEvaluationRequest apiRequest)
        {
            Hangfire.BackgroundJob.Enqueue(() => EvaluateStaffs(apiRequest.Year, apiRequest.Month, apiRequest.IsEvaluateNoModChannel, apiRequest.TeamType));
        }
        
        public async Task EvaluateStaffs(int year, int month, bool IsEvaluateNoModChannel, TeamType? teamType)
        {
            await _staffEvaluationDomainService.EvaluateStaffs(year, month, teamType, IsEvaluateNoModChannel);
            if (CurrentUser.Id.HasValue)
            {
                await _distributedEventBus.PublishAsync(new ReceivedMessageEto(CurrentUser.Id.Value, CurrentUser.UserName, L["Message.DoneKPIEvaluation"]));
            }
        }
     
        public async Task<List<StaffEvaluationExportRow>> GetExportRows(GetStaffEvaluationsInput input)
        {
            input.MaxResultCount = int.MaxValue;
            if (!IsManagerRole())
            {
                var organizationUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() {UserId = CurrentUser.Id});
                input.TeamId = organizationUnits.FirstOrDefault()?.Id;
            }

            var result = await GetListExtendAsync(input);

            return result.Items.Where(x => x.AppUser != null)
                .Select
                (
                    x => new StaffEvaluationExportRow
                    {
                        Team = x.OrganizationUnit?.DisplayName,
                        Username = x.AppUser.UserName,
                        FullName = $"{x.AppUser.Surname} {x.AppUser.Name}",
                        Month = $"{x.StaffEvaluation.Month}/{x.StaffEvaluation.Year}",
                        TotalPoint = x.StaffEvaluation.TotalPoint.ToString(CultureInfo.InvariantCulture),
                        QuantityKPI = x.StaffEvaluation.QuantityKPI.ToString(CultureInfo.InvariantCulture),
                        QualityKPI = x.StaffEvaluation.QualityKPI.ToString(CultureInfo.InvariantCulture),
                        ReviewPoint = x.StaffEvaluation.ReviewPoint.ToString(CultureInfo.InvariantCulture),
                        DirectorReview = x.StaffEvaluation.DirectorReview,
                        QuantityKPIDescription = x.StaffEvaluation.QuantityKPIDescription,
                        QualityKPIDescription = x.StaffEvaluation.QualityKPIDescription,
                        StaffEvaluationStatus = @L[$"Enum:StaffEvaluationStatus:{Convert.ToInt32(x.StaffEvaluation.StaffEvaluationStatus)}"],
                        SummaryNote = x.StaffEvaluation.SummaryNote,
                        SaleKPIAmount = x.StaffEvaluation.SaleKPIAmount.ToString("N0"),
                        BonusAmount = x.StaffEvaluation.BonusAmount.ToString("N0"),
                        BonusDescription = x.StaffEvaluation.BonusDescription,
                        AssignedTasks = x.StaffEvaluation.AssignedTasks,
                        FinesAmount  = x.StaffEvaluation.BonusAmount.ToString("N0"),
                        FinesDescription  = x.StaffEvaluation.BonusDescription,
                    }
                )
                .ToList();
        }

        public override async Task<StaffEvaluationDto> UpdateAsync(Guid id, CreateUpdateStaffEvaluationDto input)
        {
            if (input.SaleKPIAmount > 0)
            {
                input = await _staffEvaluationDomainService.UpdateSaleKPIAmount(input);
            }
            
            return await base.UpdateAsync(id, input);
            
            // var channelEvaluations = await _staffEvaluationRepository.GetListWithNavigationPropertiesAsync(appUserId: input.AppUserId, year: input.Year, month: input.Month, IsTikTokEvaluation: true);
            // var staffEvaluation = (await _staffEvaluationRepository.GetListWithNavigationPropertiesAsync(appUserId: input.AppUserId, year: input.Year, month: input.Month, IsTikTokEvaluation: false)).FirstOrDefault();
            // if (staffEvaluation is not null )
            // {
            //     staffEvaluation.StaffEvaluation.QuantityKPI = Math.Round(userChannelEvals.Average(_ => _.QuantityKPI), 1, MidpointRounding.AwayFromZero);
            //     staffEvaluation.StaffEvaluation.QualityKPI = Math.Round(userChannelEvals.Average(_ => _.QualityKPI), 1, MidpointRounding.AwayFromZero);
            //     staffEvaluation.StaffEvaluation.TotalPoint = existingEvaluation.QuantityKPI + existingEvaluation.QualityKPI + existingEvaluation.ReviewPoint;
            //     staffEvaluation.StaffEvaluation.QuantityKPIDescription = L["StaffEvaluation.Desc.Tiktok.QuantityKPI", userChannelEvals.Count];
            //     staffEvaluation.StaffEvaluation.QualityKPIDescription = L["StaffEvaluation.Desc.Tiktok.QualityKPI", userChannelEvals.Count];
            //     staffEvaluation.StaffEvaluation.StaffEvaluationStatus = StaffEvaluationStatus.Automated;
            // }
        }
        
        public Task<List<PostDetailExportRow>> GetEvaluationSeedingPostExport(GetPostEvaluationRequest request)
        {
            return _staffEvaluationDomainService.GetEvaluationSeedingPostExport(request);
        }
        
        public Task<byte[]> GetAffiliatesEvaluationExport(GetAffiliateEvaluationRequest request)
        {
            return _staffEvaluationDomainService.GetEvaluationAffiliatesExport(request);
        }

        public async Task<List<TikTokChannelEvaluationExport>> GetEvaluationTiktokChannelExport(ExportTiktokEvaluationRequest request)
        {
            var channelEvaluations = await _staffEvaluationRepository.GetListWithNavigationPropertiesAsync(year: request.Year, month: request.Month, appUserId: request.UserId, IsTikTokEvaluation: true);
            var exportData = channelEvaluations.Select
                (
                    _ => new TikTokChannelEvaluationExport
                    {
                        Team = request.TeamName,
                        Username = _.AppUser?.UserName,
                        FullName = $"{_.AppUser?.Surname} {_.AppUser?.Name}",
                        Month = $"{request.Month}/{request.Year}",
                        Channel = _.Group?.Name,
                        TotalPoint = (_.StaffEvaluation.QualityKPI + _.StaffEvaluation.QuantityKPI + _.StaffEvaluation.ReviewPoint).ToString(CultureInfo.InvariantCulture),
                        QuantityKPI = _.StaffEvaluation.QuantityKPI.ToString(CultureInfo.InvariantCulture),
                        QualityKPI = _.StaffEvaluation.QualityKPI.ToString(CultureInfo.InvariantCulture),
                        ReviewPoint = _.StaffEvaluation.ReviewPoint.ToString(CultureInfo.InvariantCulture),
                        DirectorReview = _.StaffEvaluation.DirectorReview,
                        QuantityKPIDescription = _.StaffEvaluation.QuantityKPIDescription,
                        QualityKPIDescription = _.StaffEvaluation.QualityKPIDescription,
                        StaffEvaluationStatus = @L[$"Enum:StaffEvaluationStatus:{Convert.ToInt32(_.StaffEvaluation.StaffEvaluationStatus)}"],
                        SummaryNote = _.StaffEvaluation.SummaryNote,
                        SaleKPIAmount = _.StaffEvaluation.SaleKPIAmount.ToString(CultureInfo.InvariantCulture),
                        BonusAmount = _.StaffEvaluation.BonusAmount.ToString(CultureInfo.InvariantCulture),
                        BonusDescription = _.StaffEvaluation.BonusDescription,
                        AssignedTasks = _.StaffEvaluation.AssignedTasks,
                    }
                ).ToList();;

            return exportData;
        }

        public Task<List<ContractEvaluationExport>> GetContractEvaluationExport(GetContractEvaluationRequest request)
        {
            return _staffEvaluationDomainService.GetEvaluationContractExport(request);
        }

        public async Task<List<int>> GetEvaluationYears()
        {
            var minYear = _staffEvaluationRepository.AsQueryable().Min(x => x.Year);
            var years = Enumerable.Range(minYear, DateTime.UtcNow.Year - minYear + 1).ToList();
            return await Task.FromResult(years);
        }

        public Task<bool> ContainSaleTeam(params string[] teamNames)
        {
            return Task.FromResult(teamNames.Any(teamName => _globalConfiguration.TeamTypeMapping.Sale.Contains(teamName)));
        }
    }
}