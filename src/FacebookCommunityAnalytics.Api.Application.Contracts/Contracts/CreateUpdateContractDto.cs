using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using Volo.Abp.Domain.Entities;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class CreateUpdateContractDto
    {
        public Guid? SalePersonId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? PartnerId { get; set; }

        public string ContractCode { get; set; }
        public string Content { get; set; }
        public string PaymentNote { get; set; }

        public ContractStatus ContractStatus { get; set; }
        public ContractPaymentStatus ContractPaymentStatus { get; set; }
        public ContractServiceType ContractServiceType { get; set; }
        public ContractType ContractType { get; set; }
        public PaymentChannel PaymentChannel { get; set; }

        public List<Guid> MediaIds { get; set; }

        public decimal TotalNonVATAmount => TotalValue.ToNonVATAmount(VATPercent);
        public decimal TotalValue { get; set; }
        public decimal PartialPaymentValue { get; set; }
        public decimal RemainingPaymentValue { get; set; }
        public bool IsManualCost { get; set; }
        public decimal Cost { get; set; }
        public List<GroupCostInfoDto> GroupCostInfos { get; set; }
        public decimal VATPercent { get; set; }

        public int PostCount { get; set; }
        public int VideoCount { get; set; }

        public DateTime? SignedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PaymentDueDate { get; set; }

        public CreateUpdateContractDto()
        {
            ContractStatus = ContractStatus.ContractSigned;
            ContractPaymentStatus = ContractPaymentStatus.Unpaid;
            ContractServiceType = ContractServiceType.Unknown;
            PaymentChannel = PaymentChannel.Unknown;
            VATPercent = Convert.ToDecimal(VAT.Percent8);
            GroupCostInfos = new List<GroupCostInfoDto>();
        }
    }
}