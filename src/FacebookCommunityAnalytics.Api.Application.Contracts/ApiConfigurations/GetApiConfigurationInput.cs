using System;
using System.Collections.Generic;
using System.Text;
using FacebookCommunityAnalytics.Api.Configs;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.ApiConfigurations
{
    public class GetApiConfigurationInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
    }

    public class ApiConfigurationDto : FullAuditedEntityDto<Guid>
    {
        public PayrollConfiguration PayrollConfiguration { get; set; }
    }

    public class DateTimeRangeResponse
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
}
