using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public interface ICampaignEmailDomainService : IDomainService
    {
        Task Send(Guid campaignId);
        Task SendAll();
        Task SendCampaignWelcomeEmail(string email, string password, string campaignId, Campaign campaign);
    }

    public class CampaignEmailDomainService : EmailDomainServiceBase, ICampaignEmailDomainService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignDomainService _campaignDomainService;

        public CampaignEmailDomainService(
            ISmtpEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            ICampaignRepository campaignRepository,
            ICampaignDomainService campaignDomainService) : base
            (emailSender, templateRenderer)
        {
            _campaignRepository = campaignRepository;
            _campaignDomainService = campaignDomainService;
        }

        public async Task Send(Guid campaignId)
        {
            try
            {
                var campaign = await _campaignRepository.GetAsync(campaignId);
                if (campaign == null) return;

                var campaignDto = ObjectMapper.Map<Campaign, CampaignDto>(campaign);
                Console.WriteLine($"Campaign: {campaign.Name} --- Code: {campaign.Code}");

                // NO email no need to send camp details
                if (campaign.Emails.IsNullOrWhiteSpace()) return;

                var campaignPosts = await _campaignDomainService.GetPosts(campaign.Id);
                var affiliatePosts = campaignPosts.Where(p => p.PostContentType == PostContentType.Affiliate).ToList();
                var contestPosts = campaignPosts.Where(p => p.PostContentType == PostContentType.Contest).ToList();
                var seedingPosts = campaignPosts.Where(p => p.PostContentType == PostContentType.Seeding).ToList();

                if (affiliatePosts.IsNullOrEmpty() && contestPosts.IsNullOrEmpty() && seedingPosts.IsNullOrEmpty())
                {
                    return;
                }

                List<string> shortLinks = new();
                foreach (var post in affiliatePosts)
                {
                    if (post.Shortlinks.IsNotNullOrEmpty())
                    {
                        shortLinks.AddRange(post.Shortlinks);
                    }
                }

                var affiliates = shortLinks.IsNullOrEmpty() ? new() : (await _campaignDomainService.GetAffiliatesAsync(shortLinks));

                campaignDto.Current.Seeding_TotalPost = seedingPosts.Count;
                campaignDto.Current.Seeding_TotalReaction = seedingPosts.Sum(_ => _.TotalCount);

                campaignDto.Current.Affiliate_TotalPost = affiliatePosts.Count;
                campaignDto.Current.Affiliate_TotalClick = affiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ClickCount);
                campaignDto.Current.Affiliate_TotalConversion = affiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ConversionCount);
                campaignDto.Current.Affiliate_TotalConversionAmount = affiliates.Sum(_ => _.UserAffiliate.AffConversionModel.ConversionAmount);

                campaignDto.Current.Contest_TotalPost = contestPosts.Count;
                campaignDto.Current.Contest_TotalReaction = contestPosts.Sum(_ => _.TotalCount);
                // var seeding_TotalPostKPI = campaignDto.Target.Seeding_TotalPost == 0
                //     ? campaignDto.Current.Seeding_TotalPost.ToCommaStyle()
                //     : $"{campaignDto.Current.Seeding_TotalPost.ToCommaStyle()}/{campaignDto.Target.Seeding_TotalPost.ToCommaStyle()}";
                // var seeding_TotalReactionKPI = campaignDto.Target.Seeding_TotalReaction == 0
                //     ? campaignDto.Current.Seeding_TotalReaction.ToCommaStyle()
                //     : $"{campaignDto.Current.Seeding_TotalReaction.ToCommaStyle()}/{campaignDto.Target.Seeding_TotalReaction.ToCommaStyle()}";
                //
                // var affiliate_TotalPostKPI = campaignDto.Target.Affiliate_TotalPost == 0
                //     ? campaignDto.Current.Affiliate_TotalPost.ToCommaStyle()
                //     : $"{campaignDto.Current.Affiliate_TotalPost.ToCommaStyle()}/{campaignDto.Target.Affiliate_TotalPost.ToCommaStyle()}";
                // var affiliate_TotalClickKPI = campaignDto.Target.Affiliate_TotalClick == 0
                //     ? campaignDto.Current.Affiliate_TotalClick.ToCommaStyle()
                //     : $"{campaignDto.Current.Affiliate_TotalClick.ToCommaStyle()}/{campaignDto.Target.Affiliate_TotalClick.ToCommaStyle()}";
                // var affiliate_TotalConversionKPI = campaignDto.Target.Affiliate_TotalConversion == 0
                //     ? campaignDto.Current.Affiliate_TotalConversion.ToCommaStyle()
                //     : $"{campaignDto.Current.Affiliate_TotalConversion.ToCommaStyle()}/{campaignDto.Target.Affiliate_TotalConversion.ToCommaStyle()}";
                // var affiliate_TotalConversionAmountKPI = campaignDto.Target.Affiliate_TotalConversionAmount == 0
                //     ? campaignDto.Current.Affiliate_TotalConversionAmount.ToCommaStyle()
                //     : $"{campaignDto.Current.Affiliate_TotalConversionAmount.ToCommaStyle()}/{campaignDto.Target.Affiliate_TotalConversionAmount.ToCommaStyle()}";
                //
                // var contest_TotalPostKPI = campaignDto.Target.Contest_TotalPost == 0
                //     ? campaignDto.Current.Contest_TotalPost.ToCommaStyle()
                //     : $"{campaignDto.Current.Contest_TotalPost.ToCommaStyle()}/{campaignDto.Target.Contest_TotalPost.ToCommaStyle()}";
                // var contest_TotalReactionKPI = campaignDto.Target.Contest_TotalReaction == 0
                //     ? campaignDto.Current.Contest_TotalReaction.ToCommaStyle()
                //     : $"{campaignDto.Current.Contest_TotalReaction.ToCommaStyle()}/{campaignDto.Target.Contest_TotalReaction.ToCommaStyle()}";

                var title = L["CampaignEmail.Title", campaign.Name, DateTime.UtcNow.ToShortDateString()];
                var subject = $"{title}";
                var body = $"<h2 style='text-align:left; color:RED; display:block;'>{campaign.Name}</h2>";

                var normalFontStyle = $"style='text-align:left;font-style: normal;font-weight: normal;display:inline-block;'";
                var css_OneLine = "text-align:left;table-layout: fixed; width:200px;";
                var css_Number = "text-align:left;table-layout: fixed; width:80px;";
                var sb1 = new StringBuilder();
                using (var table = new HtmlHelper.Table(sb1, "0", "main-table"))
                {
                    using var tr = table.AddRow();
                    using var thead = table.AddRowTHead();
                    table.StartBody();
                    thead.AddCell(L["CampaignEmail.DateTimeTitle"], style: css_OneLine);
                    tr.AddCell($"{campaign.StartDateTime?.ToShortDateString()} - {campaign.EndDateTime?.ToShortDateString()}");
                    table.EndBody();

                    table.StartBody();
                    thead.AddCell(L["CampaignEmail.Code"], style: css_OneLine);
                    tr.AddCell(campaign.Code);
                    table.EndBody();

                    if (campaign.Hashtags.IsNotNullOrEmpty())
                    {
                        table.StartBody();
                        thead.AddCell(L["CampaignEmail.Hashtags"], style: css_OneLine);
                        tr.AddCell(campaign.Hashtags);
                        table.EndBody();
                    }

                    if (campaign.Description.IsNotNullOrEmpty())
                    {
                        table.StartBody();
                        thead.AddCell(L["Description"], style: css_OneLine);
                        tr.AddCell(campaign.Description);
                        table.EndBody();
                    }

                    table.StartBody();
                    thead.AddCell("", style: css_OneLine);
                    table.EndBody();
                }

                var sb2 = new StringBuilder();
                using var kpiTable = new HtmlHelper.Table(sb2, "0", "main-table");
                using var kpiTr = kpiTable.AddRow();
                using var kpiThead = kpiTable.AddRowTHead();

                kpiTable.StartHead();
                kpiThead.AddCell(L["CampaignEmail.Type"], style: css_OneLine);
                kpiThead.AddCell(L["CampaignEmail.Current"], style: css_Number);
                kpiThead.AddCell(L["CampaignEmail.Target"], style: css_Number);
                kpiTable.EndHead();

                if (seedingPosts.IsNotNullOrEmpty())
                {
                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.TotalSeedingPost"]);
                    kpiTr.AddCell(campaignDto.Current.Seeding_TotalPost.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Seeding_TotalPost.ToCommaStyle());
                    kpiTable.EndBody();

                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.TotalReaction"]);
                    kpiTr.AddCell(campaignDto.Current.Seeding_TotalReaction.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Seeding_TotalReaction.ToCommaStyle());
                    kpiTable.EndBody();
                }

                if (affiliatePosts.IsNotNullOrEmpty())
                {
                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.Affiliate_TotalPost"]);
                    kpiTr.AddCell(campaignDto.Current.Affiliate_TotalPost.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Affiliate_TotalPost.ToCommaStyle());
                    kpiTable.EndBody();

                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.Affiliate_TotalClick"]);
                    kpiTr.AddCell(campaignDto.Current.Affiliate_TotalClick.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Affiliate_TotalClick.ToCommaStyle());
                    kpiTable.EndBody();

                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.Affiliate_TotalConversion"]);
                    kpiTr.AddCell(campaignDto.Current.Affiliate_TotalConversion.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Affiliate_TotalConversion.ToCommaStyle());
                    kpiTable.EndBody();

                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.Affiliate_TotalConversionAmount"]);
                    kpiTr.AddCell(campaignDto.Current.Affiliate_TotalConversionAmount.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Affiliate_TotalConversionAmount.ToCommaStyle());
                    kpiTable.EndBody();
                }

                if (contestPosts.IsNotNullOrEmpty())
                {
                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.Contest_TotalPost"]);
                    kpiTr.AddCell(campaignDto.Current.Contest_TotalPost.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Contest_TotalPost.ToCommaStyle());
                    kpiTable.EndBody();

                    kpiTable.StartBody();
                    kpiTr.AddCell(L["CampaignTarget.Contest_TotalReaction"]);
                    kpiTr.AddCell(campaignDto.Current.Contest_TotalReaction.ToCommaStyle());
                    kpiTr.AddCell(campaignDto.Target.Contest_TotalReaction.ToCommaStyle());
                    kpiTable.EndBody();
                }


                body += sb1.ToString();
                if (campaignPosts.IsNotNullOrEmpty())
                {
                    body += $"<h3>{L["KPIs"]}</h3>";
                    body += sb2.ToString();
                }

                // emails empty => send to 
                var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
                var emails = campaignDto.Emails.IsNotNullOrEmpty()
                    ? campaignDto.Emails.SplitEmails()
                    : new List<string>();

                string to;
                List<string> bccList;
                if (emails.IsNullOrEmpty())
                {
                    to = string.Join(",", GlobalConfiguration.EmailConfiguration.Recipients_CampaignEmail_Primary);
                    bccList = GlobalConfiguration.EmailConfiguration.Recipients_CampaignEmail_Secondary;
                }
                else
                {
                    to = string.Join(",", emails);
                    bccList = GlobalConfiguration.EmailConfiguration.Recipients_CampaignEmail_Primary.Union
                        (
                            GlobalConfiguration.EmailConfiguration.Recipients_CampaignEmail_Secondary
                        )
                        .ToList();
                    Console.WriteLine($"Bcc Email: {to}");
                }

                var message = new MailMessage(from, to, subject, body) { IsBodyHtml = true };
                foreach (var bcc in bccList)
                {
                    Console.WriteLine($"Bcc Email: {bcc}");
                    message.Bcc.Add(bcc.Trim());
                }

                byte[] byteArr = await _campaignDomainService.GetCampaignExcelAsync(campaignId);
                if (byteArr != null)
                {
                    message.Attachments.Add(new Attachment(GetStream(byteArr), $"{campaign.Code}.xlsx"));
                }

                // await SendUsingGmail(message);
                // await SendUsingNetworkCredentials(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task SendAll()
        {
            var campaigns = await _campaignRepository.GetListAsync();

            var endedCampaigns = campaigns.Where(_ => _.Status == CampaignStatus.Ended && _.EndDateTime >= DateTime.UtcNow.AddDays(-(int)_.CampaignReportType)).ToList();
            foreach (var endedCampaign in endedCampaigns)
            {
                await SendDailyEmail(endedCampaign);
            }

            var runningCampaigns = campaigns.Where(_ => _.Status is CampaignStatus.Started or CampaignStatus.Hold).ToList();
            foreach (var runningCampaign in runningCampaigns)
            {
                await SendDailyEmail(runningCampaign);
            }
        }

        private async Task SendDailyEmail(Campaign campaign)
        {
            var reportDays = (int)campaign.CampaignReportType;
            var startDateTime = campaign.StartDateTime;
            if (startDateTime == null || DateTime.UtcNow < startDateTime.Value || reportDays == 0) return;
            if ((DateTime.UtcNow.Date - startDateTime.Value.Date).Days % reportDays == 0)
            {
                await Send(campaign.Id);
            }
        }

        public async Task SendCampaignWelcomeEmail(string email, string password, string campaignId, Campaign campaign)
        {
            var campaignUrl = $"{GlobalConfiguration.Services.BackofficeUrl}/campaigndetail/{campaignId}";

            var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
            var to = email;
            var subject = $"[GDL] Welcome to GDL campaign: {campaign.Name}";
            var body = string.Empty;

            body += $"Hi {email} <br/>";
            body += $"<br/>";

            body += $"{L["CampaignEmail.Welcome.Body"]} <a href={campaignUrl}>{campaign.Name}</a> <br/>";
            if (password.IsNotNullOrEmpty())
            {
                body += $"Username: {email}<br/>";
                body += $"Password: {password}<br/>";
            }

            body += $"<br/>";
            body += $"Best Regards, <br/>";
            body += $"GDL Team";

            var message = new MailMessage(from, to, subject, body) {IsBodyHtml = true};

            await SendUsingGmail(message);
        }
    }
}