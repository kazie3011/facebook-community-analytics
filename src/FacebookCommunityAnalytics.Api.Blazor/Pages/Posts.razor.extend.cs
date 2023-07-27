using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Posts
    {
        protected ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Facebook"]));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Posts"]));
            return ValueTask.CompletedTask;
        }

        protected ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewPostButton"],
                async () =>
                {
                    await OpenCreatePostModal();
                },
                IconName.FileAlt,
                requiredPolicyName: ApiPermissions.Posts.Create
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
        
        private string GetGroupDisplayName(Guid? groupId)
        {
            var group = _groupDtos.FirstOrDefault(_ => _.Id == groupId);
            if (group == null) return string.Empty;
            return GroupConsts.GetGroupDisplayName(group.Title, group.GroupSourceType);
        }

        private string GetPostUrl(PostDto post)
        {
            return post.Fid.IsNullOrWhiteSpace() ? L["PostUrl"] : $"Fid:{post.Fid.Substring(post.Fid.Length - 5)}";
        }

        private async Task OnIsNotAvailableFilter_Changed(PostIsNotAvailableFilter value)
        {
            PostIsNotAvailableFilterInput = value;
            await OnSelectFilter();
        }

        private async Task OnPostContentTypeFilter_Changed(PostContentTypeFilter value)
        {
            PostContentTypeFilterInput = value;
            await OnSelectFilter();
        }

        private async Task OnPostCopyrightTypeFilter_Changed(PostCopyrightTypeFilter value)
        {
            PostCopyrightTypeFilterInput = value;
            await OnSelectFilter();
        }

        private async Task OnPostSourceTypeFilter_Changed(PostSourceTypeFilter value)
        {
            PostSourceTypeFilterInput = value;
            await OnSelectFilter();
        }

        private async void OnAuthorCellSelected(PostWithNavigationPropertiesDto dto)
        {
            if (dto != null)
            {
                Filter.AppUserId = dto.AppUser?.Id;
            }

            await Search();
            await InvokeAsync(StateHasChanged);
        }

        private async void OnGroupCellSelected(PostWithNavigationPropertiesDto dto)
        {
            if (dto != null)
            {
                Filter.GroupId = dto.Group.Id;
            }

            await Search();
            await InvokeAsync(StateHasChanged);
        }

        private async void OnPostContentTypeCellSelected(PostWithNavigationPropertiesDto dto)
        {
            if (dto != null)
            {
                PostContentTypeFilterInput = Enum.Parse<PostContentTypeFilter>(dto.Post.PostContentType.ToString());
            }

            await Search();
            await InvokeAsync(StateHasChanged);
        }

        private void CloseUpdateNotePostModal()
        {
            UpdateNotePostModal.Hide();
        }

        private void OpenEditNoteModal(PostWithNavigationPropertiesDto input)
        {
            EditingPostId = input.Post.Id;
            EditingPost = ObjectMapper.Map<PostDto, PostUpdateDto>(input.Post);
            UpdateNotePostDto = ObjectMapper.Map<PostDto, PostUpdateNoteDto>(input.Post);
            UpdateNotePostModal.Show();
        }

        private async Task UpdateNotePostAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    EditingPost.Note = UpdateNotePostDto.Note;
                    await PostsExtendAppService.UpdateAsync(EditingPostId, EditingPost);
                    await DoSearch();
                    UpdateNotePostModal.Hide();
                    await InvokeAsync(StateHasChanged);
                },
                L,
                true
            );
        }

        private void OnNewPostContentTypeChanged(PostContentType postContentType)
        {
            NewPost.PostContentType = postContentType;
            NewPost.CategoryId = null;
            NewPost.PostCopyrightType = PostCopyrightType.Unknown;
        }

        private void OnEditPostContentTypeChanged(PostContentType postContentType)
        {
            EditingPost.PostContentType = postContentType;
            EditingPost.CategoryId = null;
            EditingPost.PostCopyrightType = PostCopyrightType.Unknown;
        }

        public void OnUrlMemoChanged(string value)
        {
            NewPost.Url = value;
            if (value.Contains("instagram"))
            {
                _showGroupSelect = true;
                GroupsNullable = InstagramGroupsNullable;
            }
            else if (value.Contains("videos"))
            {
                _showGroupSelect = true;
                GroupsNullable = AllGroupsNullable;
            }
            else
            {
                _showGroupSelect = false;
                GroupsNullable = AllGroupsNullable;
            }
        }

        private async Task GetNullableCategoryLookupAsync(string newValue)
        {
            CategoriesNullable = (await PostsExtendAppService.GetCategoryLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullableGroupLookupAsync(string newValue)
        {
            GroupsNullable = (await PostsExtendAppService.GetGroupLookupAsync(new GroupLookupRequestDto { Filter = newValue, })).Items;
        }
        
        private async Task OnSelectGroup_Changed(string value)
        {
            await GetNullableGroupLookupAsync(value);
            await OnSelectFilter();
        }
        
        private async Task OnSelectUser_Changed(string value)
        {
            await GetNullableAppUserLookupAsync(value);
            await OnSelectFilter();
        }

        private async Task GetNullableAppUserLookupAsync(string newValue)
        {
            UsersNullable = (await UserInfosAppService.GetUserLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullablePartnerLookupAsync(string newValue)
        {
            PartnersNullable = (await PostsExtendAppService.GetPartnerLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullableCampaignLookupAsync(string newValue)
        {
            CampaignsNullable = (await PostsExtendAppService.GetCampaignLookupAsync
            (
                new LookupRequestDto
                {
                    Filter = newValue,
                    CreatorId = IsPartnerRole() ? CurrentUser.Id : null
                }
            )).Items;
        }

        private async Task GetRunningCampaignsLookup(string newValue)
        {
            RunningCampaignsLookup = (await PostsExtendAppService.GetRunningCampaignLookup
            (
                new LookupRequestDto
                {
                    Filter = newValue,
                    CreatorId = IsPartnerRole() ? CurrentUser.Id : null
                }
            )).Items;
        }

        private async Task OpenCreatePostModal()
        {
            NewPost = new PostCreateDto
            {
                SubmissionDateTime = DateTime.UtcNow,
                CampaignId = Guid.Empty
            };
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
            EditingPost.CampaignId ??= Guid.Empty;
            EditingPost.GroupId ??= Guid.Empty;
            EditingPost.CategoryId ??= Guid.Empty;
            EditingPostValidations.ClearAll();
            EditPostModal.Show();
        }

        private void CloseEditPostModal()
        {
            EditPostModal.Hide();
        }

        private async Task CreatePost()
        {
            await Invoke
            (
                async () =>
                {
                    await PostsExtendAppService.CreateManyAsync(NewPost);
                    await DoSearch();
                    CreatePostModal.Hide();
                },
                L,
                true
            );
        }

        private async Task DeletePost(PostWithNavigationPropertiesDto input)
        {
            var confirmResult = await UiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                await Invoke
                (
                    async () =>
                    {
                        await PostsExtendAppService.DeleteAsync(input.Post.Id);
                        await DoSearch();
                    },
                    L,
                    true,
                    BlazorComponentBaseActionType.Delete
                );
            }
        }

        private async Task UpdatePost()
        {
            await Invoke
            (
                async () =>
                {
                    await PostsExtendAppService.UpdateAsync(EditingPostId, EditingPost);
                    await DoSearch();
                    EditPostModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private async void ClearFilter()
        {
            Filter = new GetPostsInputExtend();
            await ClearSelectedComponent();

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

        private void StartDateChanged(DateTimeOffset? offset)
        {
            StartDate = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            EndDate = offset;
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
        }
    }
}