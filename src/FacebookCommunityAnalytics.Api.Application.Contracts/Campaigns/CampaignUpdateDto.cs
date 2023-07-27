using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class CampaignUpdateDto
    {
        public CampaignUpdateDto()
        {
            Target = new CampaignTargetDto();
        }
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Hashtags { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public CampaignType CampaignType { get; set; }
        public CampaignStatus Status { get; set; }
        public CampaignReportType CampaignReportType { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Emails { get; set; }
        public bool IsActive { get; set; }
        public Guid? PartnerId { get; set; }
        public CampaignTargetDto Target { get; set; }
        public List<CampaignPrizeDto> CampaignPrizes { get; set; }
    }
}