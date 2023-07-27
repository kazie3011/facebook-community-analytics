using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class GetContractsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        
        public  Guid? SalePersonId { get; set; }
        public Guid? CampaignId { get; set; }
        public List<Guid> PartnerIds { get; set; }

        public ContractStatus? ContractStatus { get; set; }
        public ContractPaymentStatus? ContractPaymentStatus { get; set; }
        public ContractServiceType ContractServiceType { get; set; }
        public ContractType ContractType { get; set; }
        public PaymentChannel PaymentChannel { get; set; }
        
        public  int PostCount { get; set; }
        public  int VideoCount { get; set; }
        
        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        public DateTime? SignedAtMin { get; set; }
        public DateTime? SignedAtMax { get; set; }
    }
}