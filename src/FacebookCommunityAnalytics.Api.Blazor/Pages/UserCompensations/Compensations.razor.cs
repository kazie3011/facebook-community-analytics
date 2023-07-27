using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Localisation.Services;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.UserCompensations
{
    public partial class Compensations
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
        private Modal CreatePayrollCaculationModal { get; set; }
        private Modal CreatePayrollModal { get; set; }
        private Modal EditPayrollModal { get; set; }
        private GetPayrollsInput Filter { get; set; }
        private DataGridEntityActionsColumn<PayrollDto> EntityActionsColumn { get; set; }
        private IBrowserDateTime BrowserDateTime { get; set; }

        private bool ShowLoading { get; set; }
        private bool isHappyDay { get; set; }

        private int month;
        private int year = 2021;

        private IReadOnlyList<int> Months = new List<int>()
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12
        };

        private IReadOnlyList<int> Years = new List<int>();

        public Compensations()
        {
            NewPayroll = new PayrollCreateDto();
            EditingPayroll = new PayrollUpdateDto();
            Filter = new GetPayrollsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting,
                IsCompensation = true
            };
            Years = Enumerable.Range(year, (DateTime.Now.Year - year) + 1).ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            SetDefaultMonthAndYear();
        }

        private void SetDefaultMonthAndYear()
        {
            month = DateTime.UtcNow.Month;
            year = DateTime.UtcNow.Year;
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
            // Toolbar.AddButton(L["CalculatePayrollButton"], async () =>
            // {
            //     await PayrollsAppService.CalculateCompensation(month, year);
            //     await Message.Success(L["Message.PayrollCalculationSuccess"]);
            //     
            //     await GetPayrollsAsync();
            //
            // }, "fas fa-calculator", requiredPolicyName: ApiPermissions.Payrolls.Create);
            //
            // Toolbar.AddButton(L["CalculatePayrollHappyDayButton"], async () =>
            // {
            //     await PayrollsAppService.CalculateCompensation(month, year, true);
            //     await Message.Success(L["Message.PayrollCalculationSuccess"]);
            //     
            //     await GetPayrollsAsync();
            //
            // }, "fas fa-calculator", requiredPolicyName: ApiPermissions.Payrolls.Create);
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

        private void OpenCreatePayrollCaculationModal(bool isHappyDay)
        {
            this.isHappyDay = isHappyDay;
            SetDefaultMonthAndYear();
            CreatePayrollCaculationModal.Show();
        }

        private void CloseCreatePayrollModal()
        {
            CreatePayrollModal.Hide();
        }
        
        private void ClosePayrollCaculationModal()
        {
            CreatePayrollCaculationModal.Hide();
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

        private void ViewCompensationDetail(PayrollDto dto)
        {
            NavigationManager.NavigateTo($"/compensation-details/{dto.Id.ToString()}");
        }

        private async Task ChooseMainPayroll(Guid payrollId)
        {
            await PayrollsAppService.ConfirmPayroll(payrollId);
            await GetPayrollsAsync();
        }

        private async Task ExportExcelAsync(Guid payrollId)
        {
            var excelBytes = await UserCompensationAppService.ExportCompensation(payrollId);
            if (excelBytes == null)
            {
                await Message.Info(L["CompensationExport.NoData"]);
                return;
            }

            var payroll = await PayrollsAppService.GetAsync(payrollId);
            var fileName = $"Compensation_{payroll.FromDateTime:yyyy-MM-dd}";
            await JsRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(excelBytes));
        }

        private async Task CalculatePayroll()
        {
            await PayrollsAppService.CalculateCompensation(month, year, isHappyDay);
            await GetPayrollsAsync();
        }
    }
}