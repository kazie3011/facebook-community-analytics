using System;
using FacebookCommunityAnalytics.Api.Configs;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace FacebookCommunityAnalytics.Api.ApiConfigurations
{
    public class ApiConfiguration : FullAuditedAggregateRoot<Guid>,IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }
        public PayrollConfiguration PayrollConfiguration { get; set; }
    }
}
