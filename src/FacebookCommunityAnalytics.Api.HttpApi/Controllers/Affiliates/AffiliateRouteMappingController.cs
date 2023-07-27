using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Affiliates;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Affiliates
{
    [RemoteService]
    [Area("app")]
    [ControllerName("AffiliateRouteMapping")]
    [Route("api/app/affiliate-mapping")]
    public class AffiliateRouteMappingController : AbpController, IAffiliateRoutingAppService
    {
        private readonly IAffiliateRoutingAppService _affiliateRoutingAppService;

        public AffiliateRouteMappingController(IAffiliateRoutingAppService affiliateRoutingAppService)
        {
            _affiliateRoutingAppService = affiliateRoutingAppService;
        }

        [HttpGet]
        [Route("get-mapping-url/{key}")]
        public Task<string> GetMappingUrl(string key)
        {
            return _affiliateRoutingAppService.GetMappingUrl(key);
        }
    }
}