using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class ConsoleHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        private readonly PostHistoryService _postHistoryService;
        private readonly UserService _userService;
        private readonly EvaluationService _evaluationService;
        private readonly CleanUpService _cleanUpService;
        private readonly CampaignService _campaignService;
        private readonly CrawlService _crawlService;
        private readonly ContractService _contractService;
        private readonly TikTokService _tikTokService;
        private readonly FileService _fileService;

        public ConsoleHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            PostHistoryService postHistoryService,
            UserService userService,
            IHostApplicationLifetime hostApplicationLifetime,
            EvaluationService evaluationService,
            CleanUpService cleanUpService,
            CampaignService campaignService,
            CrawlService crawlService,
            ContractService contractService,
            TikTokService tikTokService,
            FileService fileService
        )
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _postHistoryService = postHistoryService;
            _userService = userService;
            _hostApplicationLifetime = hostApplicationLifetime;
            _evaluationService = evaluationService;
            _cleanUpService = cleanUpService;
            _campaignService = campaignService;
            _crawlService = crawlService;
            _contractService = contractService;
            _tikTokService = tikTokService;
            _fileService = fileService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                _application.Initialize(_serviceProvider);
                System.Console.WriteLine("========================================================");
                System.Console.WriteLine("GDL PERF TRACKING - START CONSOLE APP");
                System.Console.WriteLine("========================================================");

                Debug.WriteLine("This is from console dev");
                // var userCodes = await _userService.GetUsersWithFullLinkFuid();
                // System.Console.WriteLine(string.Join(',', userCodes));

                // await TestGetProfileFacebookId();

                // await _campaignService.RemapCampaignPostsByHashtags("cenr-loveyoursmile","cernyeunucuoi");
                // await _campaignService.RemapCampaignPostsByHashtags("lasolhoteldalat","lasol");
                // await _campaignService.RemapCampaignPostsByKeywords("lasolhoteldalat","khách sạn lasol","lasol hotel");

                // await _campaignService.UpdateCampaignStatus("sgclx-hoicho");
                // await _campaignService.UpdateCampaignStatuses();
                // await _contractService.UpdateSalePersonTransaction();

                // await _cleanUpService.RemapCampaignPosts("tetdalat","lixizalopay-gdl");
                // await _cleanUpService.RemapCampaignPosts("cemdtet","lixitrongzalo");
                // await _cleanUpService.RemapCampaignPosts("onhatet","lixizalopay-onvtb");
                // await _cleanUpService.RemapCampaignPosts("tetvietnam","lixizalopay-vno");
                
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,1, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,2, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,3, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,4, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,5, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,6, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,7, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,8, 0 , 0 ,0 , DateTimeKind.Utc));
                await _crawlService.InitDataForGroupStatHistory(new DateTime(2023,1,9, 0 , 0 ,0 , DateTimeKind.Utc));

                //await _campaignService.RemapCampaignPosts();
                await _fileService.RemapMediaUrl(@"C:\Contract\HDDV CẬP NHẬT TOOL");
                await _fileService.ExportMedias(@"E:\medias.xlsx");

                //await _crawlService.UpdateGroupIdForGroupStatHistory();
                // await _contractService.CleanContractsType();
                // await _tikTokService.UpdateGroupIdTikTokVideos();
                // await Parser.Default.ParseArguments<CmdOptions>(args)
                //     .WithParsedAsync(DoExecuteAsync);

                System.Console.WriteLine("========================================================");
                System.Console.WriteLine("GDL PERF TRACKING - END CONSOLE APP");
                System.Console.WriteLine("========================================================");
                await StopAsync(cancellationToken: default);

                _hostApplicationLifetime.StopApplication();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }

        public Task TestGetProfileFacebookId()
        {
            try
            {
                var fuid1 = FacebookHelper.GetProfileFacebookId("camping89");
                var fuid2 = FacebookHelper.GetProfileFacebookId("100029054438376");
                var fuid3 = FacebookHelper.GetProfileFacebookId("https://www.facebook.com/profile.php?id=100029054438376");
                var fuid4 = FacebookHelper.GetProfileFacebookId("https://www.facebook.com/camping89");
                var fuid5 = FacebookHelper.GetProfileFacebookId("https://www.facebook.com/groups/514422389955473/user/100029054438376/");
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }

            return Task.CompletedTask;
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