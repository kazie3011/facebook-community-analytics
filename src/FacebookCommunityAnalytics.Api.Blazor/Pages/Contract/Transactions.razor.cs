using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Messages;




namespace FacebookCommunityAnalytics.Api.Blazor.Pages.Contract
{
    public partial class Transactions
    {
        private PagedResultDto<ContractTransactionWithNavigationPropertiesDto>  PagedResultDto { get; set; }
        private List<ContractTransactionWithNavigationPropertiesDto> TransactionItemsWithNav { get; set; }
        private GetContractTransactionInput Filter { get; set; }

        private const int PAGE_DEFAULT_SIZE = 30;
        private int CurrentPage = 1;
        private int TotalCount = 0;
        private string _createTransactionModalTitle { get; set; }
        private string _editTransactionModelTitle { get; set; }
        private CreateUpdateContractTransactionDto EditContractTransaction { get; set; }
        
        private Modal NewTransactionModal { get; set; }
        private Modal EditTransactionModal { get; set; }
        private bool _isVATReadonly { get; set; }
        private DateTime? createPaymentDueDate;
        private DateTime? editPaymentDueDate;
        private bool _isValidUploadTransactionImage;
        private Guid EditTransactionId { get; set; }
        private ContractDto _contractDto { get; set; }
        private bool CanEditContract { get; set; } = true;
        private bool CanDeleteContract { get; set; } = true;
        
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }
        
        private IReadOnlyList<LookupDto<Guid?>> SalePersonsLookupDtos { get; set; } = new List<LookupDto<Guid?>>();
        private Guid? selectedSalePerson;
        private ContractStatus selectedContractStatus;
        private ContractPaymentStatus selectedContractPaymentStatus;
        
        private decimal TotalTransactionVATAmount;
        private decimal TotalTransactionAmount;
        private int TransactionCount;
        
        public Transactions()
        {
            _contractDto = new ContractDto();
            EditTransactionModal = new Modal();
            EditContractTransaction = new CreateUpdateContractTransactionDto();
            _isValidUploadTransactionImage = true;
            TransactionItemsWithNav = new List<ContractTransactionWithNavigationPropertiesDto>();
            Filter = new GetContractTransactionInput();
            
        }

        protected override async Task OnInitializedAsync()
        {          
            BrowserDateTime = await GetBrowserDateTime();
            await GetSalePersonsLookupAsync();      

        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                (StartDate, EndDate) = await GetDefaultMonthDate();
                _dateRanges = await GetDateRangePicker();
                await DoSearch();
            }
        }

        private async Task DoSearch()
        {
            if (Filter.FilterText.IsNotNullOrWhiteSpace())
            {   
                Filter.SalePersonId = Guid.Empty;
                Filter.PaymentDueDateMin = null;
                Filter.PaymentDueDateMax = null;
                (StartDate, EndDate) = await GetDefaultMonthDate();
            }
            else
            {
                (Filter.PaymentDueDateMin, Filter.PaymentDueDateMax) = GetDateTimeForApi(StartDate, EndDate);

            }
            
            await GetTransactions();
            GetTransactionStatistic();
            await InvokeAsync(StateHasChanged);

        }

        private async Task GetTransactions()
        {
            PagedResultDto = await ContractAppService.GetTransactionsWithNav(Filter);
            TransactionItemsWithNav  = PagedResultDto.Items.ToList();
            TotalCount =(int) PagedResultDto.TotalCount;
        }

        private void GetTransactionStatistic()
        {
            var contractTransactionDto = TransactionItemsWithNav.Select(x => x.ContractTransaction).ToList();
            TotalTransactionVATAmount = contractTransactionDto.Sum(x => x.PartialPaymentValue);
            TotalTransactionAmount = contractTransactionDto.Sum(x => x.PartialPaymentValue.ToNonVATAmount(x.VATPercent));
            TransactionCount = contractTransactionDto.Count;
        }

        public Task OnEditTransactionValueChanged(string transactionValue)
        {
            EditContractTransaction.PartialPaymentValue = transactionValue.ToDecimalOrDefault();
            return Task.CompletedTask;
        }
  
        private async Task EditPaymentDueDateAtChange(DateTime? value)
        {
            if (value != null)
            {
                editPaymentDueDate = value;
                EditContractTransaction.PaymentDueDate = await ConvertBrowserToUniversalDateTime(editPaymentDueDate.Value);
            }
        }
        
        private void CloseEditTransactionModal()
        {
            EditContractTransaction = new CreateUpdateContractTransactionDto();
            EditTransactionModal.Hide();
        }
        private async Task DeleteTransaction(Guid transactionId)
        {
            var confirmResult = await _uiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                var success = await Invoke
                (
                    async () =>
                    {
                        await ContractAppService.DeleteTransaction(transactionId);
                        await GetTransactions();
                    },
                    L,
                    true,
                    BlazorComponentBaseActionType.Delete
                );
            }
        }
        private async Task UpdateTransaction()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await ContractAppService.UpdateTransaction(EditTransactionId, EditContractTransaction);
                    await GetTransactions();
                    EditTransactionModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }
        
        private async Task OpenEditTransactionModal(ContractTransactionDto input)
        {
            EditTransactionId = input.Id;
            EditContractTransaction = ObjectMapper.Map<ContractTransactionDto, CreateUpdateContractTransactionDto>(input);
            if (input.PaymentDueDate.HasValue)
            {
                editPaymentDueDate = await ConvertUniversalToBrowserDateTime(input.PaymentDueDate.Value);
            }
            if (_contractDto.ContractType != ContractType.Master) EditContractTransaction.VATPercent = _contractDto.VATPercent;
            EditTransactionModal.Show();
        }
        private async Task OnDate_Changed()
        {
            Filter.FilterText = string.Empty;
            await DoSearch();
        }
        private async Task OnSalePerson_Changed(Guid? value)
        { 
            Filter.FilterText = string.Empty;
           Filter.SalePersonId = value;
           await DoSearch();
        }
        
        private async Task GetSalePersonsLookupAsync()
        {
            SalePersonsLookupDtos = await ContractAppService.GetAppUserLookupAsync(new GetMembersApiRequest {TeamName = TeamMemberConsts.Sale});
        }
    }
}