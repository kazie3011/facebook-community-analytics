using Hangfire.Dashboard;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var userIdentity = context.GetHttpContext().User.Identity;
            return userIdentity is { IsAuthenticated: true };
        }
    }
}