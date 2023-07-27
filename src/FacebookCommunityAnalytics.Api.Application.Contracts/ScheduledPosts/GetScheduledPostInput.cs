using System;
using System.Collections.Generic;
using System.Text;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public class GetScheduledPostInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        public string Content { get; set; }
        public bool? IsAutoPost { get; set; }
        public DateTime? ScheduledPostDateTimeMin { get; set; }
        public DateTime? ScheduledPostDateTimeMax { get; set; }
        public DateTime? PostedAtMin { get; set; }
        public DateTime? PostedAtMax { get; set; }
        public string GroupId { get; set; }
        //public PostContentType PostContentType { get; set; }

        //public PostCopyrightType PostCopyrightType { get; set; }
        //public Guid? CategoryId { get; set; }
        //public bool IsPosted { get; set; }
        //public string Url { get; set; }
        //public Guid? AppUserId { get; set; }
        //public string Note { get; set; }
    }

    public class ScheduledPostDto : FullAuditedEntityDto<Guid>
    {
        public string Content { get; set; }
        public bool IsAutoPost { get; set; }
        public DateTime? ScheduledPostDateTime { get; set; }
        public DateTime? PostedAt { get; set; }
        public string GroupIds { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public PostContentType PostContentType { get; set; }

        public PostCopyrightType PostCopyrightType { get; set; }
        public Guid? CategoryId { get; set; }
        public string Note { get; set; }
        public bool IsPosted { get; set; }
        public string Url { get; set; }
        public Guid? AppUserId { get; set; }
    }
}
