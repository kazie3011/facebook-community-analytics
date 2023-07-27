using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.CmsKit.MongoDB;

namespace FacebookCommunityAnalytics.Api.MongoDB
{
    [DependsOn(typeof(ApiDomainModule)
        )]
    public class CmsMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<CmsMongoDbContext>(
                options =>
                {
                    options.AddDefaultRepositories(true);
                });
            
            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
            });
        }
    }
}