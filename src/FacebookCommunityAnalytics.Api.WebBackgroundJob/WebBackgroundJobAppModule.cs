using System;
using System.IO;
using FacebookCommunityAnalytics.Api.MongoDB;
using FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs;
using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.VirtualFileSystem;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob
{
    [DependsOn
    (
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAutofacModule),
        //typeof(AbpCachingStackExchangeRedisModule),
        //typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
        typeof(ApiMongoDbModule),
        typeof(AbpIdentityAspNetCoreModule),
        typeof(ApiApplicationModule),
        typeof(ApiDomainSharedModule),
        typeof(ApiMongoDbModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpBackgroundJobsHangfireModule),
        typeof(AbpEventBusRabbitMqModule)
    )]
    public class WebBackgroundJobAppModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Hangfire";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabled = false; //Disables the auditing system
            });

            Configure<AbpClockOptions>(options =>
            {
                options.Kind = DateTimeKind.Utc;
            });

            ConfigureVirtualFileSystem(context);
            ConfigureConventionalControllers();
            ConfigureCors(context, configuration);
            ConfigureHangfire(context, configuration);
            context.Services.AddRazorPages();
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<WebBackgroundJobAppModule>("FacebookCommunityAnalytics.Api"); });

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>
                (
                    options =>
                    {
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Domain.Shared", Path.DirectorySeparatorChar)));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Domain", Path.DirectorySeparatorChar)));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Application.Contracts", Path.DirectorySeparatorChar)));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Application", Path.DirectorySeparatorChar)));
                        //options.FileSets.ReplaceEmbeddedByPhysical<ApiHttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.HttpApi", Path.DirectorySeparatorChar)));
                    }
                );
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options => { options.ConventionalControllers.Create(typeof(ApiApplicationModule).Assembly); });
        }


        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors
            (
                options =>
                {
                    options.AddPolicy
                    (
                        DefaultCorsPolicyName,
                        builder =>
                        {
                            builder
                                //.WithOrigins(
                                //    configuration["App:CorsOrigins"]
                                //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                //        .Select(o => o.RemovePostFix("/"))
                                //        .ToArray()
                                //)
                                //.WithAbpExposedHeaders()
                                .AllowAnyOrigin()
                                //.SetIsOriginAllowedToAllowWildcardSubdomains()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                            //.AllowCredentials();
                        }
                    );
                }
            );
        }

        private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
        {
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            var services = context.Services;
            context.Services
                .AddAuthorization
                (
                    cfg =>
                    {
                        cfg.AddPolicy
                        (
                            DefaultCorsPolicyName,
                            cfgPolicy =>
                            {
                                cfgPolicy.AddRequirements().RequireAuthenticatedUser();
                                cfgPolicy.AddAuthenticationSchemes("oidc");
                            }
                        );
                    }
                )
                .AddAuthentication
                (
                    options =>
                    {
                        options.DefaultScheme = "Cookies";
                        options.DefaultChallengeScheme = "oidc";
                    }
                )
                .AddCookie("Cookies", options => { options.ExpireTimeSpan = TimeSpan.FromDays(365); })
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

            services.AddHangfire
            (
                cfg =>
                {
                    cfg.UseSqlServerStorage
                    (
                        configuration.GetConnectionString("Hangfire"),
                        new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
                            UseRecommendedIsolationLevel = true,
                            DisableGlobalLocks = true,
                            SchemaName = "WebBackgroundJob"
                        }
                    );
                }
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseAbpRequestLocalization();

            app.UseCors(DefaultCorsPolicyName);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseUnitOfWork();
            app.UseConfiguredEndpoints
            (
                endpoints =>
                {
                    //endpoints.MapHangfireDashboard().RequireAuthorization(DefaultCorsPolicyName);
                    endpoints.MapRazorPages();
                }
            );
            // app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            // {
            //     Authorization = new[] {new DashboardAuthorizationFilter()},
            //     IgnoreAntiforgeryToken = true       
            // });

            app.UseHangfireDashboard
            (
                "/hangfire",
                new DashboardOptions
                {
                    Authorization = new[]
                    {
                        new HangfireCustomBasicAuthenticationFilter
                        {
                            User = "admin",
                            Pass = "123321"
                        }
                    }
                }
            );
            
            var configuration = context.GetConfiguration();

            // global config
            var globalConfigurationSection = configuration.GetSection(nameof(Configs.GlobalConfiguration));
            var globalConfiguration = globalConfigurationSection.Get<Configs.GlobalConfiguration>();
            //Init all jobs
            InitJobs.Do(globalConfiguration);
        }
    }
}