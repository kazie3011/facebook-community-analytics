using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazored.Localisation.Services;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Http.Client;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class PartnerPosts
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
        private Validations NewPostValidations { get; set; }
        private PostUpdateDto EditingPost { get; set; }
        private Validations EditingPostValidations { get; set; }
        private Guid EditingPostId { get; set; }
        private Modal CreatePostModal { get; set; }
        private Modal EditPostModal { get; set; }
        
        private GetPostsInputExtend Filter { get; set; }
        
        private PostContentTypeFilter PostContentTypeFilterInput { get; set; } = PostContentTypeFilter.NoSelect;
        private PostIsNotAvailableFilter PostIsNotAvailableFilterInput { get; set; } = PostIsNotAvailableFilter.NoSelect;
        private PostCopyrightTypeFilter PostCopyrightTypeFilterInput { get; set; } = PostCopyrightTypeFilter.NoSelect;
        private PostSourceTypeFilter PostSourceTypeFilterInput { get; set; } = PostSourceTypeFilter.NoSelect;
        private Guid? SelectedCampaignIdFilter;
        private Guid? SelectedPartnerIdFilter;
        private Modal UpdateNotePostModal { get; set; }

        private PostUpdateNoteDto UpdateNotePostDto { get; set; }
        
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
        
        private IReadOnlyList<GroupDto> _groupDtos = new List<GroupDto>();
        
        private bool _showAdvancedFilter = false;
        private bool _showGroupSelect { get; set; }
        private bool _showLoading { get; set; }
        
        private IBrowserDateTime BrowserDateTime { get; set; }
        
        public PartnerPosts()
        {
            NewPost = new PostCreateDto();
            EditingPost = new PostUpdateDto();
            Filter = new GetPostsInputExtend
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting,
            };
            UpdateNotePostDto = new PostUpdateNoteDto();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            _groupDtos = (await PartnerModuleAppService.GetGroups(new GetGroupsInput()
            {
                MaxResultCount = 1000
            })).Items;
            
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            
            AllGroupsNullable = (await PartnerModuleAppService.GetGroupLookup
            (
                new GroupLookupRequestDto
                {
                    Filter = "",
                }
            ));
            
            InstagramGroupsNullable = (await PartnerModuleAppService.GetGroupLookup
            (
                new GroupLookupRequestDto
                {
                    Filter = "",
                    GroupSourceType = GroupSourceType.Instagram
                }
            ));
            
            await GetNullableGroupLookupAsync("");
            if (!IsPartnerRole())
            {
                await GetNullableAppUserLookupAsync("");
            }
            await GetNullableCategoryLookupAsync("");
            await GetNullablePartnerLookupAsync("");
            await GetNullableCampaignLookupAsync("");
            await GetRunningCampaignsLookup("");
            _dateRanges = await GetDateRangePicker();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["Post.PageTitle"].Value}");
                
                (StartDate, EndDate) = await GetDefaultDate();
                Filter.ClientOffsetInMinutes = await GetOffsetInMinutes();
                await DoSearch();
            }
        } 

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Facebook"]));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Posts"]));
            return ValueTask.CompletedTask;
        }
        
        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewPostButton"],
                async () =>
                {
                   await  OpenCreatePostModal();
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.PartnerModule.PartnerPosts
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePost = await AuthorizationService.IsGrantedAsync(ApiPermissions.Posts.Create);
            CanEditPost = await AuthorizationService.IsGrantedAsync(ApiPermissions.Posts.Edit);
            CanEditNote = await AuthorizationService.IsGrantedAsync(ApiPermissions.Posts.EditNote);
            CanDeletePost = await AuthorizationService.IsGrantedAsync(ApiPermissions.Posts.Delete);
        }

        private async Task DoSearch()
        {
            _showLoading = false;
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
            
            var result = await PartnerModuleAppService.GetPostNavs(filter);
            PostList = result.Items;
            TotalCount = (int) result.TotalCount;
            
            _showLoading = true;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;

            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PostWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task OpenCreatePostModal()
        {
            NewPost = new PostCreateDto
            {
                SubmissionDateTime = DateTime.UtcNow,
                CampaignId = Guid.Empty,
                PostContentType = PostContentType.Seeding,
                PostCopyrightType = PostCopyrightType.Unknown
            };

            if (CurrentUser.Id != null)
            {
                var partners = await PartnerModuleAppService.GetPartnersByUser(CurrentUser.Id.Value);
                if (partners.IsNotNullOrEmpty())
                {
                    NewPost.PartnerId = partners.FirstOrDefault()?.Id;
                }
            }

            NewPostValidations.ClearAll();
            CreatePostModal.Show();
            await InvokeAsync(StateHasChanged);
        }

        private void CloseCreatePostModal()
        {
            CreatePostModal.Hide();
        }

        private void OpenEditPostModal(PostWithNavigationPropertiesDto input)
        {
            EditingPostId = input.Post.Id;
            EditingPost = ObjectMapper.Map<PostDto, PostUpdateDto>(input.Post);
            if (EditingPost.CampaignId == null) EditingPost.CampaignId = Guid.Empty;
            if (EditingPost.GroupId == null) EditingPost.GroupId = Guid.Empty;
            if (EditingPost.CategoryId == null) EditingPost.CategoryId = Guid.Empty;
            EditingPostValidations.ClearAll();
            EditPostModal.Show();
        }

        private void CloseEditPostModal()
        {
            EditPostModal.Hide();
        }

        private async Task CreatePostAsync()
        {
            var success =  await Invoke(async () =>
            {
                await PartnerModuleAppService.CreateMultiplePosts(NewPost);
                await DoSearch();
                CreatePostModal.Hide();
                await InvokeAsync(StateHasChanged);

            }, L, true);
        }
        
        private async Task DeletePostAsync(PostWithNavigationPropertiesDto input)
        {
            await PartnerModuleAppService.DeletePost(input.Post.Id);
            await DoSearch();
        }

        private async Task UpdatePostAsync()
        {
            try
            {
                await PartnerModuleAppService.UpdatePost(EditingPostId, EditingPost);
                await DoSearch();
                EditPostModal.Hide();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception exception)
            {
                if (exception is AbpRemoteCallException abpRemoteCallException)
                {
                    foreach (var ex in abpRemoteCallException.Error.ValidationErrors) { await Message.Error(ex.Message); }
                }
                else { await Message.Error(exception.Message); }
            }
        }
        
        private async void ClearFilter()
        {
            Filter = new GetPostsInputExtend();
            await ClearSelectedComponent();
            
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }
        
        private void StartDateChanged(DateTimeOffset? offset)
        {
            StartDate = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            EndDate = offset;
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
        
        protected virtual async Task Search()
        {
            // todoo ???
            CurrentPage = 1;
            await DoSearch();
        }
        
        private async Task ClearSelectedComponent()
        {
            PostSourceTypeFilterInput = PostSourceTypeFilter.NoSelect;
            PostContentTypeFilterInput = PostContentTypeFilter.NoSelect;
            PostCopyrightTypeFilterInput = PostCopyrightTypeFilter.NoSelect;
            PostIsNotAvailableFilterInput = PostIsNotAvailableFilter.NoSelect;

            SelectedCampaignIdFilter = Guid.Empty;
            SelectedPartnerIdFilter = Guid.Empty;

            (StartDate, EndDate) = await GetDefaultDate();

            await JsRuntime.InvokeVoidAsync("setInputValue", "#AppUserIdInputFilter input", "");
            await JsRuntime.InvokeVoidAsync("setInputValue", "#GroupIdInputFilter input", "");
        }
    }
}