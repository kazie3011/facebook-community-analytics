using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing.Smtp;

namespace FacebookCommunityAnalytics.Api
{
    public class CustomSenderEmail : SmtpEmailSender
    {
        public GlobalConfiguration GlobalConfiguration { get; set; }
        public CustomSenderEmail(ISmtpEmailSenderConfiguration smtpConfiguration, IBackgroundJobManager backgroundJobManager) : base(smtpConfiguration, backgroundJobManager)
        {
        }

        public override async Task SendAsync(MailMessage mail, bool normalize = true)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            mail.From = new MailAddress(await SmtpConfiguration.GetDefaultFromAddressAsync());
             await SendUsingNetworkCredentials(mail);
        }
        public async Task SendUsingNetworkCredentials(MailMessage message)
        {
            var client = await BuildClientAsync();
            client.Credentials = new NetworkCredential(GlobalConfiguration.EmailConfiguration.ApiKey, GlobalConfiguration.EmailConfiguration.ApiValue);
            await client.SendMailAsync(message);
        }
    }
}