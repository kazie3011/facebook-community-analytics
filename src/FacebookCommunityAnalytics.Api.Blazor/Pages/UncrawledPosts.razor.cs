using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazored.Localisation.Services;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class UncrawledPosts : BlazorComponentBase
    {
        private int PageSize { get; set; } = 10;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        //private Guid EditingPostId { get; set; }
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }
        private IReadOnlyList<UncrawledPostDto> UncrawledPostList { get; set; }
        //private Modal EditPostModal { get; set; }
        //private UncrawledPostUpdateDto EditingPost { get; set; }
        private GetUncrawledPostsInput Filter { get; set; }
        private PostSourceTypeFilter PostSourceTypeFilter { get; set; }
        //private DateTime? editStartDateTime { get; set; }


        public UncrawledPosts()
        {
            //EditingPost = new UncrawledPostUpdateDto();
            Filter = new GetUncrawledPostsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                BrowserDateTime = await GetBrowserDateTime();
                (StartDate, EndDate) = await GetDefaultDate();
                _dateRanges = await GetDateRangePicker();
                await DoSearch();
            }
        }

        private async Task DoSearch()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                (Filter.UpdatedAtMin, Filter.UpdatedAtMax) = GetDateTimeForApi(StartDate, EndDate);
            }
            if (Filter.FilterText.IsNotNullOrEmpty())
            {
                PostSourceTypeFilter = PostSourceTypeFilter.NoSelect;
                Filter.UpdatedAtMin = null;
                Filter.UpdatedAtMax = null;
                (StartDate, EndDate) = await GetDefaultDate();
            }

            var filter = Filter.Clone();
            filter.PostSourceType = PostSourceTypeFilter == PostSourceTypeFilter.NoSelect ? null : Enum.Parse<PostSourceType>(PostSourceTypeFilter.ToString());
            
            var result = await UncrawledPostsAppService.GetListAsync(filter);
            UncrawledPostList = result.Items;
            TotalCount = (int) result.TotalCount;
            StateHasChanged();
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UncrawledPostDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await DoSearch();
        }


        // private async Task OpenEditPostModal(UncrawledPostDto input)
        // {
        //     EditingPostId = input.Id;
        //     EditingPost = ObjectMapper.Map<UncrawledPostDto, UncrawledPostUpdateDto>(input);
        //     EditPostModal.Show();
        //     editStartDateTime = await ConvertUniversalToBrowserDateTime(EditingPost.UpdatedAt);
        // }

        private async Task DeletePostModal(UncrawledPostDto input)
        {
            var resultConfirm = await _uiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (resultConfirm)
            {
                await UncrawledPostsAppService.HardDeleteAsync(input.Id);
                await DoSearch();
            }
        }

        private void StartDateChanged(DateTimeOffset? offset)
        {
            StartDate = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            EndDate = offset;
        }

        private async Task OnPostStatus_Changed(PostSourceTypeFilter value)
        {
            PostSourceTypeFilter = value;
            await OnSelectFilter();
        }
        
        private async Task OnSelectFilter()
        {
            Filter.FilterText = string.Empty;
            await DoSearch();
        }


        // private void CloseEditPostModal()
        // {
        //     EditPostModal.Hide();
        // }
        //
        // private async Task UpdatePostAsyn()
        // {
        //     await UncrawledPostsAppService.UpdateAsync(EditingPostId, EditingPost);
        //     await GetUncrawledPostAsync();
        //     EditPostModal.Hide();
        // }

        // private async Task EditStartDateTimeChange(DateTime? value)
        // {
        //     if (value != null)
        //     {
        //         editStartDateTime = value;
        //         EditingPost.UpdatedAt = await ConvertBrowserToUniversalDateTime(editStartDateTime.Value);
        //     }
        // }
    }
}