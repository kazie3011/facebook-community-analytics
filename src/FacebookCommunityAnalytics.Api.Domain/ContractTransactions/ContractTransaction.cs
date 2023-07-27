using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Medias;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.ContractTransactions
{
    public class ContractTransaction : AuditedEntity<Guid>
    {
       public Guid ContractId { get; set; }
       public Guid? SalePersonId { get; set; }
       public string Description { get; set; }
       public decimal PartialPaymentValue { get; set; }
       public DateTime? PaymentDueDate { get; set; }
       public DateTime CreatedAt { get; set; }
       public decimal VATPercent { get; set; }
       public decimal Cost { get; set; }
       public List<GroupCostDto> GroupCostInfos { get; set;}
       public bool IsManualCost { get; set; }


       public ContractTransaction()
       {
           CreatedAt  = DateTime.UtcNow;
       }
    }
}