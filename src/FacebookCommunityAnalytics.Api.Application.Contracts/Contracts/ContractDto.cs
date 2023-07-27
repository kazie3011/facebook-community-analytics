using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Medias;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class ContractDto : FullAuditedEntityDto<Guid>
    {
        public ContractDto()
        {
            MediaIds       = new List<Guid>();
            MediasDtos     = new List<MediaDto>();
            CreatedAt      = DateTime.UtcNow;
            GroupCostInfos = new List<GroupCostInfoDto>();
        }
        public  Guid? SalePersonId { get; set; }

        public Guid? CampaignId { get; set; }
        public Guid? PartnerId { get; set; }
        
        public string ContractCode { get; set; }
        public string Content { get; set; }
        public  string PaymentNote { get; set; }

        public ContractStatus ContractStatus { get; set; }
        public ContractPaymentStatus ContractPaymentStatus { get; set; }
        public ContractServiceType ContractServiceType { get; set; }
        public ContractType ContractType { get; set; }
        public PaymentChannel PaymentChannel { get; set; }
        
        public List<Guid> MediaIds { get; set; }
        public List<MediaDto> MediasDtos { get; set; }

        public decimal                TotalNonVATAmount     => TotalValue.ToNonVATAmount(VATPercent);
        public decimal                TotalValue            { get; set; }
        public decimal                PartialPaymentValue   { get; set; }
        public decimal                RemainingPaymentValue { get; set; }
        public bool                   IsManualCost          { get; set; }
        public decimal                Cost                  { get; set; }
        public List<GroupCostInfoDto> GroupCostInfos        { get; set;}
        public decimal                VATPercent            { get; set; }

        public  int PostCount { get; set; }
        public  int VideoCount { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public DateTime? SignedAt { get; set; }
        public DateTime? PaymentDueDate { get; set; }
    }
    
    public class GroupCostInfoDto
    {
        public Guid    Id   { get; set; }
        public decimal Cost { get; set; }
    }
}