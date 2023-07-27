using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class GroupStatsRequest
    {
        public List<GroupStatsItem> Items { get; set; }
    }
    
    public class GroupStatsItem
    {
        public string GroupName { get; set; }
        public int TotalInteractions { get; set; }
        public string InteractionRate { get; set; }
        public int AvgPosts { get; set; }
        public int GroupMembers { get; set; }
        public decimal? GrowthPercent { get; set; }
        public int Reactions { get; set; }
        public int GrowthNumber { get; set; }
        public GroupSourceType GroupSourceType { get; set; }
    }
}