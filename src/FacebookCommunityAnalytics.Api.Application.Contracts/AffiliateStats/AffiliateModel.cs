namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public class AffiliateModel
    {
        public int Click { get; set; }
        public int Conversion { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public decimal CommissionBonus { get; set; }
    }
}