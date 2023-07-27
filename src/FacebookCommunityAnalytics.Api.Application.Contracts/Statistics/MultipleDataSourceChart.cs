using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Statistics
{

    public class MultipleDataSourceChart<T>
    {
        public MultipleDataSourceChart()
        {
            MultipleDataCharts = new List<List<DataChartItemDto<T, string>>>();
        }

        public List<string> ChartLabels { get; set; }

        public List<List<DataChartItemDto<T, string>>> MultipleDataCharts { get; set; }
    }

    public class SingleDataSourceChart<T>
    {
        public List<string> ChartLabels { get; set; }

        public List<DataChartItemDto<T, string>> DataCharts { get; set; }
    }
}