using System;
using System.Collections.Generic;
using System.Diagnostics;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public class GetUncrawledGroupUserApiRequest
    {
    }

    public class GetUncrawledGroupUserApiResponse
    {
        public int Count { get; set; }
        public List<UncrawledGroupUserItem> Items { get; set; }

        public GetUncrawledGroupUserApiResponse()
        {
            Items = new List<UncrawledGroupUserItem>();
        }
    }

    [DebuggerDisplay("{GroupName}|{Usercode}|{Url}")]
    public class UncrawledGroupUserItem
    {
        public string GroupName { get; set; }
        public string GroupFid { get; set; }

        public List<UncrawledGroupUserCrawlUrlItem> UrlItems { get; set; }

        public UncrawledGroupUserItem()
        {
            UrlItems = new List<UncrawledGroupUserCrawlUrlItem>();
        }
    }

    public class UncrawledGroupUserCrawlUrlItem
    {
        public string GroupName { get; set; }
        public string GroupFid { get; set; }
        
        // "https://www.facebook.com/groups/956982674510656/user/100066517234397/",
        public string Url { get; set; }

        public Guid UserId { get; set; }
        public string Usercode { get; set; }
        public string Fuid { get; set; }
    }
}