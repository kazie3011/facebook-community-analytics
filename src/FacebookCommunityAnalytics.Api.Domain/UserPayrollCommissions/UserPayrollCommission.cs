using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public class UserPayrollCommission : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string OrganizationId { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }

        public virtual PostContentType PostContentType{ get; set; }

        public virtual PayrollCommissionType PayrollCommissionType { get; set; }

        public virtual double PayrollCommission { get; set; }

        public virtual decimal Amount { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }

        public UserPayrollCommission()
        {

        }

        public UserPayrollCommission(Guid id, string organizationId, string description, PayrollCommissionType payrollCommissionType, double payrollCommission, decimal amount)
        {
            Id = id;
            OrganizationId = organizationId;
            Description = description;
            PayrollCommissionType = payrollCommissionType;
            PayrollCommission = payrollCommission;
            Amount = amount;
        }
    }
}