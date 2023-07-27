using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    public interface ITikTokMCNRepository : IRepository<TikTokMCN, Guid>
    {
        Task<List<TikTokMCN>> GetListAsync(
            string filterText = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(string filterText = null, CancellationToken cancellationToken = default);
    }
}