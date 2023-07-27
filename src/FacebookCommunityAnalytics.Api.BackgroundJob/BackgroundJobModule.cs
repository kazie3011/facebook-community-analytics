using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.MongoDB;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Account;
using Volo.Abp.AuditLogging;
using Volo.Abp.Autofac;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FluentValidation;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;

namespace FacebookCommunityAnalytics.Api.BackgroundJob
{

    [DependsOn(
        typeof(ApiDomainModule),
        typeof(ApiApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule),
        //typeof(SaasHostApplicationModule),
        typeof(AbpAuditLoggingApplicationModule),
        typeof(AbpIdentityServerApplicationModule),
        typeof(AbpAccountPublicApplicationModule),
        typeof(AbpAccountAdminApplicationModule),
        typeof(LanguageManagementApplicationModule),
        typeof(TextTemplateManagementApplicationModule),
        typeof(AbpFluentValidationModule),
        typeof(ApiMongoDbModule),
        typeof(ApiApplicationContractsModule)
    )]
    public class BackgroundJobModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();

            //var services = context.Services;
            //// global config
            //var globalConfigurationSection = configuration.GetSection(nameof(GlobalConfiguration));
            //var globalConfiguration = globalConfigurationSection.Get<GlobalConfiguration>();
            //if (globalConfiguration != null) services.AddSingleton(globalConfiguration);

            //// payroll config
            //var payrollConfigurationSection = configuration.GetSection(nameof(PayrollConfiguration));
            //var payrollConfiguration = payrollConfigurationSection.Get<PayrollConfiguration>();
            //if (payrollConfiguration != null) services.AddSingleton(payrollConfiguration);

            context.Services.AddTransient<IPayrollDomainService, PayrollDomainService>();
            context.Services.AddHostedService<BackgroundJobHostedService>();
        }
    }
}
