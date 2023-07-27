using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public class UserSalaryConfigurationNavigationProperties
    {
        public AppUser AppUser { get; set; }

        public AppOrganizationUnit Team { get; set; }
        public UserSalaryConfiguration UserSalaryConfiguration { get; set; }
    }
}