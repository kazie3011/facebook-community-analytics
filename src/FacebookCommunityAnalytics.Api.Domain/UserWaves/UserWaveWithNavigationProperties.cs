using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public class UserWaveWithNavigationProperties
    {
        public UserWave UserWave { get; set; }

        public AppUser AppUser { get; set; }
        public Payroll Payroll { get; set; }
        
    }
}