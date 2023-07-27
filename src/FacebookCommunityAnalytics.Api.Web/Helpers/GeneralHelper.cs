using System.Collections.Generic;
using System.Linq;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FacebookCommunityAnalytics.Api.Web.Helpers
{
    public static class GeneralHelper
    {
        public static IList<SelectListItem> ToSelectListItems<T>(this IReadOnlyList<LookupDto<T>> input)
        {
            var result = input.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                
                Text = x.DisplayName
            }).ToList();
            return result;
        }
        public static IList<SelectListItem> ToSelectListItems<T>(this List<LookupDto<T>> input)
        {
            var result = input.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                
                Text = x.DisplayName
            }).ToList();
            return result;
        }
    }
}