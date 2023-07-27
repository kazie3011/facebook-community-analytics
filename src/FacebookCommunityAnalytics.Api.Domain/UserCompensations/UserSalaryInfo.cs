using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class UserSalaryInfo
    {
        public AppUser AppUser { get; set; }
        public UserInfo UserInfo { get; set; }
        public UserCompensation UserCompensation { get; set; }
    }
}