using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.AccountProxies;

namespace FacebookCommunityAnalytics.Api.Controllers.AccountProxies
{
    [RemoteService]
    [Area("app")]
    [ControllerName("AccountProxy")]
    [Route("api/app/account-proxies")]

    public class AccountProxyController : AbpController, IAccountProxiesAppService
    {
        private readonly IAccountProxiesAppService _accountProxiesAppService;

        public AccountProxyController(IAccountProxiesAppService accountProxiesAppService)
        {
            _accountProxiesAppService = accountProxiesAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<AccountProxyWithNavigationPropertiesDto>> GetListAsync(GetAccountProxiesInput input)
        {
            return _accountProxiesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<AccountProxyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _accountProxiesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<AccountProxyDto> GetAsync(Guid id)
        {
            return _accountProxiesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("account-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAccountLookupAsync(LookupRequestDto input)
        {
            return _accountProxiesAppService.GetAccountLookupAsync(input);
        }

        [HttpGet]
        [Route("proxy-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetProxyLookupAsync(LookupRequestDto input)
        {
            return _accountProxiesAppService.GetProxyLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<AccountProxyDto> CreateAsync(AccountProxyCreateDto input)
        {
            return _accountProxiesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<AccountProxyDto> UpdateAsync(Guid id, AccountProxyUpdateDto input)
        {
            return _accountProxiesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _accountProxiesAppService.DeleteAsync(id);
        }
    }
}