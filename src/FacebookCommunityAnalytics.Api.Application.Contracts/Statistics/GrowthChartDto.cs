using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class GrowthChartDto
    {
        public GrowthChartDto()
        {
            ChartLabels = new List<string>();
            TotalInteractionsLineCharts = new List<DataChartItemDto<double?, string>>();
            GrowthNumberBarCharts = new List<DataChartItemDto<double, string>>();
        }
        public List<string> ChartLabels { get; set; }
        public List<DataChartItemDto<double?,string>> TotalInteractionsLineCharts { get; set; } 
        public List<DataChartItemDto<double,string>> GrowthNumberBarCharts { get; set; } 
    }
}