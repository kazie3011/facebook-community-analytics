using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public interface IDailyKPITrackingEmailDomainService
    {
        Task DoSendEMails();
    }

    public class DailyKPITrackingEmailDomainService : EmailDomainServiceBase, IDailyKPITrackingEmailDomainService
    {
        private readonly IDailyKPITrackingDomainService _dailyKpiTrackingDomainService;

        public DailyKPITrackingEmailDomainService(
            ISmtpEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IDailyKPITrackingDomainService dailyKpiTrackingDomainService) : base(emailSender, templateRenderer)
        {
            _dailyKpiTrackingDomainService = dailyKpiTrackingDomainService;
        }

        public async Task DoSendEMails()
        {
            var results = await _dailyKpiTrackingDomainService.GetUnqualifiedDailyKPIResults();
            if (results.Any())
            {
                foreach (var result in results)
                {
                    await DoSendEmail(result);
                }
            }
        }

        private async Task DoSendEmail(UnqualifiedDailyKPIResult result)
        {
            var subject = L["DailyKPIEmail.Title",  DateTime.UtcNow.ToShortDateString()];
            var body = BuildEmailBodyContent(result);
            var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
            var to = string.Join(",", result.Leaders.Select(x => x.Email));
            var message = new MailMessage(from, to, subject, body) {IsBodyHtml = true};
            await SendUsingGmail(message);
        }

        private string BuildEmailBodyContent(UnqualifiedDailyKPIResult result)
        {
            var sb = new StringBuilder();
            using var table = new HtmlHelper.Table(sb, "0", "main-table");
            using var row = table.AddRow();
            using var head = table.AddRowTHead();

            const string cssOneLine = "text-align:left;table-layout: fixed; width:200px;";
            const string cssNumber = "text-align:left;table-layout: fixed; width:80px;";
            table.StartHead();
            head.AddCell(L["DailyKPIEmail.UserName"], style: cssOneLine);
            head.AddCell(L["DailyKPIEmail.Target"], style: cssNumber);
            head.AddCell(L["DailyKPIEmail.Actual"], style: cssNumber);
            table.EndHead();

            foreach (var unqualifiedResult in result.UnqualifiedResults)
            {
                table.StartBody();
                row.AddCell(unqualifiedResult.User.UserName);
                row.AddCell(unqualifiedResult.RequiredQuantity.ToString());
                row.AddCell(unqualifiedResult.ActualQuantity.ToString());
                table.EndBody();
            }

            return sb.ToString();
        }
    }
}