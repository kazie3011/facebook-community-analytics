using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class PartnerCampaignChartDto
    {
        public PartnerCampaignChartDto()
        {
            PartnerCampaignChartItems = new List<PartnerCampaignChartItem>();
        }
       public List<PartnerCampaignChartItem> PartnerCampaignChartItems { get; set; }
    }

    public class PartnerCampaignChartItem
    {
        public string Label { get; set; }
        public int TotalCampaign { get; set; }
    }
    
    public class PartnerPostTypeChartDto
    {
        public PartnerPostTypeChartDto()
        {
            PartnerPostTypeChartItems = new List<PartnerPostTypeChartItem>();
        }
       public List<PartnerPostTypeChartItem> PartnerPostTypeChartItems { get; set; }
    }

    public class PartnerPostTypeChartItem
    {
        public string Label { get; set; }
        public int TotalPost { get; set; }
    }

}