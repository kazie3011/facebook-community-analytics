using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class UserCompensationNavigationPropertiesDto
    {
        public PayrollDto Payroll { get; set; }
        public UserCompensationDto UserCompensation { get; set; }
        public AppUserDto AppUser { get; set; }
        public UserInfoDto UserInfo { get; set; }
    }
}