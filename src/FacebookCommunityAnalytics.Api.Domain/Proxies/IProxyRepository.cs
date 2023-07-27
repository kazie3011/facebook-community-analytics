using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public interface IProxyRepository : IRepository<Proxy, Guid>
    {
        Task<List<Proxy>> GetListAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}