using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Proxies;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Controllers.Proxies
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ProxyExtend")]
    [Route("api/app/proxies-extend")]
    public class ProxyExtendController : ProxyController, IProxyExtendAppService
    {
        private readonly IProxyExtendAppService _proxyExtendAppService;

        public ProxyExtendController(IProxiesAppService proxiesAppService, IProxyExtendAppService proxyExtendAppService) : base(proxiesAppService)
        {
            _proxyExtendAppService = proxyExtendAppService;
        }

        [HttpPost]
        [Route("import-proxies")]
        public Task ImportProxiesAsync(ProxyImportInput proxyImportInput)
        {
            return _proxyExtendAppService.ImportProxiesAsync(proxyImportInput);
        }

        [HttpGet]
        [Route("get-alives")]
        public Task<List<ProxyDto>> GetAlives(bool deactiveDeadProxies = true)
        {
            return _proxyExtendAppService.GetAlives(deactiveDeadProxies);
        }

        [HttpGet]
        [Route("ping")]
        public Task Ping(bool deactiveDeadProxies = true)
        {
            return _proxyExtendAppService.GetAlives(deactiveDeadProxies);
        }
    }
}