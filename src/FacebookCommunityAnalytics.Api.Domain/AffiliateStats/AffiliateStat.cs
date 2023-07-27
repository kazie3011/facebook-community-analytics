using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Diagnostics;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    [DebuggerDisplay("{CreatedAt}")]
    public class AffiliateStat : Entity<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual AffiliateOwnershipType AffiliateOwnershipType { get; set; }

        public virtual int Click { get; set; }

        public virtual int Conversion { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual decimal Commission { get; set; }

        public virtual decimal CommissionBonus { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public AffiliateStat()
        {

        }

        public AffiliateStat(Guid id, AffiliateOwnershipType affiliateOwnershipType, int click, int conversion, decimal amount, decimal commission, decimal commissionBonus, DateTime createdAt)
        {
            Id = id;
            AffiliateOwnershipType = affiliateOwnershipType;
            Click = click;
            Conversion = conversion;
            Amount = amount;
            Commission = commission;
            CommissionBonus = commissionBonus;
            CreatedAt = createdAt;
        }
    }
}