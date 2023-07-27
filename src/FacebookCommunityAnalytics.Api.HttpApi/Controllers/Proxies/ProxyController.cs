using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Proxies;

namespace FacebookCommunityAnalytics.Api.Controllers.Proxies
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Proxy")]
    [Route("api/app/proxies")]

    public class ProxyController : AbpController, IProxiesAppService
    {
        private readonly IProxiesAppService _proxiesAppService;

        public ProxyController(IProxiesAppService proxiesAppService)
        {
            _proxiesAppService = proxiesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ProxyDto>> GetListAsync(GetProxiesInput input)
        {
            return _proxiesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ProxyDto> GetAsync(Guid id)
        {
            return _proxiesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ProxyDto> CreateAsync(ProxyCreateDto input)
        {
            return _proxiesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ProxyDto> UpdateAsync(Guid id, ProxyUpdateDto input)
        {
            return _proxiesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _proxiesAppService.DeleteAsync(id);
        }
    }
}