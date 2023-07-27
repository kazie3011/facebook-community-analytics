using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.Accounts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.AccountProxies;
using IdentityServer4.Validation;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.AccountProxies.Default)]
    public class AccountProxiesAppService : ApplicationService, IAccountProxiesAppService
    {
        private readonly IAccountProxyRepository _accountProxyRepository;
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<Proxy, Guid> _proxyRepository;

        public AccountProxiesAppService(IAccountProxyRepository accountProxyRepository, IRepository<Account, Guid> accountRepository, IRepository<Proxy, Guid> proxyRepository)
        {
            _accountProxyRepository = accountProxyRepository; 
            _accountRepository = accountRepository;
            _proxyRepository = proxyRepository;
        }

        public virtual async Task<PagedResultDto<AccountProxyWithNavigationPropertiesDto>> GetListAsync(GetAccountProxiesInput input)
        {
            var totalCount = await _accountProxyRepository.GetCountAsync(input.FilterText, input.Description, input.AccountId, input.ProxyId);
            var items = await _accountProxyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.AccountId, input.ProxyId, input.Sorting, input.MaxResultCount, input.SkipCount);
            
            return new PagedResultDto<AccountProxyWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AccountProxyWithNavigationProperties>, List<AccountProxyWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<AccountProxyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<AccountProxyWithNavigationProperties, AccountProxyWithNavigationPropertiesDto>
                (await _accountProxyRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<AccountProxyDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<AccountProxy, AccountProxyDto>(await _accountProxyRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetAccountLookupAsync(LookupRequestDto input)
        {
            var query = _accountRepository.AsQueryable()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Username != null &&
                         x.Username.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Account>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Account>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetProxyLookupAsync(LookupRequestDto input)
        {
            var query = _proxyRepository.AsQueryable()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Ip != null &&
                         x.Ip.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Proxy>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Proxy>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        [Authorize(ApiPermissions.AccountProxies.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _accountProxyRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.AccountProxies.Create)]
        public virtual async Task<AccountProxyDto> CreateAsync(AccountProxyCreateDto input)
        {

            var accountProxy = ObjectMapper.Map<AccountProxyCreateDto, AccountProxy>(input);
            accountProxy.TenantId = CurrentTenant.Id;
            accountProxy = await _accountProxyRepository.InsertAsync(accountProxy, autoSave: true);
            return ObjectMapper.Map<AccountProxy, AccountProxyDto>(accountProxy);
        }

        [Authorize(ApiPermissions.AccountProxies.Edit)]
        public virtual async Task<AccountProxyDto> UpdateAsync(Guid id, AccountProxyUpdateDto input)
        {

            var accountProxy = await _accountProxyRepository.GetAsync(id);
            ObjectMapper.Map(input, accountProxy);
            accountProxy = await _accountProxyRepository.UpdateAsync(accountProxy);
            return ObjectMapper.Map<AccountProxy, AccountProxyDto>(accountProxy);
        }
    }
}