using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
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