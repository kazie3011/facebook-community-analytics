using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Campaigns
{
    public class IndexModel : AbpPageModel
    {
        [BindProperty] public GetCampaignsInput Filter { get; set; } = new();
        public IndexModel()
        {
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}