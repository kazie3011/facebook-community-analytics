using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public class UserPayrollCommissionWithNavigationProperties
    {
        public UserPayrollCommission UserPayrollCommission { get; set; }

        public AppUser AppUser { get; set; }
        public Payroll Payroll { get; set; }
        
    }
}