using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace FacebookCommunityAnalytics.Api.Blazor
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Error()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File($"Logs/logs-{DateTime.Now:ddMMyyyy}.txt"))
                // .WriteTo.Elasticsearch(
                //     new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]))
                //     {
                //         AutoRegisterTemplate = true,
                //         AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                //         IndexFormat = "gdll-log-blazor-{0:yyyy.MM}"
                //     })

#if DEBUG
                .WriteTo.Async(c => c.Console())
#endif
                .CreateLogger();

            try
            {
                Log.Information("Starting web host.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    if (!env.IsProduction())
                    {
                        config
                            .AddJsonFile(Path.Combine(env.ContentRootPath, "..", "Configs/globalconfigs.json"), true, true)
                            .AddJsonFile(Path.Combine(env.ContentRootPath, "..", $"Configs/globalconfigs.{env.EnvironmentName}.json"), true, true);
                    }
                    else
                    {
                        config
                            .AddJsonFile("Configs/globalconfigs.json", true, true)
                            .AddJsonFile($"Configs/globalconfigs.{env.EnvironmentName}.json", true, true);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseAutofac()
                .UseSerilog();
    }
}
