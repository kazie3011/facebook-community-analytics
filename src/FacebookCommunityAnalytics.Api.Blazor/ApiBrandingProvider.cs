using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace FacebookCommunityAnalytics.Api.Blazor
{
    [Dependency(ReplaceServices = true)]
    public class ApiBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "Api";
    }
}
