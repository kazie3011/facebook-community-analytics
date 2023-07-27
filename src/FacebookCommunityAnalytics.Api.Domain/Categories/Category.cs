using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Categories
{
    public class Category : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public Category()
        {

        }

        public Category(Guid id, string name)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
        }
    }
}