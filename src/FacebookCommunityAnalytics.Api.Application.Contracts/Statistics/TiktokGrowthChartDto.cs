using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class TiktokGrowthChartDto
    {
        public TiktokGrowthChartDto()
        {
            ChartLabels = new List<string>();
            TotalViewLineCharts = new List<DataChartItemDto<double?, string>>();
            TiktokFollowerGrowths = new List<TiktokFollowerGrowth>();
        }
        public List<string> ChartLabels { get; set; }
        public List<DataChartItemDto<double?,string>> TotalViewLineCharts { get; set; }
        public List<TiktokFollowerGrowth> TiktokFollowerGrowths { get; set; }
    }

    public class TiktokFollowerGrowth
    {
        public string Date { get; set; }
        public decimal Follower { get; set; }
        public decimal FollowerChange { get; set; }
    }
}