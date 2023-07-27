using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public interface IPartnerRepository : IRepository<Partner, Guid>
    {
        Task<List<Partner>> GetListAsync(
            string filterText = null,
            string name = null,
            string description = null,
            string url = null,
            string code = null,
            PartnerType? partnerType = null,
            bool? isActive = null,
            Guid? partnerUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string description = null,
            string url = null,
            string code = null,
            PartnerType? partnerType = null,
            bool? isActive = null,
            Guid? partnerUserId = null,
            CancellationToken cancellationToken = default);
    }
}