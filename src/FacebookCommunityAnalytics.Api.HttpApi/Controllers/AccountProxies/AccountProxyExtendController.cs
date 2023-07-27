using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AccountProxies;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Controllers.AccountProxies
{
    [RemoteService]
    [Area("app")]
    [ControllerName("AccountProxyExtend")]
    [Route("api/app/account-proxies-extend")]

    public class AccountProxyExtendController : AccountProxyController, IAccountProxiesExtendAppService
    {
        private readonly IAccountProxiesExtendAppService _accountProxiesExtendAppService;
        public AccountProxyExtendController(IAccountProxiesAppService accountProxiesAppService, IAccountProxiesExtendAppService accountProxiesAppService1) : base(accountProxiesAppService)
        {
            _accountProxiesExtendAppService = accountProxiesAppService1;
        }

        [HttpGet]
        [Route("rebind")]
        public virtual Task RebindAccountProxies()
        {
            return _accountProxiesExtendAppService.RebindAccountProxies();
        }
        
        [HttpGet]
        [Route("get-for-tool")]
        public Task<AccountProxyWithNavigationPropertiesDto> GetForTool()
        {
            return _accountProxiesExtendAppService.GetForTool();
        }
    }
}