using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api
{
    public class MongoDbRepositoryBase<TMongoDbContext, TEntity, TKey> : MongoDbRepository<TMongoDbContext, TEntity, TKey>
        where TMongoDbContext : IAbpMongoDbContext
        where TEntity : class, IEntity<TKey>
    {
        public MongoDbRepositoryBase(IMongoDbContextProvider<TMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public new virtual async Task<IMongoQueryable<TEntity>> GetMongoQueryableAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken = GetCancellationToken(cancellationToken);

            var dbContext = await GetDbContextAsync(cancellationToken);
            var collection = dbContext.Collection<TEntity>();


            var collation = new Collation("en_US", strength: CollationStrength.Primary);
            var aggregateOptions = new AggregateOptions { Collation = collation, AllowDiskUse = true };
            
            return ApplyDataFilters(
                dbContext.SessionHandle != null
                    ? collection.AsQueryable(dbContext.SessionHandle, aggregateOptions)
                    : collection.AsQueryable(aggregateOptions)
            );
        }
    }
}