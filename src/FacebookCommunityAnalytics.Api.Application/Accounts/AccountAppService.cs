using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Mail;
using FacebookCommunityAnalytics.Api.AccountProxies;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Accounts.Default)]
    public class AccountsAppService : ApplicationService, IAccountsAppService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountProxiesExtendAppService _accountProxiesExtendAppService;

        public AccountsAppService(IAccountRepository accountRepository, IAccountProxiesExtendAppService accountProxiesExtendAppService)
        {
            _accountRepository = accountRepository;
            _accountProxiesExtendAppService = accountProxiesExtendAppService;
        }

        public virtual async Task<PagedResultDto<AccountDto>> GetListAsync(GetAccountsInput input)
        {
            var totalCount = await _accountRepository.GetCountAsync
            (
                input.FilterText,
                input.Username,
                input.Password,
                input.TwoFactorCode,
                input.AccountType,
                input.AccountStatus,
                input.AccountCountry,
                input.IsActive
            );
            var items = await _accountRepository.GetListAsync
            (
                input.FilterText,
                input.Username,
                input.Password,
                input.TwoFactorCode,
                input.AccountType,
                input.AccountStatus,
                input.AccountCountry,
                input.IsActive,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<AccountDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Account>, List<AccountDto>>(items)
            };
        }

        public virtual async Task<List<ExportAccountDto>> GetExportAccounts(GetAccountsInput input)
        {
            var items = await _accountRepository.GetListAsync
            (
                input.FilterText,
                input.Username,
                input.Password,
                input.TwoFactorCode,
                input.AccountType,
                input.AccountStatus,
                input.AccountCountry,
                input.IsActive,
                input.Sorting
            );

            return ObjectMapper.Map<List<Account>, List<ExportAccountDto>>(items);
        }

        public virtual async Task<AccountDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Account, AccountDto>(await _accountRepository.GetAsync(id));
        }

        [Authorize(ApiPermissions.Accounts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _accountRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.Accounts.Create)]
        public virtual async Task<AccountDto> CreateAsync(AccountCreateDto input)
        {
            var account = ObjectMapper.Map<AccountCreateDto, Account>(input);
            account.TenantId = CurrentTenant.Id;
            account = await _accountRepository.InsertAsync(account, autoSave: true);
            return ObjectMapper.Map<Account, AccountDto>(account);
        }

        [Authorize(ApiPermissions.Accounts.Edit)]
        public virtual async Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
        {
            var account = await _accountRepository.GetAsync(id);
            ObjectMapper.Map(input, account);
            account = await _accountRepository.UpdateAsync(account);
            return ObjectMapper.Map<Account, AccountDto>(account);
        }

        public virtual async Task<DeactiveAccountResponse> DeactiveAccount(DeactiveAccountRequest request)
        {
            var accounts = await _accountRepository.GetListAsync(a => a.Username == request.Username);
            if (accounts.Count == 0) return new DeactiveAccountResponse();

            foreach (var account in accounts)
            {
                account.IsActive = false;
                await _accountRepository.UpdateAsync(account);
            }

            await _accountProxiesExtendAppService.RebindAccountProxies();

            return new DeactiveAccountResponse();
        }

        public virtual async Task<BannedAccountResponse> BannedAccount(string username)
        {
            var accounts = await _accountRepository.GetListAsync(a => a.Username == username);
            if (accounts.Count == 0) return new BannedAccountResponse();

            foreach (var account in accounts)
            {
                account.AccountStatus = AccountStatus.Banned;
                await _accountRepository.UpdateAsync(account);
            }

            return new BannedAccountResponse() {Username = username};
        }
    }
}