using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.MongoDB;
using Xunit;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class AccountRepositoryTests : ApiMongoDbTestBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountRepositoryTests()
        {
            _accountRepository = GetRequiredService<IAccountRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _accountRepository.GetListAsync(
                    username: "ed8355f82e0947bb80b72e84c95f5917dc95172a86394b1d93456c5826a7c086",
                    password: "cb30157ed9ea49c292967",
                    twoFactorCode: "5af65635c37d4bc18af8729d0a4cd85a2ee59da52a3e425291951702c6563bc5239dc0a"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _accountRepository.GetCountAsync(
                    username: "a8c8bf2d7cbc42588deb4291693780a3749c33f15a8d49909494771fc10295d38c41e52d7c8a4280acbbd645",
                    password: "d7e002",
                    twoFactorCode: "7e82d84cebda4e208ee23a620e70bed31fd09497fd464d74a8fb60ac45532e2ed4e7c672e1284bb5a1b955cc"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}