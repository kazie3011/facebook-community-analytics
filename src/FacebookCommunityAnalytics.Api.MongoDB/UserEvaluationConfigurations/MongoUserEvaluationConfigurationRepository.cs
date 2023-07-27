using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Identity;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public class MongoUserEvaluationConfigurationRepository : MongoDbRepositoryBase<ApiMongoDbContext, UserEvaluationConfiguration, Guid>, IUserEvaluationConfigurationRepository
    {
        private readonly IdentityUserManager _identityUserManager;

        public MongoUserEvaluationConfigurationRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider, IdentityUserManager identityUserManager)
            : base(dbContextProvider)
        {
            _identityUserManager = identityUserManager;
        }

        public async Task<UserEvaluationConfiguration> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var query = await GetMongoQueryableAsync(cancellationToken);
            var existedEvaluationConfig = query.FirstOrDefault(x => x.AppUserId == userId);
            if (existedEvaluationConfig != null) return existedEvaluationConfig;

            var user = await _identityUserManager.GetByIdAsync(userId);

            var organizationId = user?.OrganizationUnits?.FirstOrDefault()?.OrganizationUnitId;
            if (organizationId.HasValue)
            {
                existedEvaluationConfig = query.FirstOrDefault(x => x.OrganizationId == organizationId);
            }

            existedEvaluationConfig ??= query.FirstOrDefault(x => x.OrganizationId == null && x.AppUserId == null);
            return existedEvaluationConfig;
        }
        
        public async Task<UserEvaluationConfiguration> GetByTeamId(Guid teamId, CancellationToken cancellationToken)
        {
            var query = await GetMongoQueryableAsync(cancellationToken);
            
            var existedEvaluationConfig = query.FirstOrDefault(x => x.OrganizationId == teamId);
            existedEvaluationConfig ??= query.FirstOrDefault(x => x.OrganizationId == null && x.AppUserId == null);
            
            return existedEvaluationConfig;
        }
    }
}