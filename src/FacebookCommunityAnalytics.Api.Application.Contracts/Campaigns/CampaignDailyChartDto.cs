using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Statistics;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class CampaignDailyChartDto
    {
        public DateTime Date { get; set; }
        public int SeedingPostCount { get; set; }
        public int ContestPostCount { get; set; }
        public int D2cPostCount { get; set; }
        public int AffiliatePostCount { get; set; }
    }

    public class CampaignDailyChartResponse
    {
        public CampaignDailyChartResponse()
        {
            Data = new List<DataChartItemDto<int, string>>();
        }
        public List<DataChartItemDto<int, string>> Data { get; set; }
    }
  
}