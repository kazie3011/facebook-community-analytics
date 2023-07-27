using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class UserBonusConfig : FullAuditedEntity<Guid>
    {
        public BonusType BonusType { get; set; }
        public decimal BonusAmount { get; set; }
    }
}