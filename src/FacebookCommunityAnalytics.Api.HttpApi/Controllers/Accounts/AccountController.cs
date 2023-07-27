using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Accounts;

namespace FacebookCommunityAnalytics.Api.Controllers.Accounts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Account")]
    [Route("api/app/accounts")]

    public class AccountController : AbpController, IAccountsAppService
    {
        private readonly IAccountsAppService _accountsAppService;

        public AccountController(IAccountsAppService accountsAppService)
        {
            _accountsAppService = accountsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<AccountDto>> GetListAsync(GetAccountsInput input)
        {
            return _accountsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<AccountDto> GetAsync(Guid id)
        {
            return _accountsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<AccountDto> CreateAsync(AccountCreateDto input)
        {
            return _accountsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
        {
            return _accountsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _accountsAppService.DeleteAsync(id);
        }
        
        [HttpPost]
        [Route("deactive/{username}")]
        public virtual Task<DeactiveAccountResponse> DeactiveAccount(DeactiveAccountRequest request)
        {
            return _accountsAppService.DeactiveAccount(request);
        }
        
        [HttpPost]
        [Route("banned/{username}")]
        public virtual Task<BannedAccountResponse> BannedAccount(string username)
        {
            return _accountsAppService.BannedAccount(username);
        }

        [HttpPost]
        [Route("get-export-accounts")]
        public Task<List<ExportAccountDto>> GetExportAccounts(GetAccountsInput input)
        {
            return _accountsAppService.GetExportAccounts(input);
        }
    }
}