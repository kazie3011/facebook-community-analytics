using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.UserAffiliates;

namespace FacebookCommunityAnalytics.Api.AppUserAffiliateStats
{
    public class UserAffiliateStatCreateAndUpdateDto
    {
        public Guid AppUserId { get; set; }
        public AffConversionModel AffConversionModel { get; set; }
        public AffiliateOwnershipType AffiliateOwnershipType { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserAffiliateStatCreateAndUpdateDto ()
        {
            AffConversionModel = new AffConversionModel();
        }
    }
}