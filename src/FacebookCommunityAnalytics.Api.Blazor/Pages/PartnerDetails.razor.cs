using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class PartnerDetails
    {
        [Parameter] public string PartnerId { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        private PartnerDto Partner = new PartnerDto();
        private GetContractsInput Filter { get; set; }

        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }

        private ContractStatus SelectedContractStatus { get; set; }
        private Comment NewComment { get; set; }

        private bool ShowContractList { get; set; }

        public PartnerDetails()
        {
            Filter = new GetContractsInput();
            NewComment = new Comment();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await InitData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dateRanges = await GetDateRangePicker();
                (StartDate, EndDate) = await GetDefaultMonthDate();
                await InitPage();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["PartnerDetail.Update"],
                async () =>
                {
                    await UpdatePartner();
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Partners.Edit
            );
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Partner"], "/partners"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:PartnerDetails"]));
            return ValueTask.CompletedTask;
        }

        private async Task InitData()
        {
            Partner = await PartnersAppService.GetAsync(PartnerId.ToGuidOrDefault());
            await GetPartnerContractAsync();
        }

        private async Task GetPartnerContractAsync()
        {
            // if (!GlobalConfiguration.PartnerConfiguration.IsPartnerTool && Partner != null)
            // {
            //     if (SelectedContractStatus == ContractStatus.FilterNoSelect)
            //     {
            //         Filter.ContractStatus = null;
            //     }
            //     else
            //     {
            //         Filter.ContractStatus = SelectedContractStatus;
            //     }
            //
            var filter = Filter.Clone();
            filter.ContractStatus = SelectedContractStatus == ContractStatus.FilterNoSelect ? null : SelectedContractStatus;
            (filter.SignedAtMin, filter.SignedAtMax) = GetDateTimeForApi(StartDate, EndDate);
            filter.PartnerIds = new List<Guid> {Partner.Id};
            
            Partner.Contracts = await ContractAppService.GetContractNavs(filter);
            // }
            
        }
        private async Task OnDate_Changed()
        {
            await GetPartnerContractAsync();
        }
        private async Task OnSelectedPartnerContract(ContractStatus value)
        {
            SelectedContractStatus = value;
            await GetPartnerContractAsync();
        }
        private async Task UpdatePartner()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await PartnersAppService.UpdateAsync(PartnerId.ToGuidOrDefault(), ObjectMapper.Map<PartnerDto, PartnerUpdateDto>(Partner));
                    await InitData();
                    NewComment = new Comment();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private async Task AddComment()
        {
            if (NewComment.Note.IsNotNullOrEmpty())
            {
                NewComment.UserName = CurrentUser.UserName;
                NewComment.CreatedAt = DateTime.UtcNow;
                Partner.Comments.Add(NewComment);
                await UpdatePartner();
            }
        }

        private async Task ConfirmComment(Comment input)
        {
            input.IsConfirmed = true;
            await UpdatePartner();
        }

        private async Task DeleteComment(Comment input)
        {
            Partner.Comments.Remove(input);
            await UpdatePartner();
        }

        private async Task ShowHideContractListAsync()
        {
            ShowContractList = !ShowContractList;
        }
    }
}