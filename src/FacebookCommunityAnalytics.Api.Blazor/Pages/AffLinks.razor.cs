using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class AffLinks
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        private UserAffiliateCreateDto NewUserAffiliate { get; set; }
        private Validations NewUserAffiliateValidations { get; set; }

        private bool _disableAfterGetAffiliateUrl;
        private bool _disableShortlinkButton;

        private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> CategoriesNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> CampaignsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> GroupsNullable { get; set; } = new List<LookupDto<Guid?>>();

        private IList<GroupDto> _groupDtos = new List<GroupDto>();

        public AffLinks()
        {
            NewUserAffiliate = new UserAffiliateCreateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await GetNullableCampaignLookupAsync("");
            await GetNullablePartnerLookupAsync("");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["Aff.Links.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Aff.Links.Title"], "/afflink"));
            return ValueTask.CompletedTask;
        }

        private async Task ClearUserAffiliateAsync()
        {
            NewUserAffiliate = new UserAffiliateCreateDto();
            _disableAfterGetAffiliateUrl = false;
            _disableShortlinkButton = false;
            await JsRuntime.InvokeVoidAsync("clipboardCopy.resetButton");
        }


        private async Task GetNullableCampaignLookupAsync(string newValue)
        {
            CampaignsNullable = (await UserAffiliateAppService.GetCampaignLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullablePartnerLookupAsync(string newValue)
        {
            PartnersNullable = (await UserAffiliateAppService.GetPartnerLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetAffiliateUrlAsync(string url, bool isHappyDay)
        {
            if (CurrentUser.Id == null || url.HasNoValue()) return;
            NewUserAffiliate.AffiliateProviderType = AffiliateProviderType.Shopiness;

            if (url.ToLower().Contains(MarketplaceType.Shopee.ToString().ToLower()))
            {
                NewUserAffiliate.MarketplaceType = MarketplaceType.Shopee;
            }
            else if (url.ToLower().Contains(MarketplaceType.Lazada.ToString().ToLower()))
            {
                NewUserAffiliate.MarketplaceType = MarketplaceType.Lazada;
            }
            else if (url.ToLower().Contains(MarketplaceType.Tiki.ToString().ToLower()))
            {
                NewUserAffiliate.MarketplaceType = MarketplaceType.Tiki;
            }
            else
            {
                await Message.Error(L[ApiDomainErrorCodes.UserAffiliates.InvalidUrl]);
                return;
            }

            var userInfoWithNavigationProperties = await UserInfosAppService.GetByUserIdAsync(CurrentUser.Id.Value);
            var groupFid = string.Empty;
            if (NewUserAffiliate.GroupId != null) { groupFid = _groupDtos.FirstOrDefault(_ => _.Id == NewUserAffiliate.GroupId)?.Fid; }

            var affiliate = await UserAffiliateAppService.GenerateAffiliate
            (
                new GenerateShortlinkApiRequest
                {
                    Link = url,
                    UserCode = userInfoWithNavigationProperties.UserInfo.Code,
                    MarketplaceType = NewUserAffiliate.MarketplaceType,
                    AffiliateProviderType = AffiliateProviderType.Shopiness,
                    CommunityFid = groupFid,
                    CampaignId = NewUserAffiliate.CampaignId?.ToString("N"),
                    IsHappyDay = isHappyDay
                }
            );
            if (affiliate == null)
            {
                await Message.Error(L[ApiDomainErrorCodes.UserAffiliates.ShortlinkCreationFailure]);
                return;
            }
            else
            {
                NewUserAffiliate.AffiliateUrl = affiliate.Shortlink;
                NewUserAffiliate.Url = affiliate.Link;
                //await CopyToClipboard(affiliate.ShortUrl);
                _disableAfterGetAffiliateUrl = true;
                _disableShortlinkButton = true;

                await CreateUserAffiliateAsync();
            }
        }

        private async Task CreateUserAffiliateAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    NewUserAffiliate.CreatedAt = DateTime.UtcNow;
                    NewUserAffiliate.AppUserId = CurrentUser.Id;

                    await UserAffiliateAppService.CreateAsync(NewUserAffiliate);
                    await JsRuntime.InvokeVoidAsync("clipboardCopy.resetButton");
                    _disableShortlinkButton = false;
                },
                L,
                true,
                actionType: BlazorComponentBaseActionType.Create
            );
            await InvokeAsync(StateHasChanged);
        }
    }
}