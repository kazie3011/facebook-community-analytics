using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class GetUserAffiliatesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        public Guid? AppUserId { get; set; }
        public MarketplaceType? MarketplaceType { get; set; }
        public string Url { get; set; }
        public string AffiliateUrl { get; set; }
        
        public AffiliateOwnershipType AffiliateOwnershipType { get; set; } = ((AffiliateOwnershipType[])Enum.GetValues(typeof(AffiliateOwnershipType)))[1];
        public AffiliateProviderType? AffiliateProviderType { get; set; }

        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        public AffConversionModel AffConversionModel { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? CampaignId { get; set; }
        public GetUserAffiliatesInput()
        {
            AffConversionModel = new AffConversionModel();
        }
    }
}
