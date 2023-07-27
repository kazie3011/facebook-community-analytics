using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class PartnerCampaignDetail
    {
        [Parameter] public string CampaignId { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        private string _selectedTab = "CampaignFacebookTab";

        private CampaignDto _campaignDto { get; set; }
        private List<CampaignPostDto> _campaignPosts = new();
        private List<CampaignPostDto> _seedingPosts = new();
        private List<CampaignPostDto> _d2cPost = new();
        private List<TiktokWithNavigationPropertiesDto> _tikTokNavs = new();
        private List<TiktokWithNavigationPropertiesDto> _modalTikTokNavs = new();
        private List<TiktokWithNavigationPropertiesDto> _modalPageTikTokNavs = new();
        
        private int ModalTikTokPageSize { get; set; } = 5;
        private int ModalTiktokCurrentPage { get; set; } = 1;
        private int SeedingPostProgress { get; set; }
        private int SeedingReactionProgress { get; set; }
        private int TikTokVideoProgress { get; set; }
        private int TikTokViewProgress { get; set; }
        private DataGridEntityActionsColumn<CampaignPostDto> EntityActionsColumn { get; set; }
        private DataGridEntityActionsColumn<TiktokWithNavigationPropertiesDto> TikTokEntityActionsColumn { get; set; }
        private Modal CreatePostModal { get; set; }
        private Modal AddTiktokModal { get; set; }
        private Validations NewPostValidations { get; set; }
        private PostCreateDto NewPost { get; set; } = new PostCreateDto();
        private string _filterText;

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await InitData();
        }

        private async Task InitData()
        {
            _campaignDto = await PartnerModuleAppService.GetCampaign(CampaignId.ToGuidOrDefault());
            _campaignPosts = await PartnerModuleAppService.GetCampaignPosts(CampaignId.ToGuidOrDefault());
            _seedingPosts = _campaignPosts.Where(p => p.PostContentType == PostContentType.Seeding).ToList();
            _d2cPost = _campaignPosts.Where(p => p.PostContentType == PostContentType.D2C).ToList();
            await LoadTikTok();
            if (_seedingPosts.IsNullOrEmpty())
            {
                _selectedTab = "TikTokTab";
            }

            _campaignDto.Current.Seeding_TotalPost = _seedingPosts.Count;
            _campaignDto.Current.Seeding_TotalReaction = _seedingPosts.Sum(_ => _.TotalCount);

            _campaignDto.Current.TikTok_TotalVideo = _tikTokNavs.Count;
            _campaignDto.Current.TikTok_TotalView = _tikTokNavs.Sum(_ => _.Tiktok.ViewCount);
            
            SeedingPostProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Seeding_TotalPost, _campaignDto.Target.Seeding_TotalPost);
            SeedingReactionProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Seeding_TotalReaction, _campaignDto.Target.Seeding_TotalReaction);

            TikTokVideoProgress = PercentageHelper.GetPercentage(_campaignDto.Current.TikTok_TotalVideo, _campaignDto.Target.TikTok_TotalVideo);
            TikTokViewProgress = PercentageHelper.GetPercentage(_campaignDto.Current.TikTok_TotalView, _campaignDto.Target.TikTok_TotalView);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                BrowserDateTime = await BrowserDateTimeProvider.GetInstance();
                await InitPage($"GDL - {L["CampaignDetail.PageTitle"].Value}");
            }
        }
        
        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Campaigns"], "/campaigns"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:CampaignDetail"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            if (_campaignDto != null && _campaignDto.Emails.IsNotNullOrEmpty())
            {
                Toolbar.AddButton
                (
                    L["CampaignDetail.SendEmail"],
                    async () =>
                    {
                        await SendEmail(_campaignDto.Id);
                    },
                    requiredPolicyName: ApiPermissions.Campaigns.Default
                );
            }
            Toolbar.AddButton(L["ExportExcel"], async () =>
            {
                await ExportPostsAsync();
            }, requiredPolicyName: ApiPermissions.PartnerModule.PartnerCampaigns);
            
            Toolbar.AddButton(L["NewFacebookButton"],async () =>
            {
                await  OpenCreatePostModal();
            }, requiredPolicyName: ApiPermissions.PartnerModule.PartnerPosts);
            
            Toolbar.AddButton
            (
                L["AddCampaignTiktokButton"],
                async () =>
                {
                   await OpenAddTikTokModal();
                }, requiredPolicyName: ApiPermissions.PartnerModule.PartnerCampaigns
            );

            return ValueTask.CompletedTask;
        }

        private void OnSelectedTabChanged(string name)
        {
            _selectedTab = name;
        }
        
        private async Task OpenCreatePostModal()
        {
            if (IsPartnerRole())
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
            }
            CreatePostModal.Show();
            await InvokeAsync(StateHasChanged);
        } 
        
        private void CloseCreatePostModal()
        {
            CreatePostModal.Hide();
        }
        
        private async Task ExportPostsAsync()
        {
            var campaignId = CampaignId.ToGuidOrDefault();
            var data = await PartnerModuleAppService.ExportCampaign(campaignId);
            if (data.IsNullOrEmpty())
            {
                await Message.Info(L["CampaignExport.NoPost"]);
                return;
            }
            if (campaignId != Guid.Empty)
            {
                var campaign = await PartnerModuleAppService.GetCampaign(campaignId);
                var fileName = $"CP_{campaign.Code}";
                await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(data));
            }
            else
            {
                await Message.Error("CampaignExport.ValidateCampaignIdNotNull");
            }
        }

        private async Task RemoveCampaignPost(CampaignPostDto e)
        {
            var resultConfirm = await UiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (resultConfirm)
            {
                await PartnerModuleAppService.RemoveCampaignPost(e.Id);
                await InitData();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task CreatePostAsync()
        {
            var campaignId = CampaignId.ToGuidOrDefault();
            NewPost.CampaignId = campaignId;
            var success =  await Invoke(async () =>
            {
                await PartnerModuleAppService.CreateCampaignPosts(NewPost);
                await InitData();
                CreatePostModal.Hide();
                await InvokeAsync(StateHasChanged);
            
            }, L, true);
        }
        
        private void OnPostContentTypeChanged(PostContentType postContentType)
        {
            NewPost.PostContentType = postContentType;
            NewPost.CategoryId = null;
            NewPost.PostCopyrightType = PostCopyrightType.Unknown;
        }

        private string GetVideoUrl(TiktokDto tiktokVideo)
        {
            return tiktokVideo.VideoId.IsNullOrWhiteSpace() ? L["PostUrl"] : $"VideoId: {tiktokVideo.VideoId.MaybeSubstring(7, true)}";
        }

        private async Task LoadTikTok()
        {
            _tikTokNavs = await PartnerModuleAppService.GetTikToks
            (
                new GetTiktoksInputExtend()
                { 
                    CampaignId = CampaignId.ToGuidOrDefault(),
                }
            );
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task RemoveCampaignTikTok(TiktokDto input)
        {
            var resultConfirm = await _uiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (resultConfirm)
            {
                var success = await Invoke
                (
                    async () =>
                    {
                        input.CampaignId = null;
                        var updateTikTok = ObjectMapper.Map<TiktokDto, TiktokCreateUpdateDto>(input);
                        await PartnerModuleAppService.UpdateCampaignTiktok(updateTikTok, input.Id);
                        await LoadTikTok();
                    },
                    L,
                    true,
                    BlazorComponentBaseActionType.Update
                );
            }
            
        }
        
        private async Task OpenAddTikTokModal()
        {
            ModalTiktokCurrentPage = 1;
            ModalTikTokPageSize = 5;
            _filterText = string.Empty;
            _modalTikTokNavs = new List<TiktokWithNavigationPropertiesDto>();
            _modalPageTikTokNavs = new List<TiktokWithNavigationPropertiesDto>();
            AddTiktokModal.Show();
            await SearchTikTok();
        }
        
        private async Task AddTiktokToCampaign(TiktokDto input)
        {
            var success = await Invoke
            (
                async () =>
                {
                    input.CampaignId = CampaignId.ToGuidOrDefault();
                    var updateTikTok = ObjectMapper.Map<TiktokDto, TiktokCreateUpdateDto>(input);
                    await PartnerModuleAppService.UpdateCampaignTiktok(updateTikTok, input.Id);
                    AddTiktokModal.Hide();
                    await LoadTikTok();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }
        
        private void CloseAddTiktokModal()
        {
            AddTiktokModal.Hide();
        }
        
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TiktokWithNavigationPropertiesDto> e)
        {
            ModalTiktokCurrentPage = e.Page;
            ModalTikTokPageSize = e.PageSize;
            await SearchTikTok();
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task SearchTikTok()
        {
            _modalTikTokNavs = await PartnerModuleAppService.GetTikToks(new GetTiktoksInputExtend() { Search = _filterText });
            _modalPageTikTokNavs = _modalTikTokNavs.Skip((ModalTiktokCurrentPage - 1) * ModalTikTokPageSize).Take(ModalTikTokPageSize).ToList();
        }
        
        private async Task SendEmail(Guid campaignId)
        {
            await PartnerModuleAppService.SendCampaignEmail(campaignId);
            await Message.Success(L["Message.SendingEmail"]);
        }
    }
}