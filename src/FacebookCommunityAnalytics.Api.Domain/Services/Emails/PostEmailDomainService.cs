using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Posts;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public interface IPostEmailDomainService : IDomainService
    {
        Task SendUncrawlsPosts();
    }

    public class PostEmailDomainService : EmailDomainServiceBase, IPostEmailDomainService
    {
        private readonly PayrollConfiguration _payrollConfiguration;
        private readonly IPostRepository _postRepository;

        public PostEmailDomainService(ISmtpEmailSender emailSender
            , ITemplateRenderer templateRenderer
            , ApiConfigurationDomainService apiConfigurationDomainService
            , IPostRepository postRepository) : base(emailSender, templateRenderer)
        {
            _payrollConfiguration = apiConfigurationDomainService.GetPayrollConfiguration();
            _postRepository = postRepository;
        }

        public async Task SendUncrawlsPosts()
        {
            try
            {
                var config = GlobalConfiguration.EmailConfiguration;
                var subject = $"Email Auto Do Not Reply";
                var now = DateTime.UtcNow;
                var fromDateTime = new DateTime(now.Year, now.Month, _payrollConfiguration.StartDay, _payrollConfiguration.StartHour, 0, 0);
                if (fromDateTime > now)
                {
                    fromDateTime = new DateTime(now.Year, now.Month - 1, _payrollConfiguration.StartDay, _payrollConfiguration.StartHour, 0, 0);
                }

                var twoDayAgo = DateTime.UtcNow.Date.AddDays(-2);
                var posts = await _postRepository.GetListAsync(p =>
                    (p.LastCrawledDateTime < twoDayAgo && p.CreatedDateTime > fromDateTime && p.IsNotAvailable == false));
                var countPosts = posts.Count | 0;
                var body = $"Still have {countPosts} posts";
                var from = config.AdminEmail;
                var recipients = config.Recipients_Test;
                var to = recipients.FirstOrDefault();
                var ccs = recipients.Skip(1).ToList();

                var message = new MailMessage(from, to, subject, body) {IsBodyHtml = true};
                foreach (var cc in ccs)
                {
                    message.CC.Add(cc);
                }

                // await SendUsingGmail(message);
                await SendUsingGmail(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}