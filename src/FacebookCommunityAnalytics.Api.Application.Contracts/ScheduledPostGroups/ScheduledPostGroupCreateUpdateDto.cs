using System;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    public class ScheduledPostGroupCreateUpdateDto
    {
        public Guid? ScheduledPostId  { get; set; }      
        public string Fid   { get; set; }
        public bool IsPosted { get; set; }  
    }
}