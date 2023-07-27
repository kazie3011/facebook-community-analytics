
using System;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class AccountCreateDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }
        public AccountType AccountType { get; set; } = ((AccountType[])Enum.GetValues(typeof(AccountType)))[0];
        public AccountStatus AccountStatus { get; set; } = ((AccountStatus[])Enum.GetValues(typeof(AccountStatus)))[0];
        public AccountCountry AccountCountry { get; set; } = AccountCountry.Unknown;
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
    }
}