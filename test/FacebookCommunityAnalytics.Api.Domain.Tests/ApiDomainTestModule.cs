using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Modularity;

namespace FacebookCommunityAnalytics.Api
{
    [DependsOn(
        typeof(ApiMongoDbTestModule)
        )]
    public class ApiDomainTestModule : AbpModule
    {

    }
}