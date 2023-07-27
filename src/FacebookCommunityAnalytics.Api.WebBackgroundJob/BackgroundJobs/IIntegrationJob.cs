using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.HealthChecks;
using FacebookCommunityAnalytics.Api.HealthChecks.Models;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Hangfire;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public interface IIntegrationJob
    {
        Task Execute();
    }

    public abstract class BackgroundJobBase : EmailDomainServiceBase, IIntegrationJob
    {
        public IHealthCheckDomainService HealthCheckDomainService { get; set; }

        public virtual async Task Execute()
        {
            var msg = new SlackMessage
            {
                Text = "Trạng thái hoạt động của jobs",
                Channel = "general",
                Username = "SlackBotMessages",
                Attachments = new List<SlackAttachment>()
            };
            try
            {
                await DoExecute();
                var textAttachment = $"Job [{this.GetType().Name}] successful at [{DateTime.UtcNow:dd-MM-yyyy HH:mm}] UTC";
                if (GlobalConfiguration.PartnerConfiguration.IsPartnerTool)
                {
                    textAttachment = "[PARTNER] - " + textAttachment;
                }

                msg.Attachments.Add
                (
                    new SlackAttachment
                    {
                        Text = textAttachment,
                        Color = "good",
                        Fields = new List<SlackField> {new() {Title = Emoji.HeavyCheckMark + " Success"}}
                    }
                );
            }
            catch (Exception e)
            {
                var subject = $"Job [{this.GetType().Name}] failed at [{DateTime.UtcNow:dd-MM-yyyy HH:mm}] UTC";
                var textAttachment = subject;
                if (GlobalConfiguration.PartnerConfiguration.IsPartnerTool)
                {
                    textAttachment = "[PARTNER] - " + textAttachment;
                }

                textAttachment = textAttachment + "\n " + e.Message;
                textAttachment = textAttachment + "\n " + e.StackTrace;

                msg.Attachments.Add
                (
                    new SlackAttachment
                    {
                        Text = textAttachment,
                        Color = "danger",
                        Fields = new List<SlackField> {new() {Title = Emoji.HeavyCheckMark + " Failed"}}
                    }
                );
#if !DEBUG
                var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
                var recipients = GlobalConfiguration.EmailConfiguration.Recipients_Test;

                var to = recipients.FirstOrDefault();
                var message = new MailMessage(from, to, subject, textAttachment);
                await SendUsingNetworkCredentials(message);
#endif
            }

            // await HealthCheckDomainService.SendNotificationToSlack(msg);
        }

        protected abstract Task DoExecute();

        protected BackgroundJobBase(ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
        }
    }

    public static class InitJobs
    {
        public static void Do(Configs.GlobalConfiguration globalConfiguration)
        {
#if !DEBUG
            //var every15Minutes = "0 0 15 * *";
            var every2Hours = "0 */2 * * *";
            var every4Hours = "0 */4 * * *";
            var every4HoursStartAtM15 = "15 */4 * * *";
            var every4HoursStartAt1EndAt20 = "0 9-16/4 * * *";
            var every6Hours = "0 */6 * * *";
           
            if (globalConfiguration.PartnerConfiguration.IsPartnerTool)
            {
                RecurringJob.AddOrUpdate<SendCampaignMailsJob>($"EMAIL_Campaign", o => o.Execute(), Cron.Daily(1));
                RecurringJob.AddOrUpdate<SyncUserInfoJob>("USERINFO_SyncUserInfoJob", o => o.Execute(), Cron.Hourly(10));

                RecurringJob.AddOrUpdate<RetryNotAvailablePartnerPostsJob>("CRAWL_PartnerInitNotAvailablePosts", o => o.Execute(), every4Hours);
                RecurringJob.AddOrUpdate<InitPartnerUncrawledPostsJob>("CRAWL_PartnerInitUncrawledPosts", o => o.Execute(), Cron.Hourly);

                RecurringJob.AddOrUpdate<InitCampaignPostsJob>("CRAWL_InitCampaignPosts", o => o.Execute(), Cron.Hourly);

                RecurringJob.AddOrUpdate<SyncCampaignPostCountJob>("CAMP_SyncCampaignPostCount", o => o.Execute(), Cron.Daily(1));
                RecurringJob.AddOrUpdate<UpdateCampaignStatusJob>("CAMP_UpdateStatus", o => o.Execute(), Cron.Hourly);
                // RecurringJob.AddOrUpdate<InitPostHistories>("Post_InitPostHistories", o => o.Execute(), Cron.Daily(16));
                RecurringJob.AddOrUpdate<CleanUpDataJob>("CLEANUP_Data_Job", o => o.Execute(), Cron.Daily(14, 30));
            }
            else
            {
                // RecurringJob.AddOrUpdate<SampleJob>(nameof(SampleJob), o => o.Execute(), "* * * * *");
                RecurringJob.AddOrUpdate<SendSampleEmailJob>($"EMAIL_Sample", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendPayrollEmailJob>($"EMAIL_Payroll", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendCampaignMailsJob>($"EMAIL_Campaign", o => o.Execute(), Cron.Daily(1));
                RecurringJob.AddOrUpdate<SendPostsEmailJob>($"EMAIL_CrawlPost", o => o.Execute(), Cron.Daily(0));
                RecurringJob.AddOrUpdate<SendTiktokEmailsJob>($"EMAIL_Tiktok", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendDailyKpiEmailJob>($"EMAIL_DailyKpi", o => o.Execute(), Cron.Daily(1));
                
                RecurringJob.AddOrUpdate<SyncUserInfoJob>("USERINFO_SyncUserInfoJob", o => o.Execute(), Cron.Hourly(10));

                RecurringJob.AddOrUpdate<RetryNotAvailablePostsJob>("CRAWL_InitNotAvailablePosts", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<InitUncrawledPostsJob>("CRAWL_InitUncrawledPosts", o => o.Execute(), Cron.Hourly);
                RecurringJob.AddOrUpdate<InitCampaignPostsJob>("CRAWL_InitCampaignPosts", o => o.Execute(), Cron.Hourly(2));

                RecurringJob.AddOrUpdate<SyncCampaignPostCountJob>("CAMP_SyncCampaignPostCount", o => o.Execute(), Cron.Daily(1));
                RecurringJob.AddOrUpdate<UpdateCampaignStatusJob>("CAMP_UpdateStatus", o => o.Execute(), Cron.Hourly);

                RecurringJob.AddOrUpdate<GenerateStaffEvaluationsJob>($"STAFF_GenerateEvaluationsJob", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<DailyStaffEvaluationJob>($"STAFF_DailyStaffEvaluationJob", o => o.Execute(), Cron.Daily(16));
            }
#else
            if (globalConfiguration.PartnerConfiguration.IsPartnerTool)
            {
                RecurringJob.AddOrUpdate<SendCampaignMailsJob>($"EMAIL_Campaign", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SyncUserInfoJob>("USERINFO_SyncUserInfoJob", o => o.Execute(), Cron.Never);

                RecurringJob.AddOrUpdate<RetryNotAvailablePartnerPostsJob>("CRAWL_PartnerInitNotAvailablePosts", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<InitPartnerUncrawledPostsJob>("CRAWL_PartnerInitUncrawledPosts", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<InitCampaignPostsJob>("CRAWL_InitCampaignPosts", o => o.Execute(), Cron.Never);

                RecurringJob.AddOrUpdate<UpdateCampaignStatusJob>("CAMP_UpdateStatus", o => o.Execute(), Cron.Never);
                // RecurringJob.AddOrUpdate<InitPostHistories>("Post_InitPostHistories", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<CleanUpDataJob>("CLEANUP_Data_Job", o => o.Execute(), Cron.Never);
            }
            else
            {
                RecurringJob.AddOrUpdate<SampleJob>($"Sample_Test", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendSampleEmailJob>($"EMAIL_Sample", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendPayrollEmailJob>($"EMAIL_Payroll", o => o.Execute(), Cron.Never);
                // RecurringJob.AddOrUpdate<CalculateUserCompensationsJob>($"Calculate_UserCompensation", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendCampaignMailsJob>($"EMAIL_Campaign", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendPostsEmailJob>($"EMAIL_CrawlPost", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SendTiktokEmailsJob>("EMAIL_Tiktok", o => o.Execute(), Cron.Never);

                RecurringJob.AddOrUpdate<CalculateWaveMultiplierJob>("USERINFO_CalculateWaveMultiplierJob", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SyncUserInfoJob>("USERINFO_SyncUserInfoJob", o => o.Execute(), Cron.Never);

                RecurringJob.AddOrUpdate<RetryNotAvailablePostsJob>("CRAWL_InitNotAvailablePosts", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<InitUncrawledPostsJob>("CRAWL_InitUncrawledPosts", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<InitCampaignPostsJob>("CRAWL_InitCampaignPosts", o => o.Execute(), Cron.Never);

                // RecurringJob.AddOrUpdate<SyncGeneralAffStatJob>("AFF_LVL1_SyncGeneralAffStat", o => o.Execute(), Cron.Never);
                // RecurringJob.AddOrUpdate<SyncUserAffStatJob>("AFF_LVL2_SyncUserAffStatJob", o => o.Execute(), Cron.Never);
                // RecurringJob.AddOrUpdate<SyncShopinessAffiliateJob>("AFF_LVL3_SHOPINESS_SyncShopinessAffiliate", o => o.Execute(), Cron.Never);
                // RecurringJob.AddOrUpdate<SyncShopinessConversionsJob>("AFF_SyncShopinessConversions", o => o.Execute(), Cron.Never);
                // RecurringJob.AddOrUpdate<SyncUserAffOwnerJob>("AFF_SyncUserAffOwnerJob", o => o.Execute(), Cron.Never);

                RecurringJob.AddOrUpdate<SyncGeneralAffStatJob>("AFF_LVL1_SyncGeneralAffStat", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SyncShopinessStatisticAffJob>("AFF_SyncClicks_Shopiness", o => o.Execute(), Cron.Never);

                RecurringJob.AddOrUpdate<UpdateCampaignStatusJob>("CAMP_UpdateStatus", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SyncCampaignPostCountJob>("CAMP_SyncCampaignPostCount", o => o.Execute(), Cron.Never);

                RecurringJob.AddOrUpdate<CleanUpDataJob>("CLEANUP_Data_Job", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<SyncTikiOrdersJob>("Aff_TikiOrdersJob", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<GenerateStaffEvaluationsJob>($"STAFF_GenerateEvaluationsJob", o => o.Execute(), Cron.Never);
                RecurringJob.AddOrUpdate<DailyStaffEvaluationJob>($"STAFF_DailyStaffEvaluationJob", o => o.Execute(), Cron.Never);
            }

#endif
        }
    }
}