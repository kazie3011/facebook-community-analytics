using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public interface IGroupRepository : IRepository<Group, Guid>
    {
        Task<List<Group>> GetListAsync(
            string filterText = null,
            string title = null,
            string fid = null,
            string name = null,
            string altName = null,
            string url = null,
            bool? isActive = null,
            GroupSourceType? groupSourceType = null,
            GroupOwnershipType? groupOwnershipType = null,
            GroupCategoryType? groupCategoryType = null,
            TikTokContractStatus? contractStatus = null,
            List<Guid> MCNHashIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string title = null,
            string fid = null,
            string name = null,
            string altName = null,
            string url = null,
            bool? isActive = null,
            GroupSourceType? groupSourceType = null,
            GroupOwnershipType? groupOwnershipType = null,
            GroupCategoryType? groupCategoryType = null,
            TikTokContractStatus? contractStatus = null,
            List<Guid> MCNHashIds = null,
            CancellationToken cancellationToken = default);
    }
}