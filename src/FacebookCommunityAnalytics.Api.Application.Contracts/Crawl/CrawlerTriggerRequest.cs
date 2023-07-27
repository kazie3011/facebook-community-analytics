using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public class CrawlerTriggerRequest
    {
        public CrawlerTriggerRequest()
        {
            PartnerIds = new List<Guid>();
        }

        public List<Guid> PartnerIds { get; set; }
    }

    public class CrawlerTriggerCampaignRequest
    {
        public CrawlerTriggerCampaignRequest()
        {
            CampaignIds = new List<Guid>();
        }

        public List<Guid> CampaignIds { get; set; }
    }
}