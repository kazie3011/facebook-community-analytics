using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.AffiliateConversions
{
    public class AffiliateConversionDto : EntityDto<Guid>
    {
        public string ConversionItemId { get; set; }
        public string ConversionId { get; set; }
        public string Status { get; set; }
        public int SaleAmount { get; set; }
        public int Payout { get; set; }
        public int PayoutBonus { get; set; }
        public int ConversionTime { get; set; }
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
    }
}