using System;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class UserInfoAccount
    {
        public string Fid { get; set; }
        public string Name { get; set; }
        public UserInfoAccountType AccountType { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

        public UserInfoAccount()
        {
            AccountType = UserInfoAccountType.FacebookGroup;
            IsActive = true;
        }
    }
}