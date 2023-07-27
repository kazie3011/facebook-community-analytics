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

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public class MongoPayrollRepository : MongoDbRepository<ApiMongoDbContext, Payroll, Guid>, IPayrollRepository
    {
        public MongoPayrollRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Payroll>> GetListAsync(
            string filterText = null,
            string code = null,
            string title = null,
            string description = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            bool? isCompensation = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                code,
                title,
                description,
                fromDateTimeMin,
                fromDateTimeMax,
                toDateTimeMin,
                toDateTimeMax,
                isCompensation
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PayrollConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Payroll>>()
                .PageBy<Payroll, IMongoQueryable<Payroll>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string code = null,
            string title = null,
            string description = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            bool? isCompensation = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                code,
                title,
                description,
                fromDateTimeMin,
                fromDateTimeMax,
                toDateTimeMin,
                toDateTimeMax,
                isCompensation
            );
            return await query.As<IMongoQueryable<Payroll>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Payroll> ApplyFilter(
            IQueryable<Payroll> query,
            string filterText,
            string code = null,
            string title = null,
            string description = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            bool? isCompensation = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code.Contains(filterText) || e.Title.Contains(filterText) || e.Description.Contains(filterText))
                .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                .WhereIf(fromDateTimeMin.HasValue, e => e.FromDateTime >= fromDateTimeMin.Value)
                .WhereIf(fromDateTimeMax.HasValue, e => e.FromDateTime <= fromDateTimeMax.Value)
                .WhereIf(toDateTimeMin.HasValue, e => e.ToDateTime >= toDateTimeMin.Value)
                .WhereIf(toDateTimeMax.HasValue, e => e.ToDateTime <= toDateTimeMax.Value)
                .WhereIf(isCompensation.HasValue, x=>x.IsCompensation == isCompensation);
        }
    }
}