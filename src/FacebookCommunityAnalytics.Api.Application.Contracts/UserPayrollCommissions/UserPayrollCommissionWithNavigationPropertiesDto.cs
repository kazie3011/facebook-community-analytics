using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;

using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public class UserPayrollCommissionWithNavigationPropertiesDto
    {
        public UserPayrollCommissionDto UserPayrollCommission { get; set; }

        public AppUserDto AppUser { get; set; }
        public PayrollDto Payroll { get; set; }

    }
}