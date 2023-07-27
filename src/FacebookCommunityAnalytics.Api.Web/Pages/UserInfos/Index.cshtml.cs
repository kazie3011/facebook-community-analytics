using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserInfos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FacebookCommunityAnalytics.Api.Web.Pages.UserInfos
{
    public class IndexModel : AbpPageModel
    {
        public GetExtendUserInfosInput Filter { get; set; }

        private readonly IUserInfosAppService _userInfosAppService;

        public IndexModel(IUserInfosAppService userInfosAppService)
        {
            _userInfosAppService = userInfosAppService;
            Filter = new GetExtendUserInfosInput();
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}