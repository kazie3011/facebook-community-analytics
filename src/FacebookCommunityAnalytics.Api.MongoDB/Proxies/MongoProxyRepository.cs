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

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public class MongoProxyRepository : MongoDbRepository<ApiMongoDbContext, Proxy, Guid>, IProxyRepository
    {
        public MongoProxyRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Proxy>> GetListAsync(
            string filterText = null,
            string ip = null,
            int? portMin = null,
            int? portMax = null,
            string protocol = null,
            string username = null,
            string password = null,
            DateTime? lastPingDateTimeMin = null,
            DateTime? lastPingDateTimeMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, ip, portMin, portMax, protocol, username, password, lastPingDateTimeMin, lastPingDateTimeMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProxyConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Proxy>>()
                .PageBy<Proxy, IMongoQueryable<Proxy>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string ip = null,
           int? portMin = null,
           int? portMax = null,
           string protocol = null,
           string username = null,
           string password = null,
           DateTime? lastPingDateTimeMin = null,
           DateTime? lastPingDateTimeMax = null,
           bool? isActive = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, ip, portMin, portMax, protocol, username, password, lastPingDateTimeMin, lastPingDateTimeMax, isActive);
            return await query.As<IMongoQueryable<Proxy>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Proxy> ApplyFilter(
            IQueryable<Proxy> query,
            string filterText,
            string ip = null,
            int? portMin = null,
            int? portMax = null,
            string protocol = null,
            string username = null,
            string password = null,
            DateTime? lastPingDateTimeMin = null,
            DateTime? lastPingDateTimeMax = null,
            bool? isActive = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Ip.Contains(filterText) || e.Protocol.Contains(filterText) || e.Username.Contains(filterText) || e.Password.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(ip), e => e.Ip.Contains(ip))
                    .WhereIf(portMin.HasValue, e => e.Port >= portMin.Value)
                    .WhereIf(portMax.HasValue, e => e.Port <= portMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(protocol), e => e.Protocol.Contains(protocol))
                    .WhereIf(!string.IsNullOrWhiteSpace(username), e => e.Username.Contains(username))
                    .WhereIf(!string.IsNullOrWhiteSpace(password), e => e.Password.Contains(password))
                    .WhereIf(lastPingDateTimeMin.HasValue, e => e.LastPingDateTime >= lastPingDateTimeMin.Value)
                    .WhereIf(lastPingDateTimeMax.HasValue, e => e.LastPingDateTime <= lastPingDateTimeMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}