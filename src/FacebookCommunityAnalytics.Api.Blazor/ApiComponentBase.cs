using FacebookCommunityAnalytics.Api.Localization;
using Volo.Abp.AspNetCore.Components;

namespace FacebookCommunityAnalytics.Api.Blazor
{
    public abstract class ApiComponentBase : AbpComponentBase
    {
        protected ApiComponentBase()
        {
            LocalizationResource = typeof(ApiResource);
        }
    }
}