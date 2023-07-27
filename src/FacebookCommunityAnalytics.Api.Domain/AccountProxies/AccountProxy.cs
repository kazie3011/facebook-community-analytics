using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Proxies;
using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public class AccountProxy : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? ProxyId { get; set; }
        public bool IsCrawling { get; set; }
        public DateTime CrawledAt { get; set; }
        
        public AccountProxy()
        {

        }

        public AccountProxy(Guid id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}