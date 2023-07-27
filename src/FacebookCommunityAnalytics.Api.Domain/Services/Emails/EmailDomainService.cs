using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public interface IEmailDomainServiceBase : IDomainService
    {
        Task SendUsingGmail(MailMessage message);
        Task SendUsingNetworkCredentials(MailMessage message);
    }

    public abstract class EmailDomainServiceBase : BaseDomainService, IEmailDomainServiceBase
    {
        // protected IEmailSender EmailSender { get; set; }
        protected ISmtpEmailSender SmtpEmailSender { get; set; }
        protected ITemplateRenderer TemplateRenderer { get; set; }

        public EmailDomainServiceBase(ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer)
        {
            SmtpEmailSender = emailSender;
            TemplateRenderer = templateRenderer;
        }
        
        public async Task SendUsingGmail(MailMessage message)
        {
            try
            {
                var from = "";
                var password = "";
        
                var client = await SmtpEmailSender.BuildClientAsync();
                client.Port = 587;  
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;  
                client.UseDefaultCredentials = false;  
                client.Credentials = new NetworkCredential(from, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;  
                await client.SendMailAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SendUsingNetworkCredentials(MailMessage message)
        {
            try
            {
                var client = await SmtpEmailSender.BuildClientAsync();
                client.Credentials = new NetworkCredential(GlobalConfiguration.EmailConfiguration.ApiKey, GlobalConfiguration.EmailConfiguration.ApiValue);
                await client.SendMailAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected Stream GetStream(byte[] excelBytes)
        {
            using var steam = new MemoryStream(excelBytes, true);
            steam.Write(excelBytes, 0, excelBytes.Length);
            steam.Position = 0;
            return steam;
        }
    }
}