using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Tiktoks;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule.Components
{
    public partial class HomePagePosts
    {
        
        private IReadOnlyList<PostWithNavigationPropertiesDto> PostListFb { get; set; }
        private int PageSizeFb { get; set; } = 10;
        private int CurrentPageFb { get; set; } = 1;
        private string CurrentSortingFb = "Post.CreatedDateTime DESC";
        private int TotalCountFb { get; set; }
        
        
        private IReadOnlyList<TiktokWithNavigationPropertiesDto> PostListTikTok { get; set; }
        private int PageSizeTikTok { get; set; } = 10;
        private int CurrentPageTikTok { get; set; } = 1;
        private string CurrentSortingTikTok = "Post.CreatedDateTime DESC";
        private int TotalCountTikTok { get; set; }
        
        
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; } = DateTimeOffset.Now.Date.AddDays(-7);
        private DateTimeOffset? EndDate { get; set; } = DateTimeOffset.Now.Date.AddDays(1).AddTicks(-1);
        private string GetPostUrl(PostDto post)
        {
            return post.Fid.IsNullOrWhiteSpace() ? L["PostUrl"] : $"Fid:{post.Fid.Substring(post.Fid.Length - 5)}";
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitDateTimeRangePicker();
                await InvokeAsync(StateHasChanged);
            }
        }
        private async Task OnDataGridFbReadAsync(DataGridReadDataEventArgs<PostWithNavigationPropertiesDto> e)
        {
            // CurrentSortingFb = e.Columns
            //     .Where(c => c.SortDirection != SortDirection.None)
            //     .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            //     .JoinAsString(",");
            CurrentPageFb = e.Page;
            PageSizeFb = e.PageSize;
            await DoSearchFb();
            await InvokeAsync(StateHasChanged);
        }

        private async Task InitDateTimeRangePicker()
        {
            (StartDate, EndDate) = await GetDefaultWeekDate();
            _dateRanges = await GetDateRangePicker();
        }
        
        private string GetVideoUrl(TiktokDto tiktokVideo)
        {
            return tiktokVideo.VideoId.IsNullOrWhiteSpace() ? L["PostUrl"] : $"VideoId: {tiktokVideo.VideoId.MaybeSubstring(7, true)}";
        }
        private async Task OnDataGridTikTokReadAsync(DataGridReadDataEventArgs<TiktokWithNavigationPropertiesDto> e)
        {
            // CurrentSortingTikTok = e.Columns
            //     .Where(c => c.SortDirection != SortDirection.None)
            //     .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            //     .JoinAsString(",");
            CurrentPageTikTok = e.Page;
            PageSizeTikTok = e.PageSize;
            await DoSearchTikTok();
            await InvokeAsync(StateHasChanged);
        }
        
         private async Task DoSearchFb()
         {
             var filter = new GetPostsInputExtend()
             {
                 CreatedDateTimeMin = StartDate.Value.DateTime,
                 CreatedDateTimeMax = EndDate.Value.DateTime,
                 PostSourceTypes = new List<PostSourceType>()
                 {
                     PostSourceType.Group,
                     PostSourceType.Page
                 },
                 Sorting = CurrentSortingFb,
                 SkipCount = (CurrentPageFb - 1) * PageSizeFb,
                 MaxResultCount = PageSizeFb
             };
            var result = await PartnerModuleAppService.GetPostNavs(filter);
            PostListFb = result.Items;
            TotalCountFb = (int) result.TotalCount;
        }
         private async Task DoSearchTikTok()
         {
             var filter = new GetTiktoksInputExtend()
             {
                 CreatedDateTimeMin = StartDate.Value.DateTime,
                 CreatedDateTimeMax = EndDate.Value.DateTime,
                 Sorting = CurrentSortingTikTok,
                 SkipCount = (CurrentPageTikTok - 1) * PageSizeFb,
                 MaxResultCount = PageSizeTikTok
             };
            var result = await PartnerModuleAppService.GetTikToksPaging(filter);
            PostListTikTok = result.Items;
            TotalCountTikTok = (int) result.TotalCount;
        }

         private async Task ReloadDataAsync()
         {
             await DoSearchFb();
             await DoSearchTikTok();
         }
    }
}