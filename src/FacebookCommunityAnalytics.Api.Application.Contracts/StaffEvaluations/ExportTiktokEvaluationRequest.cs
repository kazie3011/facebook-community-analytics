using System;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class ExportTiktokEvaluationRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Guid UserId { get; set; }
        public string TeamName { get; set; }
    }
}