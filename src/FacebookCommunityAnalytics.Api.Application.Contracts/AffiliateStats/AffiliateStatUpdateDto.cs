using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public class AffiliateStatUpdateDto
    {
        public AffiliateOwnershipType AffiliateOwnershipType { get; set; }
        public int Click { get; set; }
        public int Conversion { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public decimal CommissionBonus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}