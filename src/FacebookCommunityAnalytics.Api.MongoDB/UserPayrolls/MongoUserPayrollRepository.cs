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

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class MongoUserPayrollRepository : MongoDbRepository<ApiMongoDbContext, UserPayroll, Guid>, IUserPayrollRepository
    {
        public MongoUserPayrollRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<UserPayrollWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userPayroll = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var payroll = await (await GetDbContextAsync(cancellationToken)).Payrolls.AsQueryable().FirstOrDefaultAsync(e => e.Id == userPayroll.PayrollId, cancellationToken: cancellationToken);
            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userPayroll.AppUserId, cancellationToken: cancellationToken);

            return new UserPayrollWithNavigationProperties
            {
                UserPayroll = userPayroll,
                Payroll = payroll,
                AppUser = appUser,

            };
        }

        public async Task<List<UserPayrollWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string code = null,
            string organizationId = null,
            ContentRoleType? contentRoleType = null,
            double? affiliateMultiplierMin = null,
            double? affiliateMultiplierMax = null,
            double? seedingMultiplierMin = null,
            double? seedingMultiplierMax = null,
            string description = null,
            decimal? waveAmountMin = null,
            decimal? waveAmountMax = null,
            decimal? bonusAmountMin = null,
            decimal? bonusAmountMax = null,
            decimal? totalAmountMin = null,
            decimal? totalAmountMax = null,
            Guid? payrollId = null,
            Guid? appUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, organizationId, contentRoleType, affiliateMultiplierMin, affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, description, waveAmountMin, waveAmountMax, bonusAmountMin, bonusAmountMax, totalAmountMin, totalAmountMax, payrollId, appUserId);
            var userPayrolls = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserPayrollConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserPayroll>>()
                .PageBy<UserPayroll, IMongoQueryable<UserPayroll>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userPayrolls.Select(s => new UserPayrollWithNavigationProperties
            {
                UserPayroll = s,
                Payroll = dbContext.Payrolls.AsQueryable().FirstOrDefault(e => e.Id == s.PayrollId && !e.IsDeleted),
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),

            }).ToList();
        }

        public async Task<List<UserPayroll>> GetListAsync(
            string filterText = null,
            string code = null,
            string organizationId = null,
            ContentRoleType? contentRoleType = null,
            double? affiliateMultiplierMin = null,
            double? affiliateMultiplierMax = null,
            double? seedingMultiplierMin = null,
            double? seedingMultiplierMax = null,
            string description = null,
            decimal? waveAmountMin = null,
            decimal? waveAmountMax = null,
            decimal? bonusAmountMin = null,
            decimal? bonusAmountMax = null,
            decimal? totalAmountMin = null,
            decimal? totalAmountMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, organizationId, contentRoleType, affiliateMultiplierMin, affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, description, waveAmountMin, waveAmountMax, bonusAmountMin, bonusAmountMax, totalAmountMin, totalAmountMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserPayrollConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UserPayroll>>()
                .PageBy<UserPayroll, IMongoQueryable<UserPayroll>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string code = null,
           string organizationId = null,
           ContentRoleType? contentRoleType = null,
           double? affiliateMultiplierMin = null,
           double? affiliateMultiplierMax = null,
           double? seedingMultiplierMin = null,
           double? seedingMultiplierMax = null,
           string description = null,
           decimal? waveAmountMin = null,
           decimal? waveAmountMax = null,
           decimal? bonusAmountMin = null,
           decimal? bonusAmountMax = null,
           decimal? totalAmountMin = null,
           decimal? totalAmountMax = null,
           Guid? payrollId = null,
           Guid? appUserId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, organizationId, contentRoleType, affiliateMultiplierMin, affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, description, waveAmountMin, waveAmountMax, bonusAmountMin, bonusAmountMax, totalAmountMin, totalAmountMax, payrollId, appUserId);
            return await query.As<IMongoQueryable<UserPayroll>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserPayroll> ApplyFilter(
            IQueryable<UserPayroll> query,
            string filterText,
            string code = null,
            string organizationId = null,
            ContentRoleType? contentRoleType = null,
            double? affiliateMultiplierMin = null,
            double? affiliateMultiplierMax = null,
            double? seedingMultiplierMin = null,
            double? seedingMultiplierMax = null,
            string description = null,
            decimal? waveAmountMin = null,
            decimal? waveAmountMax = null,
            decimal? bonusAmountMin = null,
            decimal? bonusAmountMax = null,
            decimal? totalAmountMin = null,
            decimal? totalAmountMax = null,
            Guid? payrollId = null,
            Guid? appUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code.Contains(filterText) || e.OrganizationId.Contains(filterText) || e.Description.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                    .WhereIf(!string.IsNullOrWhiteSpace(organizationId), e => e.OrganizationId.Contains(organizationId))
                    .WhereIf(contentRoleType.HasValue, e => e.ContentRoleType == contentRoleType)
                    .WhereIf(affiliateMultiplierMin.HasValue, e => e.AffiliateMultiplier >= affiliateMultiplierMin.Value)
                    .WhereIf(affiliateMultiplierMax.HasValue, e => e.AffiliateMultiplier <= affiliateMultiplierMax.Value)
                    .WhereIf(seedingMultiplierMin.HasValue, e => e.SeedingMultiplier >= seedingMultiplierMin.Value)
                    .WhereIf(seedingMultiplierMax.HasValue, e => e.SeedingMultiplier <= seedingMultiplierMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(waveAmountMin.HasValue, e => e.WaveAmount >= waveAmountMin.Value)
                    .WhereIf(waveAmountMax.HasValue, e => e.WaveAmount <= waveAmountMax.Value)
                    .WhereIf(bonusAmountMin.HasValue, e => e.BonusAmount >= bonusAmountMin.Value)
                    .WhereIf(bonusAmountMax.HasValue, e => e.BonusAmount <= bonusAmountMax.Value)
                    .WhereIf(totalAmountMin.HasValue, e => e.TotalAmount >= totalAmountMin.Value)
                    .WhereIf(totalAmountMax.HasValue, e => e.TotalAmount <= totalAmountMax.Value)
                    .WhereIf(payrollId != null && payrollId != Guid.Empty, e => e.PayrollId == payrollId)
                    .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId);
        }
    }
}