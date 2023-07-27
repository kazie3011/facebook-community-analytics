using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Partners;
using System;
using System.Collections.Generic;
using Humanizer;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class CampaignPost : Post
    {
        public string GroupName { get; set; }
        public string Username { get; set; }
    }

    public class PostHistory : Entity<Guid>, IMultiTenant
    {
        
        public virtual Guid? TenantId { get; set; }
        public Guid PostId { get; set; }
        public string Url { get; set; }

        public int LikeCount { get; set; }
        
        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int ShareCount { get; set; }

        public int TotalCount { get; set; }
        public DateTime CreatedDateTime { get; set; }   
    }

    public class Post : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual PostContentType PostContentType { get; set; }
        public virtual PostSourceType PostSourceType { get; set; }
        public virtual PostCopyrightType PostCopyrightType { get; set; }

        [NotNull] public virtual string Url { get; set; }
        public virtual List<string> Shortlinks { get; set; }

        public virtual int LikeCount { get; set; }
        
        public virtual int ViewCount { get; set; }

        public virtual int CommentCount { get; set; }

        public virtual int ShareCount { get; set; }

        public virtual int TotalCount { get; set; }
        public virtual string Content { get; set; }

        [CanBeNull] public virtual string Hashtag { get; set; }

        [CanBeNull] public virtual string Fid { get; set; }

        public virtual bool IsNotAvailable { get; set; }
        public virtual bool IsValid { get; set; }
        public virtual PostStatus Status { get; set; }

        [CanBeNull] public virtual string Note { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedFuid { get; set; }

        public virtual DateTime? CreatedDateTime { get; set; }

        public virtual DateTime? LastCrawledDateTime { get; set; }

        public virtual DateTime? SubmissionDateTime { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? PartnerId { get; set; }
        public bool IsNew { get; set; }
        public bool IsCampaignManual { get; set; }
        public bool IsPostContentTypeManual { get; set; }

        public Post()
        {
            SubmissionDateTime = DateTime.UtcNow;
            IsValid = true;
            IsNotAvailable = false;
            Shortlinks = new List<string>();
        }

        public Post(
            Guid id,
            PostContentType postContentType,
            PostCopyrightType postCopyrightType,
            string url,
            int likeCount,
            int commentCount,
            int shareCount,
            int totalCount,
            string hashtag,
            string fid,
            bool isNotAvailable,
            PostStatus status,
            string note,
            DateTime? createdDateTime = null,
            DateTime? lastCrawledDateTime = null,
            DateTime? submissionDateTime = null)
        {
            Id = id;
            Check.NotNull(url, nameof(url));
            PostContentType = postContentType;
            PostCopyrightType = postCopyrightType;
            Url = url;
            LikeCount = likeCount;
            CommentCount = commentCount;
            ShareCount = shareCount;
            TotalCount = totalCount;
            Hashtag = hashtag;
            Fid = fid;
            IsNotAvailable = isNotAvailable;
            Status = status;
            Note = note;
            CreatedDateTime = createdDateTime;
            LastCrawledDateTime = lastCrawledDateTime;
            SubmissionDateTime = submissionDateTime;
            IsValid = true;
            IsNotAvailable = false;
        }
    }
}