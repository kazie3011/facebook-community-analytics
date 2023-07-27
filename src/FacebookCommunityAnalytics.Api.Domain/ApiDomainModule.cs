using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.MultiTenancy;
using Volo.Abp;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Saas;
using Volo.Abp.BlobStoring.Database;
using Volo.CmsKit;
using Volo.CmsKit.Contact;
using Volo.CmsKit.Newsletters;
using FacebookCommunityAnalytics.Api.Integrations.Lazada;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.Affiliates;

namespace FacebookCommunityAnalytics.Api
{
    [DependsOn(
        typeof(ApiDomainSharedModule),
        typeof(AbpAuditLoggingDomainModule),
        typeof(AbpBackgroundJobsDomainModule),
        typeof(AbpFeatureManagementDomainModule),
        typeof(AbpIdentityProDomainModule),
        typeof(AbpPermissionManagementDomainIdentityModule),
        typeof(AbpIdentityServerDomainModule),
        typeof(AbpPermissionManagementDomainIdentityServerModule),
        typeof(AbpSettingManagementDomainModule),
        typeof(SaasDomainModule),
        typeof(TextTemplateManagementDomainModule),
        typeof(LeptonThemeManagementDomainModule),
        typeof(LanguageManagementDomainModule),
        typeof(VoloAbpCommercialSuiteTemplatesModule),
        typeof(AbpEmailingModule),
        typeof(CmsKitProDomainModule),
        typeof(BlobStoringDatabaseDomainModule)
        )]
    public class ApiDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddTransient<MessagingHub>();

            Configure<AbpMultiTenancyOptions>(options =>
            {
                options.IsEnabled = MultiTenancyConsts.IsEnabled;
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English", "gb"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe", "tr"));
                options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina", "si"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文", "cn"));
                options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsche", "de"));
                options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
            });
            Configure<NewsletterOptions>(options =>
            {
                options.AddPreference(
                    "Newsletter_Default",
                    new NewsletterPreferenceDefinition(
                        LocalizableString.Create<ApiResource>("NewsletterPreference_Default"),
                        privacyPolicyConfirmation: LocalizableString.Create<ApiResource>("NewsletterPrivacyAcceptMessage")
                    )
                );
            });

            ConfigureAdditionalServices(context);

            //#if DEBUG
            //            context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
            //#endif
        }

        private void ConfigureAdditionalServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<ILazadaApiConsumer, LazadaApiConsumer>();
            context.Services.AddTransient<IShopinessApiConsumer, ShopinessApiConsumer>();
            context.Services.AddTransient<ITikiAffiliateApiConsumer, TikiAffiliateApiConsumer>();
        }

        //public override void OnApplicationInitialization(ApplicationInitializationContext context)
        //{
        //    var app = context.GetApplicationBuilder();
        //}
    }
}
