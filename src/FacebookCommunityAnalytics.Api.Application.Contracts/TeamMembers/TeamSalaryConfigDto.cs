using System;

namespace FacebookCommunityAnalytics.Api.TeamMembers
{
    public class TeamSalaryConfigDto
    {
        public Guid TeamId { get; set; }
        public decimal Salary { get; set; }
    }
}