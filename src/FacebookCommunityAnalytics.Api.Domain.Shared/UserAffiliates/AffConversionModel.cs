namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class AffConversionModel
    {
        public int ClickCount { get; set; }
        public int ConversionCount { get; set; }
        public decimal ConversionAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal CommissionBonusAmount { get; set; }
    }
}