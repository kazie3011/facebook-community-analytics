using System;
using FacebookCommunityAnalytics.Api.Shared;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class GetCampaignLookupDto : LookupRequestDto
    {
        public Guid? PartnerId { get; set; }
    }
}
