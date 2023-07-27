
using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public class CampaignPostResponse
    {
        public List<CampaignPostResponseItem> Items { get; set; }
    }
     
    public class CampaignPostResponseItem
    {
        public string Url { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PostSourceType PostSourceType { get; set; }
    }
}