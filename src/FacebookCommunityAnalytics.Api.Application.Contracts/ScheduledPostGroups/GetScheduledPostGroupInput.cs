using System;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    public class GetScheduledPostGroupInput
    {
        public Guid? ScheduledPostId  { get; set; }      
        public string Fid  { get; set; }
        public bool IsPosted { get; set; }  
    }
}