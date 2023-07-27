using System;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

namespace FacebookCommunityAnalytics.Api.Dev
{
    public class ShopinessAffiliate
    {
        [Name("Link")] public string Link { get; set; }
        [Name("Click count")] public string Click { get; set; }
        [Name("Conversion count")] public string ConversionCount { get; set; }
        [Name("Conversion Amount")] public string ConversionAmount { get; set; }
        [Name("Commission Amount")] public string CommissionAmount { get; set; }
        [Name("Bonus")] public string CommissionBonusAmount { get; set; }
        [Name("UTM")] public string UTM { get; set; }
        [Name("Created At")] public string CreatedAt { get; set; }
    }

    public record UserConversion
    {
        public string UserName { get; set; }
        public long Conversion { get; set; }
    }

    public record AverageReactionUserInfo
    {
        public string UserCode { get; set; }
        public double AverageReaction { get; set; }
    }

    public class ConversionAndReactionPost
    {
        public string Url { get; set; }
        public int Click { get; set; }
        public int Conversion { get; set; }
        public string Rate { get; set; }
        public int Total { get; set; }
        public int Like { get; set; }
        public int Share { get; set; }
        public int Comment { get; set; }
    }

    public class ShopinessRawConversion
    {
        [Name("conversionItemId")] public string ConversionItemId { get; set; }
        [Name("name")] public string Name { get; set; }

        /// <summary>
        /// pending, approved, rejected
        /// </summary>
        [Name("status")]
        public string Status { get; set; }

        [Name("saleAmount")] public string SaleAmount { get; set; }
        [Name("payout")] public string Payout { get; set; }
        [Name("createdAt")] public string CreatedAt { get; set; }
        [Name("merchant")] public string Merchant { get; set; }
        [Name("utm")] public string UTM { get; set; }
        [Name("platform")] public string Platform { get; set; }
        [Name("shopId")] public string ShopId { get; set; }
        [Name("shortKey")] public string ShortKey { get; set; }
    }

    public class UserAffiliateModel
    {
        public string Shortlink { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? CampaignId { get; set; }
    }
}