using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using FacebookCommunityAnalytics.Api.Console.BackgroundJobs;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Console
{
    public class ConsoleHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        private readonly SyncShopinessStatisticAffJob _syncShopinessStatisticAffJob;
        private readonly DailyStaffEvaluationJob _dailyStaffEvaluationJob;
        private readonly SyncShopinessConversionsJob _syncShopinessConversionsJob;
        private readonly CleanUpDataJob _cleanUpDataJob;
        private readonly SendCampaignMailsJob _sendCampaignMailsJob;
        private readonly InitCampaignPostsJob _initCampaignPostsJob;
        private readonly RetryNotAvailablePostsJob _retryNotAvailablePostsJob;
        private readonly InitUncrawledPostsJob _initUncrawledPostsJob;
        private readonly InitPostHistories _initPostHistories;
        private readonly IPostEmailDomainService _postEmailDomainService;
        private readonly IDailyKPITrackingEmailDomainService _trackingEmailDomainService;
        private readonly IUserDomainService _userDomainService;
        private readonly SyncGeneralAffStatJob _syncGeneralAffStatJob;
        private readonly UpdateCampaignStatusJob _updateCampaignStatusJob;

        private readonly TikTokMigrationService _tikTokMigrationService;
        public ConsoleHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            SyncShopinessStatisticAffJob syncShopinessStatisticAffJob,
            DailyStaffEvaluationJob dailyStaffEvaluationJob,
            SyncShopinessConversionsJob syncShopinessConversionsJob,
            IHostApplicationLifetime hostApplicationLifetime,
            CleanUpDataJob cleanUpDataJob,
            SendCampaignMailsJob sendCampaignMailsJob,
            InitCampaignPostsJob initCampaignPostsJob,
            RetryNotAvailablePostsJob retryNotAvailablePostsJob,
            InitUncrawledPostsJob initUncrawledPostsJob,
            InitPostHistories initPostHistories,
            IPostEmailDomainService postEmailDomainService,
            IDailyKPITrackingEmailDomainService trackingEmailDomainService,
            IUserDomainService userDomainService,
            SyncGeneralAffStatJob syncGeneralAffStatJob,
            UpdateCampaignStatusJob updateCampaignStatusJob,
            TikTokMigrationService tikTokMigrationService)
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _syncShopinessStatisticAffJob = syncShopinessStatisticAffJob;
            _dailyStaffEvaluationJob = dailyStaffEvaluationJob;
            _syncShopinessConversionsJob = syncShopinessConversionsJob;
            _hostApplicationLifetime = hostApplicationLifetime;
            _cleanUpDataJob = cleanUpDataJob;
            _sendCampaignMailsJob = sendCampaignMailsJob;
            _initCampaignPostsJob = initCampaignPostsJob;
            _initUncrawledPostsJob = initUncrawledPostsJob;
            _initPostHistories = initPostHistories;
            _postEmailDomainService = postEmailDomainService;
            _trackingEmailDomainService = trackingEmailDomainService;
            _userDomainService = userDomainService;
            _syncGeneralAffStatJob = syncGeneralAffStatJob;
            _updateCampaignStatusJob = updateCampaignStatusJob;
            _tikTokMigrationService = tikTokMigrationService;
            _retryNotAvailablePostsJob = retryNotAvailablePostsJob;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var args = Environment.GetCommandLineArgs();
            _application.Initialize(_serviceProvider);
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("GDL PERF TRACKING - START CONSOLE APP");
            System.Console.WriteLine("========================================================");

            //await _userService.CleanUp();
            //FacebookHelper.GetCleanUrl("https://www.facebook.com/permalink.php?story_fbid=1061054238008199&id=100005129167661");
            //await _postHistoryService.CheckPostHistoriesInvalid();
            //await _postHistoryService.GetPostMissingGroups();
            //await _postHistoryService.CreatePostHistories();

            // await _tikTokMigrationService.UpdateMcns();
            // await _tikTokMigrationService.UpdateGDLChannel();

            await Parser.Default.ParseArguments<CmdOptions>(args)
                .WithParsedAsync(DoExecuteAsync);
            
            
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("GDL PERF TRACKING - END CONSOLE APP");
            System.Console.WriteLine("========================================================");
            await StopAsync(cancellationToken: default);

            _hostApplicationLifetime.StopApplication();
        }

        private async Task DoExecuteAsync(CmdOptions o)
        {
            System.Console.WriteLine($"Current Arguments Job: -j {o.Job}");
            switch (o.Job)
            {
                //Email
                case "email_campaign_sending":
                    await _sendCampaignMailsJob.Execute();
                    break;
                case "email_crawl_posts":
                    await _postEmailDomainService.SendUncrawlsPosts();
                    break;
                case "email_daily_kpi":
                    await _trackingEmailDomainService.DoSendEMails();
                    break;
                
                //UserInfo
                case "userinfo_sync":
                    await _userDomainService.SyncUserInfos();
                    break;
                
                //Crawl
                case "crawl_retry_not_available_posts":
                    await _retryNotAvailablePostsJob.Execute();
                    break;
                case "crawl_init_uncrawled_posts":
                    await _initUncrawledPostsJob.Execute();
                    break;
                case "crawl_init_campaign_posts":
                    await _initCampaignPostsJob.Execute();
                    break;
                
                //Post
                case "init_post_histories":
                    await _initPostHistories.Execute();
                    break;
                
                //Affiliate
                case "affiliate_sync_general_stat":
                    await _syncGeneralAffStatJob.Execute();
                    break;
                case "sync_click":
                    await _syncShopinessStatisticAffJob.Execute();
                    break;
                case "sync_conversion":
                    await _syncShopinessConversionsJob.Execute();
                    break;
                
                //Camp
                case "camp_update_status":
                    await _updateCampaignStatusJob.Execute();
                    break;
                
                //Eval Staff
                case "eval_staff_daily":
                    await _dailyStaffEvaluationJob.Execute();
                    break;
                
                //Clean Data
                case "clean_up_data":
                    await _cleanUpDataJob.Execute();
                    break;

                default:
                    System.Console.WriteLine("Wrong Arguments! Please try again");
                    break;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();

            return Task.CompletedTask;
        }
    }
    
    
    public class CmdOptions
    {
        [Option('j', "job", Required = true, HelpText = "Input name job need run")]
        public string Job { get; set; }
    }
}
