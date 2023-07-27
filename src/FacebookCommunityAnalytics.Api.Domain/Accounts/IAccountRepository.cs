using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public interface IAccountRepository : IRepository<Account, Guid>
    {
        Task<List<Account>> GetListAsync(
            string filterText = null,
            string username = null,
            string password = null,
            string twoFactorCode = null,
            AccountType? accountType = null,
            AccountStatus? accountStatus = null,
            AccountCountry? accountCountry = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string username = null,
            string password = null,
            string twoFactorCode = null,
            AccountType? accountType = null,
            AccountStatus? accountStatus = null,
            AccountCountry? accountCountry = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}