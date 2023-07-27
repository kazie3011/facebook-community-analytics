using System;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class GetContractEvaluationRequest
    {
        public Guid SalePersonId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
}