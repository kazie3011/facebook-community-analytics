using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public class UserPayrollCommissionUpdateDto
    {
        public string OrganizationId { get; set; }
        public string Description { get; set; }
        [Required]
        public PayrollCommissionType PayrollCommissionType { get; set; }
        public double PayrollCommission { get; set; }
        public decimal Amount { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }
    }
}