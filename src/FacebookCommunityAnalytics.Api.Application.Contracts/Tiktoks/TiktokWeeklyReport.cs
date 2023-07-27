using System;
using System.Reflection.Emit;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TiktokWeeklyTotalFollowers
    {
        public string MonthName { get; set; }
        public string WeekName { get; set; }
        public int Followers { get; set; }
        public int Views { get; set; }
        public double Videos { get; set; }

        public string ChannelName { get; set; }

        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public bool IsCurrentWeek { get; set; }
        public GroupCategoryType TiktokCategoryType { get; set; }
    }

    public class TiktokWeeklyTotalViews
    {
        public string MonthName { get; set; }
        public string WeekName { get; set; }
        public long TotalViews { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public bool IsCurrentWeek { get; set; }
    }
}
