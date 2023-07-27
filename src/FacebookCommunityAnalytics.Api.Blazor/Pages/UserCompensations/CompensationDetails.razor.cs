using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.UserCompensations
{
    public partial class CompensationDetails
    {
        [Parameter]
        public string PayrollId { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        private DateTime FromDate { get; set; }
        private DateTime ToDate { get; set; }

        private CompensationDetailDto PayrollDetail { get; set; }
        private List<CompensationAffiliateDto> UserCompensationAffiliates { get; set; }
        private Modal ShortlinkModal { get; set; }
        private string AffiliateModalUsername = string.Empty;

        private List<bool> AccordionTeams = new List<bool>();
        protected string TextAlignLeft = "text-align: left";
        protected string TextAlignRight = "text-align: right";

        public CompensationDetails()
        {
            PayrollDetail = new CompensationDetailDto();
            UserCompensationAffiliates = new List<CompensationAffiliateDto>();
            ShortlinkModal = new Modal();
        }
        
        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            PayrollId ??= string.Empty;

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
            PayrollDetail = await UserCompensationAppService.GetCompensationDetailAsync(PayrollId.ToGuidOrDefault());
            if (PayrollDetail != null)
            {
                FromDate = PayrollDetail.Payroll.FromDateTime.GetValueOrDefault();
                ToDate = PayrollDetail.Payroll.ToDateTime.GetValueOrDefault();
            }

            if (PayrollDetail!= null)
            {
                for (var i = 0; i < PayrollDetail.TeamCompensations.Count; i++)
                {
                    AccordionTeams.Add(false);
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

        private async Task OpenShortlinkModal(Guid userId, string username)
        {
            AffiliateModalUsername = username;
            UserCompensationAffiliates = await UserCompensationAppService.GetAffiliateConversions(FromDate, ToDate, userId);
            ShortlinkModal.Show();
        }
        
        private void CloseShortlinkModal()
        {
            ShortlinkModal.Show();
        }
    }

}
