using System;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.StaffEvaluations;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TikTokChannelKpiModel
    {
        public int Index { get; set; }
        public GroupDto Group { get; set; }

        public StaffEvaluationDto StaffEvaluation { get; set; }
    }

    public class GetTikTokChannelKpiRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }
    
    
    public class TikTokInternalChannelKpiModel
    {
        public int Index { get; set; }
        public GroupDto Group { get; set; }
        public long CurrentVideos { get; set; }
        public long TargetVideos { get; set; }
        public long CurrentFollowers { get; set; }
        public long TargetFollowers { get; set; }
    }
}