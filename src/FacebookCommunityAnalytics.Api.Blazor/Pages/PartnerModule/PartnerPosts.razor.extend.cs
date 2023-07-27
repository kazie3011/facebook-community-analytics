using System;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class PartnerPosts
    {
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

        private async void OnAuthorCellSelected(PostWithNavigationPropertiesDto dto)
        {
            if (dto != null)
            {
                Filter.AppUserId = dto.AppUser.Id;
            }

            await SearchAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async void OnGroupCellSelected(PostWithNavigationPropertiesDto dto)
        {
            if (dto != null)
            {
                Filter.GroupId = dto.Group.Id;
            }

            await SearchAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async void OnPostContentTypeCellSelected(PostWithNavigationPropertiesDto dto)
        {
            if (dto != null)
            {
                PostContentTypeFilterInput = Enum.Parse<PostContentTypeFilter>(dto.Post.PostContentType.ToString());
            }

            await SearchAsync();
            await InvokeAsync(StateHasChanged);
        }

        // private RichTextEdit richTextEditRef;
        //
        // public async Task OnContentChanged()
        // {
        //     NewPost.Note = await richTextEditRef.GetHtmlAsync();
        // }

        // private RichTextEdit richTextUpdateNoteRef;

        // public async Task OnUpdateNoteContentChanged()
        // {
        //     UpdateNotePostDto.Note = await richTextUpdateNoteRef.GetHtmlAsync();
        // }

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
                    await PartnerModuleAppService.UpdatePost(EditingPostId, EditingPost);
                    await DoSearch();
                    UpdateNotePostModal.Hide();
                    await InvokeAsync(StateHasChanged);
                },
                L,
                true
            );
        }

        private void OnPostContentTypeChanged(PostContentType postContentType)
        {
            NewPost.PostContentType = postContentType;
            NewPost.CategoryId = null;
            NewPost.PostCopyrightType = PostCopyrightType.Unknown;
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

        public async Task ReloadPage()
        {
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }

        private async Task GetNullableCategoryLookupAsync(string newValue)
        {
            CategoriesNullable = await PartnerModuleAppService.GetCategoryLookup(new LookupRequestDto {Filter = newValue});
        }

        private async Task GetNullableGroupLookupAsync(string newValue)
        {
            GroupsNullable = await PartnerModuleAppService.GetGroupLookup
            (
                new GroupLookupRequestDto
                {
                    Filter = newValue,
                }
            );
        }

        private async Task GetNullableAppUserLookupAsync(string newValue)
        {
            UsersNullable = (await UserInfosAppService.GetUserLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullablePartnerLookupAsync(string newValue)
        {
            PartnersNullable = await PartnerModuleAppService.GetPartnersLookup(new LookupRequestDto {Filter = newValue});
        }

        private async Task GetNullableCampaignLookupAsync(string newValue)
        {
            CampaignsNullable = (await PartnerModuleAppService.GetCampaignLookup(new GetCampaignLookupDto {Filter = newValue}));
        }

        private async Task GetRunningCampaignsLookup(string newValue)
        {
            RunningCampaignsLookup = await PartnerModuleAppService.GetRunningCampaignLookup
            (
                new LookupRequestDto
                {
                    Filter = newValue,
                    CreatorId = IsPartnerRole() ? CurrentUser.Id : null
                }
            );
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
    }
}