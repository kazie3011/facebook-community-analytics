using FacebookCommunityAnalytics.Api.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FacebookCommunityAnalytics.Api.Web.Public.Pages
{
    /* Inherit your Page Model classes from this class.
     */
    public abstract class ApiPublicPageModel : AbpPageModel
    {
        protected ApiPublicPageModel()
        {
            LocalizationResourceType = typeof(ApiResource);
        }
    }
}
