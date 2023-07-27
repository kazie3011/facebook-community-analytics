using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Categories;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public interface IUserSalaryConfigurationRepository : IRepository<UserSalaryConfiguration, Guid>
    {
        Task<List<UserSalaryConfiguration>> GetListAsync(
            string filterText = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            Guid? teamId = null,
            Guid? userId = null,
            CancellationToken cancellationToken = default);

        Task<List<UserSalaryConfigurationNavigationProperties>> GetListWithNavigationPropertiesAsync(
            Guid? teamId,
            Guid? userId,
            string inputSorting,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
    }
}