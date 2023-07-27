using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class EvaluateStaffInput
    {
        public int Month { get; set; } = DateTime.Now.Month;
        public int Year { get; set; } = DateTime.Now.Year;
    }
}