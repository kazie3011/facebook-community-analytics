using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public interface IStaffEvaluationRepository : IRepository<StaffEvaluation, Guid>
    {
        Task<List<StaffEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filter = null,
            Guid? teamId = null,
            List<Guid> teamIds = null,
            Guid? appUserId = null,
            int? month = null,
            int? year = null,
            StaffEvaluationStatus? staffEvaluationStatus = null,
            bool? IsTikTokEvaluation = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            string filter = null,
            Guid? teamId = null,
            List<Guid> teamIds = null,
            Guid? appUserId = null,
            int? month = null,
            int? year = null,
            StaffEvaluationStatus? staffEvaluationStatus = null,
            bool? IsTikTokEvaluation = null,
            CancellationToken cancellationToken = default);

        Task<StaffEvaluationWithNavigationProperties> GetWithNavigationPropertiesByUserAsync(Guid userId,int year, int month, CancellationToken cancellationToken = default);

    }
}