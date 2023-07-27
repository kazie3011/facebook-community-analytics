namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum BonusType
    {
        FilterNoSelect=0,
        
        Unknown = 1,
        
        Affiliate = 100,
        AffiliateDoSeeding = 101,
        AffiliateStaff = 102,
        AffiliateGroupLeader = 103,
        
        Seeding = 200,
        SeedingDoAffiliatePost = 201,
        SeedingDoAffiliateConversion = 202,
        
        SeedingContent = 300,
        
        Content = 400,
        
        Sale = 500,
        
        Tiktok = 600,
        
        Evaluation = 700,
        
        Holiday = 800,
        TetHolidayBonus = 801,
        TetHolidayPostBonus = 802,
        TetHolidayConversionBonus = 803,
    }
}