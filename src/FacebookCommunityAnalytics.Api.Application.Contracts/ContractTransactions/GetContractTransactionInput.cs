using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.ContractTransactions
{
    public class GetContractTransactionInput 
    {
        public string FilterText { get; set; }
        public Guid? ContractId { get; set; }
        public Guid? SalePersonId { get; set; }
        public string Description { get; set; }
        public decimal? PartialPaymentValue { get; set; }
        public DateTime? PaymentDueDateMin { get; set; }
        public DateTime? PaymentDueDateMax { get; set; }
        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
    }
}