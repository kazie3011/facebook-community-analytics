using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public interface IUserCompensationRepository : IRepository<UserCompensation,Guid>
    {
        Task<List<UserCompensation>> GetListAsync(
            string filterText = null,
            int? month = null,
            int? year = null,
            Guid? payrollId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
        Task<UserCompensationNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<UserCompensationNavigationProperties> GetWithNavigationPropertiesByUserAsync(Guid userId, int month, int year, CancellationToken cancellationToken = default);
        Task<List<UserCompensationNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            int? month = null,
            int? year = null,
            Guid? payrollId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
        Task<long> GetLongCountAsync(
            string filterText = null,
            int? month = null,
            int? year = null,
            Guid? payrollId = null,
            CancellationToken cancellationToken = default);
    }
}