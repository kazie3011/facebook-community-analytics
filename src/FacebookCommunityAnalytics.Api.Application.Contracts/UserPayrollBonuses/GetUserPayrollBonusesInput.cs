using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public class GetUserPayrollBonusesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public PayrollBonusType? PayrollBonusType { get; set; }
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public string Description { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }

        public GetUserPayrollBonusesInput()
        {

        }
    }
}