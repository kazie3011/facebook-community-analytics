using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazored.Localisation.Services;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.Statistics;
using FluentDateTime;
using FluentDateTimeOffset;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Volo.Abp;
using Volo.Abp.Http.Client;

namespace FacebookCommunityAnalytics.Api.Blazor.Shared
{
    public enum BlazorComponentBaseActionType
    {
        Create,
        Update,
        Delete,
        Get,
        GetList
    }

    public abstract class BlazorComponentBase : ApiComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IBrowserDateTimeProvider BrowserDateTimeProvider { get; set; }
        public GlobalConfiguration GlobalConfiguration { get; set; }
        protected IBrowserDateTime BrowserDateTime { get; set; }
        public IStringLocalizer<ApiDomainResource> LD { get; set; }

        protected async Task<bool> Invoke(Func<Task> action, IStringLocalizer L, bool showSuccessMessage = false, BlazorComponentBaseActionType actionType = BlazorComponentBaseActionType.Create)
        {
            try
            {
                await action();

                if (showSuccessMessage)
                {
                    switch (actionType)
                    {
                        case BlazorComponentBaseActionType.Create:
                            await Notify.Success(L["BlazorComponentBaseActionType.Create"]);
                            break;
                        case BlazorComponentBaseActionType.Update:
                            await Notify.Success(L["BlazorComponentBaseActionType.Update"]);
                            break;
                        case BlazorComponentBaseActionType.Delete:
                            await Notify.Success(L["BlazorComponentBaseActionType.Delete"]);
                            break;
                        case BlazorComponentBaseActionType.Get:
                            await Notify.Success(L["BlazorComponentBaseActionType.Get"]);
                            break;
                        case BlazorComponentBaseActionType.GetList:
                            await Notify.Success(L["BlazorComponentBaseActionType.GetList"]);
                            break;
                        default:
                            await Notify.Success(L["BlazorComponentBaseActionType.Success"]);
                            break;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                if (exception is ApiException apiException)
                {
                    await Message.Error(apiException.Message);
                }
                else if (exception is AbpRemoteCallException abpRemoteCallException)
                {
                    if (abpRemoteCallException.Error?.ValidationErrors.IsNotNullOrEmpty() ?? false)
                    {
                        foreach (var ex in abpRemoteCallException.Error.ValidationErrors)
                        {
                            await Message.Error(ex.Message);
                        }
                    }
                    else
                    {
                        await Message.Error(exception.Message);
                    }
                }else 
                {
                    await Message.Error(exception.Message);
                }

                return false;
            }
        }

        protected bool Invoke(Action action)
        {
            return Invoke(action);
        }

        // TODOO T.Anh: Vu Nguyen - review this method, can we get rid of timeZoneOffsetInMinutes?
        protected async ValueTask<DateTime> GetUTCDateTime(DateTime dateTime)
        {
            int offsetInMinutes = await JsRuntime.InvokeAsync<int>("timeZoneOffsetInMinutes");

            return dateTime.AddMinutes(offsetInMinutes);
        }

        protected async ValueTask<int> GetOffsetInMinutes()
        {
            var browserDateTime = await GetBrowserDateTime();
            var offset = browserDateTime.LocalTimeZoneInfo.BaseUtcOffset.TotalMinutes;
            return (int) offset;
        }

        #region ROLE CHECKS

        
        protected bool IsPartnerRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Partner);
        }

        protected bool IsPartnerTool()
        {
            return GlobalConfiguration.PartnerConfiguration.IsPartnerTool;
        }

