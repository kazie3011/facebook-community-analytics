using System;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public class UserSalaryConfigurationDto
    {
        public Guid? UserId { get; set; }
        public Guid? TeamId { get; set; }
        public UserPosition UserPosition { get; set; }
        public decimal Salary { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
    }
}