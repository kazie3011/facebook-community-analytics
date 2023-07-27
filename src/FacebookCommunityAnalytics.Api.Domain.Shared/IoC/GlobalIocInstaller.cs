using FacebookCommunityAnalytics.Api.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace FacebookCommunityAnalytics.Api.IoC
{
    public static class GlobalIocInstaller
    {
        public static void Configure(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var configuration = context.Services.GetConfiguration();

            // global config
            var globalConfigurationSection = configuration.GetSection(nameof(GlobalConfiguration));
            var globalConfiguration = globalConfigurationSection.Get<GlobalConfiguration>();
            if (globalConfiguration != null) services.AddSingleton(globalConfiguration);
        }
    }
}
