using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Users
{
    public class ApiUserDetailsRequest
    {
        public List<Guid> UserIds { get; set; }
        public bool? GetForPayrollCalculation { get; set; }
        public bool? GetTeamUsers { get; set; }
        public bool? GetSystemUsers { get; set; }
        public bool? GetActiveUsers { get; set; }
        public Guid? TeamId { get; set; }

        public ApiUserDetailsRequest()
        {
            UserIds = new List<Guid>();
        }
    }
}