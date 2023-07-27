using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using FacebookCommunityAnalytics.Api.Blazor.Models;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class AffReports
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();

        protected PageToolbar Toolbar { get; } = new();

        // private IReadOnlyList<UserAffiliateWithNavigationPropertiesDto> UserAffiliateList { get; set; }
        private int PageSize { get; set; } = 100;
        private int TotalCount { get; set; }
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }

        private DateTimeOffset? _fromDateTime;
        private DateTimeOffset? _toDateTime;

        private UserAffSummaryApiResponse _userAffSummary;

        private bool _showChart = false;
        private UserAffSummaryApiRequest _filter { get; set; }
        private UserAffiliateHasConversionFilter _filterHasConversion { get; set; } = UserAffiliateHasConversionFilter.NoSelect;
        private Dictionary<string, DateRange> _dateRanges { get; set; }

        public AffReports()
        {
            _userAffSummary = new UserAffSummaryApiResponse();
            _filter = new UserAffSummaryApiRequest();
        }

        protected override async Task OnInitializedAsync()
        {
            //await SetToolbarItemsAsync();
            BrowserDateTime = await GetBrowserDateTime();
            _dateRanges = await GetDateRangePicker();
            await SetBreadcrumbItemsAsync();
            await InitDataReport();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                (_fromDateTime, _toDateTime) = await GetDefaultMonthDate();
                await InitPage($"GDL - {L["Aff.Reports.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Aff.Reports.Title"], "/affreport"));
            return ValueTask.CompletedTask;
        }

        private async Task InitDataReport()
        {
            _showChart = false;
            await DoSearch();
            _showChart = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task DoSearch()
        {
            if (_fromDateTime.HasValue && _toDateTime.HasValue)
            {
                var filter = _filter.Clone();
                
                (filter.FromDateTime, filter.ToDateTime) = GetDateTimeForApi(_fromDateTime, _toDateTime);

                _userAffSummary = await _userAffiliateAppService.GetUserAffiliateSummary(filter);
            }
            // else
            // {
            //     await Message.Warn(L["SelectDateTimeRange"]);
            // }
        }

        private void StartDateChanged(DateTimeOffset? offset)
        {
            _fromDateTime = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            _toDateTime = offset;
        }
    }
}