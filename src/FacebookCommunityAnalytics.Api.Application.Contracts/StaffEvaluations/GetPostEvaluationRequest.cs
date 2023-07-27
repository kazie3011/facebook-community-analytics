using System;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class GetPostEvaluationRequest
    {
        public Guid AppUserId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
}