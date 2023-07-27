using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    public class ScheduledPostGroup : FullAuditedAggregateRoot<Guid>
    {
        public Guid? ScheduledPostId  { get; set; }      
        public string Fid  { get; set; }
        public bool IsPosted { get; set; }          

        public ScheduledPostGroup(){}

        public ScheduledPostGroup(Guid? scheduledPostId, string fid, bool isPosted)
        {
            ScheduledPostId = scheduledPostId;
            Fid = fid;
            IsPosted = isPosted;
        }
    }
}