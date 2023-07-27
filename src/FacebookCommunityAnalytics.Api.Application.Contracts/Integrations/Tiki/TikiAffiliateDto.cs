using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiki
{
    public class TikiAffiliateDto : AuditedEntityDto<Guid>
    {
        public string CommunityFid { get; set; }
        public string PartnerId { get; set; }
        public string CampaignId { get; set; }
        public string UserCode { get; set; }
        public string Link { get; set; }
        public string Shortlink { get; set; }
        public string AffiliateUrl { get; set; }
    }
    
    public class TikiAffiliateCreateUpdateDto
    {
        public string CommunityFid { get; set; }
        public string PartnerId { get; set; }
        public string CampaignId { get; set; }
        public string UserCode { get; set; }
        public string Link { get; set; }
        public string Shortlink { get; set; }
        public string AffiliateUrl { get; set; }
    }

    public class GetTikiAffiliateInput : PagedAndSortedResultRequestDto
    {
        
    }
}