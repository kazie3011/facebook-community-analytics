using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class GetStaffEvaluationsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        public Guid? TeamId { get; set; }
        public Guid? AppUserId { get; set; }
        public int Month { get; set; } = DateTime.Now.Month;
        public int Year { get; set; } = DateTime.Now.Year;
        public List<Guid> TeamIds { get; set; }
        public StaffEvaluationStatus? StaffEvaluationStatus { get; set; }
        public bool? IsTikTokEvaluation { get; set; }
    }
}