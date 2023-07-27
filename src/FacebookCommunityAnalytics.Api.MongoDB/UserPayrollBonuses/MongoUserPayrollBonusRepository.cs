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

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public class MongoUserPayrollBonusRepository : MongoDbRepository<ApiMongoDbContext, UserPayrollBonus, Guid>, IUserPayrollBonusRepository
    {
        public MongoUserPayrollBonusRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<UserPayrollBonusWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userPayrollBonus = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userPayrollBonus.AppUserId, cancellationToken: cancellationToken);
            var payroll = await (await GetDbContextAsync(cancellationToken)).Payrolls.AsQueryable().FirstOrDefaultAsync(e => e.Id == userPayrollBonus.PayrollId, cancellationToken: cancellationToken);

            return new UserPayrollBonusWithNavigationProperties
            {
                UserPayrollBonus = userPayrollBonus,
                AppUser = appUser,
                Payroll = payroll,

            };
        }

        public async Task<List<UserPayrollBonusWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            PayrollBonusType? payrollBonusType = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string description = null,
            Guid? appUserId = null,
            Guid? payrollId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, payrollBonusType, amountMin, amountMax, description, appUserId, payrollId);
            var userPayrollBonuses = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserPayrollBonusConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserPayrollBonus>>()
                .PageBy<UserPayrollBonus, IMongoQueryable<UserPayrollBonus>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userPayrollBonuses.Select(s => new UserPayrollBonusWithNavigationProperties
            {
                UserPayrollBonus = s,
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                Payroll = dbContext.Payrolls.AsQueryable().FirstOrDefault(e => e.Id == s.PayrollId && !e.IsDeleted),

            }).ToList();
        }

        public async Task<List<UserPayrollBonus>> GetListAsync(
            string filterText = null,
            PayrollBonusType? payrollBonusType = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string description = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, payrollBonusType, amountMin, amountMax, description);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserPayrollBonusConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UserPayrollBonus>>()
                .PageBy<UserPayrollBonus, IMongoQueryable<UserPayrollBonus>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           PayrollBonusType? payrollBonusType = null,
           decimal? amountMin = null,
           decimal? amountMax = null,
           string description = null,
           Guid? appUserId = null,
           Guid? payrollId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, payrollBonusType, amountMin, amountMax, description, appUserId, payrollId);
            return await query.As<IMongoQueryable<UserPayrollBonus>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserPayrollBonus> ApplyFilter(
            IQueryable<UserPayrollBonus> query,
            string filterText,
            PayrollBonusType? payrollBonusType = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string description = null,
            Guid? appUserId = null,
            Guid? payrollId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description.Contains(filterText))
                    .WhereIf(payrollBonusType.HasValue, e => e.PayrollBonusType == payrollBonusType)
                    .WhereIf(amountMin.HasValue, e => e.Amount >= amountMin.Value)
                    .WhereIf(amountMax.HasValue, e => e.Amount <= amountMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                    .WhereIf(payrollId != null && payrollId != Guid.Empty, e => e.PayrollId == payrollId);
        }
    }
}