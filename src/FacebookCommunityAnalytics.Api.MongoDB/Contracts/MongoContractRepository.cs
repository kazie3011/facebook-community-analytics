using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;


namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class MongoContractRepository : MongoDbRepositoryBase<ApiMongoDbContext, Contract, Guid>, IContractRepository
    {
        public MongoContractRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ContractWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                filter,
                createdAtMin,
                createdAtMax,
                signedAtMin,
                signedAtMax,
                contractStatus,
                contractPaymentStatus,
                salePersonId,
                partnerIds,
                cancellationToken
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ContractConsts.GetDefaultSorting(true) : sorting);
            var contracts = query.Skip(skipCount).Take(maxResultCount).ToList();
            return contracts;
        }

        public async Task<long> GetCountAsync(
            string filter = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            DateTime? signedAtMin = null,
            DateTime? signedAtMax = null,
            ContractStatus? contractStatus = null,
            ContractPaymentStatus? contractPaymentStatus = null,
            Guid? salePersonId = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                filter,
                createdAtMin,
                createdAtMax,
                signedAtMin,
                signedAtMax,
                contractStatus,
                contractPaymentStatus,
                salePersonId,
                partnerIds,
                cancellationToken
            );
            return query.LongCount();
        }

        public async Task<List<Contract>> GetListExtendAsync(
            string filter = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            DateTime? signedAtMin = null,
            DateTime? signedAtMax = null,
            ContractStatus? contractStatus = null,
            ContractPaymentStatus? contractPaymentStatus = null,
            Guid? salePersonId = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                filter,
                createdAtMin,
                createdAtMax,
                signedAtMin,
                signedAtMax,
                contractStatus,
                contractPaymentStatus,
                salePersonId,
                partnerIds,
                cancellationToken
            );
            var contracts = query.Select(_ => _.Contract).ToList();
            return contracts;
        }

        public async Task<List<ContractWithNavigationProperties>> GetContractNav(IEnumerable<Guid> contractIds)
        {
            return await ContractApplyFilter(contractIds);
        }
        
        protected virtual IQueryable<Contract> ApplyFilterExtend(
            IQueryable<Contract> query,
            DateTime? createdAtMin,
            DateTime? createdAtMax,
            DateTime? signedAtMin,
            DateTime? signedAtMax,
            ContractStatus? contractStatus,
            ContractPaymentStatus? contractPaymentStatus = null,
            Guid? salePersonId =null,
            List<Guid> partnerIds = null)
        {
            return query
                .WhereIf(createdAtMin.HasValue, e => e.CreatedAt >= createdAtMin.Value)
                .WhereIf(createdAtMax.HasValue, e => e.CreatedAt < createdAtMax.Value)
                .WhereIf(signedAtMin.HasValue, e => e.SignedAt < signedAtMin.Value)
                .WhereIf(signedAtMax.HasValue, e => e.SignedAt < signedAtMax.Value)
                .WhereIf(contractStatus.HasValue, e => e.ContractStatus == contractStatus)
                .WhereIf(contractPaymentStatus.HasValue, e => e.ContractPaymentStatus == contractPaymentStatus)
                .WhereIf(salePersonId != null, e => e.SalePersonId == salePersonId);
        }

        private async Task<IQueryable<ContractWithNavigationProperties>> ApplyFilter(
            string filter = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            DateTime? signedAtMin = null,
            DateTime? signedAtMax = null,
            ContractStatus? contractStatus = null,
            ContractPaymentStatus? contractPaymentStatus = null,
            Guid? salePersonId = null,
            IEnumerable<Guid> partnerIds = null,
            CancellationToken cancellationToken = default)
        {
            var contracts = await (await GetMongoQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);
            var dbContext = await GetDbContextAsync(cancellationToken);

            var query = from ct in contracts
                join t in dbContext.Campaigns.AsQueryable().Where(_ => !_.IsDeleted) on ct.CampaignId equals t.Id into t1
                from cp in t1.DefaultIfEmpty()
                join e in dbContext.Partners.AsQueryable().Where(_ => !_.IsDeleted) on ct.PartnerId equals e.Id into t2
                from pn in t2.DefaultIfEmpty()
                join au1 in dbContext.Users.AsQueryable().Where(_ => !_.IsDeleted) on ct.SalePersonId equals au1.Id into t3
                from sp in t3.DefaultIfEmpty()
                select new ContractWithNavigationProperties()
                {
                    Contract = ct,
                    Campaign = cp,
                    Partner = pn,
                    SalePerson = sp,
                    ContractTransactions = dbContext.ContractTransactions.AsQueryable().Where(t => ct != null && t.ContractId == ct.Id).ToList()
                };
            if (filter.IsNotNullOrWhiteSpace()) filter = filter.Trim().ToLower();
            
            return query.AsQueryable().Where(e=> e.Contract != null)
                .WhereIf
                (
                    filter.IsNotNullOrWhiteSpace(),
                    e => e.Contract != null
                         && ((e.Contract.ContractCode != null && e.Contract.ContractCode.ToLower().Contains(filter))
                             || (e.Contract.Content != null && e.Contract.Content.ToLower().Contains(filter))
                             || (e.Campaign != null && e.Campaign.Name != null && e.Campaign.Name.ToLower().Contains(filter))
                             || (e.Partner != null && e.Partner.Name != null && e.Partner.Name.ToLower().Contains(filter))
                             || (e.SalePerson != null && e.SalePerson.UserName != null && e.SalePerson.UserName.ToLower().Contains(filter)))
                )
                .WhereIf(createdAtMin.HasValue, e => e.Contract.CreatedAt.HasValue && e.Contract.CreatedAt >= createdAtMin)
                .WhereIf(createdAtMax.HasValue, e => e.Contract.CreatedAt.HasValue && e.Contract.CreatedAt <= createdAtMax)
                .WhereIf(signedAtMin.HasValue, e => e.Contract.SignedAt.HasValue && e.Contract.SignedAt >= signedAtMin)
                .WhereIf(signedAtMax.HasValue, e => e.Contract.SignedAt.HasValue && e.Contract.SignedAt <= signedAtMax)
                .WhereIf(contractStatus.HasValue, e => e.Contract.ContractStatus == contractStatus)
                .WhereIf(salePersonId.IsNotNullOrEmpty(), e => e.Contract.SalePersonId == salePersonId)
                .WhereIf(partnerIds.IsNotNullOrEmpty(), e=> e.Contract.PartnerId.HasValue && partnerIds.Contains(e.Contract.PartnerId.Value))
                .WhereIf(contractPaymentStatus.HasValue, e => e.Contract.ContractPaymentStatus == contractPaymentStatus);
        }
        
         private async Task<List<ContractWithNavigationProperties>> ContractApplyFilter(
            IEnumerable<Guid> contractIds,
            CancellationToken cancellationToken = default)
        {
            var contracts = await (await GetMongoQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);
             contracts = contracts.Where(x => x.Id.IsIn(contractIds)).ToList();
             
            var dbContext = await GetDbContextAsync(cancellationToken);

            var query = from ct in contracts
                join t in dbContext.Campaigns.AsQueryable().Where(_ => !_.IsDeleted) on ct.CampaignId equals t.Id into t1
                from cp in t1.DefaultIfEmpty()
                join e in dbContext.Partners.AsQueryable().Where(_ => !_.IsDeleted) on ct.PartnerId equals e.Id into t2
                from pn in t2.DefaultIfEmpty()
                join au1 in dbContext.Users.AsQueryable().Where(_ => !_.IsDeleted) on ct.SalePersonId equals au1.Id into t3
                from sp in t3.DefaultIfEmpty()
                select new ContractWithNavigationProperties()
                {
                    Contract = ct,
                    Campaign = cp,
                    Partner = pn,
                    SalePerson = sp,
                };
            
            return query.ToList();
        }
         
        
    }
}