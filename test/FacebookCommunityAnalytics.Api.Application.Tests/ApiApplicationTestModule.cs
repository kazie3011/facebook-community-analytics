using Volo.Abp.Modularity;

namespace FacebookCommunityAnalytics.Api
{
    [DependsOn(
        typeof(ApiApplicationModule),
        typeof(ApiDomainTestModule)
        )]
    public class ApiApplicationTestModule : AbpModule
    {

    }
}