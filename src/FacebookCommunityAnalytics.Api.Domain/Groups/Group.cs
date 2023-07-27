using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class Group : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Title { get; set; }

        [NotNull]
        public virtual string Fid { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string AltName { get; set; }

        [NotNull]
        public virtual string Url { get; set; }
        
        public virtual string Description { get; set; }
 
        public virtual bool IsActive { get; set; }
        
        public DateTime? DeactivatedAt { get; set; }

        public virtual double Point { get; set; }

        public virtual  GroupCategoryType GroupCategoryType { get; set; } 
        public virtual  GroupSourceType GroupSourceType { get; set; } 
        public virtual  GroupOwnershipType GroupOwnershipType { get; set; }
        
        // TODOO Long: change this into TiktokKPI
        public virtual TiktokEvaluation TiktokEvaluation { get; set; }
        public virtual GroupCrawlInfo CrawlInfo { get; set; }
        public virtual GroupStats Stats { get; set; }
        
        
        public TikTokTarget TikTokTarget { get; set; }

        public TikTokContractStatus ContractStatus { get; set; }
        
        public virtual List<Guid> ModeratorIds { get; set; }

        public Guid? McnId { get; set; }
        public string ThumbnailImage { get; set; }
        public string TikTokUserId { get; set; }
        public Group()
        {
            CrawlInfo = new GroupCrawlInfo();
            Stats = new GroupStats();
            ModeratorIds = new List<Guid>();
            TiktokEvaluation = new TiktokEvaluation();
            TikTokTarget = new TikTokTarget();
        }

        public Group(Guid id, string title, string fid, string name, string altName, string url, bool isActive, GroupSourceType groupSourceType,GroupOwnershipType groupOwnershipType)
        {
            Check.NotNull(title, nameof(title));
            Check.NotNull(fid, nameof(fid));
            Check.NotNull(name, nameof(name));
            Check.NotNull(url, nameof(url));
            
            Id = id;
            Title = title;
            Fid = fid;
            Name = name;
            AltName = altName;
            Url = url;
            IsActive = isActive;
            GroupSourceType = groupSourceType;
            GroupOwnershipType = groupOwnershipType;
            ModeratorIds = new List<Guid>();
            TiktokEvaluation = new();
        }
    }

    public class GroupCrawlInfo
    {
        public DateTime? GroupInfoLastCrawledDateTime { get; set; }
        public DateTime? GroupPostLastCrawledDateTime { get; set; }
        public List<string> SeedingTeams { get; set; }

        public GroupCrawlInfo()
        {
            SeedingTeams = new List<string>();
        }
    }

    public class GroupStats
    {
        public int TotalInteractions { get; set; }
        public string InteractionRate { get; set; }
        public double AvgPosts { get; set; }
        public int GroupMembers { get; set; }
        public int Reactions { get; set; }
        public decimal? GrowthPercent { get; set; }
        public int GrowthNumber { get; set; }
    }
    
    public class TiktokEvaluation
    {
        public int? TiktokVideoPerMonth { get; set; }
        public int? TiktokAverageVideoView { get; set; }
    }
    public class TikTokTarget
    {
        public long? TargetFollower { get; set; }
        public long? TargetVideo { get; set; }
        public DateTime? TargetDateTime { get; set; }
    }
}