using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazored.Localisation.Services;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Http.Client;
using FacebookCommunityAnalytics.Api.Blazor.Shared;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Posts
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        public PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<PostWithNavigationPropertiesDto> PostList { get; set; }
        private int PageSize { get; set; } = 50;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreatePost { get; set; }
        private bool CanEditPost { get; set; }
        private bool CanEditNote { get; set; }
        private bool CanDeletePost { get; set; }
        private PostCreateDto NewPost { get; set; }
        private PostUpdateDto EditingPost { get; set; }
        private PostUpdateNoteDto UpdateNotePostDto { get; set; }
        private Validations NewPostValidations { get; set; }
        private Validations EditingPostValidations { get; set; }
        private Guid EditingPostId { get; set; }
        private Modal CreatePostModal { get; set; }
        private Modal EditPostModal { get; set; }
        private Modal UpdateNotePostModal { get; set; }

        private GetPostsInputExtend Filter { get; set; }
        private PostContentTypeFilter PostContentTypeFilterInput { get; set; } = PostContentTypeFilter.NoSelect;
        private PostIsNotAvailableFilter PostIsNotAvailableFilterInput { get; set; } = PostIsNotAvailableFilter.NoSelect;
        private PostCopyrightTypeFilter PostCopyrightTypeFilterInput { get; set; } = PostCopyrightTypeFilter.NoSelect;
        private PostSourceTypeFilter PostSourceTypeFilterInput { get; set; } = PostSourceTypeFilter.NoSelect;
        private Guid? SelectedCampaignIdFilter;
        private Guid? SelectedPartnerIdFilter;
        
        private DataGridEntityActionsColumn<PostWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> CategoriesNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> GroupsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> AllGroupsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> InstagramGroupsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> CampaignsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> RunningCampaignsLookup { get; set; } = new List<LookupDto<Guid?>>();
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }

        private IList<GroupDto> _groupDtos = new List<GroupDto>();

        private bool _showAdvancedFilter = false;
        private bool _showGroupSelect { get; set; }
        private bool _showLoading { get; set; }

        private bool _rendered;

        public Posts()
        {
            NewPost = new PostCreateDto();
            EditingPost = new PostUpdateDto();
            UpdateNotePostDto = new PostUpdateNoteDto();
            
            Filter = new GetPostsInputExtend
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = null,
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();

            if (!IsPartnerRole()) await GetNullableAppUserLookupAsync("");
            await GetNullableGroupLookupAsync("");
            await GetNullableCategoryLookupAsync("");
            await GetNullablePartnerLookupAsync("");
            await GetNullableCampaignLookupAsync("");
            await GetRunningCampaignsLookup("");

            AllGroupsNullable = (await PostsExtendAppService.GetGroupLookupAsync
            (
                new GroupLookupRequestDto
                {
                    Filter = "",
                }
            )).Items;
            InstagramGroupsNullable = (await PostsExtendAppService.GetGroupLookupAsync
            (
                new GroupLookupRequestDto
                {
                    Filter = "",
                    GroupSourceType = GroupSourceType.Instagram
                }
            )).Items;

            _groupDtos = await GroupExtendAppService.GetListAsync();
            _dateRanges = await GetDateRangePicker();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                (StartDate, EndDate) = await GetDefaultDate();
                _rendered = true;
                await InitPage($"GDL - {L["Post.PageTitle"].Value}");
                await Search();
            }
        }

        private async Task DoSearch()
        {
            if (!_rendered) return;

            _showLoading = true;

            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.Fid = Filter.Fid?.Trim();
            (Filter.CreatedDateTimeMin, Filter.CreatedDateTimeMax) = GetDateTimeForApi(StartDate, EndDate);

            if (Filter.FilterText.IsNotNullOrEmpty())
            {
                Filter.FilterText = Filter.FilterText.Trim('/');
                Filter.FilterText = Filter.FilterText.Trim().ToLower();

                var postSourceType = FacebookHelper.GetPostSourceType(Filter.FilterText);
                switch (postSourceType)
                {
                    case PostSourceType.Group:
                        Filter.FilterText = Filter.FilterText.Replace("posts", "permalink");
                        break;
                    case PostSourceType.Page:
                        break;
                    case PostSourceType.Instagram:
                        break;
                    case PostSourceType.Website:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Filter.RelativeDateTimeRange = RelativeDateTimeRange.Unknown;
                Filter.CreatedDateTimeMin = null;
                Filter.CreatedDateTimeMax = null;

                await ClearSelectedComponent();
            }

            var filter = Filter.Clone();
            filter.PostContentType = PostContentTypeFilterInput == PostContentTypeFilter.NoSelect ? null : Enum.Parse<PostContentType>(PostContentTypeFilterInput.ToString());
            filter.IsNotAvailable = PostIsNotAvailableFilterInput == PostIsNotAvailableFilter.NoSelect ? null : PostIsNotAvailableFilterInput == PostIsNotAvailableFilter.IsAvailable;
            filter.PostCopyrightType = PostCopyrightTypeFilterInput == PostCopyrightTypeFilter.NoSelect ? null : Enum.Parse<PostCopyrightType>(PostCopyrightTypeFilterInput.ToString());
            filter.PostSourceType = PostSourceTypeFilterInput == PostSourceTypeFilter.NoSelect ? null : Enum.Parse<PostSourceType>(PostSourceTypeFilterInput.ToString());
            filter.CampaignId = SelectedCampaignIdFilter == Guid.Empty ? null : SelectedCampaignIdFilter;
            filter.PartnerId = SelectedPartnerIdFilter == Guid.Empty ? null : SelectedPartnerIdFilter;

            var result = await PostsExtendAppService.GetListAsync(filter);
            PostList = result.Items;
            TotalCount = (int) result.TotalCount;

            _showLoading = false;

            await InvokeAsync(StateHasChanged);
        }

        protected virtual async Task Search()
        {
            // todoo ???
            CurrentPage = 1;
            await DoSearch();
        }

        private async Task OnPostContentType_Changed(PostContentTypeFilter value)
        {
            PostContentTypeFilterInput = value;
            await OnSelectFilter();
        }

        private async Task OnCampaignFilter_Changed(Guid? value)
        {
            SelectedCampaignIdFilter = value;
            await OnSelectFilter();
        }
        
        private async Task OnPartnerFilter_Changed(Guid? value)
        {
            SelectedPartnerIdFilter = value;
            await OnSelectFilter();
        }
        
        private async Task OnSelectFilter()
        {
            Filter.FilterText = string.Empty;
            await Search();
        }
    }
}