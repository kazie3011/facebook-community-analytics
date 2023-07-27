using System;
using Microsoft.AspNetCore.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Helpers
{
    public static class BlazorHelper
    {
        // Blazor: add parm to URL
       public static string AddQueryParm( this NavigationManager MyNavigationManager, string parmName, string parmValue)
        {
            var uriBuilder = new UriBuilder(MyNavigationManager.Uri);
            var q = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            q[parmName] = parmValue;
            uriBuilder.Query = q.ToString() ?? string.Empty;
            var newUrl = uriBuilder.ToString();
            return newUrl;
        }

        // Blazor: get query parm from the URL
        public static string GetQueryParm( this NavigationManager MyNavigationManager, string parmName)
        {
            var uriBuilder = new UriBuilder(MyNavigationManager.Uri);
            var q = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            return q[parmName] ?? "";
        }
    }
}