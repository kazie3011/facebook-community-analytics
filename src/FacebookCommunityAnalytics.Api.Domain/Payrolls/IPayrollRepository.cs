using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public interface IPayrollRepository : IRepository<Payroll, Guid>
    {
        Task<List<Payroll>> GetListAsync(
            string filterText = null,
            string code = null,
            string title = null,
            string description = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            bool? isCompensation =null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string code = null,
            string title = null,
            string description = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            bool? isCompensation =null,
            CancellationToken cancellationToken = default);
    }
}