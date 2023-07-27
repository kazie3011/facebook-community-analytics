using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Statistics;

namespace FacebookCommunityAnalytics.Api.DashBoards
{
    public class ContentChartDto
    {
        public List<DataChartItemDto<decimal?,string>> TotalBarCharts { get; set; } 
        public List<DataChartItemDto<decimal?,string>> PartialPaymentBarCharts { get; set; }
    }
    
    public class ContentChartApiRequest
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public Guid? ContentPersonId { get; set; }
    }
}