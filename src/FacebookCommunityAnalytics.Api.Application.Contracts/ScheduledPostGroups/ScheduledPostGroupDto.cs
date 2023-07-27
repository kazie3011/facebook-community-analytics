using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    public class ScheduledPostGroupDto:AuditedEntityDto<Guid>
    {
        public Guid? ScheduledPostId  { get; set; }      
        public string Fid  { get; set; }
        public bool IsPosted { get; set; }  
    }
}