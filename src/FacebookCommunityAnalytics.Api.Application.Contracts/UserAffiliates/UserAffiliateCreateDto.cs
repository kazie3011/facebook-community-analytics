using FacebookCommunityAnalytics.Api.Core.Enums;
using System;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserAffiliateCreateDto
    {
        public Guid? AppUserId { get; set; }
        public MarketplaceType MarketplaceType { get; set; }
        /// <summary>
        /// TIKI or Shopiness for now (10.2021)
        /// </summary>
        public AffiliateProviderType AffiliateProviderType { get; set; }
        public AffiliateOwnershipType AffiliateOwnershipType { get; set; } = ((AffiliateOwnershipType[])Enum.GetValues(typeof(AffiliateOwnershipType)))[1];

        public string Url { get; set; }
        public string AffiliateUrl { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public AffConversionModel AffConversionModel { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? CampaignId { get; set; }
        public UserAffiliateCreateDto()
        {
            AffConversionModel = new AffConversionModel();
        }
    }
}
