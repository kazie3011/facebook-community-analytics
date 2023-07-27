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

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public class MongoUserPayrollCommissionRepository : MongoDbRepository<ApiMongoDbContext, UserPayrollCommission, Guid>, IUserPayrollCommissionRepository
    {
        public MongoUserPayrollCommissionRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<UserPayrollCommissionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userPayrollCommission = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userPayrollCommission.AppUserId, cancellationToken: cancellationToken);
            var payroll = await (await GetDbContextAsync(cancellationToken)).Payrolls.AsQueryable().FirstOrDefaultAsync(e => e.Id == userPayrollCommission.PayrollId, cancellationToken: cancellationToken);

            return new UserPayrollCommissionWithNavigationProperties
            {
                UserPayrollCommission = userPayrollCommission,
                AppUser = appUser,
                Payroll = payroll,

            };
        }

        public async Task<List<UserPayrollCommissionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string organizationId = null,
            string description = null,
            PayrollCommissionType? payrollCommissionType = null,
            double? payrollCommissionMin = null,
            double? payrollCommissionMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            Guid? appUserId = null,
            Guid? payrollId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, organizationId, description, payrollCommissionType, payrollCommissionMin, payrollCommissionMax, amountMin, amountMax, appUserId, payrollId);
            var userPayrollCommissions = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserPayrollCommissionConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserPayrollCommission>>()
                .PageBy<UserPayrollCommission, IMongoQueryable<UserPayrollCommission>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userPayrollCommissions.Select(s => new UserPayrollCommissionWithNavigationProperties
            {
                UserPayrollCommission = s,
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                Payroll = dbContext.Payrolls.AsQueryable().FirstOrDefault(e => e.Id == s.PayrollId && !e.IsDeleted),

            }).ToList();
        }

        public async Task<List<UserPayrollCommission>> GetListAsync(
            string filterText = null,
            string organizationId = null,
            string description = null,
            PayrollCommissionType? payrollCommissionType = null,
            double? payrollCommissionMin = null,
            double? payrollCommissionMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, organizationId, description, payrollCommissionType, payrollCommissionMin, payrollCommissionMax, amountMin, amountMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserPayrollCommissionConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UserPayrollCommission>>()
                .PageBy<UserPayrollCommission, IMongoQueryable<UserPayrollCommission>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string organizationId = null,
           string description = null,
           PayrollCommissionType? payrollCommissionType = null,
           double? payrollCommissionMin = null,
           double? payrollCommissionMax = null,
           decimal? amountMin = null,
           decimal? amountMax = null,
           Guid? appUserId = null,
           Guid? payrollId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, organizationId, description, payrollCommissionType, payrollCommissionMin, payrollCommissionMax, amountMin, amountMax, appUserId, payrollId);
            return await query.As<IMongoQueryable<UserPayrollCommission>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserPayrollCommission> ApplyFilter(
            IQueryable<UserPayrollCommission> query,
            string filterText,
            string organizationId = null,
            string description = null,
            PayrollCommissionType? payrollCommissionType = null,
            double? payrollCommissionMin = null,
            double? payrollCommissionMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            Guid? appUserId = null,
            Guid? payrollId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.OrganizationId.Contains(filterText) || e.Description.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(organizationId), e => e.OrganizationId.Contains(organizationId))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(payrollCommissionType.HasValue, e => e.PayrollCommissionType == payrollCommissionType)
                    .WhereIf(payrollCommissionMin.HasValue, e => e.PayrollCommission >= payrollCommissionMin.Value)
                    .WhereIf(payrollCommissionMax.HasValue, e => e.PayrollCommission <= payrollCommissionMax.Value)
                    .WhereIf(amountMin.HasValue, e => e.Amount >= amountMin.Value)
                    .WhereIf(amountMax.HasValue, e => e.Amount <= amountMax.Value)
                    .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                    .WhereIf(payrollId != null && payrollId != Guid.Empty, e => e.PayrollId == payrollId);
        }
    }
}