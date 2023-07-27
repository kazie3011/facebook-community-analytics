using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.ScheduledPostGroups;
using FacebookCommunityAnalytics.Api.ScheduledPosts;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    public class ScheduledPostGroupWithNavigationProperties
    {
        public ScheduledPostGroup ScheduledPostGroup { get; set; }
        public ScheduledPost ScheduledPost { get; set; }
        public Group Group { get; set; }
    }
}