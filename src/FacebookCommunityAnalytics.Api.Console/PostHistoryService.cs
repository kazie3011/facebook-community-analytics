using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using LdapForNet;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console
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
    }

    public class PostHistoryService : ITransientDependency
    {
        private readonly IRepository<PostHistory, Guid> _postHistoryRepository;
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Group, Guid> _groupRepository;

        private readonly IPostDomainService _postDomainService;

        public PostHistoryService(
            IRepository<PostHistory, Guid> postHistoryRepository,
            IRepository<Post, Guid> postRepository,
            IRepository<Group, Guid> groupRepository,
            IPostDomainService postDomainService)
        {
            _postHistoryRepository = postHistoryRepository;
            _postRepository = postRepository;
            _groupRepository = groupRepository;
            _postDomainService = postDomainService;
        }

        public async Task CheckPostHistoriesInvalid()
        {
            //var postIds = await _postRepository.AsQueryable().Where(x => !x.IsDeleted).Select(x => x.Id).ToDynamicListAsync<Guid>();

            //var postHistoriesInvalid = await _postHistoryRepository.AsQueryable().Where(x => !postIds.Contains(x.PostId)).ToDynamicListAsync<PostHistory>();

            var postHistories = await (await _postHistoryRepository.GetListAsync()).Where
                (
                    x => x.CreatedDateTime.Day < 18
                )
                .ToDynamicListAsync<PostHistory>();

            // var listRemove = new List<PostHistory>();
            // foreach (var g in postHistories.GroupBy(x=>x.PostId))
            // {
            //     if (g.ToList().Count == 1)
            //     {
            //         continue;
            //     }
            //
            //     listRemove.Add(g.FirstOrDefault());
            // }
            //
            await _postHistoryRepository.DeleteManyAsync(postHistories);
        }

        public async Task GetPostMissingGroups()
        {
            var groups = await _groupRepository.Where(x => !x.IsDeleted).ToDynamicListAsync<Group>();
            var groupIds = groups.Select(x => x.Id).ToList();

            var posts = await _postRepository.Where(x => !x.IsDeleted && !groupIds.Contains(x.GroupId.Value)).ToDynamicListAsync<Post>();
            foreach (var post in posts)
            {
                var newUrl = FacebookHelper.ExtractLinkGroupOrPage(post.Url);
                var groupFid = FacebookHelper.GetGroupFid(newUrl).ToLowerInvariant();
                var group = groups.FirstOrDefault
                (
                    x => x.Fid.ToLowerInvariant().Equals(groupFid)
                         || x.Name.ToLowerInvariant().Equals(groupFid)
                         || x.Url.Contains(groupFid)
                );
                if (group != null)
                {
                    post.GroupId = group.Id;
                    post.Url = newUrl;
                    await _postRepository.UpdateAsync(post);
                }
            }
        }

        public async Task CreatePostHistories()
        {
            await _postDomainService.InitPostHistories();
        }
    }
}