using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class GetAccountsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }
        public AccountType? AccountType { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        public AccountCountry? AccountCountry { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }

        public GetAccountsInput()
        {

        }
    }
}