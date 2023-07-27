using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Affiliates;
using FacebookCommunityAnalytics.Api.Integrations.Tiki;
using Microsoft.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Controllers
{
    [Route("aff")]
    public class AffController : Controller
    {
        private readonly IAffiliateRoutingAppService _affiliateRoutingAppService;

        public AffController(IAffiliateRoutingAppService affiliateRoutingAppService)
        {
            _affiliateRoutingAppService = affiliateRoutingAppService;
        }

        // GET
        [Route("{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var redirectUrl = await _affiliateRoutingAppService.GetMappingUrl(id);
            return Redirect(redirectUrl);
        }
    }
}