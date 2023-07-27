using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Contracts;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.ContractTransactions
{
    public interface IContractTransactionRepository : IRepository<ContractTransaction, Guid>
    {
        Task<List<ContractTransactionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string description = null,
            decimal? partialPaymentValue = null,
            DateTime? paymentDueDateMin = null,
            DateTime? paymentDueDateMax = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? contractId = null,
            Guid? salePersonId = null,
            CancellationToken cancellationToken = default);
        Task<List<ContractTransaction>> GetListExtendAsync(
            string filterText = null,
            string description = null,
            decimal? partialPaymentValue = null,
            DateTime? paymentDueDateMin = null,
            DateTime? paymentDueDateMax = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? contractId = null,
            Guid? salePersonId = null,
            CancellationToken cancellationToken = default);
    }
}