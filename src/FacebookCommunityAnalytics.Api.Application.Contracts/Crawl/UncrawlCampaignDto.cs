using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public class UncrawledCampaignPostsApiRequest
    {
        public PostSourceType? PostSourceType { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public AccountType AccountType { get; set; }
        public bool IsNew { get; set; }
    }
    
    public class UncrawledCampaignPostsApiResponse
    {
        public long Count { get; set; }
        public List<UncrawledItemDto> Items { get; set; }
        public List<AccountProxyWithNavigationPropertiesDto> AccountProxies { get; set; }
    }

}