using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Flurl;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class TikTokService : LoggerService, ITransientDependency
    {
        private readonly IRepository<TiktokStat, Guid> _tiktokStatRepository;
        private readonly IRepository<GroupStatsHistory, Guid> _groupStatsHistoriesRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ITiktokRepository _tiktokRepository;

        public TikTokService(
            IRepository<TiktokStat, Guid> tiktokStatRepository,
            IRepository<GroupStatsHistory, Guid> groupStatsHistoriesRepository,
            IGroupRepository groupRepository,
            ITiktokRepository tiktokRepository)
        {
            _tiktokStatRepository = tiktokStatRepository;
            _groupStatsHistoriesRepository = groupStatsHistoriesRepository;
            _groupRepository = groupRepository;
            _tiktokRepository = tiktokRepository;
        }

        public async Task InitTiktokStatHistory(DateTime dateTime)
        {
            var tikTokStatHistories = _tiktokStatRepository
                .Where(x => x.Date >= dateTime.AddDays(-1))
                .Where(x => x.Date <= dateTime.AddDays(1))
                .ToList();

            var groupByTiktokStats = tikTokStatHistories.GroupBy(_ => _.Hashtag).ToList();
            foreach (var groupByTiktokStat in groupByTiktokStats)
            {
                if (tikTokStatHistories.FirstOrDefault(_ => _.Date == dateTime && _.Hashtag == groupByTiktokStat.FirstOrDefault()?.Hashtag) == null)
                {
                    var tikTokStat = new TiktokStat()
                    {
                        Count = groupByTiktokStat.FirstOrDefault()!.Count,
                        Date = dateTime,
                        Hashtag = groupByTiktokStat.FirstOrDefault()?.Hashtag,
                    };
                    await _tiktokStatRepository.InsertAsync(tikTokStat);
                }
            }
        }

        public async Task InitTikTokGroup(DateTime dateTime)
        {
            var groupStatsHistories = _groupStatsHistoriesRepository
                .Where(x => x.GroupSourceType == GroupSourceType.Tiktok)
                .Where(x => x.CreatedAt >= dateTime.AddDays(-1))
                .Where(x => x.CreatedAt <= dateTime.AddDays(1))
                .ToList();
            
            var groupStatsGroupBy = groupStatsHistories.GroupBy(_ => _.GroupFid);
            foreach (var group in groupStatsGroupBy)
            {
                if(group.Any(_ => _.CreatedAt.HasValue && _.CreatedAt.Value.Date == dateTime.Date)) 
                    continue;
                if (groupStatsHistories.FirstOrDefault(_ => _.CreatedAt == dateTime && _.GroupFid == group.Key && _.GroupSourceType == group.FirstOrDefault()!.GroupSourceType) == null)
                {
                    await _groupStatsHistoriesRepository.InsertAsync
                    (
                        new GroupStatsHistory()
                        {
                            GroupFid = group.Key,
                            GroupSourceType = @group.FirstOrDefault()!.GroupSourceType,
                            CreatedAt = dateTime,
                            TotalInteractions = (int)group.Average(_ => _.TotalInteractions),
                            InteractionRate = group.FirstOrDefault()?.InteractionRate,
                            AvgPosts = group.Average(_ => _.AvgPosts),
                            GroupMembers = (int)group.Average(_ => _.GroupMembers),
                            Reactions = (int)group.Average(_ => _.Reactions),
                            GrowthPercent = group.Average(_ => _.GrowthPercent),
                            GrowthNumber = (int)group.Average(_ => _.GrowthNumber)
                        }
                    );
                }
            }
        }

        public async Task UpdateGroupIdTikTokVideos()
        {
            var tiktokGroups = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok);
            var tiktokVideos = await _tiktokRepository.GetListExtendAsync();
            tiktokVideos = tiktokVideos.Select
                (
                    _ =>
                    {
                        _.GroupId = tiktokGroups.FirstOrDefault(g => _.Url.Contains((new Url(g.Url.Replace("?", "")).Path)))?.Id;
                        return _;
                    }
                )
                .Where(_ => _.GroupId.IsNotNullOrEmpty())
                .ToList();

            foreach (var partition in tiktokVideos.Partition(1000)) { await _tiktokRepository.UpdateManyAsync(partition); }
        }
    }
}