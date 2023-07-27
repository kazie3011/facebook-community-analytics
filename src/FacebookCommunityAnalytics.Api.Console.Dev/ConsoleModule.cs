using FacebookCommunityAnalytics.Api.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{

    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(AbpAutofacModule),
        typeof(ApiDomainModule),
        typeof(ApiDomainSharedModule),
        typeof(ApiMongoDbModule)
    )]
    public class ConsoleModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();
            context.Services.AddHostedService<ConsoleHostedService>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                //Add all mappings defined in the assembly of the MyModule class
                options.AddMaps<ConsoleModule>();
                options.AddProfile<ApiConsoleDevAutoMapperProfile>();
            });
        }
    }
}