        protected bool IsAdminRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Admin);
        }

        protected bool IsDirectorRole()
        {
            return IsAdminRole() || CurrentUser.IsInRole(RoleConsts.Director);
        }

        protected bool IsManagerRole()
        {
            return IsDirectorRole() || CurrentUser.IsInRole(RoleConsts.Manager);
        }

        protected bool IsSupervisorRole()
        {
            return IsManagerRole() || CurrentUser.IsInRole(RoleConsts.Supervisor);
        }

        protected bool IsNotManagerRole()
        {
            return !IsManagerRole();
        }

        protected bool IsSaleAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Sale);
        }

        protected bool IsCampAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.CampaignViewer) || CurrentUser.IsInRole(RoleConsts.CampaignCreator);
        }

        protected bool IsSaleAdminRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.SaleAdmin);
        }

        protected bool IsLeaderRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Leader);
        }

        protected bool IsTikTokRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Tiktok);
        }
        
        protected bool IsNotLeaderRole()
        {
            return !IsLeaderRole();
        }

        protected bool IsStaffRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Staff);
        }


        #endregion
        
        
        protected async Task LogAsync(string message)
        {
            await JsRuntime.InvokeVoidAsync("console.log", message);
        }

        protected async Task InitPage(string title = null)
        {
            if (title.IsNotNullOrEmpty())
            {
                await JsRuntime.InvokeVoidAsync("setTitle", title);
            }
            await JsRuntime.InvokeVoidAsync("HiddenMenuOnMobile");
        }
        
        protected async Task<IBrowserDateTime> GetBrowserDateTime()
        {
            var browserDateTime = await BrowserDateTimeProvider.GetInstance();
            return browserDateTime;
        }
        
        protected async Task<DateTime?> ConvertToBrowserDateTime(DateTime? dateTime)
        {
            if (dateTime is null) return null;
            
            return await ConvertToBrowserDateTime(dateTime.Value);
        }

        protected async Task<DateTime> ConvertToBrowserDateTime(DateTime dateTime)
        {
            var browserDateTime = await GetBrowserDateTime();
            var localTimeZoneInfo = browserDateTime.LocalTimeZoneInfo;
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, localTimeZoneInfo);

            return localDateTime;
        }
        
        protected async Task<string> ConvertToBrowserDateString(DateTime dateTime)
        {
            var localDateTime = await ConvertToBrowserDateTime(dateTime);
            return localDateTime.Humanize();
        }
        
        protected async Task<Dictionary<string, DateRange>> GetDateRangePicker()
        {
            var browserDateTime = await GetBrowserDateTime();
            var dateTime = DateTime.SpecifyKind(browserDateTime.Now.Date, kind: DateTimeKind.Unspecified);
    
            var nowDateOffset = new DateTimeOffset(dateTime,browserDateTime.LocalTimeZoneInfo.BaseUtcOffset);
            var (payrollFromTime, payrollToTime) = GetPayrollDateTime(nowDateOffset.Year, nowDateOffset.Month);
            
            var payrollFromTimeOffset = new DateTimeOffset(payrollFromTime.Add(browserDateTime.LocalTimeZoneInfo.BaseUtcOffset), browserDateTime.LocalTimeZoneInfo.BaseUtcOffset);
            var payrollEndTimeOffset = new DateTimeOffset(payrollToTime.Add(browserDateTime.LocalTimeZoneInfo.BaseUtcOffset), browserDateTime.LocalTimeZoneInfo.BaseUtcOffset);
            
            var dateRanges = new Dictionary<string, DateRange>
            {
                {
                    L[$"{RelativeDateTimeRange.Today.GetDescriptionOrName()}"], new DateRange()
                    {
                        Start = nowDateOffset,
                        End = nowDateOffset.Add(new TimeSpan(23, 59, 59))
                    }
                },
                {
                    L[$"{RelativeDateTimeRange.Yesterday.GetDescriptionOrName()}"], new DateRange()
                    {
                        Start = nowDateOffset.AddDays(-1),
                        End = nowDateOffset.AddDays(-1).Add(new TimeSpan(23, 59, 59))
                    }
                },
                {
                    L[$"{RelativeDateTimeRange.Last7Days.GetDescriptionOrName()}"], new DateRange()
                    {
                        Start = nowDateOffset.AddDays(-7),
                        End = nowDateOffset.Add(new TimeSpan(23, 59, 59))
                    }
                },
                {
                    L[$"{RelativeDateTimeRange.ThisMonth.GetDescriptionOrName()}"], new DateRange()
                    {
                        Start = nowDateOffset.FirstDayOfMonth(),
                        End = nowDateOffset.LastDayOfMonth().Add(new TimeSpan(23, 59, 59))
                    }
                }
            };
            
            if (!IsPartnerTool())
            {
                dateRanges.Add(L[$"{RelativeDateTimeRange.PayrollCircle.GetDescriptionOrName()}"],
                    new DateRange()
                    {
                        Start = payrollFromTimeOffset,
                        End = payrollEndTimeOffset,
                    });
            }
            
            dateRanges.Add(L[$"{RelativeDateTimeRange.LastYear.GetDescriptionOrName()}"],
                new DateRange
                {
                    Start = nowDateOffset.AddYears(-1).Date,
                    End = nowDateOffset.Add(new TimeSpan(23, 59, 59)),
                });

            return dateRanges;
        }

        protected KeyValuePair<DateTime, DateTime> GetPayrollDateTime(int year, int month)
        {
            var date = DateTime.SpecifyKind(new DateTime(year, month, GlobalConfiguration.GlobalPayrollConfiguration.PayrollStartDay , 0, 0, 0), DateTimeKind.Unspecified);
            var to = DateTime.SpecifyKind(new DateTime(year, month, GlobalConfiguration.GlobalPayrollConfiguration.PayrollEndDay, 23, 59, 59), DateTimeKind.Unspecified);

            var from = date.AddMonths(-1);
            
            var fromDateTime = from.AddHours(GlobalConfiguration.GlobalPayrollConfiguration.PayrollTimeZone);
            var toDateTime = to.AddHours(GlobalConfiguration.GlobalPayrollConfiguration.PayrollTimeZone);

            return new KeyValuePair<DateTime, DateTime>(fromDateTime, toDateTime);
        }

        // protected async Task<KeyValuePair<DateTimeOffset, DateTimeOffset>> GetDefaultDate()
        // {
        //     var browserDateTime = await GetBrowserDateTime();
        //     var fromDate = browserDateTime.Now.Date;
        //     var endDate = fromDate.Date.Add(new TimeSpan(23, 59, 59));
        //     
        //     return new KeyValuePair<DateTimeOffset, DateTimeOffset>(fromDate, endDate);
        // }
        
        protected async Task<KeyValuePair<DateTimeOffset, DateTimeOffset>> GetDefaultDate()
        {
            var browserDateTime = await GetBrowserDateTime();
            var fromDate = DateTime.SpecifyKind(browserDateTime.Now.Date, kind: DateTimeKind.Unspecified);
            var endDate = fromDate.Date.Add(new TimeSpan(23, 59, 59));
    
            var fromDateOffset = new DateTimeOffset(fromDate,browserDateTime.LocalTimeZoneInfo.BaseUtcOffset);
            var toDateOffset = new DateTimeOffset(endDate,browserDateTime.LocalTimeZoneInfo.BaseUtcOffset);
            return new KeyValuePair<DateTimeOffset, DateTimeOffset>(fromDateOffset, toDateOffset);
        }
        protected async Task<KeyValuePair<DateTimeOffset, DateTimeOffset>> GetDefaultMonthDate()
        {
            var (startDate, endDate) = await GetDefaultDate();
            startDate = startDate.AddMonths(-1);

            return new KeyValuePair<DateTimeOffset, DateTimeOffset>(startDate, endDate);
        }
        protected async Task<KeyValuePair<DateTimeOffset, DateTimeOffset>> GetDefaultWeekDate()
        {
            var (startDate, endDate) = await GetDefaultDate();
            startDate = startDate.AddDays(-7);
            return new KeyValuePair<DateTimeOffset, DateTimeOffset>(startDate, endDate);
        }
        
        protected async Task<KeyValuePair<DateTimeOffset, DateTimeOffset>> GetYesterday()
        {
            var (startDate, endDate) = await GetDefaultDate();
            return new KeyValuePair<DateTimeOffset, DateTimeOffset>(startDate.AddDays(-1), endDate.AddDays(-1));
        }

        protected KeyValuePair<DateTime?, DateTime?> GetDateTimeForApi(DateTimeOffset? fromDateOffset, DateTimeOffset? toDateTimeOffset)
        {
            if (!fromDateOffset.HasValue || !toDateTimeOffset.HasValue) return new KeyValuePair<DateTime?, DateTime?>(null, null);

            var fromDateTime = fromDateOffset.Value.UtcDateTime;
            var toDateTime = toDateTimeOffset.Value.UtcDateTime;

            // todoo vu: need to add 23h ???

            return new KeyValuePair<DateTime?, DateTime?>(fromDateTime, toDateTime);
        }


        protected async Task<DateTime> ConvertUniversalToBrowserDateTime(DateTime utcDateTime)
        {
            var browserDateTime = await GetBrowserDateTime();
            return DateTime.SpecifyKind(utcDateTime.AddFluentTimeSpan(browserDateTime.LocalTimeZoneInfo.BaseUtcOffset), DateTimeKind.Unspecified);
            
            // utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            //
            // if (utcDateTime.Kind == DateTimeKind.Utc)
            // {
            //     var browserDateTime = await GetBrowserDateTime();
            //     return DateTime.SpecifyKind(utcDateTime.AddFluentTimeSpan(browserDateTime.LocalTimeZoneInfo.BaseUtcOffset), DateTimeKind.Unspecified);
            // }
            //
            // return utcDateTime;
        }

        protected async Task<DateTime> ConvertBrowserToUniversalDateTime(DateTime browserTime)
        {
            if (browserTime.Kind == DateTimeKind.Unspecified)
            {
                var browserDateTime = await GetBrowserDateTime();
                return DateTime.SpecifyKind(browserTime.AddFluentTimeSpan(-browserDateTime.LocalTimeZoneInfo.BaseUtcOffset), DateTimeKind.Utc);
            }

            return browserTime;
        }

        protected async Task<KeyValuePair<DateTime?, DateTime?>> GetBrowserDateTimeCurrentMonth()
        {
            var browserDateTime = await GetBrowserDateTime();
            var now = DateTime.UtcNow;
            var from = new DateTimeOffset
            (
                now.Year,
                now.Month,
                1,
                0,
                0,
                0,
                browserDateTime.LocalTimeZoneInfo.BaseUtcOffset
            );
            var to = from.AddMonths(1).AddTicks(-1);
            return GetDateTimeForApi(from, to);
        }
        
        protected string GetUrlFromChannelId(string channelId)
        {
            return $"https://www.tiktok.com/@{channelId}";
        }
    }
}