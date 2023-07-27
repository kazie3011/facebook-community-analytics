using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Configs
{
    public class PayrollConfiguration
    {
        public int StartDay { get; set; }
        public int StartHour { get; set; }
        public int EndDay { get; set; }
        public int EndHour { get; set; }
        public int PayrollTimeZone { get; set; }

        /// <summary>
        /// Branding Group Names. e.g. ghiendalat
        /// </summary>
        public List<string> BrandingGroupFids { get; set; }
        public PayrollSeedingConfig Seeding { get; set; }
        public PayrollAffiliateConfig Affiliate { get; set; }
        public WaveMultiplierConfig WaveMultiplier { get; set; }

        public List<GroupModerator> GroupModerators { get; set; }
    }

    public class PayrollSeedingConfig
    {
        public int MinReactionCount { get; set; }
        public int SeedingGroupExclusive_Threshold_Max { get; set; }
        public int SeedingCopy_Threshold_Max { get; set; }
        public int SeedingVIA_Threshold_Max { get; set; }
        public int MinVIAReactionCount { get; set; }
        public decimal SeedingKPI_Wave { get; set; }
        public decimal SeedingKPI_Count { get; set; }
        public PayrollSeedingWaveConfig Wave { get; set; }
        public PayrollSeedingBonusConfig Bonus { get; set; }
    }

    public class PayrollSeedingWaveConfig
    {
        public decimal SeedingGroupExclusive { get; set; }
        public decimal SeedingCopy { get; set; }
        public decimal SeedingFanPageExclusive { get; set; }
        public decimal SeedingVIA { get; set; }
    }

    public class PayrollSeedingBonusConfig
    {
        public decimal LeaderCommission_Percentage { get; set; }
        public decimal LeaderCommission_Max { get; set; }

        public decimal SeedingTopGroupReactionCount_Level1 { get; set; }
        public decimal SeedingTopGroupReactionCount_Level2 { get; set; }
        
        public decimal SeedingTop1Bonus { get; set; }
        public decimal SeedingTop2Bonus { get; set; }
        public decimal SeedingTop3Bonus { get; set; }
        
        public decimal SeedingAverageReactionTop100Post { get; set; }

        public decimal SeedingTop1PostCount { get; set; }
        public decimal SeedingTop2PostCount { get; set; }
        public decimal SeedingTop3PostCount { get; set; }

        public decimal SeedingTop1ReactionCount { get; set; }
        public decimal SeedingTop2ReactionCount { get; set; }
        public decimal SeedingTop3ReactionCount { get; set; }
        
        public decimal SeedingTop1LikeCount { get; set; }
        public decimal SeedingTop2LikeCount { get; set; }
        public decimal SeedingTop3LikeCount { get; set; }
        
        public decimal SeedingTop1ShareCount { get; set; }
        public decimal SeedingTop2ShareCount { get; set; }
        public decimal SeedingTop3ShareCount { get; set; }
        
        public decimal SeedingTop1CommentCount { get; set; }
        public decimal SeedingTop2CommentCount { get; set; }
        public decimal SeedingTop3CommentCount { get; set; }

        public decimal SeedingTop1VIAPostCount { get; set; }
        public decimal SeedingTop2VIAPostCount { get; set; }
        public decimal SeedingTop3VIAPostCount { get; set; }
        
        
        public decimal SeedingKPI_VIA1_Amount { get; set; }
        public decimal SeedingKPI_VIA1_Count { get; set; }
        
        public decimal SeedingKPI_VIA2_Amount { get; set; }
        public decimal SeedingKPI_VIA2_Count { get; set; }

        public decimal WavePerformance { get; set; }
    }

    public class PayrollAffiliateConfig
    {
        public int MinReactionCount { get; set; }

        public PayrollAffiliateWaveConfig Wave { get; set; }
        public PayrollAffiliateWaveEditorConfig WaveEditor { get; set; }
        public PayrollAffiliateBonusConfig Bonus { get; set; }
    }

    public class PayrollAffiliateWaveConfig
    {
        public decimal AffiliateSingleCopy { get; set; }
        public decimal AffiliateSingleExclusive { get; set; }
        public decimal AffiliateSingleRemake { get; set; }

        public decimal AffiliateMultipleCopy { get; set; }
        public decimal AffiliateMultipleExclusive { get; set; }
        public decimal AffiliateMultipleRemake { get; set; }
        public decimal AffiliateNotAchieved { get; set; }
        public decimal AffiliateHappyDayExclusive { get; set; }
    }

    public class PayrollAffiliateWaveEditorConfig
    {
        public decimal AffiliateSingle { get; set; }
        public decimal AffiliateMultiple { get; set; }
    }

    public class PayrollAffiliateBonusConfig
    {
        public decimal ShopeeAmountPerConversion { get; set; }
        public decimal LazadaAmountPerConversion { get; set; }
        public decimal LeaderCommission_Percentage { get; set; }
        public decimal LeaderCommission_Max { get; set; }
    }

    public class WaveMultiplierConfig
    {
        public decimal BasePromotionRate { get; set; }
        
        public decimal AffiliateMultiplierBase { get; set; }
        public decimal AffiliateMultiplierMax { get; set; }

        public decimal Seeding_Staff_MultiplierBase { get; set; }
        public decimal Seeding_Staff_MultiplierMax { get; set; }
        public int Seeding_Staff_PromotionDayCountBase { get; set; }
        public int Seeding_Staff_PromotionDayCountMax { get; set; }
        public int Affiliate_PromotionDayCountBase { get; set; }
        public decimal Seeding_Leader_MultiplierBase { get; set; }
        public decimal Seeding_Leader_MultiplierMax { get; set; }
        public int Seeding_Leader_PromotionDayCountBase { get; set; }
        public int Seeding_Leader_PromotionDayCountMax { get; set; }

    }

    public class GroupModerator
    {
        public string UserCode { get; set; }
        public Dictionary<string, decimal> GroupWave { get; set; }
    }
}