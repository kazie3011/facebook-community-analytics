using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Categories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<List<Category>> GetListAsync(
            string filterText = null,
            string name = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            CancellationToken cancellationToken = default);

        Task<List<Category>> GetsByName(string filterText, CancellationToken cancellationToken = default);
    }
}