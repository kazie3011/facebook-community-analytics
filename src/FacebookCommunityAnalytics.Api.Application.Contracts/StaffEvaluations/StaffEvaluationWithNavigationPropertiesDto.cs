using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.UserInfos;
using JetBrains.Annotations;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class StaffEvaluationWithNavigationPropertiesDto
    {
        public StaffEvaluationDto StaffEvaluation { get; set; }
        [CanBeNull] public AppUserDto AppUser { get; set; }
        [CanBeNull] public OrganizationUnitDto OrganizationUnit { get; set; }
        [CanBeNull] public UserInfoDto Info { get; set; }
        [CanBeNull] public GroupDto Group { get; set; }
    }
}