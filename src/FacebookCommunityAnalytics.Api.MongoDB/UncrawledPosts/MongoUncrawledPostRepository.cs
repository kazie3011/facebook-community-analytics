using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public class MongoUncrawledPostRepository : MongoDbRepository<ApiMongoDbContext, UncrawledPost, Guid>, IUncrawledPostRepository
    {
        public MongoUncrawledPostRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<UncrawledPost>> GetListAsync(
            string filterText = null,
            string url = null,
            PostSourceType? postSourceType = null,
            DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, url, postSourceType, updatedAtMin, updatedAtMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UncrawledPostConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UncrawledPost>>()
                .PageBy<UncrawledPost, IMongoQueryable<UncrawledPost>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string url = null,
           PostSourceType? postSourceType = null,
           DateTime? updatedAtMin = null,
           DateTime? updatedAtMax = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, url, postSourceType, updatedAtMin, updatedAtMax);
            return await query.As<IMongoQueryable<UncrawledPost>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UncrawledPost> ApplyFilter(
            IQueryable<UncrawledPost> query,
            string filterText,
            string url = null,
            PostSourceType? postSourceType = null,
            DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null)
        {
            return query
                .Where( e => e.PostSourceType != PostSourceType.Instagram)
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Url.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(url), e => e.Url.Contains(url))
                    .WhereIf(postSourceType.HasValue, e => e.PostSourceType == postSourceType)
                    .WhereIf(updatedAtMin.HasValue, e => e.UpdatedAt >= updatedAtMin.Value)
                    .WhereIf(updatedAtMax.HasValue, e => e.UpdatedAt <= updatedAtMax.Value);
        }
    }
}