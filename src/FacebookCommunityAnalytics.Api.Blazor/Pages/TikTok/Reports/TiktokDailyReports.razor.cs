using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Reports
{
    public partial class TiktokDailyReports : BlazorComponentBase
    {
        private bool _rendered;
        private IReadOnlyList<TiktokWithNavigationPropertiesDto> TiktokPosts { get; set; }
        private int PageSize { get; set; } = 25;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private GetTiktoksInputExtend Filter { get; set; }
        private Dictionary<string, DateRange> DateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }
        private bool ShowLoading { get; set; } = true;
        private Modal ExportTikTokModal { get; set; }
        private string ExportTikTokFileName { get; set; }
        private bool isExportAll { get; set; }

        public TiktokDailyReports()
        {
            TiktokPosts = new List<TiktokWithNavigationPropertiesDto>();
            Filter = new GetTiktoksInputExtend {MaxResultCount = 25};
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            DateRanges = await GetDateRangePicker();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                (StartDate, EndDate) = await GetDefaultDate();

                Filter = new GetTiktoksInputExtend
                {
                    MaxResultCount = PageSize,
                    SendEmail = false
                };
                _rendered = true;
                await InitPage($"GDL - {L["Tiktok.PageTitle"].Value}");
                await DoSearch();
            }
        }

        private async Task ExportPostsAsync()
        {
            Filter.Search = Filter.Search?.Trim('/').TrimSafe();
            if (isExportAll)
            {
                Filter.SkipCount = 0;
                Filter.MaxResultCount = int.MaxValue;
            }
            else
            {
                Filter.MaxResultCount = PageSize;
                Filter.SkipCount = (CurrentPage - 1) * Filter.MaxResultCount;
            }

            var filter = Filter.Clone();
            (filter.CreatedDateTimeMin, filter.CreatedDateTimeMax) = GetDateTimeForApi(StartDate, EndDate);
            
            filter.TikTokMcnType = TikTokMCNType.MCNGdl;
            
            var tiktokVideos = await TiktokStatsAppService.GetExportRows(filter);
            foreach (var row in tiktokVideos.Where(x => x.Category.IsNotNullOrWhiteSpace()))
            {
                row.Category = L[$"Enum:GroupCategoryType:{(int) row.Category.ToEnumerable<GroupCategoryType>()}"];
                row.CreatedDateTime = DateTime.Parse(BrowserDateTime.ConvertToBrowserTime(row.CreatedDateTime));
            }

            if (StartDate != null && EndDate != null)
            {
                var title = ExportTikTokFileName.IsNullOrWhiteSpace() ? $"Tiktok_DailyReport_{StartDate.Value.Date:yyyy-MM-dd}_{EndDate.Value.Date:yyyy-MM-dd}" : ExportTikTokFileName;
                var fileName = $"{title}.xlsx";
                
                var excelBytes = ExportHelper.GenerateTiktokExcelBytes(L, tiktokVideos, fileName);

                await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
            }

            ExportTikTokModal.Hide();
        }

        public async Task OnSelectedDateTime()
        {
            Filter.Search = string.Empty;
            await DoSearch();
        }
        public async Task DoSearch()
        {
            if (!_rendered)
            {
                return;
            }

            ShowLoading = true;

            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            
            var filter = Filter.Clone();
            (filter.CreatedDateTimeMin, filter.CreatedDateTimeMax) = GetDateTimeForApi(StartDate, EndDate);
            if (filter.Search.IsNotNullOrEmpty())
            {
                filter.Search = Filter.Search?.Trim('/').TrimSafe();
                filter.CreatedDateTimeMin = null;
                filter.CreatedDateTimeMax = null;
                (StartDate, EndDate) = await GetDefaultDate();
            }

            filter.TikTokMcnType = TikTokMCNType.MCNGdl;
            
            var result = await TiktokStatsAppService.GetListAsync(filter);

            TiktokPosts = result.Items;
            TotalCount = (int) result.TotalCount;

            ShowLoading = false;

            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TiktokWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;

            await DoSearch();
        }

        private string GetVideoUrl(TiktokDto tiktokVideo)
        {
            return tiktokVideo.VideoId.IsNullOrWhiteSpace() ? L["PostUrl"] : $"VideoId: {tiktokVideo.VideoId.MaybeSubstring(7, true)}";
        }

        private void StartDateChanged(DateTimeOffset? offset)
        {
            StartDate = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            EndDate = offset;
        }

        private void OpenExportTikTokModal()
        {
            isExportAll = false;
            ExportTikTokFileName = string.Empty;
            ExportTikTokModal.Show();
        }

        private void CloseExportTikTokModal()
        {
            ExportTikTokModal.Hide();
        }

        private void OnExportAllChange(bool value)
        {
            isExportAll = value;
        }
    }
}