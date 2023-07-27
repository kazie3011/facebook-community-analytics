using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class ContentMedia
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        private bool CanCreateContentMedia { get; set; }
        private bool CanEditContentMedia { get; set; }
        private bool CanDeleteContentMedia { get; set; }
        private List<MediaDto> MediaDtos { get; set; }
        private Modal CreateModal { get; set; } = new();
        private GetMediasInput Filter { get; set; } = new() {MaxResultCount = 1000};
        private TagsInputControl _tagsInputControl { get; set; }
        private SubmitButton _buttonSubmitForm { get; set; }

        private bool _showLoading { get; set; }

        //**********************************Upload Image*****************************
        private int _progress;
        private string _info;
        private RadzenUpload _createContentUpload;
        private MediaCategory _selectedMediaCategory;
        private int _selectedMediaCategoryFilter = -1;

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetContentsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("PreventEnterKey", "CreateForm");
                await InitPage($"GDL - {L["Content.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ContentMedia"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["Content.ContentMedia.NewContent.Button"],
                () =>
                {
                    OpenCreateModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Content.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateContentMedia = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Content.Create);
            CanEditContentMedia = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Content.Edit);
            CanDeleteContentMedia = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Content.Delete);
        }

        private void OpenCreateModal()
        {
            CreateModal.Show();
        }

        private void CloseCreateModal()
        {
            _tagsInputControl.ClearTags();
            _createContentUpload.Dispose();
            _buttonSubmitForm.Disabled = false;
            CreateModal.Hide();
        }

        private async Task GetContentsAsync()
        {
            try
            {
                var listContentMedia = await _mediaAppService.GetContentMediaAsync(Filter);
                MediaDtos = listContentMedia.Items.ToList();
                foreach (var item in MediaDtos)
                {
                    if (item.FileData.IsNullOrEmpty()) continue;
                    item.ImagePath = $"data:image/gif;base64,{Convert.ToBase64String(item.FileData, 0, item.FileData.Length)}";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void OnChange(UploadChangeEventArgs args, string name)
        {
        }

        private void OnProgress(UploadProgressArgs args, string name)
        {
            this._info = $"% '{name}' / {args.Loaded} of {args.Total} bytes.";
            this._progress = args.Progress;
        }

        private async Task CreateUploadAsync()
        {
            _showLoading = true;
            _buttonSubmitForm.Disabled = true;
            await _createContentUpload.Upload();
        }

        public async Task OnComplete(UploadCompleteEventArgs args)
        {
            try
            {
                var mediaModelResults = JsonConvert.DeserializeObject<List<MediaDto>>(args.RawResponse);
                if (mediaModelResults != null)
                {
                    var tags = _tagsInputControl.Tags;
                    foreach (var item in mediaModelResults)
                    {
                        var updateDto = ObjectMapper.Map<MediaDto, MediaCreateUpdateDto>(item);
                        updateDto.Tags = tags;
                        updateDto.MediaCategory = _selectedMediaCategory;
                        await _mediaAppService.UpdateAsync(item.Id, updateDto);
                    }
                }

                _showLoading = false;
                await Message.Success(L["Content.UploadFiles.Success"]);
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
            catch (Exception)
            {
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
        }

        private void OnError(UploadErrorEventArgs args)
        {
            var x = args.Message;
        }

        private async Task RemoveImage(Guid mediaId)
        {
            await _mediaAppService.DeleteAsync(mediaId);
            await GetContentsAsync();
        }

        private async Task DownloadImage(string url)
        {
            await JSRuntime.InvokeVoidAsync("open", url, "_blank");
        }

        private void OnSelectedCategoryChanged(int value)
        {
            _selectedMediaCategoryFilter = value;
            if (value > 0)
            {
                Filter.MediaCategory = (MediaCategory) value;
            }
            else
            {
                Filter.MediaCategory = null;
            }
        }
    }
}