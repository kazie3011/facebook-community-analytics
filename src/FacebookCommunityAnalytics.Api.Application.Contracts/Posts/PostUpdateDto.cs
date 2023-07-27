using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostUpdateDto
    {
        [Required]
        public PostContentType PostContentType { get; set; }
        [Required]
        public PostCopyrightType PostCopyrightType { get; set; }
        [Required]
        public string Url { get; set; }
        public List<string> Shortlinks { get; set; } = new List<string>();
        [Required]
        public int LikeCount { get; set; }
        [Required]
        public int CommentCount { get; set; }
        [Required]
        public int ShareCount { get; set; }
        [Required]
        public int TotalCount { get; set; }
        public string Hashtag { get; set; }
        public string Fid { get; set; }
        public bool IsNotAvailable { get; set; }
        public bool IsValid { get; set; }
        [Required]
        public PostStatus Status { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedFuid { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? LastCrawledDateTime { get; set; }
        public DateTime? SubmissionDateTime { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? PartnerId { get; set; }
        public bool IsCampaignManual { get; set; }
        public bool IsPostContentTypeManual { get; set; }
    }
}