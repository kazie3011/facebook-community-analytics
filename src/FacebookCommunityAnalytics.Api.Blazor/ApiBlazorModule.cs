#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Net;
using Blazored.Localisation;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FacebookCommunityAnalytics.Api.Blazor.Menus;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.MultiTenancy;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Server.LeptonTheme;
using Volo.Abp.AspNetCore.Components.Server.LeptonTheme.Bundling;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.Blazor.Server;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Pro.Blazor.Server;
using Volo.Abp.IdentityServer.Blazor.Server;
using Volo.Abp.LanguageManagement.Blazor.Server;
using Volo.Abp.LeptonTheme.Management.Blazor.Server;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TextTemplateManagement.Blazor.Server;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Saas.Host.Blazor.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using FacebookCommunityAnalytics.Api.Blazor.Components.Layout;
using FacebookCommunityAnalytics.Api.Blazor.MessageEvents;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Localization;
using StackExchange.Redis;
using Volo.Abp.Account.Pro.Admin.Blazor.Server;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.CmsKit.Pro.Admin.Web;

namespace FacebookCommunityAnalytics.Api.Blazor
{
    [DependsOn
    (
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpAutofacModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpAspNetCoreMvcClientModule),
        typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
        typeof(AbpHttpClientIdentityModelWebModule),
        typeof(AbpAspNetCoreMvcUiLeptonThemeModule),
        typeof(AbpAspNetCoreComponentsServerLeptonThemeModule),
        typeof(AbpAccountAdminBlazorServerModule),
        typeof(AbpAuditLoggingBlazorServerModule),
        typeof(AbpIdentityProBlazorServerModule),
        typeof(LeptonThemeManagementBlazorServerModule),
        typeof(AbpIdentityServerBlazorServerModule),
        typeof(LanguageManagementBlazorServerModule),
        typeof(SaasHostBlazorServerModule),
        typeof(TextTemplateManagementBlazorServerModule),
        //typeof(CmsKitProAdminWebModule),
        typeof(ApiHttpApiModule),
        typeof(ApiHttpApiClientModule),
        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpEventBusRabbitMqModule),
        typeof(GlobalConfigurationModule)
    )]
    public class ApiBlazorModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>
            (
                options =>
                {
                    options.AddAssemblyResource
                    (
                        typeof(ApiResource),
                        typeof(ApiDomainSharedModule).Assembly,
                        typeof(ApiApplicationContractsModule).Assembly,
                        typeof(ApiBlazorModule).Assembly
                    );
                }
            );
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();
            if (MultiTenancyConsts.IsEnabled)
            {
                Configure<AbpTenantResolveOptions>
                (
                    options =>
                    {
                        options.AddDomainTenantResolver("{0}." + configuration["App:Domain"]);
                    }
                );
            }

            Configure<AbpClockOptions>
            (
                options =>
                {
                    options.Kind = DateTimeKind.Utc;
                }
            );

            ConfigureUrls(configuration);
            ConfigureBundles();
            ConfigureAuthentication(context, configuration);
            ConfigureAutoMapper();
            ConfigureVirtualFileSystem(hostingEnvironment);
            ConfigureLocalizationServices();
            ConfigureSwaggerServices(context.Services);
            ConfigureCache(configuration);
            ConfigureRedis(context, configuration, hostingEnvironment);
            ConfigureBlazorise(context);
            ConfigureRouter();
            ConfigureMenu(configuration);
            ConfigureLeptonTheme();

            context.Services.AddServerSideBlazor()
                .AddCircuitOptions
                (
                    o =>
                    {
                        o.DetailedErrors = true;
                    }
                );

            context.Services.Configure<AbpExceptionHandlingOptions>
            (
                options =>
                {
                    options.SendExceptionsDetailsToClients = true;
                }
            );

            //Config Delay Text Blazor
            // context.Services.AddBlazorise(options =>
            // {
            //     options.DelayTextOnKeyPress = true;  
            //     options.DelayTextOnKeyPressInterval = 300;
            // });

            context.Services.AddBlazoredLocalisation();
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>
            (
                options =>
                {
                    options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
                }
            );
        }

        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>
            (
                options =>
                {
                    // MVC UI
                    options.StyleBundles.Configure
                    (
                        LeptonThemeBundles.Styles.Global,
                        bundle =>
                        {
                            bundle.AddFiles("/global-styles.css");
                        }
                    );

                    // Blazor UI
                    options.StyleBundles.Configure
                    (
                        BlazorLeptonThemeBundles.Styles.Global,
                        bundle =>
                        {
                            bundle.AddFiles("/blazor-global-styles.css");
                            bundle.AddFiles("/styles/theme-lepton6.css");
                            //You can remove the following line if you don't use Blazor CSS isolation for components
                            bundle.AddFiles("/FacebookCommunityAnalytics.Api.Blazor.styles.css");
                        }
                    );
                }
            );
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication
                (
                    options =>
                    {
                        options.DefaultScheme = "Cookies";
                        options.DefaultChallengeScheme = "oidc";
                    }
                )
                .AddCookie
                (
                    "Cookies",
                    options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromDays(365);
                    }
                )
                .AddAbpOpenIdConnect
                (
                    "oidc",
                    options =>
                    {
                        options.Authority = configuration["AuthServer:Authority"];
                        options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                        ;
                        options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                        options.ClientId = configuration["AuthServer:ClientId"];
                        options.ClientSecret = configuration["AuthServer:ClientSecret"];

                        options.SaveTokens = true;
                        options.GetClaimsFromUserInfoEndpoint = true;

                        options.Scope.Add("role");
                        options.Scope.Add("email");
                        options.Scope.Add("phone");
                        options.Scope.Add("Api");
                    }
                );
        }

        private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>
                (
                    options =>
                    {
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiDomainSharedModule>
                            (Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}FacebookCommunityAnalytics.Api.Domain.Shared"));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiApplicationContractsModule>
                            (Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}FacebookCommunityAnalytics.Api.Application.Contracts"));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiBlazorModule>(hostingEnvironment.ContentRootPath);
                    }
                );
            }
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen
            (
                options =>
                {
                    options.SwaggerDoc
                    (
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "Api API",
                            Version = "v1"
                        }
                    );
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        private void ConfigureLocalizationServices()
        {
            Configure<AbpLocalizationOptions>
            (
                options =>
                {
                    options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
                    options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                    options.Languages.Add(new LanguageInfo("en", "en", "English"));
                    options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
                    options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
                    options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
                    options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                    options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
                    options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                    options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                    options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
                    options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
                    options.Languages.Add(new LanguageInfo("es", "es", "Español"));
                }
            );
        }

        private void ConfigureCache(IConfiguration configuration)
        {
            Configure<AbpDistributedCacheOptions>
            (
                options =>
                {
                    options.KeyPrefix = "Api:";
                }
            );
        }

        private void ConfigureRedis(
            ServiceConfigurationContext context,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment)
        {
            if (!hostingEnvironment.IsDevelopment())
            {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);

                ClearAllRedisCache(configuration);

                context.Services
                    .AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "Api-Protection-Keys");
            }
        }


        private void ClearAllRedisCache(IConfiguration configuration)
        {
            var options = ConfigurationOptions.Parse(configuration["Redis:Configuration"]);
            options.AllowAdmin = true;
            var redis = ConnectionMultiplexer.Connect(options);

            var endpoints = redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = redis.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }


        private void ConfigureBlazorise(ServiceConfigurationContext context)
        {
            context.Services
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
        }

        private void ConfigureMenu(IConfiguration configuration)
        {
            Configure<AbpNavigationOptions>
            (
                options =>
                {
                    options.MenuContributors.Add(new ApiMenuContributor(configuration));
                }
            );
        }

        private void ConfigureLeptonTheme()
        {
            Configure<Volo.Abp.AspNetCore.Components.Web.LeptonTheme.LeptonThemeOptions>
            (
                options =>
                {
                    options.FooterComponent = typeof(MainFooterComponent);
                }
            );
        }

        private void ConfigureRouter()
        {
            Configure<AbpRouterOptions>
            (
                options =>
                {
                    options.AppAssembly = typeof(ApiBlazorModule).Assembly;
                }
            );
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>
            (
                options =>
                {
                    options.AddMaps<ApiBlazorModule>();
                }
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var env = context.GetEnvironment();
            var app = context.GetApplicationBuilder();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            app.UseAuthorization();
            app.UseSwagger();
            app.UseAbpSwaggerUI
            (
                options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api API");
                }
            );
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}