using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Payrolls
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<PayrollDto> PayrollList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreatePayroll { get; set; }
        private bool CanEditPayroll { get; set; }
        private bool CanDeletePayroll { get; set; }
        private PayrollCreateDto NewPayroll { get; set; }
        private Validations NewPayrollValidations { get; set; }
        private PayrollUpdateDto EditingPayroll { get; set; }
        private Validations EditingPayrollValidations { get; set; }
        private Guid EditingPayrollId { get; set; }
        private Modal CreatePayrollModal { get; set; }
        private Modal EditPayrollModal { get; set; }
        private GetPayrollsInput Filter { get; set; }
        private DataGridEntityActionsColumn<PayrollDto> EntityActionsColumn { get; set; }

        public Payrolls()
        {
            NewPayroll = new PayrollCreateDto();
            EditingPayroll = new PayrollUpdateDto();
            Filter = new GetPayrollsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["PayrollScreen.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Payrolls"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["CalculatePayrollButton"],
                async () =>
                {
                    await PayrollsAppService.CalculatePayroll();
                    await Message.Success(L["Message.PleaseWaitCalculatePayroll"]);
                },
                "fas fa-calculator",
                requiredPolicyName: ApiPermissions.Payrolls.Create
            );

            Toolbar.AddButton
            (
                L["CalculatePayrollHappyDayButton"],
                async () =>
                {
                    await PayrollsAppService.CalculatePayroll(true);
                    await Message.Success(L["Message.PleaseWaitCalculatePayrollHappyDay"]);
                },
                "fas fa-calculator",
                requiredPolicyName: ApiPermissions.Payrolls.Create
            );
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePayroll = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Payrolls.Create);
            CanEditPayroll = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Payrolls.Edit);
            CanDeletePayroll = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Payrolls.Delete);
        }

        private async Task GetPayrollsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PayrollsAppService.GetListAsync(Filter);
            PayrollList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPayrollsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PayrollDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPayrollsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreatePayrollModal()
        {
            NewPayroll = new PayrollCreateDto
            {
                FromDateTime = DateTime.UtcNow,
                ToDateTime = DateTime.UtcNow
            };
            NewPayrollValidations.ClearAll();
            CreatePayrollModal.Show();
        }

        private void CloseCreatePayrollModal()
        {
            CreatePayrollModal.Hide();
        }

        private void OpenEditPayrollModal(PayrollDto input)
        {
            EditingPayrollId = input.Id;
            EditingPayroll = ObjectMapper.Map<PayrollDto, PayrollUpdateDto>(input);
            EditingPayrollValidations.ClearAll();
            EditPayrollModal.Show();
        }

        private async Task DeletePayrollAsync(PayrollDto input)
        {
            await PayrollsAppService.DeleteAsync(input.Id);
            await GetPayrollsAsync();
        }

        private async Task CreatePayrollAsync()
        {
            await PayrollsAppService.CreateAsync(NewPayroll);
            await GetPayrollsAsync();
            CreatePayrollModal.Hide();
        }

        private void CloseEditPayrollModal()
        {
            EditPayrollModal.Hide();
        }

        private async Task UpdatePayrollAsync()
        {
            await PayrollsAppService.UpdateAsync(EditingPayrollId, EditingPayroll);
            await GetPayrollsAsync();
            EditPayrollModal.Hide();
        }

        private void ViewPayrollDetail(PayrollDto dto)
        {
            NavigationManager.NavigateTo($"/payroll-details/{dto.Id.ToString()}");
        }

        private async Task ChooseMainPayroll(Guid payrollId)
        {
            await PayrollsAppService.ConfirmPayroll(payrollId);
            await GetPayrollsAsync();
        }

        private async Task SendEmailPayrollAsync(Guid payrollId)
        {
            await PayrollsAppService.SendEmail(payrollId);
            // await PayrollEmailDomainService.Send(true, payrollId.ToString());
        }
    }
}