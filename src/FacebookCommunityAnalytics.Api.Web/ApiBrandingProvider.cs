using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Web
{
    [Dependency(ReplaceServices = true)]
    public class ApiBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "Api";
    }
}
