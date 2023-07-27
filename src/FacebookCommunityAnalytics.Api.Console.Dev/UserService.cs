using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class UserService : ITransientDependency
    {
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IRepository<UserInfo, Guid> _userInfoRepository;

        private readonly ITeamMemberDomainService _teamMemberDomainService;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IUserDomainService _userDomainService;

        public UserService(
            IRepository<AppUser, Guid> userRepository,
            IRepository<UserInfo, Guid> userInfoRepository,
            ITeamMemberDomainService teamMemberDomainService,
            IOrganizationDomainService organizationDomainService,
            IUserDomainService userDomainService)
        {
            _userRepository = userRepository;
            _userInfoRepository = userInfoRepository;
            _teamMemberDomainService = teamMemberDomainService;
            _organizationDomainService = organizationDomainService;
            _userDomainService = userDomainService;
        }

        public async Task CleanUp()
        {
            var x = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest {IsGDLNode = true});
            var uiToUpdate = new List<UserInfo>();
            foreach (var team in x)
            {
                var appUsers = await _userDomainService.GetTeamMembers(team.Id);
                foreach (var appUser in appUsers)
                {
                    var ui = await _userInfoRepository.FindAsync(_ => _.AppUserId == appUser.Id);
                    if (ui != null)
                    {
                        ui.IsGDLStaff = true;
                        uiToUpdate.Add(ui);
                    }
                }
            }

            await _userInfoRepository.UpdateManyAsync(uiToUpdate);
        }

        public async Task<List<string>> GetUsersWithFullLinkFuid()
        {
            var userinfos = await _userInfoRepository.GetListAsync();

            var list = new List<UserInfo>();
            foreach (var userinfo in userinfos)
            {
                if (userinfo.Accounts.IsNotNullOrEmpty() && userinfo.Accounts.Any(acc=>acc.Fid.Contains("facebook.com") || acc.Fid.Contains("/user")))
                {
                    list.Add(userinfo);
                }
            }

            return list.Select(_ => _.Code).ToList();
        }
    }
}