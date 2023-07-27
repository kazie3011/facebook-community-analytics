namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class DataChartItemDto<TValue, TType>
    {
        public string Display { get; set; }
        public TValue Value { get; set; }
        public TType Type { get; set; }
    }
}