using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public interface IUncrawledPostRepository : IRepository<UncrawledPost, Guid>
    {
        Task<List<UncrawledPost>> GetListAsync(
            string filterText = null,
            string url = null,
            PostSourceType? postSourceType = null,
            DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string url = null,
            PostSourceType? postSourceType = null,
            DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            CancellationToken cancellationToken = default);
    }
}