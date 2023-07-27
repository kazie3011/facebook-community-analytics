using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class GetUserPayrollsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Code { get; set; }
        public string OrganizationId { get; set; }
        public ContentRoleType? ContentRoleType { get; set; }
        public double? AffiliateMultiplierMin { get; set; }
        public double? AffiliateMultiplierMax { get; set; }
        public double? SeedingMultiplierMin { get; set; }
        public double? SeedingMultiplierMax { get; set; }
        public string Description { get; set; }
        public decimal? WaveAmountMin { get; set; }
        public decimal? WaveAmountMax { get; set; }
        public decimal? BonusAmountMin { get; set; }
        public decimal? BonusAmountMax { get; set; }
        public decimal? TotalAmountMin { get; set; }
        public decimal? TotalAmountMax { get; set; }
        public Guid? PayrollId { get; set; }
        public Guid? AppUserId { get; set; }

        public GetUserPayrollsInput()
        {

        }
    }
}