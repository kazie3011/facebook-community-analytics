using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class UserPayrollUpdateDto
    {
        public string Code { get; set; }
        public string OrganizationId { get; set; }
        [Required]
        public ContentRoleType ContentRoleType { get; set; }
        public double AffiliateMultiplier { get; set; }
        public double SeedingMultiplier { get; set; }
        public string Description { get; set; }
        public decimal WaveAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? PayrollId { get; set; }
        public Guid? AppUserId { get; set; }
    }
}