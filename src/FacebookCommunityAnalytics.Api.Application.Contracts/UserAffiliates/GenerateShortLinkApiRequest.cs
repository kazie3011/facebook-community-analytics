using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class GenerateShortlinkApiRequest
    {
        public string UserCode { get; set; }
        public string CommunityFid { get; set; }
        public string PartnerId { get; set; }
        public string CampaignId { get; set; }
        public MarketplaceType MarketplaceType { get; set; }
        public AffiliateProviderType AffiliateProviderType { get; set; } = AffiliateProviderType.Unknown;
        public string Link { get; set; }
        public string Shortlink { get; set; }
        public bool IsHappyDay { get; set; }
    }

    public class UserAffDetailApiRequest
    {
        public string UserCode { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
    
    public class UserAffDetailDto
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
}