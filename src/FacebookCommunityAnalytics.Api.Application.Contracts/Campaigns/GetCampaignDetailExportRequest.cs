using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class GetCampaignDetailExportRequest
    {
        public List<PostExportRow> Posts { get; set; }
        public List<UserAffiliateExportRow> Affiliates { get; set; }
        public List<TiktokExportRow> TikToks { get; set; }
    }
}