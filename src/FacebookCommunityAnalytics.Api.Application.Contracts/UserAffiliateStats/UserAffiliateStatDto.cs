using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.AppUserAffiliateStats
{
    public class UserAffiliateStatDto : AuditedEntityDto<Guid>
    {
        public Guid AppUserId { get; set; }
        public AffConversionModel AffConversionModel { get; set; }
        public AffiliateOwnershipType AffiliateOwnershipType { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserAffiliateStatDto ()
        {
            AffConversionModel = new AffConversionModel();
        }
    }
}