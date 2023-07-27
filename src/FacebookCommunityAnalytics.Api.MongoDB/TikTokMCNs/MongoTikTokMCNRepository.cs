using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using FacebookCommunityAnalytics.Api.Tiktoks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    public class MongoTikTokMCNRepository : MongoDbRepositoryBase<ApiMongoDbContext, TikTokMCN, Guid>, ITikTokMCNRepository
    {
        public MongoTikTokMCNRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        private IQueryable<TikTokMCN> ApplyFilter(IQueryable<TikTokMCN> query,string filterText)
        {
            query = query.WhereIf(filterText.IsNotNullOrEmpty(), x => x.Name.Contains(filterText) || x.HashTag.Contains(filterText));
            return query;
        }

        public async Task<List<TikTokMCN>> GetListAsync(
            string filterText = null,
            string sorting = null,
            int maxResultCount = Int32.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText);
            var tikTokMCNs = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TiktokMCNConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<TikTokMCN>>()
                .PageBy<TikTokMCN, IMongoQueryable<TikTokMCN>>(skipCount, maxResultCount)
                .ToListAsync
                (
                    GetCancellationToken(cancellationToken)
                );
            return tikTokMCNs;
        }

        public async Task<long> GetCountAsync(string filterText = null, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText);
            return await query.As<IMongoQueryable<TikTokMCN>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }
    }
}