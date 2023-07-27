using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public class GetUserSalaryConfigurationInput : PagedAndSortedResultRequestDto
    {
        public Guid? UserId { get; set; }
        public Guid? TeamId { get; set; }
    }
}
