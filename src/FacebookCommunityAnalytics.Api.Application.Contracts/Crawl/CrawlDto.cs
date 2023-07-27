using System;
using System.Collections.Generic;
using System.Text;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public class UncrawledPostsApiRequest
    {
        public PostSourceType? PostSourceType { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        
        public AccountType AccountType { get; set; }
    }
    
    public class UncrawledPostsApiResponse
    {
        public long Count { get; set; }
        public List<UncrawledItemDto> Items { get; set; }
        
        public List<AccountProxyWithNavigationPropertiesDto> AccountProxies { get; set; }
    }
    
    public class UncrawledItemDto
    {
        public string Url { get; set; }
        public PostSourceType PostSourceType { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class SaveCrawlResultApiRequest
    {
        public List<CrawledPostDto> Items { get; set; }
    }

    public class CrawledPostDto
    {
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
        public bool IsNotAvailable { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreateFuid { get; set; }
    }
}
