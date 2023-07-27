using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace FacebookCommunityAnalytics.Api.Console
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
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
                //         IndexFormat = "gdll-log-console-{0:yyyy.MM}"
                //     })

#if DEBUG
                .WriteTo.Async(c => c.Console())
#endif
                .CreateLogger();
            try
            {
                Log.Information("Starting console host.");
                await CreateHostBuilder(args).RunConsoleAsync();
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

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseAutofac()
                .UseSerilog()
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
                .ConfigureServices((hostContext, services) =>
                {
                    // services.Configure<ConsoleLifetimeOptions>(options =>  // configure the options
                    //     options.SuppressStatusMessages = true);            // in code
                    services.AddApplication<ConsoleModule>();
                });
    }
}
