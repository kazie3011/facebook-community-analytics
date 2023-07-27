using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public class UserPayrollBonusWithNavigationProperties
    {
        public UserPayrollBonus UserPayrollBonus { get; set; }

        public AppUser AppUser { get; set; }
        public Payroll Payroll { get; set; }
        
    }
}