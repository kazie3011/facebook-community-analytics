using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public class UserPayrollBonusUpdateDto
    {
        [Required]
        public PayrollBonusType PayrollBonusType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }
    }
}