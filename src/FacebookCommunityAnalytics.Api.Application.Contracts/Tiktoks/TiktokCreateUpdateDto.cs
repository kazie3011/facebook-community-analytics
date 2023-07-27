using System;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TiktokCreateUpdateDto
    {
        public  string VideoId { get; set; }
        public  string ChannelId { get; set; }

        public  string Url { get; set; }

        public  string ShortUrl { get; set; }

        public  int ViewCount { get; set; }
        public  int LikeCount { get; set; }
        public  int CommentCount { get; set; }
        public  int ShareCount { get; set; }
        public  int TotalCount { get; set; }

        public  string Hashtag { get; set; }

        public  DateTime? CreatedDateTime { get; set; }

        public  DateTime? LastCrawledDateTime { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? PartnerId { get; set; }
        public string ThumbnailImage { get; set; }
        public bool IsNew { get; set; }
    }
}