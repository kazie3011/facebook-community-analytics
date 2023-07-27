using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public class GetAffiliateStatsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public AffiliateOwnershipType? AffiliateOwnershipType { get; set; }
        public int? ClickMin { get; set; }
        public int? ClickMax { get; set; }
        public int? ConversionMin { get; set; }
        public int? ConversionMax { get; set; }
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public decimal? CommissionMin { get; set; }
        public decimal? CommissionMax { get; set; }
        public decimal? CommissionBonusMin { get; set; }
        public decimal? CommissionBonusMax { get; set; }
        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        
        public  int? ClientOffsetInMinutes { get; set; } 

        public GetAffiliateStatsInput()
        {

        }
    }
}