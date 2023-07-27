using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace FacebookCommunityAnalytics.Api.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(ApiMongoDbModule),
        typeof(CmsMongoDbModule),
        typeof(ApiApplicationContractsModule)
    )]
    public class ApiDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false;
            });
        }
    }
}