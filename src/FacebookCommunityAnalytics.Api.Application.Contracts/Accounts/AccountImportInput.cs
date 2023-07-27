using FacebookCommunityAnalytics.Api.Core.Helpers;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class AccountImportInput
    {
        public AccountImportInput()
        {
            Items = new List<AccountImportDto>();
        }
        public List<AccountImportDto> Items { get; set; }
    }

    public class AccountImportDto
    {
        [Column(1)]
        public string Username { get; set; }

        [Column(2)]
        public string Password { get; set; }

        [Column(3)]
        public string TwoFactorCode { get; set; }

        [Column(4)]
        public string AccountType { get; set; }
        
        [Column(5)]
        public string AccountStatus { get; set; }
        [Column(6)]
        public string Email { get; set; }
        [Column(7)]
        public string EmailPassword { get; set; }
        
    }
}
