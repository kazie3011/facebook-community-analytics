using System;
using Volo.Abp.Domain.Entities;

namespace FacebookCommunityAnalytics.Api.AffiliateConversions
{
    public class AffiliateConversion : Entity<Guid>
    {
        public string ConversionItemId { get; set; }
        public string ConversionId { get; set; }
        public string Status { get; set; }
        public int SaleAmount { get; set; }
        public int Payout { get; set; }
        public int PayoutBonus { get; set; }
        public long ConversionTime { get; set; }
        public string Platform { get; set; }
        public string SubId1 { get; set; }
        public string SubId2 { get; set; }
        public string SubId3 { get; set; }
        public string ShopId { get; set; }
        public string ShortKey { get; set; }
        public string ShopName { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string Campaign { get; set; }
        public bool IsHappyDay { get; set; } = false;
    }
}