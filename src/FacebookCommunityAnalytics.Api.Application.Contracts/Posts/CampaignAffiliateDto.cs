using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class CampaignAffiliateDto : EntityDto<Guid>
    {
        public string Author { get; set; }
        public MarketplaceType MarketplaceType { get; set; }
        public AffiliateProviderType AffiliateProviderType { get; set; }
        public string Url { get; set; }
        public string GroupName { get; set; }
        public string Shortlink { get; set; }
        public int ClickCount { get; set; }
        public int ConversionCount { get; set; }
        public decimal ConversionAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public double Progress { get; set; }
    }
}