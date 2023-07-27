using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Nito.AsyncEx;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Payslip
    {
        private string _userOrganization;

        protected List<BreadcrumbItem> BreadcrumbItems = new();

        public Payslip()
        {
            UserPayrollNavDto = new UserPayrollWithNavigationPropertiesDto();
        }


        [Parameter] public string UserPayrollId { get; set; }

        [Parameter] public string UserCode { get; set; }

        protected PageToolbar Toolbar { get; } = new();
        private DateTime FromDate { get; set; }
        private DateTime ToDate { get; set; }
        private UserInfoWithNavigationPropertiesDto UserInfo { get; set; }
        private UserPayrollWithNavigationPropertiesDto UserPayrollNavDto { get; set; }
        private IList<OrganizationUnitDto> _orgDtos = new List<OrganizationUnitDto>();
        private PayrollConfiguration PayrollConfig { get; set; }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            UserPayrollId ??= string.Empty;
            UserCode ??= string.Empty;

            PayrollConfig = await ApiConfigurationAppService.GetPayrollConfiguration();
            var now = DateTime.UtcNow;

            (FromDate, ToDate) = GetPayrollDateTime(now.Year, now.Month);

            await SetBreadcrumbItemsAsync();
            await GetUserPayrollDetailAsync();
            _orgDtos = await TeamMemberAppService.GetList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["UserPayrollDetails.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["UserPayrollDetails.BreadcrumbItem"]));
            return ValueTask.CompletedTask;
        }

        private async Task OnViewDetailButtonClicked()
        {
            UserPayrollId = null;
            await GetUserPayrollDetailAsync();
        }

        private async Task GetUserPayrollDetailAsync()
        {
            if (UserPayrollId.IsNullOrWhiteSpace() && UserCode.IsNullOrWhiteSpace())
            {
                UserInfo = await UserInfosAppService.GetByUserIdAsync(CurrentUser.GetId());
                UserPayrollNavDto = await UserPayrollsAppService.GenerateUserPayroll
                (
                    new UserPayrollRequest
                    {
                        UserCode = UserCode,
                        FromDateTime = await GetUTCDateTime(FromDate),
                        ToDateTime = await GetUTCDateTime(ToDate)
                    }
                );
            }
            else if (UserPayrollId.IsNotNullOrWhiteSpace())
            {
                UserPayrollNavDto = await UserPayrollsAppService.GetWithNavigationPropertiesAsync(UserPayrollId.ToGuidOrDefault());
                UserInfo = await UserInfosAppService.GetByUserIdAsync(UserPayrollNavDto.AppUser.Id);
            }
            else
            {
                UserPayrollNavDto = await UserPayrollsAppService.GenerateUserPayroll
                (
                    new UserPayrollRequest
                    {
                        UserCode = UserCode,
                        FromDateTime = await GetUTCDateTime(FromDate),
                        ToDateTime = await GetUTCDateTime(ToDate),
                    }
                );
                UserInfo = await UserInfosAppService.GetByCodeAsync(UserCode);
            }

            if (UserInfo == null) return;

            _userOrganization = string.Empty;
            var userOrganizations = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest()
            {
                UserId = UserInfo.AppUser.Id
            });
            foreach (var organization in userOrganizations)
            {
                _userOrganization += $"{organization.DisplayName} ";
            }
        }

        private bool IsDescContainUrl(string desc)
        {
            if (desc.IsNullOrWhiteSpace()) return false;

            var parts = desc.Split('-');
            return parts.Length >= 2;
        }

        private string GetDescriptionUrl(string desc)
        {
            if (desc.IsNullOrWhiteSpace()) return desc;

            var parts = desc.Split('-');
            if (parts.Length < 2) return string.Empty;

            return parts[1];
        }


        private string GetOrgName(Guid orgId)
        {
            var org = _orgDtos.FirstOrDefault(_ => _.Id == orgId);
            return org == null ? string.Empty : $"{org.DisplayName}";
        }

        private string GetToolTipByBonusType(PayrollBonusType bonusType)
        {
            switch (bonusType)
            {
                case PayrollBonusType.SeedingTop1PostCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop1PostCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop1ReactionCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop1ReactionCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop1LikeCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop1LikeCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop1ShareCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop1ShareCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop1CommentCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop1CommentCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop1VIAPostCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop1VIAPostCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop2PostCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop2VIAPostCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop2ReactionCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop2ReactionCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop2LikeCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop2LikeCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop2ShareCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop2ShareCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop2CommentCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop2CommentCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop2VIAPostCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop2VIAPostCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop3PostCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop3PostCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop3ReactionCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop3ReactionCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop3LikeCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop3LikeCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop3ShareCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop3ShareCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop3CommentCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop3CommentCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTop3VIAPostCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTop3VIAPostCount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingTopLevel1GroupReactionCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTopGroupReactionCount_Level1.ToCommaStyle();
                case PayrollBonusType.SeedingTopLevel2GroupReactionCount:
                    return PayrollConfig.Seeding.Bonus.SeedingTopGroupReactionCount_Level2.ToCommaStyle();
                case PayrollBonusType.PayrollPerformance:
                    return PayrollConfig.Seeding.Bonus.WavePerformance.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.WavePerformance:
                    return PayrollConfig.Seeding.Bonus.WavePerformance.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingKPI_VIA1:
                    return PayrollConfig.Seeding.Bonus.SeedingKPI_VIA1_Amount.ToCommaStyle() + " / " + @L["Posts"];
                case PayrollBonusType.SeedingKPI_VIA2:
                    return PayrollConfig.Seeding.Bonus.SeedingKPI_VIA1_Amount.ToCommaStyle() + " / " + @L["Posts"];
                default:
                    return bonusType != null ? @L[bonusType.ToString()] : string.Empty;
            }
        }

        private string GetTooltipWaveType(WaveType waveType)
        {
            switch (waveType)
            {
                case WaveType.AffiliateSingleExclusive:
                    return PayrollConfig.Affiliate.Wave.AffiliateSingleExclusive.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.AffiliateSingleCopy:
                    return PayrollConfig.Affiliate.Wave.AffiliateSingleCopy.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.AffiliateSingleRemake:
                    return PayrollConfig.Affiliate.Wave.AffiliateSingleRemake.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.AffiliateMultipleExclusive:
                    return PayrollConfig.Affiliate.Wave.AffiliateMultipleExclusive.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.AffiliateMultipleCopy:
                    return PayrollConfig.Affiliate.Wave.AffiliateMultipleCopy.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.AffiliateMultipleRemake:
                    return PayrollConfig.Affiliate.Wave.AffiliateMultipleCopy.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.AffiliateSingleEditor:
                    return PayrollConfig.Affiliate.WaveEditor.AffiliateSingle.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.AffiliateMultipleEditor:
                    return PayrollConfig.Affiliate.WaveEditor.AffiliateMultiple.ToCommaStyle() + " / " + @L["Posts"];
                // case WaveType.SeedingExclusive:
                //     return PayrollConfig.Seeding.Wave.SeedingExclusive.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.SeedingCopy:
                    return PayrollConfig.Seeding.Wave.SeedingCopy.ToCommaStyle() + " / " + @L["Posts"];
                case WaveType.SeedingVIA:
                    return PayrollConfig.Seeding.Wave.SeedingVIA.ToCommaStyle() + " / " + @L["Posts"];
                // case WaveType.SeedingRemake:
                //     return PayrollConfig.Seeding.Wave.SeedingRemake.ToCommaStyle() + " / " + @L["Posts"];
                // case WaveType.SeedingKPI1:
                //     return PayrollConfig.Seeding.Bonus.SeedingKPI_VIA1_Amount.ToCommaStyle()+ " / " + @L["Posts"];
                // case WaveType.SeedingKPI2:
                //     return PayrollConfig.Seeding.Bonus.SeedingKPI_VIA2_Amount.ToCommaStyle()+ " / " + @L["Posts"];
                case WaveType.Mod:
                    return L["UserPayrollDetails.Payslip.GroupModBonusHint"];
                default:
                    return waveType != null ? @L[waveType.ToString()] : string.Empty;
            }
        }
    }
}