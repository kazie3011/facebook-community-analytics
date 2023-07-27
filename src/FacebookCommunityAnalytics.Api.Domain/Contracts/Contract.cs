using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.GroupCosts;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class Contract : FullAuditedEntity<Guid>
    {
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

        public decimal             TotalValue            { get; set; }
        public decimal             PartialPaymentValue   { get; set; }
        public decimal             RemainingPaymentValue { get; set; }
        public bool                IsManualCost    { get; set; }
        public decimal             Cost                  { get; set; }
        public List<GroupCostInfo> GroupCostInfos        { get; set;}
        public decimal             VATPercent            { get; set; }

        public  int PostCount { get; set; }
        public  int VideoCount { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public DateTime? SignedAt { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        
        public Contract()
        {
            MediaIds       = new List<Guid>();
            GroupCostInfos = new List<GroupCostInfo>();
        }
    }

    public class GroupCostInfo
    {
        public Guid Id { get; set; }
        public decimal Cost { get; set; }
    }
}