using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class Tiktok : FullAuditedAggregateRoot<Guid>
    {
        public  string VideoId { get; set; }
        public  string ChannelId { get; set; }

        [Required]
        public  string Url { get; set; }

        public  string ShortUrl { get; set; }

        public  int ViewCount { get; set; }
        public  int LikeCount { get; set; }
        public  int CommentCount { get; set; }
        public  int ShareCount { get; set; }
        public  int TotalCount { get; set; }

        public  string Hashtag { get; set; }
        
        public string Content { get; set; }

        public  DateTime? CreatedDateTime { get; set; }
        public  DateTime? LastCrawledDateTime { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? GroupId { get; set; }
        public string ThumbnailImage { get; set; }
        /// <summary>
        /// Used for exporting data
        /// </summary>
        public bool IsNew { get; set; }
        
        public Tiktok()
        {
        }
    }
}