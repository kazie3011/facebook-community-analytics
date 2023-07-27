using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class UserCompensationRepository : MongoDbRepository<ApiMongoDbContext, UserCompensation, Guid>, IUserCompensationRepository
    {
        public UserCompensationRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<UserCompensation>> GetListAsync(
            string filterText,
            int? month,
            int? year,
            Guid? payrollId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter(await GetMongoQueryableAsync(cancellationToken), filterText, month, year, payrollId);
            var userCompensations = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserCompensationConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserCompensation>>()
                .PageBy<UserCompensation, IMongoQueryable<UserCompensation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
            return userCompensations;
        }

        public async Task<UserCompensationNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userCompensation = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            return new UserCompensationNavigationProperties()
            {
                UserCompensation = userCompensation,
                Payroll = await (await GetDbContextAsync(cancellationToken)).Payrolls.AsQueryable().FirstOrDefaultAsync(e => e.Id == userCompensation.PayrollId, cancellationToken: cancellationToken),
                AppUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userCompensation.UserId, cancellationToken: cancellationToken),
                UserInfo = await (await GetDbContextAsync(cancellationToken)).UserInfos.AsQueryable().FirstOrDefaultAsync(e => e.AppUserId == userCompensation.UserId, cancellationToken: cancellationToken)
            };
        }

        public async Task<UserCompensationNavigationProperties> GetWithNavigationPropertiesByUserAsync(Guid userId, int month, int year, CancellationToken cancellationToken = default)
        {
            var userCompensation = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => 
                    e.UserId == userId 
                    && e.Month == month 
                    && e.Year == year, GetCancellationToken(cancellationToken));
            if (userCompensation == null)
            {
                return null;
            }
            return new UserCompensationNavigationProperties()
            {
                UserCompensation = userCompensation,
                Payroll = await (await GetDbContextAsync(cancellationToken)).Payrolls.AsQueryable().FirstOrDefaultAsync(e => e.Id == userCompensation.PayrollId, cancellationToken: cancellationToken),
                AppUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userCompensation.UserId, cancellationToken: cancellationToken),
                UserInfo = await (await GetDbContextAsync(cancellationToken)).UserInfos.AsQueryable().FirstOrDefaultAsync(e => e.AppUserId == userCompensation.UserId, cancellationToken: cancellationToken)
            };
        }

        public async Task<List<UserCompensationNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText,
            int? month,
            int? year,
            Guid? payrollId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter(await GetMongoQueryableAsync(cancellationToken), filterText, month, year, payrollId);
            var userCompensations = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserCompensationConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserCompensation>>()
                .PageBy<UserCompensation, IMongoQueryable<UserCompensation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userCompensations.Select
                (
                    x => new UserCompensationNavigationProperties
                    {
                        UserCompensation = x,
                        Payroll = dbContext.Payrolls.AsQueryable().FirstOrDefault(u => u.Id == x.PayrollId),
                        UserInfo = dbContext.UserInfos.AsQueryable().FirstOrDefault(u => u.AppUserId == x.UserId),
                        AppUser = dbContext.Users.AsQueryable().FirstOrDefault(u => u.Id == x.UserId)
                    }
                )
                .ToList();
        }

        public async Task<long> GetLongCountAsync(
            string filterText,
            int? month,
            int? year,
            Guid? payrollId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter(await GetMongoQueryableAsync(cancellationToken), filterText, month, year, payrollId);
            return await query.As<IMongoQueryable<UserCompensation>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        private async Task<IQueryable<UserCompensation>> ApplyFilter(
            IQueryable<UserCompensation> query,
            string filterText,
            int? month,
            int? year,
            Guid? payrollId = null)
        {
            var userIds = new List<Guid>();
            if (filterText.IsNotNullOrEmpty())
            {
                var dbContext = await GetDbContextAsync();
                userIds = dbContext.Users.AsQueryable()
                    .WhereIf
                    (
                        filterText.IsNotNullOrEmpty(),
                        x => x.Email.Contains(filterText)
                             || x.UserName.Contains(filterText)
                    )
                    .Select(x => x.Id)
                    .ToList();
            }

            query = query.WhereIf(userIds.IsNotNullOrEmpty(), x => userIds.Contains(x.UserId))
                .WhereIf(month.HasValue, x => x.Month == month)
                .WhereIf(year.HasValue, x => x.Year == year)
                .WhereIf(payrollId.HasValue, x=> x.PayrollId == payrollId);

            return query;
        }
    }
}