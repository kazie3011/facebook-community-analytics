using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Blazorise.RichTextEdit;
using FacebookCommunityAnalytics.Api.Blazor.Models;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.ScheduledPosts;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class ScheduledPosts
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<ScheduledPostDto> ScheduledPostList { get; set; }
        private IReadOnlyList<SchedulePostWithNavigationPropertiesDto> SchedulePostWithNavigationPropertiesDtos { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateScheduledPost { get; set; }
        private bool CanEditScheduledPost { get; set; }
        private bool CanDeleteScheduledPost { get; set; }
        private ScheduledPostCreateDto NewScheduledPost { get; set; }
        private Validations NewScheduledPostValidations { get; set; }
        private ScheduledPostUpdateDto EditingScheduledPost { get; set; }
        private Validations EditingScheduledPostValidations { get; set; }
        private Guid EditingScheduledPostId { get; set; }
        private Modal CreateScheduledPostModal { get; set; }
        private Modal EditScheduledPostModal { get; set; }
        private GetScheduledPostInput Filter { get; set; }
        private DataGridEntityActionsColumn<SchedulePostWithNavigationPropertiesDto> EntityActionsColumn { get; set; }

        private RichTextEdit richTextCreateRef;
        private RichTextEdit richTextEditRef;


        private IReadOnlyList<GroupDto> groupDtos = new List<GroupDto>();
        private IList<GroupDto> createPostGroupSelecteds = new List<GroupDto>();
        private IList<GroupDto> editPostGroupSelecteds = new List<GroupDto>();
        private IReadOnlyList<LookupDto<Guid?>> CategoriesNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
        string selectedTab = string.Empty;

        private List<UploadFileModelResult> uploadFileModelResults = new List<UploadFileModelResult>();

        public ScheduledPosts()
        {
            NewScheduledPost = new ScheduledPostCreateDto();
            EditingScheduledPost = new ScheduledPostUpdateDto();
            Filter = new GetScheduledPostInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            groupDtos = (await GroupsAppService.GetListAsync(new GetGroupsInput {MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount})).Items;

            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetNullableCategoryLookupAsync("");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["ScheduledPosts.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ScheduledPosts"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["ScheduledPost.NewScheduledPost"],
                () =>
                {
                    OpenCreateScheduledPostModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.ScheduledPosts.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateScheduledPost = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.ScheduledPosts.Create);
            CanEditScheduledPost = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.ScheduledPosts.Edit);
            CanDeleteScheduledPost = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.ScheduledPosts.Delete);
        }

        private async Task GetScheduledPostsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ScheduledPostsAppService.GetListWithNavigationPropertiesAsync(Filter);
            SchedulePostWithNavigationPropertiesDtos = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetScheduledPostsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SchedulePostWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetScheduledPostsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateScheduledPostModal()
        {
            selectedTab = L["ScheduledPostCreateInfo"];

            NewScheduledPost = new ScheduledPostCreateDto
            {
                ScheduledPostDateTime = DateTime.UtcNow,
            };
            NewScheduledPostValidations.ClearAll();
            uploadFileModelResults.Clear();
            createPostGroupSelecteds.Clear();

            CreateScheduledPostModal.Show();
        }

        private void CloseCreateScheduledPostModal()
        {
            CreateScheduledPostModal.Hide();
        }

        private async Task DeleteScheduledPostAsync(SchedulePostWithNavigationPropertiesDto input)
        {
            var success = await Invoke
            (
                async () =>
                {
                    await ScheduledPostsAppService.DeleteAsync(input.ScheduledPost.Id);
                    await GetScheduledPostsAsync();
                },
                L,
                true,
                actionType: BlazorComponentBaseActionType.Delete
            );
        }

        private async Task CreateScheduledPostAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    //if (!NewScheduledPostValidations.ValidateAll()) return;
                    NewScheduledPost.AppUserId = CurrentUser.Id;
                    if (uploadFileModelResults.Any())
                    {
                        NewScheduledPost.Images = uploadFileModelResults.Select(_ => _.Url).ToList();
                    }

                    if (createPostGroupSelecteds.Any())
                    {
                        NewScheduledPost.GroupIds = string.Join(",", createPostGroupSelecteds.Select(_ => _.Fid).ToList());
                    }

                    await ScheduledPostsAppService.CreateAsync(NewScheduledPost);
                    await GetScheduledPostsAsync();
                    CreateScheduledPostModal.Hide();
                },
                L,
                true,
                actionType: BlazorComponentBaseActionType.Create
            );
        }

        private void OpenEditScheduledPostModal(SchedulePostWithNavigationPropertiesDto input)
        {
            EditingScheduledPostValidations.ClearAll();
            uploadFileModelResults.Clear();
            editPostGroupSelecteds.Clear();

            selectedTab = L["ScheduledPostEditInfo"];

            EditingScheduledPostId = input.ScheduledPost.Id;
            EditingScheduledPost = ObjectMapper.Map<ScheduledPostDto, ScheduledPostUpdateDto>(input.ScheduledPost);

            if (EditingScheduledPost.GroupIds.IsNotNullOrEmpty())
            {
                var editGroupIds = EditingScheduledPost.GroupIds.Split(',').ToList();
                editPostGroupSelecteds = groupDtos.Where(_ => editGroupIds.Contains(_.Fid)).ToList();
            }

            EditScheduledPostModal.Show();
        }

        private async Task UpdateScheduledPostAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    //if (!EditingScheduledPostValidations.ValidateAll()) return;
                    if (uploadFileModelResults.Any())
                    {
                        EditingScheduledPost.Images.AddRange(uploadFileModelResults.Select(_ => _.Url).ToList());
                    }

                    EditingScheduledPost.GroupIds = editPostGroupSelecteds.Any()
                        ? string.Join(",", editPostGroupSelecteds.Select(_ => _.Fid).ToList())
                        : string.Empty;

                    await ScheduledPostsAppService.UpdateAsync(EditingScheduledPostId, EditingScheduledPost);
                    await GetScheduledPostsAsync();
                    EditScheduledPostModal.Hide();
                },
                L,
                true,
                actionType: BlazorComponentBaseActionType.Update
            );
        }

        private void CloseEditScheduledPostModal()
        {
            EditScheduledPostModal.Hide();
        }

        public async Task OnCreateContentChanged()
        {
            NewScheduledPost.Content = await richTextCreateRef.GetTextAsync();
        }

        public async Task OnEditContentChanged()
        {
            EditingScheduledPost.Content = await richTextEditRef.GetTextAsync();
        }

        #region Upload Files

        private void OnSelectedTabChanged(string name)
        {
            selectedTab = name;
        }

        RadzenUpload createPostUpload;
        RadzenUpload editPostUpload;
        int progress;
        string info;

        private void OnChange(UploadChangeEventArgs args, string name)
        {
        }

        private void OnProgress(UploadProgressArgs args, string name)
        {
            this.info = $"% '{name}' / {args.Loaded} of {args.Total} bytes.";
            this.progress = args.Progress;

            if (args.Progress != 100)
            {
                return;
            }
        }

        private async Task CreateUploadAsync()
        {
            await createPostUpload.Upload();
        }

        private async Task EditUploadAsync()
        {
            await editPostUpload.Upload();
        }

        public void OnComplete(UploadCompleteEventArgs args)
        {
            try
            {
                uploadFileModelResults = JsonConvert.DeserializeObject<List<UploadFileModelResult>>(args.RawResponse);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void OnError(UploadErrorEventArgs args)
        {
            var x = args.Message;
        }

        #endregion

        private void OnPostContentTypeChanged(PostContentType postContentType)
        {
            NewScheduledPost.PostContentType = postContentType;
            NewScheduledPost.CategoryId = null;
            NewScheduledPost.PostCopyrightType = PostCopyrightType.Unknown;
        }

        private void OnEditingPostContentTypeChanged(PostContentType postContentType)
        {
            EditingScheduledPost.PostContentType = postContentType;
            EditingScheduledPost.CategoryId = null;
            EditingScheduledPost.PostCopyrightType = PostCopyrightType.Unknown;
        }

        private async Task GetNullableCategoryLookupAsync(string newValue)
        {
            CategoriesNullable = (await PostsExtendAppService.GetCategoryLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullableAppUserLookupAsync(string newValue)
        {
            UsersNullable = (await PostsExtendAppService.GetAppUserLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }
    }
}