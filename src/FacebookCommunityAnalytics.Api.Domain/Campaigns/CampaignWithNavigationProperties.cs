using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Partners;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class CampaignWithNavigationProperties
    {
        // public CampaignWithNavigationProperties()
        // {
        //     Contracts = new List<Contract>();
        // }
        public Campaign Campaign { get; set; }

        public Partner Partner { get; set; }

        public int TotalPostFacebook { get; set; }
        public int TotalPostTiktok { get; set; }
        
        //public List<Contract> Contracts { get; set; }
        
    }
}