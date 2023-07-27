using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public class UserEvaluationConfigurationDto
    {
        public Guid Id { get; set; }
        public virtual Guid? OrganizationId { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? TeamId { get; set; }
        public UserPosition? UserPosition { get; set; }
        public AffiliateEvaluationConfigurationDto Affiliate { get; set; } = new();
        public SeedingEvaluationConfigurationDto Seeding { get; set; } = new();
        public SaleEvaluationConfigurationDto Sale { get; set; } = new();
        public TiktokEvaluationConfigurationDto Tiktok { get; set; } = new();

        public ContentEvaluationConfigurationDto Content { get; set; } = new();
        public ContentEvaluationConfigurationDto SeedingContent { get; set; } = new();
        public ContentEvaluationConfigurationDto DaNang { get; set; } = new();
        public ContentEvaluationConfigurationDto Dalat { get; set; } = new();
    }

    public class SaleEvaluationConfigurationDto
    {
        public decimal? ContractAmountKPI { get; set; }
        public decimal? PaidContractAmountKPI { get; set; }
    }

    public class TiktokEvaluationConfigurationDto
    {
        public int? TiktokVideoPerMonth { get; set; }
        public int? TiktokAverageVideoView { get; set; }
    }

    public class ContentEvaluationConfigurationDto
    {
        public int? ContentPostQuantity { get; set; }
        public int? MinimumPostReactions { get; set; }
    }

    public class AffiliateEvaluationConfigurationDto
    {
        public int? AffiliatePostQuantity { get; set; }
        public int? MinConversionCount { get; set; }
    }

    public class SeedingEvaluationConfigurationDto
    {
        public int? SeedingPostQuantity { get; set; }
        public int? MinimumPostReactions { get; set; }
    }
}