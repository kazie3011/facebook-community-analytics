using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostDto : FullAuditedEntityDto<Guid>
    {
        public PostContentType PostContentType { get; set; }
        public PostCopyrightType PostCopyrightType { get; set; }
        public string Url { get; set; }
        public List<string> Shortlinks { get; set; } = new List<string>();
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public int TotalCount { get; set; }
        public string Hashtag { get; set; }
        public string Fid { get; set; }
        public bool IsNotAvailable { get; set; }
        public bool IsValid { get; set; }
        public PostStatus Status { get; set; }       
        public PostSourceType PostSourceType { get; set; }
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