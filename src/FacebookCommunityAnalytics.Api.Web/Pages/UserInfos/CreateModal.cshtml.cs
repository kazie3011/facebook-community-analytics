using CanteenManagement.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CanteenManagement.UserInfos;

namespace CanteenManagement.Web.Pages.UserInfos
{
    public class CreateModalModel : CanteenManagementPageModel
    {
        [BindProperty]
        public UserInfoCreateDto UserInfo { get; set; }

        private readonly IUserInfosAppService _userInfosAppService;

        public CreateModalModel(IUserInfosAppService userInfosAppService)
        {
            _userInfosAppService = userInfosAppService;
        }

        public async Task OnGetAsync()
        {
            UserInfo = new UserInfoCreateDto();

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _userInfosAppService.CreateAsync(UserInfo);
            return NoContent();
        }
    }
}