using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Channels
{
    public partial class TiktokChannels
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        public PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<LookupDto<Guid>> StaffUsersLookupDtos { get; set; } = new List<LookupDto<Guid>>();
        private IEnumerable<Guid> ModeratorUserIds = new List<Guid>();
        private GroupCreateDto NewGroupCreateDto { get; set; }
        private Modal CreateGroupModal { get; set; }
        private string selectedTab = "GDLExternalChannel";

        private GDLMCNInternalChannel _gdlmcnInternalChannel;
        private GDLMCNExternalChannel _gdlmcnExternalChannel;
        private MCNYANChannel _yanChannel;
        private TikTokMCNDto _mcnGDLTikTok;

        private Validations creationValidations;

        public TiktokChannels()
        {
            NewGroupCreateDto = new GroupCreateDto
            {
                IsActive = true
            };
        }

        protected override async Task OnInitializedAsync()
        {
            //await SetPermissionsAsync();
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["TiktokChannels"].Value}");
                await GetStaffUsersLookupAsync();
                _mcnGDLTikTok = await GroupExtendAppService.GetMCNGDLTikTok();
            }
        }

        // private async Task SetPermissionsAsync()
        // {
        //     CanCreateGroup = await AuthorizationService.IsGrantedAsync(ApiPermissions.Tiktok.Channels);
        // }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Tiktok"]));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:TiktokChannels"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["TiktokChannels.AddTiktokChannel"],
                () =>
                {
                    OpenCreateGroupModal();
                    return Task.CompletedTask;
                },
                IconName.FilePdf,
                requiredPolicyName: ApiPermissions.Tiktok.Channels
            );
            return ValueTask.CompletedTask;
        }
        
        private async Task GetStaffUsersLookupAsync()
        {
            StaffUsersLookupDtos = (await GroupsAppService.GetStaffUserLookupAsync(new LookupRequestDto {MaxResultCount = 1000}));
        }
        private void OpenCreateGroupModal()
        {
            NewGroupCreateDto = new GroupCreateDto { IsActive = true };
            NewGroupCreateDto.GroupOwnershipType = selectedTab switch
            {
                "GDLExternalChannel" => GroupOwnershipType.GDLExternal,
                "GDLInternalChannel" => GroupOwnershipType.GDLInternal,
                "YANChannel" => GroupOwnershipType.YAN,
                _ => NewGroupCreateDto.GroupOwnershipType
            };
            ModeratorUserIds = new List<Guid>();
            CreateGroupModal.Show();
        }

        private async Task DoSearch()
        {
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnSelectedTabChanged(string name)
        {
            selectedTab = name;
            await InvokeAsync(StateHasChanged);
        }

        private void CloseCreateGroupModal()
        {
            CreateGroupModal.Hide();
        }

        private Task CreateModeratorIdsSelectedValuesChanged(object value)
        {
            if (value != null)
            {
                var ids = value is IEnumerable<Guid> guids ? guids.ToList() : new List<Guid> { (Guid)value };
                NewGroupCreateDto.ModeratorIds = ids;
            }
            else
            {
                NewGroupCreateDto.ModeratorIds = new List<Guid>();
            }

            return Task.CompletedTask;
        }

        private async Task CreateGroupAsync()
        {
            await Invoke
            (
                async () =>
                {
                    if (creationValidations.ValidateAll())
                    {
                        NewGroupCreateDto.Name = NewGroupCreateDto.Fid;
                        NewGroupCreateDto.McnId = _mcnGDLTikTok.Id;
                        await GroupsAppService.CreateAsync(NewGroupCreateDto);
                        await DoSearch();
                        CreateGroupModal.Hide();
                    }
                },
                L,
                true
            );
        }
    }
}