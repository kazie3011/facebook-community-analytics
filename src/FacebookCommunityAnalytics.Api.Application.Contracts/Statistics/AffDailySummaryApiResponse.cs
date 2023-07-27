using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.AffiliateStats;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class AffDailySummaryApiResponse
    {
        public AffDailySummaryApiResponse()
        {
            Items = new List<AffDailySummaryItem>();
        }

        public List<AffDailySummaryItem> Items { get; set; }
    }

    public class AffDailySummaryItem : AffiliateModel
    {
        public string Display { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}