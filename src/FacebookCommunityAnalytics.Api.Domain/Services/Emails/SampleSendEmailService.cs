using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Notifications.Emails;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;
using Volo.Abp.Threading;

namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public interface ISampleSendEmailService : IDomainService
    {
        Task Send(SampleEmailModel model);
    }

    public class SampleSendEmailService : EmailDomainServiceBase, ISampleSendEmailService
    {
        public SampleSendEmailService(
            ISmtpEmailSender smtpEmailSender
            , ITemplateRenderer templateRenderer) : base(smtpEmailSender, templateRenderer)
        {
        }

        public async Task Send(SampleEmailModel model)
        {
            await DoSend(model);
        }
        
        private async Task DoSend(SampleEmailModel model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var config = GlobalConfiguration.EmailConfiguration;

            //var body = await TemplateRenderer.RenderAsync(EmailTemplateConsts.Sample, model);
            var body = $"Current Server IP: {GetLocalIPAddress()} \n";
            var subject = $"TEST EMAIL - Current language is {L["Language.vi"]}  CAN BE DELETED from {GlobalConfiguration.EmailConfiguration.AdminEmail} at {DateTime.UtcNow} UTC";

            var from = config.AdminEmail;
            var recipients = config.Recipients_Test;
            var to = recipients.FirstOrDefault();
            var ccs = recipients.Skip(1).ToList();

            var message = new MailMessage(from, to, subject, body) {IsBodyHtml = true};
            foreach (var cc in ccs)
            {
                message.CC.Add(cc);
            }

            var sampleStreamText = "This is a sample stream content generated from sample email";
            using var stream = new MemoryStream(sampleStreamText.GetBytes());
            var attachment = new Attachment(stream, new ContentType("text/plain"));

            message.Attachments.Add(attachment);

            await SendUsingGmail(message);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}