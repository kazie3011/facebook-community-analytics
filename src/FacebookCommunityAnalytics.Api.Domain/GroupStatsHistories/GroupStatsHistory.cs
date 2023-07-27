using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.GroupStatsHistories
{
    public class GroupStatsHistory : AuditedEntity<Guid>
    {
        public string GroupFid { get; set; }
        public GroupSourceType GroupSourceType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int TotalInteractions { get; set; }
        public string InteractionRate { get; set; }
        public double AvgPosts { get; set; }
        public int GroupMembers { get; set; }
        public int Reactions { get; set; }
        public decimal? GrowthPercent { get; set; }
        public int GrowthNumber { get; set; }
        public Guid? GroupId { get; set; }
    }
}