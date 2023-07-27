using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class AccountDto : FullAuditedEntityDto<Guid>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }
        public AccountType AccountType { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public AccountCountry AccountCountry { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
        public bool IsActive { get; set; }
    }
}