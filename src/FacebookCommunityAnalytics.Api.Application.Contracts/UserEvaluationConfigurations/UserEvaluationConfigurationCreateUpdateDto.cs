using System;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public class UserEvaluationConfigurationCreateUpdateDto
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
}