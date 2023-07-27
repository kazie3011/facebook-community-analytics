using System;
using System.Collections.Generic;
using System.Text;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserAffiliateDto : FullAuditedEntityDto<Guid>
    {
        public Guid? AppUserId { get; set; }
        
        public MarketplaceType MarketplaceType { get; set; }
        
        /// <summary>
        /// TIKI or Shopiness for now (10.2021)
        /// </summary>
        public AffiliateProviderType AffiliateProviderType { get; set; }
        
        public string Url { get; set; }
        public string AffiliateUrl { get; set; }

        public AffiliateOwnershipType AffiliateOwnershipType { get; set; } = ((AffiliateOwnershipType[])Enum.GetValues(typeof(AffiliateOwnershipType)))[1];
        public DateTime? CreatedAt { get; set; }
        public AffConversionModel AffConversionModel { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public Guid? GroupId { get; set; }
        
        public Guid? PartnerId { get; set; }
        
        public Guid? CampaignId { get; set; }
        public UserAffiliateDto()
        {
            AffConversionModel = new AffConversionModel();
        }
    }
}
