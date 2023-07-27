using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class MongoAccountRepository : MongoDbRepository<ApiMongoDbContext, Account, Guid>, IAccountRepository
    {
        public MongoAccountRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Account>> GetListAsync(
            string filterText = null,
            string username = null,
            string password = null,
            string twoFactorCode = null,
            AccountType? accountType = null,
            AccountStatus? accountStatus = null,
            AccountCountry? accountCountry = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                username,
                password,
                twoFactorCode,
                accountType,
                accountStatus,
                accountCountry,
                isActive
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AccountConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Account>>()
                .PageBy<Account, IMongoQueryable<Account>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string username = null,
            string password = null,
            string twoFactorCode = null,
            AccountType? accountType = null,
            AccountStatus? accountStatus = null,
            AccountCountry? accountCountry = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                username,
                password,
                twoFactorCode,
                accountType,
                accountStatus,
                accountCountry,
                isActive
            );
            return await query.As<IMongoQueryable<Account>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Account> ApplyFilter(
            IQueryable<Account> query,
            string filterText,
            string username = null,
            string password = null,
            string twoFactorCode = null,
            AccountType? accountType = null,
            AccountStatus? accountStatus = null,
            AccountCountry? accountCountry = null,
            bool? isActive = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Username.Contains(filterText) || e.Password.Contains(filterText) || e.TwoFactorCode.Contains(filterText))
                .WhereIf(!string.IsNullOrWhiteSpace(username), e => e.Username == username)
                .WhereIf(!string.IsNullOrWhiteSpace(password), e => e.Password == password)
                .WhereIf(!string.IsNullOrWhiteSpace(twoFactorCode), e => e.TwoFactorCode == twoFactorCode)
                .WhereIf(accountType.HasValue, e => e.AccountType == accountType)
                .WhereIf(accountStatus.HasValue, e => e.AccountStatus == accountStatus)
                .WhereIf(accountCountry.HasValue, e => e.AccountCountry == accountCountry)
                .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}