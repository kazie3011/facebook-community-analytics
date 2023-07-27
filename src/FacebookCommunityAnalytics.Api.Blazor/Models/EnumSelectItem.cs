namespace FacebookCommunityAnalytics.Api.Blazor.Models
{
    public class EnumSelectItem<T>
    {
        public string DisplayName { get; set; }
        public T Value { get; set; }
    }
}