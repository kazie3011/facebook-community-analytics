using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.UserInfos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Partners
{
    public class IndexModel : AbpPageModel
    {
        public IndexModel()
        {
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}