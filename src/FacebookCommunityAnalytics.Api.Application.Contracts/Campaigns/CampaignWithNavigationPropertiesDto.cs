using FacebookCommunityAnalytics.Api.Partners;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Contracts;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class CampaignWithNavigationPropertiesDto
    {
        // public CampaignWithNavigationPropertiesDto()
        // {
        //     Contracts = new List<ContractDto>();
        // }
        public CampaignDto Campaign { get; set; }

        public PartnerDto Partner { get; set; }
        
        public int TotalPostFacebook { get; set; }
        public int TotalPostTiktok { get; set; }
        
        //public List<ContractDto> Contracts { get; set; }

    }
}