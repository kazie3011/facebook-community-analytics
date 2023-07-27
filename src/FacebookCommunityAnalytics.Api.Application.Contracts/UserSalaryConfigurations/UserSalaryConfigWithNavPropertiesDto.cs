using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public class UserSalaryConfigWithNavPropertiesDto
    {
        public AppUserDto AppUser { get; set; }
        public OrganizationUnitDto Team { get; set; }
        public UserSalaryConfigurationDto UserSalaryConfiguration { get; set; }
    }
}