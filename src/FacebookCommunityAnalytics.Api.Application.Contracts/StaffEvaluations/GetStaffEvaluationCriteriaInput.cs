using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class GetStaffEvaluationCriteriaInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        public Guid? TeamId { get; set; }
        public int? MaxPointMin { get; set; }
        public int? MaxPointMax { get; set; }
        public EvaluationType? EvaluationType { get; set; }
    }
}