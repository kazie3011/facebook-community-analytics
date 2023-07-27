using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public class UserPayrollBonusDto : FullAuditedEntityDto<Guid>
    {
        public PayrollBonusType PayrollBonusType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }
    }
}