using System;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class ContractEvaluationExport
    {
        public string ContractCode { get; set; }
        public string Description { get; set; }
        public decimal PartialPaymentValue { get; set; }
        public string CreatedAt { get; set; }
    }
}