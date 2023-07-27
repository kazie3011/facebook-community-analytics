using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public class UserPayrollCommissionDto : FullAuditedEntityDto<Guid>
    {
        public string OrganizationId { get; set; }
        public string Description { get; set; }
        public PayrollCommissionType PayrollCommissionType { get; set; }
        public double PayrollCommission { get; set; }
        public decimal Amount { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }
    }
}