using CanteenManagement.Shared;
using CanteenManagement.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using CanteenManagement.UserInfos;

namespace CanteenManagement.Web.Pages.UserInfos
{
    public class EditModalModel : CanteenManagementPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public UserInfoUpdateDto UserInfo { get; set; }

        public AppUserDto AppUser { get; set; }

        private readonly IUserInfosAppService _userInfosAppService;

        public EditModalModel(IUserInfosAppService userInfosAppService)
        {
            _userInfosAppService = userInfosAppService;
        }

        public async Task OnGetAsync()
        {
            var userInfoWithNavigationPropertiesDto = await _userInfosAppService.GetWithNavigationPropertiesAsync(Id);
            UserInfo = ObjectMapper.Map<UserInfoDto, UserInfoUpdateDto>(userInfoWithNavigationPropertiesDto.UserInfo);

            AppUser = userInfoWithNavigationPropertiesDto.AppUser;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _userInfosAppService.UpdateAsync(Id, UserInfo);
            return NoContent();
        }
    }
}