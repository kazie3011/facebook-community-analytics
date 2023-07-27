using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public class MongoAccountProxyRepository : MongoDbRepository<ApiMongoDbContext, AccountProxy, Guid>, IAccountProxyRepository
    {
        public MongoAccountProxyRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<AccountProxyWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var accountProxy = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var account = await (await GetDbContextAsync(cancellationToken)).Accounts.AsQueryable().FirstOrDefaultAsync(e => e.Id == accountProxy.AccountId, cancellationToken: cancellationToken);
            var proxy = await (await GetDbContextAsync(cancellationToken)).Proxies.AsQueryable().FirstOrDefaultAsync(e => e.Id == accountProxy.ProxyId, cancellationToken: cancellationToken);

            return new AccountProxyWithNavigationProperties
            {
                AccountProxy = accountProxy,
                Account = account,
                Proxy = proxy,

            };
        }

        public async Task<List<AccountProxyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string description = null,
            Guid? accountId = null,
            Guid? proxyId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, description, accountId, proxyId);

            var accountProxies = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AccountProxyConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<AccountProxy>>()
                .PageBy<AccountProxy, IMongoQueryable<AccountProxy>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return accountProxies.Select(s => new AccountProxyWithNavigationProperties
            {
                AccountProxy = s,
                Account = dbContext.Accounts.AsQueryable().FirstOrDefault(e => e.Id == s.AccountId && e.IsActive && !e.IsDeleted),
                Proxy = dbContext.Proxies.AsQueryable().FirstOrDefault(e => e.Id == s.ProxyId && e.IsActive && !e.IsDeleted),

            }).ToList();
        }

        public async Task<List<AccountProxy>> GetListAsync(
            string filterText = null,
            string description = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, description);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AccountProxyConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<AccountProxy>>()
                .PageBy<AccountProxy, IMongoQueryable<AccountProxy>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string description = null,
           Guid? accountId = null,
           Guid? proxyId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, description, accountId, proxyId);
            return await query.As<IMongoQueryable<AccountProxy>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<AccountProxy> ApplyFilter(
            IQueryable<AccountProxy> query,
            string filterText,
            string description = null,
            Guid? accountId = null,
            Guid? proxyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(accountId != null && accountId != Guid.Empty, e => e.AccountId == accountId)
                    .WhereIf(proxyId != null && proxyId != Guid.Empty, e => e.ProxyId == proxyId);
        }
    }
}