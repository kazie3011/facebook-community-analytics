using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.TeamMembers
{
    [Authorize]
    [RemoteService(IsEnabled = false)]
    public class TeamMemberAppService : ApiAppService, ITeamMemberAppService
    {
        private readonly IOrganizationUnitRepository _organizationUnitRepository;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IUserDomainService _userDomainService;
        private readonly ITeamMemberDomainService _teamMemberDomainService;

        public TeamMemberAppService(IOrganizationUnitRepository organizationUnitRepository
            , IOrganizationDomainService organizationDomainService
            , IUserDomainService userDomainService,
            ITeamMemberDomainService teamMemberDomainService)
        {
            _organizationUnitRepository = organizationUnitRepository;
            _organizationDomainService = organizationDomainService;
            _userDomainService = userDomainService;
            _teamMemberDomainService = teamMemberDomainService;
        }

        public async Task<List<Guid>> GetManagedUserIds(Guid userId)
        {
            return await _userDomainService.GetManagedUserIds(userId);
        }

        public async Task<List<AppUserDto>> GetUsersByRole(Guid userId, string roleConstant)
        {
            var appUsers = await _userDomainService.GetUsersByRole(userId, roleConstant);
            return ObjectMapper.Map<List<AppUser>,List<AppUserDto>>(appUsers);
        }

        public async Task AssignRoleToUsers(AssignRoleRequest request)
        {
            await _userDomainService.AssignRoleToUsers(request);
        }

        public async Task<List<OrganizationUnitDto>> GetList()
        {
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(await _organizationUnitRepository.GetListAsync());
        }

        public async Task<List<OrganizationUnitDto>> GetTeams(GetChildOrganizationUnitRequest request)
        {
            var orgUnits = await _organizationDomainService.GetTeams(request);
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(orgUnits);
        }
        
        public async Task<List<AppUserDto>> GetTeamMembers(Guid orgUnitId)
        {
            var users =  await _userDomainService.GetTeamMembers(orgUnitId);
            return ObjectMapper.Map<List<AppUser>, List<AppUserDto>>(users);
        }

        public async Task<List<TeamMemberDto>> GetMembers(GetMembersApiRequest input)
        {
            return ObjectMapper.Map<List<UserDetail>, List<TeamMemberDto>>(await _userDomainService.GetMembers(input));
        }

        public Task AssignTeam(AssignTeamApiRequest apiRequest)
        {
            return _userDomainService.AssignTeam(apiRequest);
        }

        public Task UpdateMemberConfig(UpdateMemberConfigApiRequest apiRequest)
        {
            return _userDomainService.UpdateMemberConfig(apiRequest);
        }

        public Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input)
        {
            return _teamMemberDomainService.GetAppUserLookupAsync(input);
        }
    }
}