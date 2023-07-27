using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public class SchedulePostWithNavigationPropertiesDto
    {
        public ScheduledPostDto ScheduledPost { get; set; }
        public CategoryDto Category { get; set; }
        public AppUserDto AppUser { get; set; }
        public UserInfoDto AppUserInfo { get; set; }
        public List<GroupDto> Groups { get; set; }
    }
}
