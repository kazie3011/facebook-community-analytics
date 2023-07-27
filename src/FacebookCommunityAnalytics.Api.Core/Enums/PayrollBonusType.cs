using System;

namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum PayrollBonusType
    {
        Unknown = 0,

        // COMMUNITY
        AffiliateTopConversionRate = 1,
        AffiliateTopConversionUnitCount = 2,
        AffiliateTopConversionTotalValue = 3,
        AffiliateTopConversionPerformance = 4,

        [Obsolete] SeedingTop1PostCount = 100,
        [Obsolete] SeedingTop1ReactionCount = 101,
        [Obsolete] SeedingTop1LikeCount = 102,
        [Obsolete] SeedingTop1ShareCount = 103,
        [Obsolete] SeedingTop1CommentCount = 104,
        [Obsolete] SeedingTop1VIAPostCount = 105,

        [Obsolete] SeedingTop2PostCount = 110,
        [Obsolete] SeedingTop2ReactionCount = 111,
        [Obsolete] SeedingTop2LikeCount = 112,
        [Obsolete] SeedingTop2ShareCount = 113,
        [Obsolete] SeedingTop2CommentCount = 114,
        [Obsolete] SeedingTop2VIAPostCount = 115,

        [Obsolete] SeedingTop3PostCount = 120,
        [Obsolete] SeedingTop3ReactionCount = 121,
        [Obsolete] SeedingTop3LikeCount = 122,
        [Obsolete] SeedingTop3ShareCount = 123,
        [Obsolete] SeedingTop3CommentCount = 124,
        [Obsolete] SeedingTop3VIAPostCount = 125,

        SeedingTop1TeamPostAverageReaction = 100,
        SeedingTop1TeamPostViaReaction = 101,
        SeedingTop1TeamReaction = 102,
        SeedingTop1TeamShareCount = 103,
        SeedingTop1TeamCommentCount = 104,

        SeedingTop2TeamPostAverageReaction = 110,
        SeedingTop2TeamPostViaReaction = 111,
        SeedingTop2TeamReaction = 112,
        SeedingTop2TeamShareCount = 113,
        SeedingTop2TeamCommentCount = 114,

        SeedingTop3TeamPostAverageReaction = 120,
        SeedingTop3TeamPostViaReaction = 121,
        SeedingTop3TeamReaction = 122,
        SeedingTop3TeamShareCount = 123,
        SeedingTop3TeamCommentCount = 124,

        AverageReactionTop100Post = 190,
        SeedingTopLevel1GroupReactionCount = 198,
        SeedingTopLevel2GroupReactionCount = 199,

        // LEADER
        PayrollTotal = 200,
        WaveTotal = 201,
        BonusTotal = 202,

        PayrollSeedingTotal = 203,
        WaveSeedingTotal = 204,
        BonusSeedingTotal = 205,

        PayrollAffiliateTotal = 206,
        WaveAffiliateTotal = 207,
        BonusAffiliateTotal = 208,

        PayrollPerformance = 209,
        WavePerformance = 210,
        BonusPerformance = 211,

        PayrollSeedingPerformance = 212,
        WaveSeedingPerformance = 213,
        BonusSeedingPerformance = 214,

        PayrollAffiliatePerformance = 215,
        WaveAffiliatePerformance = 216,
        BonusAffiliatePerformance = 217,

        GroupFollowerKPI = 299,

        // SEEDING
        SeedingKPI_VIA1 = 300, // 1000 reaction => bonus 20k/post
        SeedingKPI_VIA2 = 301, // 500 reaction => bonus 10k/post

        // AFFILIATE
        AffiliateConversion = 400,
        AffiliateConversionEditor = 401,

        AffiliateConversion_Top1 = 500,
        AffiliateConversion_Top2 = 501,
        AffiliateConversion_Top3 = 502,
        AffiliateConversion_Top4_10 = 503,
        AffiliateConversion_BestTeam = 504,
    }
}