using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserAffiliateExportOutput
    {
        public List<UserAffiliateExportRow> Items { get; set; }
    }

    public class UserAffiliateExportRow
    {
        public string Campaign { get; set; }
        public string Group { get; set; }
        public string MarketplaceType { get; set; }
        public string Url { get; set; }
        public string AffiliateUrl { get; set; }
        public string ClickCount { get; set; }
        public string ConversionCount { get; set; }
        public string ConversionAmount { get; set; }
        public string CommissionAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}