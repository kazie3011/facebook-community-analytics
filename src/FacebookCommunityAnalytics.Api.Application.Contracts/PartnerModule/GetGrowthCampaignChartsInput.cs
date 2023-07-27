using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.PartnerModule
{
    public class GetGrowthCampaignChartsInput
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        
        public Guid? PartnerId { get; set; }
        public List<Guid> CampaignIds { get; set; }

        public GetGrowthCampaignChartsInput()
        {
            CampaignIds = new List<Guid>();
        }
    }
}