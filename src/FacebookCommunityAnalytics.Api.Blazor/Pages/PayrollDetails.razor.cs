using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using Flurl;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class PayrollDetails
    {
        [Parameter] public string PayrollId { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        private DateTime FromDate { get; set; }
        private DateTime ToDate { get; set; }

        private PayrollDetailResponse PayrollDetail { get; set; }
        private Dictionary<TeamTotalAmountCalculator, List<UserPayrollDto>> _teamPayrolls;
        private List<CommunityBonusBlazorModel> _communityBonusBlazorModels;

        private BonusTotalAmountCalculator BonusTotalCalculator = new();
        protected string TextAlignLeft = "text-align: left";
        protected string TextAlignRight = "text-align: right";

        public PayrollDetails()
        {
            PayrollDetail = new PayrollDetailResponse();
            _teamPayrolls = new Dictionary<TeamTotalAmountCalculator, List<UserPayrollDto>>();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            PayrollId ??= string.Empty;
            GlobalConfiguration = await PayrollsAppService.GetGlobalConfiguration();

            var now = DateTime.UtcNow;
            (FromDate, ToDate) = GetPayrollDateTime(now.Year, now.Month);

            await SetBreadcrumbItemsAsync();
            await GetPayrollDetailsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["PayrollDetails.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            //BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["PayrollDetails.BreadcrumbItem"]));
            return ValueTask.CompletedTask;
        }

        public async Task GetPayrollDetailsAsync()
        {
            PayrollDetail = await PayrollsAppService.GetPayrollDetail
            (
                new PayrollDetailRequest
                {
                    PayrollId = PayrollId.ToNullableGuid(),
                }
            );
            if (PayrollDetail != null)
            {
                FromDate = PayrollDetail.FromDateTime;
                ToDate = PayrollDetail.ToDateTime;
                if (!IsManagerRole())
                {
                    var currentUserOrganization = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest
                    {
                        UserId = (Guid) CurrentUser.Id
                    });
                    foreach (var organization in currentUserOrganization)
                    {
                        var payrolls = PayrollDetail.UserPayrolls.Where(_ => _.OrganizationId == organization.Id.ToString()).ToList();
                        var teamCalculator = new TeamTotalAmountCalculator()
                        {
                            TotalRecord = payrolls.Count,
                            TotalWave = payrolls.Select(_ => _.WaveAmount).Sum(),
                            TotalBonus = payrolls.Select(_ => _.BonusAmount).Sum(),
                            TotalAmount = payrolls.Select(_ => _.TotalAmount).Sum(),
                        };
                        teamCalculator.TeamName = organization != null ? organization.DisplayName : L["PayrollEmail.ErrorTeamStaff"];
                        _teamPayrolls.Add(teamCalculator, payrolls);
                    }
                }
                else
                {
                    var orgUnits = await TeamMemberAppService.GetList();
                    foreach (var orgGroup in PayrollDetail.UserPayrolls.GroupBy(_ => _.OrganizationId))
                    {
                        var orgId = orgGroup.Key.ToNullableGuid();
                        var payrolls = orgGroup.ToList();

                        var teamCalculator = new TeamTotalAmountCalculator
                        {
                            TotalRecord = payrolls.Count,
                            TotalWave = payrolls.Select(_ => _.WaveAmount).Sum(),
                            TotalBonus = payrolls.Select(_ => _.BonusAmount).Sum(),
                            TotalAmount = payrolls.Select(_ => _.TotalAmount).Sum(),
                        };

                        if (orgId == null)
                        {
                            teamCalculator.TeamName = L["PayrollEmail.NoTeamStaff"];
                            payrolls = payrolls.Where(p => p.TotalAmount > 0).ToList();
                        }
                        else
                        {
                            var orgUnit = orgUnits.FirstOrDefault(_ => _.Id == orgId);
                            teamCalculator.TeamName = orgUnit != null ? orgUnit.DisplayName : L["PayrollEmail.ErrorTeamStaff"];
                        }

                        if (payrolls.IsNotNullOrEmpty())
                        {
                            _teamPayrolls.Add(teamCalculator, payrolls);
                        }
                    }

                    _teamPayrolls = (from entry in _teamPayrolls orderby entry.Key.TeamName ascending select entry).ToDictionary(_ => _.Key, _ => _.Value);

                    var users = PayrollDetail.UserPayrolls.Select(_ => _.User).ToList();
                    var models = new List<CommunityBonusBlazorModel>();
                    var orderedModels = new List<CommunityBonusBlazorModel>();
                    foreach (var item in PayrollDetail.CommunityBonuses)
                    {
                        if (item.AppUserId == null)
                        {
                            return;
                        }

                        var appUser = users.FirstOrDefault(_ => _.Id == item.AppUserId.Value);
                        var organizationName = PayrollDetail.UserPayrolls.FirstOrDefault(_ => _.AppUserId.Value == item.AppUserId.Value).OrganizationName;

                        var communityBonusBlazorModel = new CommunityBonusBlazorModel
                        {
                            OrganizationName = organizationName,
                            Name = appUser?.Name,
                            UserName = appUser?.UserName,
                            Email = appUser?.Email,
                            PhoneNumber = appUser?.PhoneNumber,
                            PayrollBonusType = L[$"Enum:PayrollBonusType:{(int) item.PayrollBonusType}"],
                            Amount = item.Amount,
                            Description = item.Description
                        };

                        models.Add(communityBonusBlazorModel);
                    }

                    foreach (var item in PayrollDetail.TeamBonuses)
                    {
                        var communityBonusBlazorModel = new CommunityBonusBlazorModel
                        {
                            OrganizationName = item.TeamName,
                            Name = string.Empty,
                            PayrollBonusType = L[$"Enum:PayrollBonusType:{(int) item.PayrollBonusType}"],
                            Amount = item.Amount,
                            Description = item.Description
                        };
                        models.Add(communityBonusBlazorModel);
                    }

                    foreach (var item in models.GroupBy(_ => _.OrganizationName).OrderBy(_ => _.Key))
                    {
                        orderedModels.AddRange(item.ToList());
                        orderedModels.Add(new CommunityBonusBlazorModel());
                    }

                    if (orderedModels.Any()) orderedModels.Remove(orderedModels.Last());

                    _communityBonusBlazorModels = orderedModels;

                    BonusTotalCalculator.Name = L["PayrollEmail.CommunityBonuses", FromDate, ToDate];
                    BonusTotalCalculator.TotalRecord = models.Count();
                    BonusTotalCalculator.TotalBonus = models.Select(_ => _.Amount).Sum();
                }
            }
        }

        private bool IsDescContainUrl(string desc)
        {
            if (desc.IsNullOrWhiteSpace()) return false;

            var parts = desc.Split('-');
            return parts.Length == 2;
        }

        private string GetDescriptionUrl(string desc)
        {
            if (desc.IsNullOrWhiteSpace()) return desc;

            var parts = desc.Split('-');
            if (parts.Length < 2) return string.Empty;

            return parts[1];
        }
    }

    public class CommunityBonusBlazorModel
    {
        public string OrganizationName { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PayrollBonusType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class TeamTotalAmountCalculator
    {
        public string TeamName { get; set; }
        public int TotalRecord { get; set; }
        public decimal TotalWave { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class BonusTotalAmountCalculator
    {
        public string Name { get; set; }
        public int TotalRecord { get; set; }
        public decimal TotalBonus { get; set; }
    }
}