using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.UserAffiliateStats
{
    public class UserAffiliateStat : AuditedEntity<Guid>
    {
        public Guid AppUserId { get; set; }
        public AffConversionModel AffConversionModel { get; set; }
        public AffiliateOwnershipType AffiliateOwnershipType { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserAffiliateStat ()
        {
            AffConversionModel = new AffConversionModel();
        }
    }
}