using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.HealthChecks;
using FacebookCommunityAnalytics.Api.HealthChecks.Models;
using FacebookCommunityAnalytics.Api.Services.Emails;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public interface IIntegrationJob
    {
        Task Execute();
    }

    public abstract class BackgroundJobBase : BaseDomainService, IIntegrationJob
    {
        public IHealthCheckDomainService HealthCheckDomainService { get; set; }
        public IEmailDomainServiceBase EmailDomainService { get; set; }

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
                throw new Exception("Test");

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
                var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
                var recipients = GlobalConfiguration.EmailConfiguration.Recipients_TiktokEmail;

                var to = recipients.FirstOrDefault();
                var message = new MailMessage(from, to, subject, textAttachment);
                await EmailDomainService.SendUsingNetworkCredentials(message);
            }

            //await HealthCheckDomainService.SendNotificationToSlack(msg);
        }

        protected abstract Task DoExecute();
    }

}