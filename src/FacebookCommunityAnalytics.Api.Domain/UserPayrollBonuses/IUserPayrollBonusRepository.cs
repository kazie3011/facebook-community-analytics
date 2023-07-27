using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public interface IUserPayrollBonusRepository : IRepository<UserPayrollBonus, Guid>
    {
        Task<UserPayrollBonusWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<UserPayrollBonusWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<List<UserPayrollBonus>> GetListAsync(
                    string filterText = null,
                    PayrollBonusType? payrollBonusType = null,
                    decimal? amountMin = null,
                    decimal? amountMax = null,
                    string description = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            PayrollBonusType? payrollBonusType = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string description = null,
            Guid? appUserId = null,
            Guid? payrollId = null,
            CancellationToken cancellationToken = default);
    }
}