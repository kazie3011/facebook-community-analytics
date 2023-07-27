using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Posts;
using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class CampaignDto : FullAuditedEntityDto<Guid>
    {
        public CampaignDto()
        {
            Target = new CampaignTargetDto();
            Current = new CampaignTargetDto();
            Contracts = new List<ContractDto>();
        }
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
        public CampaignTargetDto Current { get; set; }
        public List<ContractDto> Contracts { get; set; }
        public int FacebookCount { get; set; }
        public int TikTokCount { get; set; }
        public int TotalLike { get; set; }
        public int TotalShare { get; set; }
        public int TotalComment { get; set; }
        public int TotalReaction { get; set; }
        public List<CampaignPrizeDto> CampaignPrizes { get; set; }
        public string QuantityKPIDescription { get; set; }
        public string QualityKPIDescription { get; set; }
        public decimal QuantityKPI { get; set;}
        public decimal QualityKPI { get; set; }
    }

    public class CampaignTargetDto
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
    
    public class CampaignPrizeDto
    {
        public Guid? AppUserId { get; set; }
        public Guid? PostId { get; set; }
        public string Author  { get; set; }
        public string PostFid  { get; set; }
        public string PrizeName { get; set; }
        public int PrizeNumber { get; set; }
        public string Note { get; set; }
        [CanBeNull] public PostDto Post { get; set; }
    }
}