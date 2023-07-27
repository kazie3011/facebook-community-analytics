using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    
    public class CrawlService : LoggerService
    {
        public           ICrawlDomainService                  CrawlDomainService { get; set; }
        private readonly IRepository<GroupStatsHistory, Guid> _groupStatsHistoriesRepository;
        private readonly IGroupRepository                     _groupRepository;
        private readonly IPostRepository                      _postRepository;


        public CrawlService(IRepository<GroupStatsHistory, Guid> groupStatsHistoriesRepository , IGroupRepository groupRepository, IPostRepository postRepository)
        {
            _groupStatsHistoriesRepository = groupStatsHistoriesRepository;
            _groupRepository               = groupRepository;
            _postRepository           = postRepository;
        }

        public async Task UpdateGroupIdForGroupStatHistory()
        {
           var groups = await _groupRepository.GetListAsync();
           var groupStatsHistories = await _groupStatsHistoriesRepository.GetListAsync();
           var oldChannel = groups.FirstOrDefault(_ => _.Fid == "bikellykimngan99");
           groupStatsHistories = groupStatsHistories.Select(
               _ =>
               {
                   _.GroupId = groups.FirstOrDefault(g => g.Fid == _.GroupFid)?.Id;
                   if (_.GroupFid == "banhbeokysu")
                   {
                       _.GroupId = oldChannel?.Id;
                   }
                   return _;
               }).ToList();
           foreach (var partition in groupStatsHistories.Partition(1000))
           {
               await _groupStatsHistoriesRepository.UpdateManyAsync(partition);
           }
        }
        
        public async Task InitDataForGroupStatHistory(DateTime dateTime, int preDays = 5, int nextDays = 5)
        {
            var groupStatsHistories = _groupStatsHistoriesRepository
                .Where(x => x.GroupSourceType != GroupSourceType.Tiktok)
                .Where(x => x.CreatedAt >= dateTime.AddDays(-preDays))
                .Where(x => x.CreatedAt <= dateTime.AddDays(nextDays))
                .ToList();
            
            var groupStatsGroupBy = groupStatsHistories.GroupBy(_ => _.GroupFid);
            foreach (var group in groupStatsGroupBy)
            {
                var groupStatsHistory = groupStatsHistories.FirstOrDefault(_ => _.CreatedAt == dateTime && _.GroupFid == group.Key);
                if (groupStatsHistory is null)
                {
                    await _groupStatsHistoriesRepository.InsertAsync
                    (
                        new GroupStatsHistory()
                        {
                            GroupFid = group.Key,
                            GroupSourceType = @group.FirstOrDefault()!.GroupSourceType,
                            CreatedAt = dateTime,
                            TotalInteractions = (int) group.Average(_ => _.TotalInteractions),
                            InteractionRate = group.FirstOrDefault()?.InteractionRate,
                            AvgPosts = group.Average(_ => _.AvgPosts),
                            GroupMembers = (int) group.Average(_ => _.GroupMembers),
                            Reactions = (int) group.Average(_ => _.Reactions),
                            GrowthPercent = group.Average(_ => _.GrowthPercent),
                            GrowthNumber = (int) group.Average(_ => _.GrowthNumber)
                        }
                    );
                    continue;
                }

                groupStatsHistory.TotalInteractions = (int)group.Average(_ => _.TotalInteractions);
                groupStatsHistory.InteractionRate   = group.FirstOrDefault()?.InteractionRate;
                groupStatsHistory.AvgPosts          = group.Average(_ => _.AvgPosts);
                groupStatsHistory.GroupMembers      = (int) group.Average(_ => _.GroupMembers);
                groupStatsHistory.Reactions         = (int) group.Average(_ => _.Reactions);
                groupStatsHistory.GrowthPercent     = group.Average(_ => _.GrowthPercent);
                groupStatsHistory.GrowthNumber      = (int)group.Average(_ => _.GrowthNumber);
                await _groupStatsHistoriesRepository.UpdateAsync(groupStatsHistory);
            }
        }
        
        public async Task InitCampaignPosts()
        {
            await CrawlDomainService.InitCampaignPosts();
        }

        public async Task CheckCrawlApi()
        {
            var posts = await _postRepository.GetListExtendAsync(fid: "5489077504458813");
            var post  = posts.FirstOrDefault();
            if (post is null) return;
            await CrawlDomainService.SaveCrawlResult(new()
            {
                Items = new List<CrawledPostDto>()
                {
                    new CrawledPostDto
                    {
                        Url            = post.Url,
                        Urls           = post.Shortlinks,
                        Content        = post.Content,
                        HashTags       = post.Hashtag?.Split('#').ToList(),
                        LikeCount      = post.LikeCount,
                        CommentCount   = post.CommentCount,
                        ShareCount     = post.ShareCount,
                        CreatedBy      = post.CreatedBy,
                        CreateFuid     = post.CreatedFuid,
                        IsNotAvailable = post.IsNotAvailable,
                        CreatedAt      = null
                    }
                }
            });
        }
    }
}