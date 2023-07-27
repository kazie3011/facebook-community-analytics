using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using JetBrains.Annotations;
using MongoDB.Driver;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.ContractTransactions
{
    public class MongoContractTransactionRepository : MongoDbRepositoryBase<ApiMongoDbContext, ContractTransaction, Guid>, IContractTransactionRepository
    {
        public MongoContractTransactionRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }

        public async Task<List<ContractTransactionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string description = null,
            decimal? partialPaymentValue = null,
            DateTime? paymentDueDateMin = null,
            DateTime? paymentDueDateMax = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? contractId = null,
            Guid? salePersonId = null,
            CancellationToken cancellationToken = default)
        {
            return (await ApplyFilter(filterText, description, partialPaymentValue, paymentDueDateMin, paymentDueDateMax, createdAtMin, createdAtMax, contractId, salePersonId, cancellationToken)).ToList();
        }


      

        public async Task<List<ContractTransaction>> GetListExtendAsync(
            string filterText = null,
            string description = null,
            decimal? partialPaymentValue = null,
            DateTime? paymentDueDateMin = null,
            DateTime? paymentDueDateMax = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? contractId = null,
            Guid? salePersonId = null,
            CancellationToken cancellationToken = default)
        {
            return (await ApplyFilter(filterText, description, partialPaymentValue,paymentDueDateMin, paymentDueDateMax, createdAtMin, createdAtMax, contractId, salePersonId,cancellationToken)).Select(_ => _.ContractTransaction).ToList();
        }

        private async Task<IQueryable<ContractTransactionWithNavigationProperties>> ApplyFilter(
            string filterText = null,
            string description = null,
            decimal? partialPaymentValue = null,
            DateTime? paymentDueDateMin = null,
            DateTime? paymentDueDateMax = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? contractId = null,
            Guid? salePersonId = null,
            CancellationToken cancellationToken = default)
        {
            var contractTransactions = await (await GetMongoQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = from ct in contractTransactions
                join au1 in dbContext.Users.AsQueryable().Where(_ => !_.IsDeleted) on ct.SalePersonId equals au1.Id into t3
                from sp in t3.DefaultIfEmpty()
                join au2 in dbContext.Contracts.AsQueryable() on ct.ContractId equals au2.Id into t1
                from ctr in t1.DefaultIfEmpty()

                select new ContractTransactionWithNavigationProperties()
                {
                    ContractTransaction = ct,
                    SalePerson = sp,
                    Contract = ctr
                };
            return query.AsQueryable()
                .Where(e => e.ContractTransaction != null)
                .WhereIf(filterText.IsNotNullOrWhiteSpace(), _ =>_.ContractTransaction.Description != null && _.ContractTransaction.Description.Contains(filterText)
                                                                 ||(_.Contract != null && _.Contract.ContractCode.Contains(filterText))    
                                                                 || (_.SalePerson != null && _.SalePerson.Name.Contains(filterText)))
                .WhereIf(contractId.IsNotNullOrEmpty(), _ => _.ContractTransaction.ContractId == contractId)
                .WhereIf(salePersonId.IsNotNullOrEmpty(), _ => _.ContractTransaction.SalePersonId == salePersonId)
                .WhereIf(description.IsNotNullOrEmpty(), _ => _.ContractTransaction.Description == description)
                .WhereIf(partialPaymentValue != null, _ => _.ContractTransaction.PartialPaymentValue == partialPaymentValue)
                .WhereIf(paymentDueDateMin != null, _ => _.ContractTransaction.PaymentDueDate >= paymentDueDateMin)
                .WhereIf(paymentDueDateMax != null, _ => _.ContractTransaction.PaymentDueDate <= paymentDueDateMax)
                .WhereIf(createdAtMin != null, _ => _.ContractTransaction.CreatedAt >= createdAtMin)
                .WhereIf(createdAtMax != null, _ => _.ContractTransaction.CreatedAt <= createdAtMax);

        }
    }
}