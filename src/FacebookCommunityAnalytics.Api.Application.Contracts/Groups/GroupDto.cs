using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class GroupDto : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string Fid { get; set; }
        public string Name { get; set; }
        public string AltName { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public double Point { get; set; }
        public List<Guid> ModeratorIds { get; set; }

        public GroupCategoryType GroupCategoryType { get; set; }
        public GroupSourceType GroupSourceType { get; set; }
        public GroupOwnershipType GroupOwnershipType { get; set; }
        public GroupCrawlInfoDto CrawlInfo { get; set; }
        public GroupStatsDto Stats { get; set; }
        
        public TikTokTargetDto TikTokTarget { get; set; }

        public TikTokContractStatus ContractStatus { get; set; }

        public Guid? McnId { get; set; }
        public string ThumbnailImage { get; set; }
        public string TikTokUserId { get; set; }
        public GroupDto()
        {
            CrawlInfo = new GroupCrawlInfoDto();
            Stats = new GroupStatsDto();
            ModeratorIds = new List<Guid>();
            CrawlInfo = new GroupCrawlInfoDto();
            TikTokTarget = new TikTokTargetDto();
        }
    }
    public class GroupStatsDto
    {
        public int TotalInteractions { get; set; }
        public string InteractionRate { get; set; }
        public double AvgPosts { get; set; }
        public int GroupMembers { get; set; }
        public int Reactions { get; set; }
        public decimal? GrowthPercent { get; set; }
        public int  GrowthNumber { get; set;}    
    }
    public class GroupCrawlInfoDto
    {
        public DateTime? GroupInfoLastCrawledDateTime { get; set; }
        public DateTime? GroupPostLastCrawledDateTime { get; set; }
        public List<string> SeedingTeams { get; set; }

        public GroupCrawlInfoDto()
        {
            SeedingTeams = new List<string>();
        }
    }

    public class TikTokTargetDto
    {
        public long? TargetFollower { get; set; }
        public long? TargetVideo { get; set; }
        public DateTime? TargetDateTime { get; set; }
    }
}