using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.ScheduledPosts;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    public class ScheduledPostGroupWithNavigationPropertiesDto
    {
        public ScheduledPostGroupDto ScheduledPostGroup { get; set; }
        public ScheduledPostDto ScheduledPost { get; set; }
        public GroupDto Group { get; set; }
    }
}