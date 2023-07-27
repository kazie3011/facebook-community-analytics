using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using JetBrains.Annotations;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class StaffEvaluationWithNavigationProperties
    {
        public StaffEvaluation StaffEvaluation { get; set; }
        [CanBeNull] public AppUser AppUser { get; set; }
        [CanBeNull] public AppOrganizationUnit OrganizationUnit { get; set; } 
        [CanBeNull] public UserInfo Info { get; set; }
        [CanBeNull] public Group Group { get; set; }
    }
}