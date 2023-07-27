using System;

namespace FacebookCommunityAnalytics.Api.Blazor.Models
{
    public class ConversionModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ClickCount { get; set; }
        public int ConversionCount { get; set; }
        public decimal ConversionAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal CommissionBonusAmount { get; set; }
    }
}