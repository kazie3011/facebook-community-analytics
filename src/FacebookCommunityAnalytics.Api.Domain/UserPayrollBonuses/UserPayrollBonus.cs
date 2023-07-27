using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public class UserPayrollBonus : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }
        public virtual PayrollBonusType PayrollBonusType { get; set; }
        public virtual decimal Amount { get; set; }
        [CanBeNull]
        public virtual string Description { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }

        public UserPayrollBonus()
        {
        }

        public UserPayrollBonus(Guid id, PayrollBonusType payrollBonusType, decimal amount, string description)
        {
            Id = id;
            PayrollBonusType = payrollBonusType;
            Amount = amount;
            Description = description;
        }
    }
}