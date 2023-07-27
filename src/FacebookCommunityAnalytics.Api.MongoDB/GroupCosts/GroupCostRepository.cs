using System;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.GroupCosts
{
    public class GroupCostRepository : MongoDbRepositoryBase<ApiMongoDbContext, GroupCost, Guid>,IGroupCostRepository
    {
        public GroupCostRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }
    }
}