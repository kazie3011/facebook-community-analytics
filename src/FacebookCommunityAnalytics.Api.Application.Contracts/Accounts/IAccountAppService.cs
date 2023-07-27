using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public interface IAccountsAppService : IApplicationService
    {
        Task<PagedResultDto<AccountDto>> GetListAsync(GetAccountsInput input);

        Task<AccountDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<AccountDto> CreateAsync(AccountCreateDto input);

        Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input);
        Task<DeactiveAccountResponse> DeactiveAccount(DeactiveAccountRequest request);
        Task<BannedAccountResponse> BannedAccount(string username);
        Task<List<ExportAccountDto>> GetExportAccounts(GetAccountsInput input);
    }
}