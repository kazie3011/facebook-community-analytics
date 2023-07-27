using System;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class GetAffiliateEvaluationRequest
    {
        public Guid UserId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
}