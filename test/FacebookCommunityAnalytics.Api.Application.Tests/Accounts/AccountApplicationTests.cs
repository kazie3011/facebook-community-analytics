using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class AccountsAppServiceTests : ApiApplicationTestBase
    {
        private readonly IAccountsAppService _accountsAppService;
        private readonly IRepository<Account, Guid> _accountRepository;

        public AccountsAppServiceTests()
        {
            _accountsAppService = GetRequiredService<IAccountsAppService>();
            _accountRepository = GetRequiredService<IRepository<Account, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _accountsAppService.GetListAsync(new GetAccountsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("f549b911-374a-4720-b250-b65fc9a39424")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _accountsAppService.GetAsync(Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new AccountCreateDto
            {
                Username = "5e0a7ab9062044feaee6821191210429c9c76f510ec74f02b15f9",
                Password = "cd581f4caf6247c69c131199ed3d45b313cd543ebff541ad9cebfc25ed2",
                TwoFactorCode = "112f01caf8c545a7b0"
            };

            // Act
            var serviceResult = await _accountsAppService.CreateAsync(input);

            // Assert
            var result = await _accountRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Username.ShouldBe("5e0a7ab9062044feaee6821191210429c9c76f510ec74f02b15f9");
            result.Password.ShouldBe("cd581f4caf6247c69c131199ed3d45b313cd543ebff541ad9cebfc25ed2");
            result.TwoFactorCode.ShouldBe("112f01caf8c545a7b0");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new AccountUpdateDto()
            {
                Username = "f36b0b08505a4e18bff592502b1d496519c6ea1eaba44c71876f52d200ec036d9ddaff11181f48fa89cc889e113d0",
                Password = "deb7fcf4d7214",
                TwoFactorCode = "db70a5"
            };

            // Act
            var serviceResult = await _accountsAppService.UpdateAsync(Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9"), input);

            // Assert
            var result = await _accountRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Username.ShouldBe("f36b0b08505a4e18bff592502b1d496519c6ea1eaba44c71876f52d200ec036d9ddaff11181f48fa89cc889e113d0");
            result.Password.ShouldBe("deb7fcf4d7214");
            result.TwoFactorCode.ShouldBe("db70a5");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _accountsAppService.DeleteAsync(Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9"));

            // Assert
            var result = await _accountRepository.FindAsync(c => c.Id == Guid.Parse("12aa1ef9-69e1-4156-8168-e9343b3aa4e9"));

            result.ShouldBeNull();
        }
    }
}