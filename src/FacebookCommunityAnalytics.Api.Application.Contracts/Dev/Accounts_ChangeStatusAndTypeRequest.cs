using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Dev
{
    public class Accounts_ChangeStatusAndTypeRequest
    {
        public AccountType FromType { get; set; }
        public AccountType ToType { get; set; }

        public AccountStatus FromStatus { get; set; }
        public AccountStatus ToStatus { get; set; }

        public int Count { get; set; }

        public Accounts_ChangeStatusAndTypeRequest()
        {
            Count = 10;
        }
    }
}