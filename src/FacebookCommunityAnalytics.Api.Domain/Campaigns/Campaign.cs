using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Partners;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class  Campaign : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string Code { get; set; }
        
        [CanBeNull]
        public virtual string Hashtags { get; set; }
        
        [CanBeNull]
        public virtual string Keywords { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }
        public CampaignType CampaignType { get; set; }
        public virtual CampaignStatus Status { get; set; }
        public CampaignReportType CampaignReportType { get; set; }
        public virtual DateTime? StartDateTime { get; set; }
        public virtual DateTime? EndDateTime { get; set; }
        public string Emails { get; set; }
        public virtual bool IsActive { get; set; }
        public Guid? PartnerId { get; set; }
        public CampaignTarget Target { get; set; }
        public int FacebookCount { get; set; }
        public int TikTokCount { get; set; }
        public int TotalLike { get; set; }
        public int TotalShare { get; set; }
        public int TotalComment { get; set; }
        public int TotalReaction { get; set; }
        public List<CampaignPrize> CampaignPrizes { get; set; }
        

        public Campaign()
        {
            Target = new CampaignTarget();
            CampaignPrizes = new List<CampaignPrize>();
        }

        public Campaign(Guid id, string name, string code, string hashtags, string description, CampaignStatus status, bool isActive, DateTime? startDateTime = null, DateTime? endDateTime = null)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Code = code;
            Hashtags = hashtags;
            Description = description;
            Status = status;
            IsActive = isActive;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }
    }

    public class CampaignTarget
    {
        // Seeding Target
        public int Seeding_TotalPost { get; set; }
        public int Seeding_TotalReaction { get; set; }
        public int Seeding_AvgReaction { get; set; }

        // D2C/Affiliate
        public int Affiliate_TotalPost { get; set; }
        public int Affiliate_TotalClick { get; set; }
        public int Affiliate_TotalConversion { get; set; }
        public decimal Affiliate_TotalConversionAmount { get; set; }
        public int Affiliate_AvgClick { get; set; }
        
        // Contest 
        public int Contest_TotalPost { get; set; }
        public int Contest_TotalReaction { get; set; }
        public int Contest_AvgReaction { get; set; }
        
        //TikTok
        public int TikTok_TotalVideo { get; set; }
        public int TikTok_TotalView { get; set; }
        
        //PR
        public int PR_TotalPost { get; set; }
        public int PR_TotalReaction { get; set; }
        public int PR_AvgReaction { get; set; }
    }

    public class CampaignPrize
    {
        public Guid? AppUserId { get; set; }
        public Guid? PostId { get; set; }
        public string PostFid  { get; set; }
        public string PrizeName { get; set; }
        public int PrizeNumber { get; set; }
        public string Note { get; set; }
    }
}