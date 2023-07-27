using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.ScheduledPostGroups;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.LeptonTheme.Management;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public class SchedulePostWithNavigationProperties
    {
        public ScheduledPost ScheduledPost { get; set; }
        public Category Category { get; set; }
        public AppUser AppUser { get; set; }
        public UserInfo AppUserInfo { get; set; }
        
        public List<ScheduledPostGroup> ScheduledPostGroups { get; set; } 
        
        public List<Group> Groups { get; set; }
    }
}
