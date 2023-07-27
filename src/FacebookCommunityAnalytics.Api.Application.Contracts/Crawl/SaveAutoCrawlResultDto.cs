using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public class SaveAutoCrawlResultApiRequest
    {
        public CrawlType CrawlType { get; set; }
        public string GroupFid { get; set; }
        public List<AutoCrawlPost> Items { get; set; }

        public SaveAutoCrawlResultApiRequest()
        {
            Items = new List<AutoCrawlPost>();
        }
    }
    public class SaveAutoCrawlResultApiResponse
    {
        public bool Success { get; set; }
    }
    
    public class AutoCrawlPost
    {
        public PostSourceType PostSourceType { get; set; }
        
        public string Url { get; set; }

        /// <summary>
        /// This is used for affiliate urls (usually called short links)
        /// </summary>
        public List<string> Urls { get; set; }

        public string Content { get; set; }
        public List<string> HashTags { get; set; }
        
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreateFuid { get; set; }

        public bool IsNotAvailable { get; set; }
        
        public string GroupFid { get; set; }
        public string CampaignCode { get; set; }
        public string PartnerCode { get; set; }

        public AutoCrawlPost()
        {
            Urls = new List<string>();
            HashTags = new List<string>();
        }
    }
}