using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Proxies;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IAccountProxiesDomainService : IDomainService
    {
        Task RebindAccountProxies();
    }

    public class AccountProxiesDomainService : BaseDomainService, IAccountProxiesDomainService
    {
        private readonly IProxyDomainService _proxyDomainService;
        private readonly IAccountProxyRepository _accountProxyRepository;
        private readonly IRepository<Account, Guid> _accountRepository;

        public AccountProxiesDomainService(
            IRepository<Account, Guid> accountRepository,
            IAccountProxyRepository accountProxyRepository,
            IProxyDomainService proxyDomainService)
        {
            _accountRepository = accountRepository;
            _accountProxyRepository = accountProxyRepository;
            _proxyDomainService = proxyDomainService;
        }

        public async Task RebindAccountProxies()
        {
            await DoRebindDeactiveProxies();
            await DoRebindExistingProxies();
            await DoRebindNewProxies();
        }

        private async Task DoRebindDeactiveProxies()
        {
            var currentAccountProxies = await _accountProxyRepository.GetListAsync();
            var activeProxyIds = (await  _proxyDomainService.GetAlives()).Select(x=>x.Id).ToList();
            var deactiveAccountProxies = currentAccountProxies.Where(acc => !acc.ProxyId.GetValueOrDefault().IsIn(activeProxyIds));
            await _accountProxyRepository.DeleteManyAsync(deactiveAccountProxies);
        }

        private async Task DoRebindNewProxies()
        {
            var aliveProxies = await _proxyDomainService.GetAlives();
            var currentAccountProxies = await _accountProxyRepository.GetListAsync();
            var freeProxies = aliveProxies.Where(ap => currentAccountProxies.All(cap => cap.ProxyId != ap.Id));

            var accounts = await _accountRepository.GetListAsync(acc => acc.IsActive && !acc.IsDeleted && acc.AccountType != AccountType.Unknown);
            var aliveAccounts = accounts.Where(acc => acc.AccountStatus == AccountStatus.Active);
            var freeAccounts = aliveAccounts.Where(acc => currentAccountProxies.All(c => c.AccountId != acc.Id));
            var freeAccounntStack = new Stack<Account>(freeAccounts);

            var newAccountProxies = new List<AccountProxy>();
            foreach (var proxy in freeProxies)
            {
                for (int i = 0; i < GlobalConfiguration.CrawlConfiguration.AccountPerProxy; i++)
                {
                    if (freeAccounntStack.IsNotNullOrEmpty())
                    {
                        var freeAccount = freeAccounntStack.Pop();
                        var entity = new AccountProxy
                        {
                            Description = $"Automated at {DateTime.UtcNow} UTC",
                            AccountId = freeAccount.Id,
                            ProxyId = proxy.Id,
                            TenantId = CurrentTenant.Id
                        };
                        newAccountProxies.Add(entity);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (newAccountProxies.IsNotNullOrEmpty()) { await _accountProxyRepository.InsertManyAsync(newAccountProxies); }
        }

        private async Task DoRebindExistingProxies()
        {
            var currentAccountProxies = await _accountProxyRepository.GetListAsync();

            var accounts = await _accountRepository.GetListAsync(acc => acc.IsActive && !acc.IsDeleted && acc.AccountType != AccountType.Unknown);
            var aliveAccounts = accounts.Where(acc => acc.AccountStatus == AccountStatus.Active);
            var bannedAccounts = accounts.Where(acc => acc.AccountStatus == AccountStatus.Banned || acc.AccountStatus == AccountStatus.LoginApprovalNeeded);
            var bannedAccountIds = bannedAccounts.Select(x => x.Id).ToList();
            var freeAccounts = aliveAccounts.Where(acc => currentAccountProxies.All(c => c.AccountId != acc.Id));
            var freeAccountStacks = new Stack<Account>(freeAccounts);

            var newAccountProxies = new List<AccountProxy>();
            var updateAccountProxies = new List<AccountProxy>();
            foreach (var group in currentAccountProxies.GroupBy(x => x.ProxyId))
            {
                var proxyId = group.Key;
                var accProxies = group.ToList();

                var availableSlot = GlobalConfiguration.CrawlConfiguration.AccountPerProxy - accProxies.Count;
                if (availableSlot > 0)
                {
                    for (int i = 0; i < availableSlot; i++)
                    {
                        if (freeAccountStacks.IsNotNullOrEmpty())
                        {
                            var freeAccont = freeAccountStacks.Pop();
                            var entity = new AccountProxy
                            {   
                                Description = $"Automated at {DateTime.UtcNow} UTC",
                                AccountId = freeAccont.Id,
                                ProxyId = proxyId,
                                TenantId = CurrentTenant.Id
                            };
                            newAccountProxies.Add(entity);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                var bannedAccountProxies = accProxies.Where(x => bannedAccountIds.Contains(x.AccountId.Value)).ToList();
                foreach (var item in bannedAccountProxies)
                {
                    if (freeAccountStacks.IsNotNullOrEmpty())
                    {
                        var freeAccount = freeAccountStacks.Pop();
                        item.AccountId = freeAccount.Id;
                        item.Description = $"Automated at {DateTime.UtcNow} UTC";
                        updateAccountProxies.Add(item);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (newAccountProxies.IsNotNullOrEmpty()) { await _accountProxyRepository.InsertManyAsync(newAccountProxies); }
            if (updateAccountProxies.IsNotNullOrEmpty()) { await _accountProxyRepository.UpdateManyAsync(updateAccountProxies); }
        }
    }
}