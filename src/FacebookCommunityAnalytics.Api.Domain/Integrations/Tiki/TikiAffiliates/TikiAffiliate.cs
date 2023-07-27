using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates
{
    public class TikiAffiliate : AuditedEntity<Guid>
    {
        public string CommunityFid { get; set; }
        public string PartnerId { get; set; }
        public string CampaignId { get; set; }
        public string UserCode { get; set; }
        public string Shortlink { get; set; }
        public string Link { get; set; }
        public string AffiliateUrl { get; set; }
    }

    public class TikiAffiliateStat : Entity<Guid>
    {
        public string Shortlink { get; set; }
        public int Click { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}