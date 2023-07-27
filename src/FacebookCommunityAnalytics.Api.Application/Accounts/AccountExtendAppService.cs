using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class AccountExtendAppService : ApplicationService, IAccountExtendAppService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountExtendAppService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task ImportAccountsAsync(AccountImportInput accountImportInput)
        {
            foreach (var item in accountImportInput.Items)
            {
                var count = await _accountRepository.GetCountAsync(username: item.Username.Trim());
                if (count <= 0)
                {
                    var account = ObjectMapper.Map<AccountImportDto, Account>(item);
                    account.Username = item.Username.Trim();
                    account.AccountStatus = (AccountStatus) item.AccountStatus.EnumParse<AccountStatus>();
                    account.AccountType = (AccountType) item.AccountType.EnumParse<AccountType>();
                    account.AccountCountry = AccountCountry.Vietnam;
                    account.IsActive = true;
                    await _accountRepository.InsertAsync(account);
                }
                else
                {
                    var account = await _accountRepository.GetAsync(x => x.Username ==  item.Username.Trim() && x.IsDeleted == false);
                    account.Password = item.Password;
                    account.TwoFactorCode = item.TwoFactorCode;
                    account.AccountStatus = (AccountStatus) item.AccountStatus.EnumParse<AccountStatus>();
                    account.AccountType = (AccountType) item.AccountType.EnumParse<AccountType>();
                    account.AccountCountry = AccountCountry.Vietnam;
                    account.Email = item.Email;
                    account.EmailPassword = item.EmailPassword;
                    await _accountRepository.UpdateAsync(account);
                }
            }
        }
    }
}