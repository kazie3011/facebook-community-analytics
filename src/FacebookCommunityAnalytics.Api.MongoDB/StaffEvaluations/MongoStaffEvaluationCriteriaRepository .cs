using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class MongoStaffEvaluationCriteriaRepository : MongoDbRepositoryBase<ApiMongoDbContext, StaffEvaluationCriteria, Guid>, IStaffEvaluationCriteriaRepository
    {
        public MongoStaffEvaluationCriteriaRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }

        public async Task<List<StaffEvaluationCriteria>> GetListAsync(
            string filter = null,
            Guid? teamId = null,
            int? maxPointMin = null,
            int? maxPointMax = null,
            EvaluationType? evaluationType = null,
            int maxResultCount = Int32.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                filter,
                teamId,
                maxPointMin,
                maxPointMax,
                evaluationType,
                cancellationToken
            );
            var staffEvaluationCriteria = query.Skip(skipCount).Take(maxResultCount).ToList();
            return staffEvaluationCriteria;
        }

        public async Task<long> GetCountAsync(
            string filter = null,
            Guid? teamId = null,
            int? maxPointMin = null,
            int? maxPointMax = null,
            EvaluationType? evaluationType = null,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                filter,
                teamId,
                maxPointMin,
                maxPointMax,
                evaluationType,
                cancellationToken
            );
            return query.LongCount();
        }
        
        private async Task<IQueryable<StaffEvaluationCriteria>> ApplyFilter(
            string filter = null,
            Guid? teamId = null,
            int? maxPointMin = null,
            int? maxPointMax = null,
            EvaluationType? evaluationType = null,
            CancellationToken cancellationToken = default)
        {
            var staffEvaluationCriteria = await (await GetMongoQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);

            var query = from ct in staffEvaluationCriteria
                select ct;
            if (filter.IsNotNullOrWhiteSpace()) filter = filter.Trim().ToLower();
            return query.AsQueryable().WhereIf(filter.IsNotNullOrWhiteSpace(), e => e != null && ((e.CriteriaName!= null && e.CriteriaName.ToLower().Contains(filter))
                                                                                                  || (e.Description!= null && e.Description.ToLower().Contains(filter))
                                                                                                  || (e.Note!= null && e.Note.ToLower().Contains(filter))))
                .WhereIf(teamId.HasValue, e => e.TeamId == teamId)
                .WhereIf(maxPointMin.HasValue, e => e.MaxPoint >= maxPointMin.Value)
                .WhereIf(maxPointMax.HasValue, e => e.MaxPoint <= maxPointMax.Value)
                .WhereIf(evaluationType.HasValue, e => e.EvaluationType == evaluationType);
        }
    }
}