using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.Users;
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.Identity;
using Volo.Abp.TextTemplating;


namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public interface IPayrollEmailDomainService : IDomainService
    {
        Task Send(bool isHappyDay, Guid? payrollId);
    }

    public class PayrollEmailDomainService : EmailDomainServiceBase, IPayrollEmailDomainService
    {
        private readonly IPayrollDomainService _payrollDomainService;

        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly IRepository<UserInfo, Guid> _userInfoRepository;
        private readonly IOrganizationUnitRepository _organizationUnitRepository;
        private readonly IApiConfigurationDomainService _apiConfigurationDomainService;

        public PayrollEmailDomainService(
            ISmtpEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IPayrollDomainService payrollDomainService,
            IOrganizationDomainService organizationDomainService,
            IRepository<AppUser, Guid> appUserRepository,
            IRepository<UserInfo, Guid> userInfoRepository,
            IOrganizationUnitRepository organizationUnitRepository,
            IApiConfigurationDomainService apiConfigurationDomainService) : base(emailSender, templateRenderer)
        {
            _payrollDomainService = payrollDomainService;
            _organizationDomainService = organizationDomainService;
            _appUserRepository = appUserRepository;
            _userInfoRepository = userInfoRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _apiConfigurationDomainService = apiConfigurationDomainService;
        }

        public async Task Send(bool isHappyDay, Guid? payrollId)
        {
            var payrollConfig = _apiConfigurationDomainService.GetPayrollConfiguration();
            var configuration = GlobalConfiguration;

            var response = new PayrollResponse();
            if (payrollId.IsNotNullOrEmpty())
            {
                var payrollDetailRequest = new PayrollDetailRequest
                {
                    PayrollId = payrollId,
                };
                response = await _payrollDomainService.GetPayrollDetail(payrollDetailRequest);
                isHappyDay = response.IsHappyDay;
            }
            else { response = await _payrollDomainService.CalculateUserPayrolls(true, true, isHappyDay); }

            var fromDateTime = response.FromDateTime.AddHours(payrollConfig.PayrollTimeZone);
            var toDateTime = response.ToDateTime.AddHours(payrollConfig.PayrollTimeZone);

            var title = L["PayrollEmail.Title"] + " " + (isHappyDay ? PayrollConsts.HappyDay : string.Empty);
            var subject = $"{title} " + $"{fromDateTime:dd/MM/yyyy} - " + $"{toDateTime:dd/MM/yyyy}";
            var body = $"<h2 style='text-align:center; color:RED; display:block;'>{subject}</h2>";

            var orgUnits = await _organizationUnitRepository.GetListAsync();

            body += $"<b>{L["PayrollEmail.TeamCount"]}:</b> {response.UserPayrolls.Select(_ => _.OrganizationId).Distinct().Count()} <br/>";
            body += $"<b>{L["PayrollEmail.StaffCount"]}:</b> {response.UserPayrolls.Count} <br/>";
            body += $"<b>{L["PayrollEmail.WaveTotalAmount"]}:</b> {response.UserPayrolls.Select(p => p.WaveAmount).Sum().ToVND()} <br/>";
            body += $"<b>{L["PayrollEmail.BonusTotalAmount"]}:</b> {response.UserPayrolls.Select(p => p.BonusAmount).Sum().ToVND()} <br/>";
            body += $"<b>{L["PayrollEmail.TotalAmount"]}:</b> {response.UserPayrolls.Select(p => p.TotalAmount).Sum().ToVND()} <br/> <hr/>";

            var _teamPayrolls = new Dictionary<string, List<UserPayroll>>();
            foreach (var orgGroup in response.UserPayrolls.GroupBy(_ => _.OrganizationId))
            {
                var orgId = orgGroup.Key.ToNullableGuid();
                var payrolls = orgGroup.ToList();
                if (orgId != null)
                {
                    var orgUnit = orgUnits.FirstOrDefault(_ => _.Id == orgId);
                    _teamPayrolls.Add(orgUnit != null ? orgUnit.DisplayName : L["PayrollEmail.ErrorTeamStaff"], payrolls);
                    continue;
                }

                _teamPayrolls.Add(L["PayrollEmail.NoTeamStaff"], payrolls);
            }

            var sortedDict = from entry in _teamPayrolls orderby entry.Key ascending select entry;
            _teamPayrolls = sortedDict.ToDictionary(_ => _.Key, _ => _.Value);

            foreach (var payrollTeam in _teamPayrolls)
            {
                var teamName = string.Empty;
                if (payrollTeam.Key.IsNotNullOrWhiteSpace()) { teamName = payrollTeam.Key; }

                body += await RenderTeamTable(teamName, payrollTeam.Value);
            }

            if (!isHappyDay)
            {
                var communityBonusesSubject = (isHappyDay ? $" - {PayrollConsts.HappyDay}" : string.Empty);
                body += $"<br />";
                body
                    += $"<h2 style='text-align:center; color:RED; display:block;'>{L["PayrollEmail.CommunityBonuses", fromDateTime.ToDateFormatted(), toDateTime.ToDateFormatted()]} {communityBonusesSubject} </h2>";
                body += $"<b>Tổng thưởng</b>: {response.UserPayrolls.Select(p => p.BonusAmount).Sum().ToVND()}<br/>";
                body += $"<br />";
                body += await RenderCommunityBonusTable(response.CommunityBonuses, response.Commissions);
            }

            var from = configuration.EmailConfiguration.AdminEmail;
            var recipients = configuration.EmailConfiguration.Recipients_PayrollEmail;
            if (isHappyDay) recipients = configuration.EmailConfiguration.Recipients_AccountantEmail;

            var to = recipients.FirstOrDefault();
            var ccs = recipients.Skip(1).ToList();

            var message = new MailMessage(from, to, subject, body) {IsBodyHtml = true};
            foreach (var cc in ccs) { message.CC.Add(cc); }

            // await SendUsingGmail(message);
            await SendUsingGmail(message);

        }

        private async Task<string> RenderTeamTable(string teamName, List<UserPayroll> userPayrolls)
        {
            var body = $"<h3 style='text-align:center; color:Green; display:block;'>Team: {teamName}</h3>";
            body += $"<b>{L["PayrollEmail.WaveTotalAmountTeam"]}:</b> {userPayrolls.Select(_ => _.WaveAmount).Sum().ToVND()}<br/>";
            body += $"<b>{L["PayrollEmail.BonusTotalAmountTeam"]}:</b> {userPayrolls.Select(_ => _.BonusAmount).Sum().ToVND()}<br/>";
            body += $"<b>{L["PayrollEmail.TotalAmountTeam"]}:</b> {userPayrolls.Select(_ => _.TotalAmount).Sum().ToVND()}<br/>";
            body += "<br />";

            var css_OneLine = "table-layout: fixed; width:70px;";
            var sb = new StringBuilder();
            using (var table = new HtmlHelper.Table(sb, "1", "main-table"))
            {
                table.StartHead();
                using (var thead = table.AddRowTHead())
                {
                    thead.AddCell(L["PayrollEmail.OrganizationUnit.DisplayName"]);
                    thead.AddCell(L["PayrollEmail.AppUser.Name"]);
                    thead.AddCell(L["PayrollEmail.AppUser.UserName"]);
                    thead.AddCell(L["PayrollEmail.AppUser.Email"]);
                    thead.AddCell(L["PayrollEmail.AppUser.Phone"]);
                    thead.AddCell(L["PayrollEmail.ContentRoleType"]);
                    thead.AddCell(L["PayrollEmail.AffiliateMultiplier"]);
                    thead.AddCell(L["PayrollEmail.SeedingMultiplier"]);
                    thead.AddCell(L["PayrollEmail.SeedingWaveAmount"], style: css_OneLine);
                    thead.AddCell(L["PayrollEmail.SeedingBonusAmount"], style: css_OneLine);
                    thead.AddCell(L["PayrollEmail.AffiliateWaveAmount"], style: css_OneLine);
                    thead.AddCell(L["PayrollEmail.AffiliateBonusAmount"], style: css_OneLine);
                    thead.AddCell(L["PayrollEmail.WaveAmount"], style: css_OneLine);
                    thead.AddCell(L["PayrollEmail.BonusAmount"], style: css_OneLine);
                    thead.AddCell(L["PayrollEmail.TotalAmount"], style: css_OneLine);
                    thead.AddCell(L["PayrollEmail.Detail"]);
                }

                table.EndHead();

                table.StartBody();
                foreach (var userPayrollRow in userPayrolls)
                {
                    try
                    {
                        var organizationUnit = (await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest()
                        {
                            IsGDLNode = true,
                            UserId = userPayrollRow.User.Id
                        })).FirstOrDefault();

                        using var tr = table.AddRow();
                        tr.AddCell(organizationUnit?.DisplayName);
                        tr.AddCell(userPayrollRow.User.Name);
                        tr.AddCell(userPayrollRow.User.UserName);
                        tr.AddCell(userPayrollRow.User.Email);
                        tr.AddCell(userPayrollRow.User.PhoneNumber);
                        tr.AddCell(L[$"Enum:ContentRoleType:{(int) userPayrollRow.ContentRoleType}"]);
                        tr.AddCell(userPayrollRow.UserInfo.AffiliateMultiplier.ToString(CultureInfo.InvariantCulture));
                        tr.AddCell(userPayrollRow.UserInfo.SeedingMultiplier.ToString(CultureInfo.InvariantCulture));
                        tr.AddCell(userPayrollRow.SeedingWaves.Sum(_ => _.Amount).ToVND());
                        tr.AddCell(userPayrollRow.SeedingBonuses.Sum(_ => _.Amount).ToVND());
                        tr.AddCell(userPayrollRow.AffiliateWaves.Sum(_ => _.Amount).ToVND());
                        tr.AddCell(userPayrollRow.AffiliateBonuses.Sum(_ => _.Amount).ToVND());
                        tr.AddCell(userPayrollRow.WaveAmount.ToVND());
                        tr.AddCell(userPayrollRow.BonusAmount.ToVND());
                        tr.AddCell(userPayrollRow.TotalAmount.ToVND());

                        var viewDetailUrl = $"{GlobalConfiguration.Services.BackofficeUrl}/payslip/{userPayrollRow.UserInfo.Code}/{userPayrollRow.Id}";
                        tr.AddCell($"<a href='{viewDetailUrl}' target='_blank'>{L["PayrollEmail.View"]}</a>");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("===================exception " + e.Message);
#if DEBUG
                        throw;
#endif
                    }
                }

                table.EndBody();
            }

            body += sb.ToString();

            body += "<br /> <br /> <br />";

            return body;
        }

        private async Task<string> RenderCommunityBonusTable(List<UserPayrollBonus> commissionBonus, List<UserPayrollCommission> commissions)
        {
            var css_OneLine = "table-layout: fixed; width:100px;";
            var sb = new StringBuilder();
            using (var table = new HtmlHelper.Table(sb, "1", "main-table"))
            {
                table.StartHead();
                using (var thead = table.AddRowTHead())
                {
                    thead.AddCell(L["PayrollEmail.OrganizationUnit.DisplayName"]);
                    thead.AddCell(L["PayrollEmail.AppUser.Name"]);
                    thead.AddCell(L["PayrollEmail.AppUser.UserName"]);
                    thead.AddCell(L["PayrollEmail.AppUser.Email"]);
                    thead.AddCell(L["PayrollEmail.AppUser.Phone"]);
                    thead.AddCell(L["PayrollEmail.PayrollBonusType"]);
                    thead.AddCell(L["PayrollEmail.Amount"], style: css_OneLine);
                    thead.AddCell(L["Description"]);
                }

                table.EndHead();

                table.StartBody();

                var models = new List<PayrolEmailBonusModel>();
                foreach (var item in commissionBonus)
                {
                    try
                    {
                        var appUser = await _appUserRepository.FindAsync(item.AppUserId.Value);
                        var userInfo = await _userInfoRepository.FindAsync(_ => _.AppUserId == item.AppUserId.Value);
                        var organizationUnit = (await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest()
                        {
                            IsGDLNode = true,
                            UserId = item.AppUserId
                        })).FirstOrDefault();

                        var m = new PayrolEmailBonusModel();
                        m.OrganizationUnitName = organizationUnit?.DisplayName;
                        m.Name = appUser?.Name;
                        m.UserName = appUser?.UserName;
                        m.Email = appUser?.Email;
                        m.PhoneNumber = appUser?.PhoneNumber;
                        m.PayrollBonusType = L[$"Enum:PayrollBonusType:{(int) item.PayrollBonusType}"];
                        m.Amount = item.Amount.ToVND();
                        m.Description = item.Description;

                        models.Add(m);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("===================exception " + e.Message);
#if DEBUG
                        throw;
#endif
                    }
                }

                foreach (var item in commissions)
                {
                    try
                    {
                        var appUser = await _appUserRepository.FindAsync(item.AppUserId.Value);
                        var userInfo = await _userInfoRepository.FindAsync(_ => _.AppUserId == item.AppUserId.Value);
                        var organizationUnit = (await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest()
                        {
                            IsGDLNode = true,
                            UserId = item.AppUserId
                        })).FirstOrDefault();

                        var m = new PayrolEmailBonusModel();
                        m.OrganizationUnitName = organizationUnit?.DisplayName;
                        m.Name = appUser?.Name;
                        m.UserName = appUser?.UserName;
                        m.Email = appUser?.Email;
                        m.PhoneNumber = appUser?.PhoneNumber;
                        m.PayrollBonusType = L[$"Enum:PayrollCommissionType:{(int) item.PayrollCommissionType}"];
                        m.Amount = item.Amount.ToVND();
                        m.Description = item.Description;

                        models.Add(m);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("===================exception " + e.Message);
#if DEBUG
                        throw;
#endif
                    }
                }

                foreach (var row in models.OrderBy(m => m.OrganizationUnitName))
                {
                    using var tr = table.AddRow();
                    tr.AddCell(row.OrganizationUnitName);
                    tr.AddCell(row.Name);
                    tr.AddCell(row.UserName);
                    tr.AddCell(row.Email);
                    tr.AddCell(row.PhoneNumber);
                    tr.AddCell(row.PayrollBonusType);
                    tr.AddCell(row.Amount);
                    tr.AddCell(row.Description);
                }

                table.EndBody();
            }

            return sb.ToString();
        }

        public class PayrolEmailBonusModel
        {
            public string OrganizationUnitName { get; set; }
            public string Name { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string PayrollBonusType { get; set; }
            public string Amount { get; set; }
            public string Description { get; set; }
        }
    }
}