using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class GetUserAffiliatesInputExtend : GetUserAffiliatesInput
    {
        public RelativeDateTimeRange RelativeDateTimeRange { get; set; }
        public bool? HasConversion { get; set; }
        public ConversionOwnerFilter ConversionOwnerFilter { get; set; } = ConversionOwnerFilter.Own;
        public Guid? OrgUnitId { get; set; }
        public IEnumerable<string> Shortlinks { get; set; }
    }
}