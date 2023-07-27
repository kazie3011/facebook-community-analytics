using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class AccountsDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsDataSeedContributor(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await _accountRepository.InsertAsync(new Account
            (
                id: Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9"),
                username: "ed8355f82e0947bb80b72e84c95f5917dc95172a86394b1d93456c5826a7c086",
                password: "cb30157ed9ea49c292967",
                twoFactorCode: "5af65635c37d4bc18af8729d0a4cd85a2ee59da52a3e425291951702c6563bc5239dc0a",
                true,
                AccountType.Unknown
            ));

            await _accountRepository.InsertAsync(new Account
            (
                id: Guid.Parse("f549b911-374a-4720-b250-b65fc9a39424"),
                username: "a8c8bf2d7cbc42588deb4291693780a3749c33f15a8d49909494771fc10295d38c41e52d7c8a4280acbbd645",
                password: "d7e002",
                twoFactorCode: "7e82d84cebda4e208ee23a620e70bed31fd09497fd464d74a8fb60ac45532e2ed4e7c672e1284bb5a1b955cc",
                true,
                AccountType.Unknown
            ));
        }
    }
}