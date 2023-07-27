using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class CampaignCreateDto
    {
        public CampaignCreateDto()
        {
            Target = new CampaignTargetDto();
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Hashtags { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public CampaignType CampaignType { get; set; } = ((CampaignType[])Enum.GetValues(typeof(CampaignType)))[0];
        public CampaignStatus Status { get; set; } = ((CampaignStatus[])Enum.GetValues(typeof(CampaignStatus)))[0];
        public CampaignReportType CampaignReportType { get; set; } = CampaignReportType.OneWeek;
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Emails { get; set; }
        public bool IsActive { get; set; }
        public Guid? PartnerId { get; set; }
        
        public CampaignTargetDto Target { get; set; }
    }
}