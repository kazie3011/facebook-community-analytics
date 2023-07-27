using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Exports;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Newtonsoft.Json;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public interface ITiktokEmailDomainService : IDomainService
    {
        Task SendTiktokDaily();
    }

    public class TiktokEmailDomainService : EmailDomainServiceBase, ITiktokEmailDomainService
    {
        private readonly ITiktokDomainService _tiktokDomainService;
        private readonly IExportDomainService _exportDomainService;

        public TiktokEmailDomainService(
            ISmtpEmailSender smtpEmailSender,
            ITemplateRenderer templateRenderer,
            ITiktokDomainService tiktokDomainService,
            IExportDomainService exportDomainService) : base(smtpEmailSender, templateRenderer)
        {
            _tiktokDomainService = tiktokDomainService;
            _exportDomainService = exportDomainService;
        }

        public async Task SendTiktokDaily()
        {
            var now = DateTime.UtcNow.Date;
            var startDate = now.AddDays(-1);
            var endDate = now.AddDays(1);

            var filter = new GetTiktoksInputExtend()
            {
                CreatedDateTimeMin = startDate,
                CreatedDateTimeMax = endDate,
                SendEmail = true,
                MaxResultCount = Int32.MaxValue
            };

            var tiktokVideos = await _tiktokDomainService.GetExportRows(filter);

            var vnTimeZoneKey = "SE Asia Standard Time";
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            foreach (var row in tiktokVideos)
            {
                row.Category = L[$"Enum:GroupCategoryType:{(int)row.Category.ToEnumerable<GroupCategoryType>()}"];
                row.CreatedDateTime = TimeZoneInfo.ConvertTimeFromUtc(row.CreatedDateTime, vnTimeZone);
            }

            var title = L["TiktokEmail.TiktokDailyReport"];

            var excelBytes = _exportDomainService.GenerateTiktokExcelBytes(tiktokVideos, title);

            var subject = $"{title} " + $"{now:dd/MM/yyyy}";

            var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
            var recipients = GlobalConfiguration.EmailConfiguration.Recipients_TiktokEmail;

            var to = recipients.FirstOrDefault();
            var ccs = recipients.Skip(1).ToList();

            if (to != null)
            {
                var body = excelBytes != null ? string.Empty : L["TiktokEmail.NoPost"];

                var message = new MailMessage(@from, to, subject, body);
                foreach (var cc in ccs) { message.CC.Add(cc); }

                if (excelBytes != null)
                {
                    message.Attachments.Add(new Attachment(GetStream(excelBytes), $"{subject}.xlsx"));
                }

                await SendUsingGmail(message);

                var apiRequest = ObjectMapper.Map<GetTiktoksInputExtend, UpdateTiktokStateApiRequest>(filter);
                apiRequest.IsNew = false;

                await _tiktokDomainService.UpdateTiktokState(apiRequest);
            }
        }
    }
}