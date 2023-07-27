using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public class GetPayrollsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? FromDateTimeMin { get; set; }
        public DateTime? FromDateTimeMax { get; set; }
        public DateTime? ToDateTimeMin { get; set; }
        public DateTime? ToDateTimeMax { get; set; }
        public bool IsCompensation { get; set; }
        public GetPayrollsInput()
        {

        }
    }
}