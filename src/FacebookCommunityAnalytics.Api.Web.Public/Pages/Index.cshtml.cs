using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.CmsSites;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace FacebookCommunityAnalytics.Api.Web.Public.Pages
{
    public class IndexModel : ApiPublicPageModel
    {
        private readonly ICmsSiteAppService _cmsSiteAppService;
        private CmsSiteDto CmsSiteSetting { get; set; }
        public IndexModel(ICmsSiteAppService cmsSiteAppService)
        {
            _cmsSiteAppService = cmsSiteAppService;
        }

        public async Task OnGetAsync()
        {
            CmsSiteSetting = await _cmsSiteAppService.GetCurrentSite();
        }
        
        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
    }
}
