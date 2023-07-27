using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;

using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public class UserWaveWithNavigationPropertiesDto
    {
        public UserWaveDto UserWave { get; set; }

        public AppUserDto AppUser { get; set; }
        public PayrollDto Payroll { get; set; }

    }
}