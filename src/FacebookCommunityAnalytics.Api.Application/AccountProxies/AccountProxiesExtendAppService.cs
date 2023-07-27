using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.AccountProxies.Default)]
    public class AccountProxiesExtendAppService : AccountProxiesAppService, IAccountProxiesExtendAppService
    {
        private readonly IProxyExtendAppService _proxyExtendAppService;
        private readonly IAccountProxyRepository _accountProxyRepository;
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<Proxy, Guid> _proxyRepository;
        private readonly IAccountProxiesDomainService _accountProxiesDomainService;

        public AccountProxiesExtendAppService(IAccountProxyRepository accountProxyRepository, IRepository<Account, Guid> accountRepository, IRepository<Proxy, Guid> proxyRepository, IProxyExtendAppService proxyExtendAppService,
            IAccountProxiesDomainService accountProxiesDomainService) : base(accountProxyRepository, accountRepository, proxyRepository)
        {
            _accountProxyRepository = accountProxyRepository;
            _accountRepository = accountRepository;
            _proxyRepository = proxyRepository;
            _proxyExtendAppService = proxyExtendAppService;
            _accountProxiesDomainService = accountProxiesDomainService;
        }

        [Authorize(ApiPermissions.AccountProxies.Create)]
        public async Task RebindAccountProxies()
        {
            await _accountProxiesDomainService.RebindAccountProxies();
        }
        
        [AllowAnonymous]
        public virtual async Task<AccountProxyWithNavigationPropertiesDto> GetForTool()
        {
            var input = new GetAccountProxiesInput()
            {
                MaxResultCount = 1000
            };
            var items = await _accountProxyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.AccountId, input.ProxyId, input.Sorting, input.MaxResultCount, input.SkipCount);
            var result = items.FirstOrDefault(_ => _.Proxy.IsActive && _.Account.IsActive);
            return ObjectMapper.Map<AccountProxyWithNavigationProperties, AccountProxyWithNavigationPropertiesDto>(result);
        }
    }
}