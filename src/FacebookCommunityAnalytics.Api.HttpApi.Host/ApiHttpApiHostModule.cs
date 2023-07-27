using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FacebookCommunityAnalytics.Api.MongoDB;
using FacebookCommunityAnalytics.Api.MultiTenancy;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using FacebookCommunityAnalytics.Api.HealthChecks;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace FacebookCommunityAnalytics.Api
{
    [DependsOn
    (
        typeof(ApiHttpApiModule),
        typeof(AbpAutofacModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
        typeof(AbpIdentityAspNetCoreModule),
        typeof(ApiApplicationModule),
        typeof(ApiDomainSharedModule),
        typeof(ApiMongoDbModule),
        typeof(CmsMongoDbModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpEventBusRabbitMqModule),
        typeof(AbpBackgroundJobsHangfireModule)
    )]
    public class ApiHttpApiHostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            
            Configure<AbpClockOptions>(options =>
            {
                options.Kind = DateTimeKind.Utc;
            });
            
            ConfigureUrls(configuration);
            ConfigureConventionalControllers();
            ConfigureAuthentication(context, configuration);
            ConfigureSwagger(context, configuration);
            ConfigureCache(configuration);
            ConfigureVirtualFileSystem(context);
            ConfigureRedis(context, configuration, hostingEnvironment);
            ConfigureCors(context, configuration);
            ConfigureExternalProviders(context);
            ConfigureHealthChecks(context);
            ConfigureHangfire(context);
            
        }

        private void ConfigureHangfire(ServiceConfigurationContext context)
        {            
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            GlobalJobFilters.Filters.Add(new LogEverythingAttribute());
            var configuration = context.Services.GetConfiguration();
            context.Services.AddHangfire
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
                            DisableGlobalLocks = true
                        }
                    );
                }
            );
        }

        private void ConfigureHealthChecks(ServiceConfigurationContext context)
        {
            context.Services.AddApiHealthChecks();
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>
            (
                options =>
                {
                    options.Applications["MVCPublic"].RootUrl = configuration["App:MVCPublicUrl"];
                    options.Applications["Angular"].RootUrl = configuration["App:AngularUrl"];
                    options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
                    options.Applications["Angular"].Urls[AccountUrlNames.EmailConfirmation] = "account/email-confirmation";
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

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            Configure<AbpVirtualFileSystemOptions>
            (
                options =>
                {
                    options.FileSets.AddEmbedded<ApiHttpApiHostModule>("FacebookCommunityAnalytics.Api");
                }
            );

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>
                (
                    options =>
                    {
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiDomainSharedModule>
                            (Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Domain.Shared", Path.DirectorySeparatorChar)));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiDomainModule>
                            (Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Domain", Path.DirectorySeparatorChar)));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiApplicationContractsModule>
                            (Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Application.Contracts", Path.DirectorySeparatorChar)));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiApplicationModule>
                            (Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.Application", Path.DirectorySeparatorChar)));
                        options.FileSets.ReplaceEmbeddedByPhysical<ApiHttpApiModule>
                            (Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}FacebookCommunityAnalytics.Api.HttpApi", Path.DirectorySeparatorChar)));
                    }
                );
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>
            (
                options =>
                {
                    options.ConventionalControllers.Create(typeof(ApiApplicationModule).Assembly);
                }
            );
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer
                (
                    options =>
                    {
                        options.Authority = configuration["AuthServer:Authority"];
                        options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                        options.Audience = "Api";
                    }
                );
        }

        private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGenWithOAuth
            (
                configuration["AuthServer:Authority"],
                new Dictionary<string, string> {{"Api", "Api API"}},
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
                                .WithOrigins
                                (
                                    configuration["App:CorsOrigins"]
                                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                        .Select(o => o.Trim().RemovePostFix("/"))
                                        .ToArray()
                                )
                                .WithAbpExposedHeaders()
                                .SetIsOriginAllowedToAllowWildcardSubdomains()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        }
                    );
                }
            );
        }

        private void ConfigureExternalProviders(ServiceConfigurationContext context)
        {
            context.Services
                .AddDynamicExternalLoginProviderOptions<GoogleOptions>
                (
                    GoogleDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ClientId);
                        options.WithProperty(x => x.ClientSecret, isSecret: true);
                    }
                )
                .AddDynamicExternalLoginProviderOptions<MicrosoftAccountOptions>
                (
                    MicrosoftAccountDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ClientId);
                        options.WithProperty(x => x.ClientSecret, isSecret: true);
                    }
                )
                .AddDynamicExternalLoginProviderOptions<TwitterOptions>
                (
                    TwitterDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ConsumerKey);
                        options.WithProperty(x => x.ConsumerSecret, isSecret: true);
                    }
                );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCors(DefaultCorsPolicyName);

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

                    var configuration = context.GetConfiguration();
                    options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                    options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
                }
            );
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseUnitOfWork();
            app.UseConfiguredEndpoints();
            
            
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

        }
    }
}