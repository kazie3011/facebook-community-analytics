using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Anonymous;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Anonymous
{
    [AllowAnonymous]
    [Area("app")]
    [ControllerName("Anonymous")]
    [Route("api/app/anonymous")]
    public class AnonymousController : AbpController
    {
        private readonly IAnonymousAppService _anonymousAppService;
        public AnonymousController(IAnonymousAppService anonymousAppService)
        {
            _anonymousAppService = anonymousAppService;
        }
        
        [HttpPost]
        [Route("Email")]
        public async Task Email()
        {
            var formCollection = await Request.ReadFormAsync();
            _anonymousAppService.ProcessEmail(formCollection);
        }

        [HttpGet]
        [Route("SecurityCode")]
        public string GetSecurityCode()
        {
            return _anonymousAppService.GetSecurityCode();
        }
    }
}