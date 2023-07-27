using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IAffiliateStatsEmailDomainService : IDomainService
    {
        Task SendUserAffConversionSyncResultEmail(SyncUserAffConversionEmailModel emailModel);
    }
    
    public class AffiliateStatsEmailDomainService : EmailDomainServiceBase, IAffiliateStatsEmailDomainService
    {
        public AffiliateStatsEmailDomainService(ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
        }
        
        public async Task SendUserAffConversionSyncResultEmail(SyncUserAffConversionEmailModel emailModel)
        {
            var model = new Model
            {
                ShortlinkTotal = emailModel.ShortlinkCount,
                GdlShortlinks = emailModel.GDL.Count,
                HappyDayShortlinks = emailModel.HPD.Count,
                ClickCount = emailModel.GetAllAffs().Sum(x => x.AffConversionModel.ClickCount),
                CommissionTotal = emailModel.GDL.Sum(x => x.AffConversionModel.CommissionAmount) + emailModel.GDL.Sum(x => x.AffConversionModel.CommissionBonusAmount),
                ConversionCount = emailModel.GDL.Sum(x => x.AffConversionModel.ConversionCount),
                AmountTotal = emailModel.GDL.Sum(x => x.AffConversionModel.ConversionAmount),
                ApiTimeTaken = emailModel.ApiTimeTaken
            };
            
            var subject = $"INTRADAY AFFILIATE STATS - {DateTime.UtcNow.AddDays(-1):yyyy/MM/dd d}";
            var body = $"<h2 style='text-align:center; color:RED; display:block;'>{subject}</h2>";
            body += $"<b>{L["SendAffiliatesModel.ShortlinkTotal"]}:</b> {model.ShortlinkTotal:n0} <br/>";
            body += $"<b>{L["SendAffiliatesModel.GdlShortlinks"]}:</b> {model.GdlShortlinks:n0} <br/>";
            body += $"<b>{L["SendAffiliatesModel.HappyDayShortlinks"]}:</b> {model.HappyDayShortlinks:n0} <br/>";
            body += $"<b>{L["SendAffiliatesModel.AmountTotal"]}:</b> {model.AmountTotal:n0} <br/>";
            body += $"<b>{L["SendAffiliatesModel.CommissionTotal"]}:</b> {model.CommissionTotal:n0} <br/>";
            body += $"<b>{L["SendAffiliatesModel.ConversionCount"]}:</b> {model.ConversionCount:n0} <br/>";
            body += $"<b>{L["SendAffiliatesModel.ClickCount"]}:</b> {model.ClickCount:n0} <br/>";
            body += $"<b>{L["SendAffiliatesModel.ApiTime"]}:</b> {DateTime.UtcNow.AddDays(-1):dd/MM/yyyy} ({L["SendAffiliatesModel.ApiTimeTaken"]} {model.ApiTimeTaken.Hours:00}:{model.ApiTimeTaken.Minutes:00}:{model.ApiTimeTaken.Seconds:00})<br/>";
            body += $"<b>{L["SendAffiliatesModel.ApiTimeTaken"]}: {model.ApiTimeTaken:c}<br/>";
            // body += $"<b>{L["SendAffiliatesModel.ApiTimeTaken"]}: {sendAffiliatesModel.ApiTimeTaken.Hours:00}:{sendAffiliatesModel.ApiTimeTaken.Minutes:00}:{sendAffiliatesModel.ApiTimeTaken.Seconds:00}<br/>";
            body += $"(Noted: all shortlinks from {model.FromDateTime:dd/MM/yyyy} to {model.ToDateTime:dd/MM/yyyy})";
            
            var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
            var recipients = GlobalConfiguration.EmailConfiguration.Recipients_Test;

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
        
        class Model
        {
            public int ShortlinkTotal { get; set; }
            public int GdlShortlinks { get; set; }
            public int HappyDayShortlinks { get; set; }
            public decimal AmountTotal { get; set; }
            public decimal CommissionTotal { get; set; }
            public int ConversionCount { get; set; }
            public int ClickCount { get; set; }
            public TimeSpan ApiTimeTaken { get; set; }
            public DateTime FromDateTime { get; set; }
            public DateTime ToDateTime { get; set; }
        }
    }
}