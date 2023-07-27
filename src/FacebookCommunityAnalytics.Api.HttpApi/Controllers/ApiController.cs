using FacebookCommunityAnalytics.Api.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class ApiController : AbpController
    {
        protected ApiController()
        {
            LocalizationResource = typeof(ApiResource);
        }
    }
}