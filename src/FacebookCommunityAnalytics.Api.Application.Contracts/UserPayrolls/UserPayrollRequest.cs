using System;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class UserPayrollRequest
    {
        public string UserCode { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
}
