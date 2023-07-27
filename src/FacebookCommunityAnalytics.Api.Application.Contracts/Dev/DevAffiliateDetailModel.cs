using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Dev
{
    public class DevAffiliateDetailModel
    {
        public string UserCode { get; set; }

        public int PostCount { get; set; }
        public List<string> Urls { get; set; }
        public List<string> ShortUrlsFromPosts { get; set; }
        public int ShortUrlsFromPostsCount { get; set; }

        public List<string> ShortUrlsFromShopiness { get; set; }
        public int ShortUrlsFromShopinessCount { get; set; }
        public int ClickCount { get; set; }
        public int ConversionCount { get; set; }
    }
    
    public class TotalAffiliate
    {
        public int Click { get; set; }
        public int Conversion { get; set; }
        public decimal ConversionAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal CommissionBonusAmount { get; set; }
    }
    
    public class TeamAffiliatePayroll
    {
        public string Name { get; set; }
        public int CountMember { get; set; }
        public string AffiliateWave { get; set; }
        public string AffiliateBonus { get; set; }
        public string AffiliateTotal { get; set; }
    }
}