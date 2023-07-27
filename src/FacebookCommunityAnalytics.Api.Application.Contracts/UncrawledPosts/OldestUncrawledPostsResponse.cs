using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public class OldestUncrawledPostsResponse
    {
        public List<OldestUncrawledPostsResponseItem> Items { get; set; }
    }

    public class OldestUncrawledPostsResponseItem
    {
        public string Url { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PostSourceType PostSourceType { get; set; }
    }
}