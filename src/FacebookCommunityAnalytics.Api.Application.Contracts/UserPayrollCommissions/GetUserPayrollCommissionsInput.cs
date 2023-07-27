using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public class GetUserPayrollCommissionsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string OrganizationId { get; set; }
        public string Description { get; set; }
        public PayrollCommissionType? PayrollCommissionType { get; set; }
        public double? PayrollCommissionMin { get; set; }
        public double? PayrollCommissionMax { get; set; }
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }

        public GetUserPayrollCommissionsInput()
        {

        }
    }
}