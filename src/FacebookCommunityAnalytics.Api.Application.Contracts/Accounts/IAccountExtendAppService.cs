using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public interface IAccountExtendAppService : IApplicationService
    {
        Task ImportAccountsAsync(AccountImportInput accountImportInput);
    }
}
