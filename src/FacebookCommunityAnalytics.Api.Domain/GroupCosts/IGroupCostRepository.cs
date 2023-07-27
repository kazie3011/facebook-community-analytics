using System;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.GroupCosts
{
    public interface IGroupCostRepository : IRepository<GroupCost, Guid>
    {
        
    }
}