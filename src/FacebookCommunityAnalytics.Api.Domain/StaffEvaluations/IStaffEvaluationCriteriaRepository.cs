using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public interface IStaffEvaluationCriteriaRepository : IRepository<StaffEvaluationCriteria, Guid>
    {
        Task<List<StaffEvaluationCriteria>> GetListAsync(
            string filter = null,
            Guid? teamId = null,
            int? maxPointMin = null,
            int? maxPointMax = null,
            EvaluationType? evaluationType = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            string filter = null,
            Guid? teamId = null,
            int? maxPointMin = null,
            int? maxPointMax = null,
            EvaluationType? evaluationType = null,
            CancellationToken cancellationToken = default);
    }
    
}