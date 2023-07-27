using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public class MongoUserSalaryConfigurationRepository: MongoDbRepository<ApiMongoDbContext, UserSalaryConfiguration, Guid>, IUserSalaryConfigurationRepository
    {
        public MongoUserSalaryConfigurationRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }


        public Task<List<UserSalaryConfiguration>> GetListAsync(
            string filterText = null,
            string sorting = null,
            int maxResultCount = Int32.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<long> GetCountAsync(Guid? teamId = null, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                teamId,
                userId,
                cancellationToken
            );
            return query.LongCount();
        }

        public async Task<List<UserSalaryConfigurationNavigationProperties>> GetListWithNavigationPropertiesAsync(
            Guid? teamId,
            Guid? userId,
            string inputSorting,
            int maxResultCount,
            int skipCount,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                teamId,
                userId,
                cancellationToken
            );
            
            var userSalaryConfigs = query.Skip(skipCount).Take(maxResultCount).ToList();
            return userSalaryConfigs;
        }
        
         private async Task<IQueryable<UserSalaryConfigurationNavigationProperties>> ApplyFilter(
            Guid? teamId = null,
            Guid? userId = null,
            CancellationToken cancellationToken = default)
        {
            var userSalaryConfigs = await (await GetMongoQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);
            var dbContext = await GetDbContextAsync(cancellationToken);

            var query = from ct in userSalaryConfigs
                join e in Queryable.Where(dbContext.Users.AsQueryable(), _ => !_.IsDeleted) on ct.UserId equals e.Id into t2
                from pn in t2.DefaultIfEmpty()
                join og in Queryable.Where(dbContext.OrganizationUnits.AsQueryable(), _=>!_.IsDeleted) on ct.TeamId equals og.Id into t3
                from ogt in t3.DefaultIfEmpty()
             
                
                select new UserSalaryConfigurationNavigationProperties()
                {
                    UserSalaryConfiguration = ct,
                    AppUser = pn,
                    Team = ogt
                };

            return query.AsQueryable()
                .WhereIf(teamId.IsNotNullOrEmpty(), e => e.UserSalaryConfiguration.TeamId.IsNotNullOrEmpty() && e.UserSalaryConfiguration.TeamId == teamId)
                .WhereIf(userId.IsNotNullOrEmpty(), e => e.UserSalaryConfiguration.UserId.IsNotNullOrEmpty() && e.UserSalaryConfiguration.UserId == userId);
        }
    }
}