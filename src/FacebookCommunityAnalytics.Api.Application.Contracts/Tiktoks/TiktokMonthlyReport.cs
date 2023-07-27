using System;
using System.Reflection.Emit;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TikTokMonthlyTotalFollowers
    {
        public string MonthName { get; set; }
        public string TimeTitle { get; set; }
        public int Followers { get; set; }
        public int Views { get; set; }
        public double Videos { get; set; }
        public double Average { get; set; }
        public string ChannelName { get; set; }
        public bool IsCurrentMonth { get; set; }
        public GroupCategoryType TiktokCategoryType { get; set; }
        public long Ticks { get; set; }
    }

    public class TikTokMonthlyTotalViews
    {
        public string MonthName { get; set; }
        public string TimeTitle { get; set; }
        public long TotalViews { get; set; }
        public bool IsCurrentMonth { get; set; }
    }

    public class TikTokMCNReport
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string HashTag { get; set; }
        public int TotalChannel { get; set; }
        public long TotalFollowers { get; set; }
    }
}