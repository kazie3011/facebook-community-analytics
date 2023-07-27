using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.UserInfos

{
    public class UserProfileDto
    {
        public UserProfileDto()
        {
        }

        public UserInfoDto UserInfo { get; set; }

        public AppUserDto AppUser { get; set; }

        public OrganizationUnitDto Team { get; set; }
    }
}