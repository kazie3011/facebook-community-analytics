using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    [RemoteService(isEnabled: false)]
    [Authorize(ApiPermissions.StaffEvaluations.Default)]
    public class StaffEvaluationCriteriaAppService 
        : BaseCrudApiAppService<StaffEvaluationCriteria, StaffEvaluationCriteriaDto, Guid, GetStaffEvaluationCriteriaInput, CreateUpdateStaffEvaluationCriteriaDto>
            , IStaffEvaluationCriteriaAppService
    {
        private readonly IStaffEvaluationDomainService _staffEvaluationDomainService;
        private readonly IStaffEvaluationCriteriaRepository _staffEvaluationCriteriaRepository;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IdentityUserManager _identityUserManager;
        public StaffEvaluationCriteriaAppService(IRepository<StaffEvaluationCriteria, Guid> repository,
            IStaffEvaluationDomainService staffEvaluationDomainService,
            IStaffEvaluationCriteriaRepository staffEvaluationCriteriaRepository,
            IOrganizationDomainService organizationDomainService,
            IdentityUserManager identityUserManager) : base(repository)
        {
            _staffEvaluationDomainService = staffEvaluationDomainService;
            _staffEvaluationCriteriaRepository = staffEvaluationCriteriaRepository;
            _organizationDomainService = organizationDomainService;
            _identityUserManager = identityUserManager;
        }

        public async Task<List<OrganizationUnitDto>> GetTeams()
        {
            var currentUser = await _identityUserManager.GetByIdAsync(CurrentUser.GetId());
            return await _staffEvaluationDomainService.GetEvaluationTeams(currentUser);
        }

        public override async Task<StaffEvaluationCriteriaDto> CreateAsync(CreateUpdateStaffEvaluationCriteriaDto input)
        {
            if (input.TeamId == Guid.Empty) input.TeamId = null;
                return await base.CreateAsync(input);
        }

        public override async Task<StaffEvaluationCriteriaDto> UpdateAsync(Guid id, CreateUpdateStaffEvaluationCriteriaDto input)
        {
            if (input.TeamId == Guid.Empty) input.TeamId = null;
            return await base.UpdateAsync(id, input);
        }

        public async Task<PagedResultDto<StaffEvaluationCriteriaDto>> GetListExtendAsync(GetStaffEvaluationCriteriaInput input)
        {
            if (!IsManagerRole())
            {
                var organizationUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest()
                {
                    UserId = CurrentUser.Id
                });
                input.TeamId = organizationUnits.FirstOrDefault()?.Id;
            }


            var total = await _staffEvaluationCriteriaRepository.GetCountAsync
            (
                input.FilterText,
                input.TeamId,
                input.MaxPointMin,
                input.MaxPointMax,
                input.EvaluationType
            );

            var items = await _staffEvaluationCriteriaRepository.GetListAsync
            (
                input.FilterText,
                input.TeamId,
                input.MaxPointMin,
                input.MaxPointMax,
                input.EvaluationType,
                input.MaxResultCount,
                input.SkipCount
            );
            return new PagedResultDto<StaffEvaluationCriteriaDto>()
            {
                TotalCount = total,
                Items = ObjectMapper.Map<List<StaffEvaluationCriteria>, List<StaffEvaluationCriteriaDto>>(items)
            };
        }
    }
}