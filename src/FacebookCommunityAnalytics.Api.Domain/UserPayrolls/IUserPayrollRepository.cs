using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public interface IUserPayrollRepository : IRepository<UserPayroll, Guid>
    {
        Task<UserPayrollWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<UserPayrollWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<List<UserPayroll>> GetListAsync(
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}