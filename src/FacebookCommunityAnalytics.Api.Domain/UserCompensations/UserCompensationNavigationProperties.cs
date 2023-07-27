using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class UserCompensationNavigationProperties
    {
        public Payroll Payroll { get; set; }
        public UserCompensation UserCompensation { get; set; }
        public AppUser AppUser { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}