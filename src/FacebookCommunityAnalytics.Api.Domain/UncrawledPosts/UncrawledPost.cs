using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public class UncrawledPost : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string Url { get; set; }
        
        public virtual PostSourceType PostSourceType { get; set; }

        public virtual DateTime UpdatedAt { get; set; }
        
        public UncrawledPost()
        {

        }

        public UncrawledPost(Guid id, string url, DateTime updatedAt)
        {
            Id = id;
            Url = url;
            UpdatedAt = updatedAt;
        }
    }
}