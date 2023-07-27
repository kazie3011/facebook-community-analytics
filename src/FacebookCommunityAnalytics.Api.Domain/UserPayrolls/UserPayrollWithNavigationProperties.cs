using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class UserPayrollWithNavigationProperties
    {
        public UserPayroll UserPayroll { get; set; }

        public Payroll Payroll { get; set; }
        public AppUser AppUser { get; set; }
        
    }
}