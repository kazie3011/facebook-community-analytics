using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.GroupStatsHistories
{
    public class GroupStatsHistoryDto : AuditedEntityDto<Guid>
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
    }
}