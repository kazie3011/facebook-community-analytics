using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Users;

using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class UserPayrollWithNavigationPropertiesDto
    {
        public UserPayrollDto UserPayroll { get; set; }

        public PayrollDto Payroll { get; set; }
        public AppUserDto AppUser { get; set; }

    }
}