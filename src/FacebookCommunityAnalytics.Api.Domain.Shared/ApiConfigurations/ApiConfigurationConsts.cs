using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookCommunityAnalytics.Api.ApiConfigurations
{
    public class ApiConfigurationConsts
    {
        private const string DefaultSorting = "{0}ConfigurationAt desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ApiConfiguration." : string.Empty);
        }
    }
}
