using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public class ScheduledPostCreateDto
    {
        public string Content { get; set; }

        public bool IsAutoPost { get; set; } = true;
        public DateTime? ScheduledPostDateTime { get; set; }
        public DateTime? PostedAt { get; set; }
        
        public string GroupIds { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public PostContentType PostContentType { get; set; } = PostContentType.Affiliate;
        public PostCopyrightType PostCopyrightType { get; set; } = ((PostCopyrightType[])Enum.GetValues(typeof(PostCopyrightType)))[0];
        public Guid? CategoryId { get; set; }
        public string Note { get; set; }
        public bool IsPosted { get; set; }
        public string Url { get; set; }
        public Guid? AppUserId { get; set; }
    }
}