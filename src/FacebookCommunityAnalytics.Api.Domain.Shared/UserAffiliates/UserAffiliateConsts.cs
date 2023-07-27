using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserAffiliateConsts
    {
        private const string DefaultSorting = "{0}CreatedAt asc";
        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserAffiliate." : string.Empty);
        }

    }
}
