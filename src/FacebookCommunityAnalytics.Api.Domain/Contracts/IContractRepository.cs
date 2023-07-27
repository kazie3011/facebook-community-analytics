using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public interface IContractRepository : IRepository<Contract, Guid>
    {
        Task<List<ContractWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filter = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            DateTime? signedAtMin = null,
            DateTime? signedAtMax = null,
            ContractStatus? contractStatus = null,
            ContractPaymentStatus? contractPaymentStatus = null,
            Guid? salePersonId = null,
            List<Guid> partnerIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filter = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            DateTime? signedAtMin = null,
            DateTime? signedAtMax = null,
            ContractStatus? contractStatus = null,
            ContractPaymentStatus? contractPaymentStatus = null,
            Guid? salePersonId = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default);

        Task<List<Contract>> GetListExtendAsync(
            string filter = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            DateTime? signedAtMin = null,
            DateTime? signedAtMax = null,
            ContractStatus? contractStatus = null,
            ContractPaymentStatus? contractPaymentStatus = null,
            Guid? salePersonId = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default);

        Task<List<ContractWithNavigationProperties>> GetContractNav(IEnumerable<Guid> contractIds);
    }
}