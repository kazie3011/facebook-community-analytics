using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.TeamMembers
{
    public interface ITeamMemberAppService : IApplicationService
    {
        Task<List<Guid>> GetManagedUserIds(Guid userId);
        Task<List<AppUserDto>> GetUsersByRole(Guid userId, string roleConstant);
        Task AssignRoleToUsers(AssignRoleRequest request);
        Task<List<OrganizationUnitDto>> GetTeams(GetChildOrganizationUnitRequest request);
        Task<List<OrganizationUnitDto>> GetList();
        Task<List<AppUserDto>> GetTeamMembers(Guid orgUnitId);
        Task<List<TeamMemberDto>> GetMembers(GetMembersApiRequest input);
        Task AssignTeam(AssignTeamApiRequest apiRequest);
        Task UpdateMemberConfig(UpdateMemberConfigApiRequest apiRequest);
        Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input);
    }
}