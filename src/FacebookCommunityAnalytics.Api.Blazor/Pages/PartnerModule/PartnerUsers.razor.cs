using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class PartnerUsers : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; set; } = new();
        
        private string Email { get; set; }
        private string Name { get; set; }
        private string SurName { get; set; }

        private List<AppUserDto> AppUserDtos { get; set; }

        public PartnerUsers()
        {
            AppUserDtos = new List<AppUserDto>();
        }

        protected override async Task OnInitializedAsync()
        {
           // await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await DoGetPartnerUsersAsync();
        }
        
        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Partner.Users"]));
            return ValueTask.CompletedTask;
        }

        private async Task DeletePartnerUserAsync(AppUserDto input)
        {
            var confirmResult = await UiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                await _partnerModuleAppService.RemovePartnerUser(input.Id);
                await DoGetPartnerUsersAsync();
            }
        }
        private async Task DoGetPartnerUsersAsync()
        {
            AppUserDtos = await _partnerModuleAppService.GetPartnerUsers();
        }

        private async Task DoCreateUserAsync()
        {
            if (await _partnerModuleAppService.UserExist(Email))
            {
                await UiMessageService.Warn(L["Partner.User.EmailExisted"]);
            }
            else
            {
                await _partnerModuleAppService.AddPartnerUser(Email, Name, SurName);
                await DoGetPartnerUsersAsync();
            }
        }
    }
}