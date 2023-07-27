using FacebookCommunityAnalytics.Api.Groups;

namespace FacebookCommunityAnalytics.Api.GroupStatsHistories
{
    public class GroupStatsViewDto
    {
        //public GroupDto Group { get; set; }
        public string ChannelName { get; set; }
        public string Url { get; set; }
        public long TotalView { get; set; }
    }

    public class TopChannelDto
    {
        public int Index { get; set;}
        public GroupDto Group { get; set; }
        public long StartTotalFollower { get; set; }
        public long EndTotalFollower { get; set; }
        public long IncrementFollower { get; set; }
        public double GrowthPercent { get; set; }
    }
}