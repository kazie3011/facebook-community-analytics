using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Controllers.TeamMembers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("TeamMember")]
    [Route("api/app/team-members")]
    public class TeamMemberController : ApiController, ITeamMemberAppService
    {
        private readonly ITeamMemberAppService _teamMemberAppService;

        public TeamMemberController(ITeamMemberAppService teamMemberAppService)
        {
            _teamMemberAppService = teamMemberAppService;
        }

        [HttpGet("managed-user-ids/{userId}")]
        public async Task<List<Guid>> GetManagedUserIds(Guid userId)
        {
            return await _teamMemberAppService.GetManagedUserIds(userId);
        }

        [HttpGet("users-by-role/{id}/{role}")]
        public async Task<List<AppUserDto>> GetUsersByRole(Guid id, string role)
        {
            return await _teamMemberAppService.GetUsersByRole(id, role);
        }

        [HttpGet("assign-roles-to-users/{id}/{role}")]
        public Task AssignRoleToUsers(AssignRoleRequest request)
        {
            return _teamMemberAppService.AssignRoleToUsers(request);
        }

        [HttpGet("list")]
        public Task<List<OrganizationUnitDto>> GetList()
        {
            return _teamMemberAppService.GetList();
        }

        [HttpGet("teams")]
        public Task<List<OrganizationUnitDto>> GetTeams([FromQuery]GetChildOrganizationUnitRequest request)
        {
            return _teamMemberAppService.GetTeams(request);
        }

        [HttpGet("members")]
        public Task<List<AppUserDto>> GetTeamMembers(Guid orgUnitId)
        {
            return _teamMemberAppService.GetTeamMembers(orgUnitId);
        }

        [HttpGet]
        [Route("get-members")]
        public Task<List<TeamMemberDto>> GetMembers(GetMembersApiRequest input)
        {
            return _teamMemberAppService.GetMembers(input);
        }

        [HttpPut]
        [Route("assign-team")]
        public Task AssignTeam(AssignTeamApiRequest apiRequest)
        {
            return _teamMemberAppService.AssignTeam(apiRequest);
        }

        [HttpPut]
        [Route("update-member-config")]
        public Task UpdateMemberConfig(UpdateMemberConfigApiRequest apiRequest)
        {
            return _teamMemberAppService.UpdateMemberConfig(apiRequest);
        }
        [HttpGet]
        [Route("get-app-users-lookup")]
        public Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input)
        {
            return _teamMemberAppService.GetAppUserLookupAsync(input);
        }
    }
}