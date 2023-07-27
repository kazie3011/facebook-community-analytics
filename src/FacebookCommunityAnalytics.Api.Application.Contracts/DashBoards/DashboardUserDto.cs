using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.DashBoards
{
    public class DashboardUserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
    }
}