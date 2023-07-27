using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Charts;
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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Enums;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Statistics;
using Microsoft.AspNetCore.Authorization;
using Tooltips = ChartJs.Blazor.Common.Tooltips;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    [Authorize]
    public partial class CampaignDetail
    {
        [Parameter] public string CampaignIdOrCode { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        private string _selectedTab = "AnnouncePostTab";

        private CampaignDto _campaignDto { get; set; }
        private List<CampaignPostDto> _facebookPosts = new();
        private List<CampaignPostDto> _announcePosts = new();
        private List<CampaignPostDto> _seedingPosts = new();
        private List<CampaignPostDto> _contestPosts = new();
        private List<CampaignPostDto> _prPosts = new();
        private List<CampaignPostDto> _affiliatePosts = new();
        private List<UserAffiliateWithNavigationPropertiesDto> _affiliates = new();
        private List<TiktokWithNavigationPropertiesDto> _tikTokNavs = new();
        private List<TiktokWithNavigationPropertiesDto> _modalTikTokNavs = new();
        private List<TiktokWithNavigationPropertiesDto> _modalPageTikTokNavs = new();
        private Chart _campaignDailyChart = new();
        private BarConfig _campaignDailyChartConfig = new();
        private CampaignDailyChartResponse _statistics = new();


        private int ModalTikTokPageSize { get; set; } = 5;
        private int ModalTiktokCurrentPage { get; set; } = 1;
        private int SeedingPostProgress { get; set; }
        private int SeedingReactionProgress { get; set; }
        private int ContestPostProgress { get; set; }
        private int ContestReactionProgress { get; set; }
        private int PRPostProgress { get; set; }
        private int PRReactionProgress { get; set; }
        private int AffiliatePostProgress { get; set; }
        private int AffiliateClickProgress { get; set; }
        private int AffiliateConversionProgress { get; set; }
        private int AffiliateConversionAmountProgress { get; set; }
        private int TikTokVideoProgress { get; set; }
        private int TikTokViewProgress { get; set; }
        private string ContestAuthorCaption { get; set; }
        private DataGridEntityActionsColumn<CampaignPostDto> EntityActionsColumn { get; set; }
        private DataGridEntityActionsColumn<TiktokWithNavigationPropertiesDto> TikTokEntityActionsColumn { get; set; }
        private CampaignUpdateDto EditingCampaign = new();
        private CampaignPrizeDto CampaignPrize = new();
        private Modal CreatePostModal { get; set; }
        private Modal AddTiktokModal { get; set; }
        private Modal CampaignPrizeConfigModal { get; set; }
        private Modal SettingPostCampaignPrizeModal { get; set; }
        private Validations NewPostValidations { get; set; }
        private PostCreateDto NewPost { get; set; } = new PostCreateDto();
        private string _filterText;
        private IList<CampaignPrizeDto> _campaignPrizes = new List<CampaignPrizeDto>();

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetBreadcrumbItemsAsync();
            InitCampaignDailyChartConfig();
        }


        private async Task InitData()
        {
            _campaignDto = await CampaignsAppService.GetByIdOrCode(CampaignIdOrCode);
            _facebookPosts = await CampaignsAppService.GetPosts(_campaignDto.Id);
            _affiliatePosts = _facebookPosts.Where(p => p.PostContentType == PostContentType.Affiliate).ToList();
            _contestPosts = _facebookPosts.Where(p => p.PostContentType == PostContentType.Contest).ToList();
            _prPosts = _facebookPosts.Where(p => p.PostContentType == PostContentType.PR).ToList();
            _seedingPosts = _facebookPosts.Where(p => p.PostContentType == PostContentType.Seeding).ToList();
            _announcePosts = _facebookPosts.Where(p => p.PostContentType == PostContentType.Seeding || p.PostContentType == PostContentType.Affiliate || p.PostContentType == PostContentType.D2C).ToList();
            await LoadTikTok();

            //first selected tab name
            if (_announcePosts.IsNotNullOrEmpty())
            {
                _selectedTab = "AnnouncePostTab";
            }
            else if (_contestPosts.IsNotNullOrEmpty())
            {
                _selectedTab = "ContestTab";
            }
            else if (_tikTokNavs.IsNotNullOrEmpty())
            {
                _selectedTab = "TikTokTab";
            }
            else if (_prPosts.IsNotNullOrEmpty())
            {
                _selectedTab = "PRPostTab";
            }

            List<string> shortLinks = new();
            foreach (var post in _affiliatePosts.Where(x => x.Shortlinks.IsNotNullOrEmpty()))
            {
                shortLinks.AddRange(post.Shortlinks);
            }

            _affiliates = shortLinks.IsNullOrEmpty() ? new List<UserAffiliateWithNavigationPropertiesDto>() : (await GetUserAffiliates(shortLinks));

            _campaignDto.Current.Seeding_TotalPost = _seedingPosts.Count;
            _campaignDto.Current.Seeding_TotalReaction = _seedingPosts.Sum(_ => _.TotalCount);

            _campaignDto.Current.Affiliate_TotalPost = _affiliatePosts.Count;
            _campaignDto.Current.Affiliate_TotalClick = _affiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ClickCount);
            _campaignDto.Current.Affiliate_TotalConversion = _affiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ConversionCount);
            _campaignDto.Current.Affiliate_TotalConversionAmount = _affiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ConversionAmount);

            _campaignDto.Current.Contest_TotalPost = _contestPosts.Count;
            _campaignDto.Current.Contest_TotalReaction = _contestPosts.Sum(_ => _.TotalCount);
            ContestAuthorCaption = $"{L["AuthorName"]} ({_contestPosts.Where(_ => _.Username.IsNotNullOrEmpty()).DistinctBy(_ => _.Username).Count()})";
            
            _campaignDto.Current.PR_TotalPost = _prPosts.Count;
            _campaignDto.Current.PR_TotalReaction = _prPosts.Sum(_ => _.TotalCount);

            _campaignDto.Current.TikTok_TotalVideo = _tikTokNavs.Count;
            _campaignDto.Current.TikTok_TotalView = _tikTokNavs.Sum(_ => _.Tiktok.ViewCount);

            SeedingPostProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Seeding_TotalPost, _campaignDto.Target.Seeding_TotalPost);
            SeedingReactionProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Seeding_TotalReaction, _campaignDto.Target.Seeding_TotalReaction);

            AffiliatePostProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Affiliate_TotalPost, _campaignDto.Target.Affiliate_TotalPost);
            AffiliateClickProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Affiliate_TotalClick, _campaignDto.Target.Affiliate_TotalClick);
            AffiliateConversionProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Affiliate_TotalConversion, _campaignDto.Target.Affiliate_TotalConversion);
            AffiliateConversionAmountProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Affiliate_TotalConversionAmount, _campaignDto.Target.Affiliate_TotalConversionAmount);

            ContestPostProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Contest_TotalPost, _campaignDto.Target.Contest_TotalPost);
            ContestReactionProgress = PercentageHelper.GetPercentage(_campaignDto.Current.Contest_TotalReaction, _campaignDto.Target.Contest_TotalReaction);

            TikTokVideoProgress = PercentageHelper.GetPercentage(_campaignDto.Current.TikTok_TotalVideo, _campaignDto.Target.TikTok_TotalVideo);
            TikTokViewProgress = PercentageHelper.GetPercentage(_campaignDto.Current.TikTok_TotalView, _campaignDto.Target.TikTok_TotalView);

            //Update Post for prize and check Post in prize exist in campaign contest
            _campaignDto.CampaignPrizes = _campaignDto.CampaignPrizes.Select
            (
                _ =>
                {
                    if (_.PostId.IsNotNullOrEmpty())
                    {
                        var post = _contestPosts.FirstOrDefault(p => p.Id == _.PostId);
                        if (post is null)
                        {
                            _.PostFid = string.Empty;
                            _.PostId = null;
                        }
                        else
                        {
                            _.Post = post;
                        }
                    }
                    return _;
                }
            ).ToList();
            await CampaignsAppService.UpdateCampaignPrizes(_campaignDto.Id, ObjectMapper.Map<CampaignDto, CampaignUpdateDto>(_campaignDto));
            
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["CampaignDetail.PageTitle"].Value}");
                await InitData();
                await SetToolbarItemsAsync();
                await RenderCampaignDailyChart();
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


            return ValueTask.CompletedTask;
        }

        private async Task ExportExcel()
        {
            await Invoke
            (
                async () =>
                {
                    await ExportPostsAsync();
                },
                L,
                true
            );

        }

        private async Task OpenReportFacebook()
        {
            await Invoke
            (
                async () =>
                {
                    await OpenCreatePostModal();
                },
                L,
                true
            );
        }

        private async Task OpenReportTiktok()
        {
            await Invoke
            (
                async () =>
                {
                    await OpenAddTikTokModal();
                },
                L,
                true
            );
        }

        private async Task SendEmail(Guid campaignId)
        {
            await CampaignsAppService.SendCampaignEmail(campaignId);
            await Message.Success(L["Message.SendingEmail"]);
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
                    input.CampaignId = _campaignDto.Id;
                    var updateTikTok = ObjectMapper.Map<TiktokDto, TiktokCreateUpdateDto>(input);
                    await CampaignsAppService.UpdateCampaignTiktok(updateTikTok, input.Id);
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
                        await CampaignsAppService.UpdateCampaignTiktok(updateTikTok, input.Id);
                        await LoadTikTok();
                    },
                    L,
                    true,
                    BlazorComponentBaseActionType.Update
                );
            }
        }

        private async Task SearchTikTok()
        {
            _modalTikTokNavs = await CampaignsAppService.GetTikToks(new GetTiktoksInputExtend() {Search = _filterText});
            _modalPageTikTokNavs = _modalTikTokNavs.Where(_ => _.Tiktok.CampaignId != _campaignDto.Id).Skip((ModalTiktokCurrentPage - 1) * ModalTikTokPageSize).Take(ModalTikTokPageSize).ToList();
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TiktokWithNavigationPropertiesDto> e)
        {
            ModalTiktokCurrentPage = e.Page;
            ModalTikTokPageSize = e.PageSize;
            await SearchTikTok();
            await InvokeAsync(StateHasChanged);
        }

        private async Task ExportPostsAsync()
        {
            if (_campaignDto is null)
            {
                await Message.Error("CampaignExport.ValidateCampaignIdNotNull");
                return;
            }

            var data = await CampaignsAppService.ExportCampaign(_campaignDto.Id);
            if (data.IsNullOrEmpty())
            {
                await Message.Info(L["CampaignExport.NoPost"]);
                return;
            }

            var fileName = $"CP_{_campaignDto.Code}";
            await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(data));
        }

        private async Task RemoveCampaignPost(CampaignPostDto e)
        {
            var resultConfirm = await _uiMessageService.Confirm(string.Format(L["RemoveCampaignPostMessage"], e.Fid, _campaignDto?.Name));
            if (resultConfirm)
            {
                await CampaignsAppService.RemoveCampaignPost(e.Id);
                await InitData();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task CreatePostAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    NewPost.CampaignId = _campaignDto.Id;
                    NewPost.IsCampaignManual = true;
                    NewPost.IsPostContentTypeManual = true;
                    await CampaignsAppService.CreateCampaignPosts(NewPost);
                    await InitData();
                    CreatePostModal.Hide();
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
            switch (postContentType)
            {
                case PostContentType.Unknown:
                    NewPost.PostCopyrightType = PostCopyrightType.Unknown;
                    break;
                case PostContentType.Affiliate:
                    NewPost.PostCopyrightType = PostCopyrightType.Unknown;
                    break;
                case PostContentType.Seeding:
                    NewPost.PostCopyrightType = PostCopyrightType.Unknown;
                    break;
                case PostContentType.Contest:
                    NewPost.PostCopyrightType = PostCopyrightType.Unknown;
                    break;
            }
        }

        private async Task<List<UserAffiliateWithNavigationPropertiesDto>> GetUserAffiliates(List<string> shortLinks)
        {
            var affiliates = new List<UserAffiliateWithNavigationPropertiesDto>();
            var partitions = shortLinks.Partition(20).ToList();
            var stack = new Stack<IEnumerable<string>>(partitions);
            while (true)
            {
                if (stack.TryPop(out var list))
                {
                    affiliates.AddRange(await CampaignsAppService.GetAffiliatesAsync(list.ToList()));
                }
                else
                {
                    break;
                }
            }

            return affiliates;
        }

        private string GetVideoUrl(TiktokDto tiktokVideo)
        {
            return tiktokVideo.VideoId.IsNullOrWhiteSpace() ? L["PostUrl"] : $"VideoId: {tiktokVideo.VideoId.MaybeSubstring(7, true)}";
        }

        private async Task LoadTikTok()
        {
            _tikTokNavs = await CampaignsAppService.GetTikToks
            (
                new GetTiktoksInputExtend
                {
                    CampaignId = _campaignDto.Id,
                }
            );
            await InvokeAsync(StateHasChanged);
        }
        
        private void OpenPrizeConfigModal()
        {
            _campaignPrizes = _campaignDto.CampaignPrizes.IsNotNullOrEmpty() ? _campaignDto.CampaignPrizes.Clone() : new List<CampaignPrizeDto>();
            DefaultPrizeConfig();
            CampaignPrizeConfigModal.Show();
        }
        private async Task CreatePrizeConfigAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    EditingCampaign = ObjectMapper.Map<CampaignDto, CampaignUpdateDto>(_campaignDto);
                    EditingCampaign.CampaignPrizes = _campaignPrizes.Where(_ => _.PrizeName.IsNotNullOrEmpty()).ToList();
                    if (EditingCampaign.CampaignPrizes.Count != EditingCampaign.CampaignPrizes.DistinctBy(_ => _.PrizeName).Count())
                    {
                        await Message.Error(LD["ApiDomain:Campaign:PrizeExisted"]);
                        return;
                    }
                    await CampaignsAppService.UpdateCampaignPrizes(_campaignDto.Id, EditingCampaign);
                    await InitData();
                    CampaignPrizeConfigModal.Hide();
                    await InvokeAsync(StateHasChanged);
                },
                L,
                true
            );
        }

        private void ClosePrizeConfigModal()
        {
            CampaignPrizeConfigModal.Hide();
        }
        
        private void OpenSettingPostPrizeModal(CampaignPrizeDto input)
        {
            CampaignPrize = input;
            SettingPostCampaignPrizeModal.Show();
        }
        
        private async Task UpdatePrizeAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    EditingCampaign = ObjectMapper.Map<CampaignDto, CampaignUpdateDto>(_campaignDto);
                    if (EditingCampaign.CampaignPrizes.FirstOrDefault(_ => _.PrizeName == CampaignPrize.PrizeName) == null) return;
                    CampaignPrize.AppUserId = CurrentUser.Id;
                    EditingCampaign.CampaignPrizes = EditingCampaign.CampaignPrizes.Select
                    (
                        _ =>
                        {
                            if (_.PrizeName == CampaignPrize.PrizeName)
                            {
                                _ = CampaignPrize;
                            }
                            return _;
                        }
                    ).ToList();
                    await CampaignsAppService.UpdateCampaignPrizes(_campaignDto.Id, EditingCampaign);
                    await InitData();
                    SettingPostCampaignPrizeModal.Hide();
                    await InvokeAsync(StateHasChanged);
                },
                L,
                true
            );
        }
        
        private void CloseSettingPrizeModal()
        {
            SettingPostCampaignPrizeModal.Hide();
        }

        // private async Task DeletePrize(CampaignPrizeDto input)
        // {
        //     EditingCampaign = ObjectMapper.Map<CampaignDto, CampaignUpdateDto>(_campaignDto);
        //     if (EditingCampaign.CampaignPrizes.Select(_ => _.PrizeName).Contains(input.PrizeName))
        //     {
        //         var confirmResult = await _uiMessageService.Confirm(L["DeleteConfirmationMessage"]);
        //         if (confirmResult)
        //         {
        //             var success = await Invoke
        //             (
        //                 async () =>
        //                 {
        //                     EditingCampaign.CampaignPrizes.Remove(input);
        //                     await CampaignsAppService.UpdateCampaignPrizes(_campaignDto.Id, EditingCampaign);
        //                     await InitData();
        //                     await InvokeAsync(StateHasChanged);
        //                 },
        //                 L,
        //                 true,
        //                 BlazorComponentBaseActionType.Delete
        //             );
        //         }
        //     }
        // }

        private void DeletePrize(CampaignPrizeDto input)
        {
            _campaignPrizes.Remove(input);
        }

        private void AddPrizeConfig()
        {
            _campaignPrizes.Add(new CampaignPrizeDto());
        }
        
        private void InitCampaignDailyChartConfig()
        {
            _campaignDailyChartConfig = new BarConfig()
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Index.Chart.DailyPostChart"].ToString(),
                        FontColor = ChartColorHelper.PrimaryColor,
                        FontSize = 20
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis> {new BarCategoryAxis {Stacked = true}},
                        YAxes = new List<CartesianAxis> {new BarLinearCartesianAxis {Stacked = true}}
                    }
                }
            };
        }

        private async Task RenderCampaignDailyChart()
        {
            _campaignDailyChartConfig.Data.Labels.Clear();
            _campaignDailyChartConfig.Data.Datasets.Clear();
            var now = DateTime.UtcNow.Date;
            var toDate = new DateTimeOffset
            (
                now.Year,
                now.Month,
                now.Day,
                0,
                0,
                0,
                BrowserDateTime.LocalTimeZoneInfo.BaseUtcOffset
            );
            var fromDate = toDate.AddMonths(-1);
        
            _statistics = await CampaignsAppService.GetCampaignDailyChartStats(_campaignDto.Id, fromDate, toDate);
        
            var datasets = new List<IDataset<int>>();
            var indexLine = 0;
            foreach (var g in _statistics.Data.GroupBy(p => p.Type))
            {
                IDataset<int> datasetAvgPost = new BarDataset<int>(g.Select(_ => _.Value).ToList())
                {
                    Label = g.Key.ToString(),
                    BackgroundColor = ChartColorHelper.GetBorderColor(indexLine),
                    BorderWidth = 1
                };
                datasets.Add(datasetAvgPost);
                indexLine++;
            }
        
            var labels = _statistics.Data.Select(p => p.Display).Distinct().ToArray();
        
            _campaignDailyChartConfig.Data.Labels.AddRange(labels);
        
            _campaignDailyChartConfig.Data.Datasets.AddRange(datasets);
        
            await _campaignDailyChart.Update();
            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _campaignDailyChartConfig.CanvasId,
                false,
                false,
                true
            );
        }

        private void DefaultPrizeConfig()
        {
            var count = _campaignPrizes.Count;
            if (count < 3)
            {
                for (var i = 0; i < 3 - count; i++)
                {
                    var prizeNumbers = _campaignPrizes.Select(_ => _.PrizeNumber).ToList();
                    var num = i + 1;
                    while (prizeNumbers.Contains(num))
                    {
                        num = num + 1;
                    }
                    _campaignPrizes.Add(new CampaignPrizeDto()
                    {
                        PrizeNumber = num
                    });
                }
            }
        }
    }  
}