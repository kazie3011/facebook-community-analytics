using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public class AffiliateStatDto : EntityDto<Guid>
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