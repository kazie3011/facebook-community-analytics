using FacebookCommunityAnalytics.Api.Cms.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Account;
using Volo.Abp.AuditLogging;
using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Azure;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FluentValidation;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.CmsKit;
using Volo.CmsKit.Admin.Pages;
using Volo.Saas.Host;

namespace FacebookCommunityAnalytics.Api
{
    [DependsOn(
        typeof(ApiDomainModule),
        typeof(ApiApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule),
        typeof(SaasHostApplicationModule),
        typeof(AbpAuditLoggingApplicationModule),
        typeof(AbpIdentityServerApplicationModule),
        typeof(AbpAccountPublicApplicationModule),
        typeof(AbpAccountAdminApplicationModule),
        typeof(LanguageManagementApplicationModule),
        typeof(LeptonThemeManagementApplicationModule),
        typeof(CmsKitProApplicationModule),
        typeof(TextTemplateManagementApplicationModule),
        typeof(AbpFluentValidationModule),
        typeof(AbpBlobStoringAzureModule)
        )]
    public class ApiApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            // //TODO Huy
            // context.Services.Replace(
            //     ServiceDescriptor.Transient<IPageAdminAppService, CmsPageAppService>()
            // );
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<ApiApplicationModule>();
            });
            
            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    container.UseAzure(azure =>
                    {
                        azure.ConnectionString = configuration["AzureBlob:ConnectString"];
                        azure.ContainerName =  configuration["AzureBlob:ContainerName"];
                        azure.CreateContainerIfNotExists = true;
                    });
                });
            });

        }
    }
}
