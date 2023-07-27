using FacebookCommunityAnalytics.Api.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Timing;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IGroupDomainService : IDomainService
    {
        Task<List<Group>> CreateGroups(List<Group> groups);
        Task<Group> GetAsync(GetGroupApiRequest request);
        Task<Group> GetOrCreateAsync(GetGroupApiRequest request);
        Task<List<Group>> GetManyAsync(GetGroupApiRequest request);
        Task UpdateGroupStats(GroupStatsRequest request);
        Task CleanGroupStats();
        Task<List<TikTokChannelKpiModel>> GetTikTokChannelKpiModels(GetTikTokChannelKpiRequest request);
        Task<List<GroupStatsView>> GetTopViewChannelsTwoWeek(int numberChannel);

        Task<List<Group>> GetMCNGDLChannels();
        Task<List<Group>> GetMCNVietNamChannels();
    }

    public class GroupDomainService : BaseDomainService, IGroupDomainService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IRepository<GroupStatsHistory, Guid> _groupStatsHistoryRepository;
        private readonly ITiktokRepository _tiktokRepository;
        private readonly IRepository<TikTokMCN,Guid> _tikTokMcnRepository;
        private readonly IRepository<TiktokVideoStat> _tiktokVideoStatRepository;
        private readonly IStaffEvaluationRepository _staffEvaluationRepository;

        public GroupDomainService(IGroupRepository groupRepository, IRepository<GroupStatsHistory, Guid> groupStatsHistoryRepository, ITiktokRepository tiktokRepository,
            IRepository<TikTokMCN,Guid> tikTokMcnRepository,
            IRepository<TiktokVideoStat> tiktokVideoStatRepository,
            IStaffEvaluationRepository staffEvaluationRepository)
        {
            _groupRepository = groupRepository;
            _groupStatsHistoryRepository = groupStatsHistoryRepository;
            _tiktokRepository = tiktokRepository;
            _tikTokMcnRepository = tikTokMcnRepository;
            _tiktokVideoStatRepository = tiktokVideoStatRepository;
            _staffEvaluationRepository = staffEvaluationRepository;
        }

        public async Task<List<Group>> CreateGroups(List<Group> groups)
        {
            var groupEntities = new List<Group>();
            foreach (var groupItem in groups)
            {
                var group = await _groupRepository.GetAsync(_ => _.Fid == groupItem.Fid.Trim());
                if (group != null) { groupEntities.Add(group); }
                else { groupEntities.Add(await _groupRepository.InsertAsync(groupItem)); }
            }

            return groupEntities;
        }

        public async Task<Group> GetAsync(GetGroupApiRequest request)
        {
            return (await GetManyAsync(request)).FirstOrDefault();
        }

        public async Task<Group> GetOrCreateAsync(GetGroupApiRequest request)
        {
            var group = await GetAsync(request);
            if (group !=null) return group;

            if (request.GroupSourceType != null)
            {
                return await _groupRepository.InsertAsync
                (
                    new Group
                    {
                        Fid = request.GroupFid,
                        Name = request.GroupFid,
                        GroupSourceType = request.GroupSourceType.Value,
                        IsActive = true
                    }
                );
            }

            return null;
        }


        public async Task<List<Group>> GetManyAsync(GetGroupApiRequest request)
        {
            var groups = await _groupRepository.GetListAsync();
            
            groups = groups
                .WhereIf(request.GroupCategoryType.HasValue, x => x.GroupCategoryType == request.GroupCategoryType)
                .WhereIf(request.GroupSourceType.HasValue, x => x.GroupSourceType == request.GroupSourceType)
                .WhereIf(request.GroupOwnershipType.HasValue, x => x.GroupOwnershipType == request.GroupOwnershipType)
                .WhereIf(request.ContractStatus.HasValue, x => x.ContractStatus == request.ContractStatus)
                .WhereIf
                (
                    request.GroupFid.IsNotNullOrWhiteSpace(),
                    x => (x.Fid != null && string.Equals(x.Fid, request.GroupFid, StringComparison.OrdinalIgnoreCase))
                         || (x.Name != null && string.Equals(x.Name, request.GroupFid, StringComparison.OrdinalIgnoreCase))
                         || (x.Url != null && x.Url.Contains(request.GroupFid))
                )
                .ToList();
            
            return groups;
        }
        
        public List<Group> GetGroups(GroupOwnershipType? groupOwnershipType = null)
        {
            var groups = _groupRepository.WhereIf(groupOwnershipType != null, _ => _.GroupOwnershipType == groupOwnershipType).ToList();

            return groups.OrderBy(_ => _.Title).ToList();
        }

        public async Task UpdateGroupStats(GroupStatsRequest request)
        {
            var groups = await _groupRepository.GetListAsync(g => g.IsActive);
            foreach (var group in groups)
            {
                var item = request.Items.FirstOrDefault(i => group.Title.LevenshteinDistance(i.GroupName) <= 1);
                if (item != null && group.GroupSourceType == item.GroupSourceType)
                {
                    var stats = await _groupStatsHistoryRepository.FirstOrDefaultAsync
                    (
                        s => s.GroupFid == group.Fid
                             && s.CreatedAt.Value == DateTime.UtcNow.Date.AddDays(-1)
                    );

                    var isNew = stats == null;
                    if (isNew) { stats = new GroupStatsHistory(); }

                    stats.GroupFid = group.Fid;
                    stats.GroupId = group.Id;
                    stats.GroupSourceType = group.GroupSourceType;
                    stats.TotalInteractions = item.TotalInteractions;
                    stats.InteractionRate = item.InteractionRate;
                    stats.AvgPosts = item.AvgPosts;
                    stats.GroupMembers = item.GroupMembers;
                    stats.GrowthPercent = item.GrowthPercent;
                    stats.GrowthNumber = item.GrowthNumber;
                    stats.CreatedAt = DateTime.UtcNow.Date.AddDays(-1);

                    if (isNew) { await _groupStatsHistoryRepository.InsertAsync(stats); }
                    else { await _groupStatsHistoryRepository.UpdateAsync(stats); }

                    group.Stats = new GroupStats
                    {
                        AvgPosts = item.AvgPosts,
                        GroupMembers = item.GroupMembers,
                        GrowthPercent = item.GrowthPercent,
                        GrowthNumber = item.GrowthNumber,
                        InteractionRate = item.InteractionRate,
                        TotalInteractions = item.TotalInteractions
                    };
                    await _groupRepository.UpdateAsync(group);
                }
            }
        }

        public async Task CleanGroupStats()
        {
            var groups = await _groupRepository.GetListAsync();
            foreach (var group in groups)
            {
                group.Stats = new GroupStats();
                await _groupRepository.UpdateAsync(group);
            }
        }

        public async Task<List<TikTokChannelKpiModel>> GetTikTokChannelKpiModels(GetTikTokChannelKpiRequest request)
        {
            var tiktokChannelKpiModels = new List<TikTokChannelKpiModel>();
            var tiktokChannels = await _groupRepository.GetListAsync(_ => _.GroupSourceType == GroupSourceType.Tiktok 
                                                                          && _.GroupOwnershipType == GroupOwnershipType.GDLInternal 
                                                                          && _.IsActive == true);
            var communityEvaluations = await _staffEvaluationRepository.GetListAsync(x => x.Month == request.Month && x.Year == request.Year && x.CommunityId != null);

            var index = 0;
            foreach (var channel in tiktokChannels)
            {
                index++;
                var communityEvaluation = communityEvaluations.FirstOrDefault(_ => _.CommunityId == channel.Id);
                tiktokChannelKpiModels.Add
                (
                    new TikTokChannelKpiModel()
                    {
                        Index = index,
                        Group = ObjectMapper.Map<Group, GroupDto>(channel),
                        StaffEvaluation = ObjectMapper.Map<StaffEvaluation, StaffEvaluationDto>(communityEvaluation)
                    }
                );
            }

            return tiktokChannelKpiModels;
        }

        public async Task<List<GroupStatsView>> GetTopViewChannelsTwoWeek(int numberChannel)
        {
            var endDateTime = DateTime.UtcNow;
            var startDateTime = endDateTime.AddDays(-14);
            var tiktokVideoStats =(await  _tiktokVideoStatRepository.GetQueryableAsync()).Where(x => x.Date >= startDateTime && x.Date <= endDateTime).ToList();
            var channelIds = tiktokVideoStats.Select(x => x.ChannelId).Distinct().ToList();
            var tiktokChannels = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok);
            
            var result = new List<GroupStatsView>();
            foreach (var channelId in channelIds)
            {
                var totalView = tiktokVideoStats.Where(x => x.ChannelId == channelId).Sum(s => s.ViewCount);
                result.Add(new GroupStatsView()
                {
                    ChannelName = channelId,
                    Url = tiktokChannels.FirstOrDefault(_ => _.Fid == channelId)?.Url,
                    TotalView = totalView
                });
            }
            return result.OrderByDescending(x=>x.TotalView).Take(numberChannel).ToList();
        }

        public async Task<List<Group>> GetMCNGDLChannels()
        {
            var mcnVietNamIds = (await  _tikTokMcnRepository.GetQueryableAsync()).Where(x => x.MCNType == TikTokMCNType.MCNGdl).Select(x=>x.Id).ToList();
            var channels = (await _groupRepository.GetQueryableAsync()).Where(x => x.GroupSourceType == GroupSourceType.Tiktok && x.IsActive && x.McnId.HasValue && mcnVietNamIds.Contains(x.McnId.Value)).ToList();
            return channels;
        }
        
        public async Task<List<Group>> GetMCNVietNamChannels()
        {
            var mcnVietNamIds = (await  _tikTokMcnRepository.GetQueryableAsync()).Select(x=>x.Id).ToList();
            var channels = (await _groupRepository.GetQueryableAsync()).Where(x => x.GroupSourceType == GroupSourceType.Tiktok && x.IsActive && x.McnId.HasValue && mcnVietNamIds.Contains(x.McnId.Value)).ToList();
            return channels;
        }
    }
}