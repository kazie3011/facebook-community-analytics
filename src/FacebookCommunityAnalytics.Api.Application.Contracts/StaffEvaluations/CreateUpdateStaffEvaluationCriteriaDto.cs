using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class CreateUpdateStaffEvaluationCriteriaDto
    {
        public Guid? TeamId { get; set; }
        
        [Required]
        public string CriteriaName { get; set; }
        public int MaxPoint { get; set; }
        public EvaluationType EvaluationType { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}