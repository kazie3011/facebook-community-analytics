using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IOrganizationDomainService : IDomainService
    {
        Task<List<OrganizationUnit>> GetTeams(GetChildOrganizationUnitRequest request);
        Task<OrganizationUnit> GetTeam(Guid orgUnitId);
        Task<OrganizationUnit> GetTeam(string teamName);
    }

    public class OrganizationDomainService : BaseDomainService, IOrganizationDomainService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;

        public OrganizationDomainService(
            IdentityUserManager userManager,
            IRepository<OrganizationUnit, Guid> organizationUnitRepository)
        {
            _userManager = userManager;
            _organizationUnitRepository = organizationUnitRepository;
        }

        // Get node Team (example: GDL, Yan...)
        public async Task<List<OrganizationUnit>> GetTeams(GetChildOrganizationUnitRequest request)
        {
            var organizationUnits = await _organizationUnitRepository.GetListAsync();
            if (request.UserId != null)
            {
                organizationUnits = await GetOrganizationUnits(request.UserId.GetValueOrDefault());
            }

            var parentIds = organizationUnits.Select(_ => _.ParentId).Distinct().ToList();
            var childrenTeams = organizationUnits.Where(_ => _.ParentId != null && !parentIds.Contains(_.Id)).OrderBy(_ => _.DisplayName).ToList();

            if (request.IsGDLNode is not null && request.IsGDLNode.Value)
            {
                childrenTeams = childrenTeams.Where(_ => _.DisplayName.ToLower().Contains("gdl")).ToList();
            }
            return childrenTeams;
        }

        public async Task<OrganizationUnit> GetTeam(Guid orgUnitId)
        {
            return await _organizationUnitRepository.GetAsync(orgUnitId);
        }

        public async Task<OrganizationUnit> GetTeam(string teamName)
        {
            var team = (await _organizationUnitRepository.GetListAsync())
                .FirstOrDefault(x => x.DisplayName.Equals(teamName,StringComparison.InvariantCultureIgnoreCase));

            return team;
        }

        private async Task<List<OrganizationUnit>> GetOrganizationUnits(Guid userId)
        {
            var identityUser = await _userManager.FindByIdAsync(userId.ToString());
            if (identityUser?.OrganizationUnits == null) return new List<OrganizationUnit>();

            var orgUnits = await _userManager.GetOrganizationUnitsAsync(identityUser);
            return orgUnits;
        }
    }
}