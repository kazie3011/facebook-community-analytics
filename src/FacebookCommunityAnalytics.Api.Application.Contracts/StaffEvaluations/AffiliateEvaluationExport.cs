using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserCompensations;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class AffiliateEvaluationExport
    {
        public List<CompensationAffiliateDto> ShortLinks { get; set; }
        public List<PostDetailExportRow> AffiliatePosts { get; set; }
    }
}