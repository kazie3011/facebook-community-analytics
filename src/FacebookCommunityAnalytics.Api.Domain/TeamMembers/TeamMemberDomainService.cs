using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.TeamMembers
{
    public interface ITeamMemberDomainService : IDomainService
    {
        Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input);
    }
    public class TeamMemberDomainService : BaseDomainService, ITeamMemberDomainService
    {
        private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IUserInfoRepository _userInfoRepository;

        public TeamMemberDomainService(IRepository<OrganizationUnit, Guid> organizationUnitRepository,
            IRepository<AppUser, Guid> userRepository,
            IIdentityUserRepository identityUserRepository,
            IUserInfoRepository userInfoRepository)
        {
            _organizationUnitRepository = organizationUnitRepository;
            _userRepository = userRepository;
            _identityUserRepository = identityUserRepository;
            _userInfoRepository = userInfoRepository;
        }

        public async Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input)
        {
            var usersLookUp = new List<LookupDto<Guid?>>();
            var organizationUnit = _organizationUnitRepository.AsQueryable()
                .WhereIf(input.TeamName.IsNotNullOrEmpty(), o => o.DisplayName.ToLower().Contains(input.TeamName.ToLower()))
                .WhereIf(input.TeamId.HasValue, x=> x.Id == input.TeamId)
                .WhereIf(input.TeamIds.IsNotNullOrEmpty(), x=> input.TeamIds.Contains(x.Id))
                .FirstOrDefault();
            if (organizationUnit != null)
            {
                input.TeamId = organizationUnit.Id;
            }

            if (input.FilterText.IsNotNullOrEmpty())
            {
                var users = _userRepository.AsQueryable()
                    .Where
                    (
                        _ => _.UserName.ToLower().Trim().Contains(input.FilterText.ToLower().Trim())
                             || (_.Name.IsNotNullOrEmpty() && _.Name.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.Surname.IsNotNullOrEmpty() && _.Surname.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.Email.IsNotNullOrEmpty() && _.Email.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                    )
                    .ToList();

                usersLookUp = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(users);
            }

            if (input.TeamId != null && input.TeamId != Guid.Empty)
            {
                var users = await _identityUserRepository.GetUsersInOrganizationUnitAsync(input.TeamId.Value);
                usersLookUp = ObjectMapper.Map<List<IdentityUser>, List<LookupDto<Guid?>>>(users);
            }
            else
            {
                var users = await _userRepository.GetListAsync();
                usersLookUp = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(users);
            }

            if (input.UserPosition.HasValue)
            {
                var userInfos = await _userInfoRepository.GetListWithNavigationPropertiesExtendAsync(userPosition: input.UserPosition);
                usersLookUp = usersLookUp.Where(_ => userInfos.Select(u => u.UserInfo.AppUserId).Contains(_.Id)).ToList();
            }

            return usersLookUp;
        }
    }
}