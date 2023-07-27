using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public interface IUserPayrollCommissionRepository : IRepository<UserPayrollCommission, Guid>
    {
        Task<UserPayrollCommissionWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<UserPayrollCommissionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<List<UserPayrollCommission>> GetListAsync(
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}