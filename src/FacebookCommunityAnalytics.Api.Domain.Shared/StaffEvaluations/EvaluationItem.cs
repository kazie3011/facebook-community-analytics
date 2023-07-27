using System;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class EvaluationItem
    {
        public Guid CriteriaId  { get; set; }
        public int Points  { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}