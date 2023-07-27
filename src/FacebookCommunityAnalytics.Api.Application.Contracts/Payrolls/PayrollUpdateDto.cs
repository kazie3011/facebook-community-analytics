using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public class PayrollUpdateDto
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public bool IsCompensation { get; set; }
    }
}