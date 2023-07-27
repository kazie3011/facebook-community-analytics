using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.Statistics;
using Microsoft.Extensions.Localization;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api
{
    public abstract class BaseDomainService : DomainService
    {
        public GlobalConfiguration GlobalConfiguration { get; set; }
        public IStringLocalizer<ApiResource> L { get; set; }
        public IStringLocalizer<ApiDomainResource> LD { get; set; }
        public PayrollConfiguration PayrollConfiguration { get; set; }
        public Volo.Abp.ObjectMapping.IObjectMapper ObjectMapper { get; set; }

        protected KeyValuePair<DateTime, DateTime> GetPayrollDateTime(int year, int month)
        {
            var date = DateTime.SpecifyKind 
            ( 
                new DateTime 
                ( 
                    year, 
                    month, 
                    GlobalConfiguration.GlobalPayrollConfiguration.PayrollStartDay, 
                    0, 
                    0, 
                    0 
                ), 
                DateTimeKind.Utc 
            ); 
             
            var toDate = DateTime.SpecifyKind 
            ( 
                new DateTime 
                ( 
                    year, 
                    month, 
                    GlobalConfiguration.GlobalPayrollConfiguration.PayrollEndDay, 
                    23, 
                    59, 
                    59 
                ), 
                DateTimeKind.Utc 
            );

            var fromDate = date.AddMonths(-1);

            var fromDateTime = fromDate.AddHours(GlobalConfiguration.GlobalPayrollConfiguration.PayrollTimeZone);
            var toDateTime = toDate.AddHours(GlobalConfiguration.GlobalPayrollConfiguration.PayrollTimeZone);

            return new KeyValuePair<DateTime, DateTime>(fromDateTime, toDateTime);
        }
        
        protected List<TimeFrame> GetDailyTimeFrames(DateTimeOffset? fromDateOffset, DateTimeOffset? toDateTimeOffset)
        {
            var timeFrames = new List<TimeFrame>();
            if (fromDateOffset.HasValue && toDateTimeOffset.HasValue)
            {
                for (var day = fromDateOffset.Value; day.Date <= toDateTimeOffset.Value.Date; day = day.AddDays(1))
                {
                    timeFrames.Add(new TimeFrame()
                    {
                        Display = day.Date.ToString("dd-MM"),
                        FromDateTime  = day.UtcDateTime,
                        ToDateTime = day.UtcDateTime.AddDays(1).AddTicks(-1)
                    });
                }
            }
            return timeFrames;
        }
    }
}