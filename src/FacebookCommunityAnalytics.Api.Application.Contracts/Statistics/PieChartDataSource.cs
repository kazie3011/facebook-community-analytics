using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class PieChartDataSource<T>
    {
        public PieChartDataSource()
        {
            Items = new List<PieChartItem<T>>();
        }

        public List<PieChartItem<T>> Items { get; set; }
    }
}

public class PieChartItem<T>
{
    public string Label { get; set; }
    public T Value { get; set; }
    public T ValuePercent { get; set; }
}