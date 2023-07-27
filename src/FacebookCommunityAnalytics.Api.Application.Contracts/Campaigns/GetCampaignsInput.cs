using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class GetCampaignsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Hashtags { get; set; }
        public string Description { get; set; }
        public CampaignType? CampaignType { get; set; }
        public CampaignStatus? Status { get; set; }
        public CampaignReportType? CampaignReportType { get; set; }
        public DateTime? StartDateTimeMin { get; set; }
        public DateTime? StartDateTimeMax { get; set; }
        public DateTime? EndDateTimeMin { get; set; }
        public DateTime? EndDateTimeMax { get; set; }
        public string Emails { get; set; }
        public bool? IsActive { get; set; }
        public Guid? PartnerId { get; set; }
        public string CurrentUserEmail { get; set; }
        public List<Guid> PartnerIds { get; set; }
        public GetCampaignsInput()
        {
            PartnerIds = new List<Guid>();
        }
    }
}