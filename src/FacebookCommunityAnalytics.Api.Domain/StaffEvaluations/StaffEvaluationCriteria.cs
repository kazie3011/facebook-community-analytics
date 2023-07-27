using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class StaffEvaluationCriteria : AuditedEntity<Guid>
    {
        public Guid? TeamId { get; set; }
        public string CriteriaName { get; set; }
        public int MaxPoint { get; set; }
        public EvaluationType EvaluationType { get; set; }
        public string Description  { get; set; }
        public string Note   { get; set; }
    }
}