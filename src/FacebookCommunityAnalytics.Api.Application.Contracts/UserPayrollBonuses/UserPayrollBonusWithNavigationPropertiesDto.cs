using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;

using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public class UserPayrollBonusWithNavigationPropertiesDto
    {
        public UserPayrollBonusDto UserPayrollBonus { get; set; }

        public AppUserDto AppUser { get; set; }
        public PayrollDto Payroll { get; set; }

    }
}