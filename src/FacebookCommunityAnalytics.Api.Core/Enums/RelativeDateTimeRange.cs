namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum RelativeDateTimeRange
    {
        Unknown = 0,
        Today = 10,
        Yesterday = 11,
        Last7Days = 20,
        ThisMonth = 31,
        PayrollCircle = 41,
        LastYear = 50
    }

    // public static class RelativeDateTimeRangeExtensions
    // {
    //     public static DateTime? ToDateTime(this RelativeDateTimeRange range)
    //     {
    //         var utcNow = DateTime.UtcNow;
    //
    //         var dateTime = utcNow;
    //         switch (range)
    //         {
    //             case RelativeDateTimeRange.Unknown:
    //                 return null;
    //             case RelativeDateTimeRange.Today:
    //                 break;
    //             case RelativeDateTimeRange.Yesterday:
    //                 dateTime = dateTime.AddDays(-1);
    //                 break;
    //             case RelativeDateTimeRange.Last7Days:
    //                 dateTime = dateTime.AddDays(-7);
    //                 break;
    //             case RelativeDateTimeRange.LastMonth:
    //                 dateTime = dateTime.AddMonths(-1);
    //                 break;
    //             case RelativeDateTimeRange.Last3Months:
    //                 dateTime = dateTime.AddMonths(-3);
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException(nameof(range), range, null);
    //         }
    //
    //         return dateTime.Date;
    //     }
    // }
}