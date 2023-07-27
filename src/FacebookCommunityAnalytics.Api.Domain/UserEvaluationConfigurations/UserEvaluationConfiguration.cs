using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public class UserEvaluationConfiguration : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? TeamId { get; set; }
        public UserPosition? UserPosition { get; set; }
        public AffiliateEvaluationConfiguration Affiliate { get; set; } = new();

        public SeedingEvaluationConfiguration Seeding { get; set; } = new();
        public SaleEvaluationConfiguration Sale { get; set; } = new();
        public TiktokEvaluationConfiguration Tiktok { get; set; } = new();
        public ContentEvaluationConfiguration Content { get; set; } = new();
        public ContentEvaluationConfiguration SeedingContent { get; set; } = new();
        public ContentEvaluationConfiguration DaNang { get; set; } = new();
        public ContentEvaluationConfiguration Dalat { get; set; } = new();
    }

    public class SaleEvaluationConfiguration
    {
        public decimal? ContractAmountKPI { get; set; }
        public decimal? PaidContractAmountKPI { get; set; }
    }

    public class TiktokEvaluationConfiguration
    {
        /// <summary>
        /// Quantity per month
        /// </summary>
        public int? TiktokVideoPerMonth { get; set; }

        /// <summary>
        /// Average view per video
        /// </summary>
        public int? TiktokAverageVideoView { get; set; }
    }

    public class ContentEvaluationConfiguration
    {
        public int? ContentPostQuantity { get; set; }
        public int? MinimumPostReactions { get; set; }
    }

    public class AffiliateEvaluationConfiguration
    {
        public int? AffiliatePostQuantity { get; set; }
        public int? MinConversionCount { get; set; }
    }

    public class SeedingEvaluationConfiguration
    {
        public int? SeedingPostQuantity { get; set; }
        public int? MinimumPostReactions { get; set; }
    }
}