using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.Contract
{
    public partial class ContractDashboard
    {
        private string selectedTabName = "Contracts";
        private Transactions _transactions;
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        private CreateUpdateContractDto NewContract { get; set; }
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<ContractWithNavigationPropertiesDto> PageContractDtos { get; set; }
        private IReadOnlyList<ContractWithNavigationPropertiesDto> ContractDtos { get; set; }
        private CreateUpdateContractDto EditingContract { get; set; }
        private ContractWithNavigationPropertiesDto DeletedContract { get; set; }
        private Guid EditingContractId { get; set; }
        private Modal EditContractModal { get; set; }
        private Modal ConfirmationModal { get; set; }
        private GetContractsInput Filter { get; set; }
        private int TotalCount { get; set; }
        private int PageSize { get; set; } = 25;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private bool CanCreateContract { get; set; }
        private bool CanEditContract { get; set; }
        private bool CanDeleteContract { get; set; }
        private bool DisablePartnerEditingModal { get; set; }
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }
        private IList<SalePersonModel> salePersonEditing = new List<SalePersonModel>();
        private decimal TotalContracts { get; set; }
        private decimal TotalDebtByTerm { get; set; }
        private decimal TotalContractsVAT { get; set; }
        private decimal TotalTransaction { get; set; }
        private decimal TotalVatTransaction { get; set; }
        private decimal PartialPaymentTotal { get; set; }
        private decimal RemainingPaymentTotal { get; set; }
        private decimal CostTotal { get; set; }
        private string ConfirmModalTitle { get; set; }
        private bool _firstLoadList { get; set; } = true;
        private Modal CreateContractModal { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> SalePersonsLookupDtos { get; set; } = new List<LookupDto<Guid?>>();
        private bool DisablePartnerNewModal { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<CampaignDto> Campaigns { get; set; } = new List<CampaignDto>();
        private TextEdit NewAmountField = new();
        private TextEdit NewCostField = new();
        private DateTime? createSignAt;
        private DateTime? createDueDate;
        private Modal CreateGroupCostsModal { get; set; }
        private List<GroupCostDto> ActiveGroupCosts { get; set; } = new();
        private List<GroupCostDto> GroupCosts { get; set; }
        private List<GroupCostDto> EditGroupCosts { get; set; }
        private List<GroupCostDto> ChangedGroupCosts { get; set; }
        private const decimal MAX_COST_AMOUNT = 100000000000;
        private IEnumerable<Guid> NewContractGroupCostIds { get; set; }
        private DateTime? editSignAt;
        private DateTime? editDueDate;
        private Guid? selectedSalePerson;
        private ContractStatus selectedContractStatus;
        private ContractPaymentStatus selectedContractPaymentStatus;
        private TextEdit EditAmountField = new();
        private TextEdit EditCostField = new();
        private IEnumerable<Guid> EditContractGroupCostIds { get; set; }

        //Transaction
        private       Modal                                                          ContractNavModal      { get; set; }
        private       Dictionary<string, DateRange>                                  TransactionDateRanges { get; set; }
        private       DateTimeOffset?                                                TransactionStartDate  { get; set; }
        private       DateTimeOffset?                                                TransactionEndDate    { get; set; }
        private       GetContractTransactionInput                                    TransactionFilter     { get; set; }
        private       decimal                                                        TotalTransactionVATAmount;
        private       decimal                                                        TotalTransactionAmount;
        private       int                                                            TransactionCount;
        private       List<ContractTransactionWithNavigationPropertiesDto>           TransactionItemsWithNav { get; set; }
        private const int                                                            PAGE_DEFAULT_SIZE = 30;
        private       int                                                            TransactionTotalCount   { get; set; }
        private       Modal                                                          EditTransactionModal    { get; set; }
        private       CreateUpdateContractTransactionDto                             EditContractTransaction { get; set; }
        private       DateTime?                                                      editPaymentDueDate;
        private       bool                                                           _isVATReadonly    { get; set; }
        private       Guid                                                           EditTransactionId { get; set; }
        private       PagedResultDto<ContractTransactionWithNavigationPropertiesDto> PagedResultDto    { get; set; }
        private       ContractTransactionWithNavigationPropertiesDto                 ContractNav       { get; set; }
        private       ContractDto                                                    _contractDto      { get; set; }
        private       IEnumerable<Guid>                                              ContractGroupCostIds;


        public ContractDashboard()
        {
            NewContract = new CreateUpdateContractDto()
            {
                ContractType = ContractType.Service,
                VATPercent = (decimal) VAT.Percent8
            };
            Filter = new GetContractsInput();
            DeletedContract = new ContractWithNavigationPropertiesDto();
            PageContractDtos = new List<ContractWithNavigationPropertiesDto>();
            ContractDtos = new List<ContractWithNavigationPropertiesDto>();
            NewContract = new CreateUpdateContractDto()
            {
                ContractType = ContractType.Service,
                VATPercent = (decimal) VAT.Percent8
            };
            EditingContract = new CreateUpdateContractDto();
            GroupCosts = new();
            ChangedGroupCosts = new();
            EditGroupCosts = new();
            _contractDto = new ContractDto();
            TransactionFilter = new GetContractTransactionInput();
            TransactionItemsWithNav = new List<ContractTransactionWithNavigationPropertiesDto>();
            ContractNav = new ContractTransactionWithNavigationPropertiesDto();
            ContractNav.Contract.ContractType = ContractType.Service;
            ContractNav.Contract.VATPercent = (decimal) VAT.Percent8;
            EditContractTransaction = new CreateUpdateContractTransactionDto();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetCampaigns();
            await GetNullablePartnerLookupAsync("");
            await GetSalePersonsLookupAsync();
            await GetGroupCosts();
        }

        private async Task InitFilter()
        {
            var salePersonId = NavigationManager.GetQueryParm("SalePersonId");
            if (salePersonId.IsNotNullOrEmpty())
            {
                Filter.SalePersonId = salePersonId.ToNullableGuid();
            }

            var contractStatus = NavigationManager.GetQueryParm("ContractStatus");
            if (contractStatus.IsNotNullOrEmpty())
            {
                Filter.ContractStatus = Convert.ToInt32(contractStatus).ToEnum<ContractStatus>();
            }

            var contractPaymentStatus = NavigationManager.GetQueryParm("ContractPaymentStatus");
            if (contractPaymentStatus.IsNotNullOrEmpty())
            {
                Filter.ContractPaymentStatus = Convert.ToInt32(contractPaymentStatus).ToEnum<ContractPaymentStatus>();
            }

            var minDate = NavigationManager.GetQueryParm("SignedAtMin");
            if (minDate.IsNotNullOrEmpty())
            {
                if (DateTime.TryParse(minDate, out var dateTime))
                {
                    StartDate = await ConvertUniversalToBrowserDateTime(dateTime);
                }
            }

            var maxDate = NavigationManager.GetQueryParm("SignedAtMax");
            if (maxDate.IsNotNullOrEmpty())
            {
                if (DateTime.TryParse(maxDate, out var dateTime))
                {
                    EndDate = await ConvertUniversalToBrowserDateTime(dateTime);
                }
            }

            var sorting = NavigationManager.GetQueryParm("Sorting");
            if (sorting.IsNotNullOrEmpty())
            {
                CurrentSorting = sorting;
            }

            var maxResultCount = NavigationManager.GetQueryParm("MaxResultCount");
            if (maxResultCount.IsNotNullOrEmpty())
            {
                PageSize = maxResultCount.ToIntODefault();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                (StartDate, EndDate) = await GetDefaultMonthDate();
                _dateRanges = await GetDateRangePicker();

                TransactionDateRanges = await GetDateRangePicker();
                (TransactionStartDate, TransactionEndDate) = await GetDefaultMonthDate();

                await InitFilter();
                await DoSearch();
                await DoSearchTransaction();
                await InitPage($"GDL - {L["Campaigns.PageTitle"].Value}");
            }
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateContract = await AuthorizationService.IsGrantedAsync(ApiPermissions.Contracts.Create);
            CanEditContract = await AuthorizationService.IsGrantedAsync(ApiPermissions.Contracts.Edit);
            CanDeleteContract = await AuthorizationService.IsGrantedAsync(ApiPermissions.Contracts.Delete);
        }

        private async Task OnSelectedTabChanged(string name)
        {
            selectedTabName = name;
            await InvokeAsync(StateHasChanged);
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Contracts"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["Contract.NewContract.Button"],
                async () =>
                {
                    await OpenCreateContractModal();
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Contracts.Create
            );
            Toolbar.AddButton
            (
                L["Contract.ConfigGroup.Button"],
                () =>
                {
                    OpenCreateGroupCostModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Contracts.Create
            );
            return ValueTask.CompletedTask;
        }

        private async Task DoSearch()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            // text filter works separately
            if (Filter.FilterText.IsNotNullOrWhiteSpace())
            {
                selectedSalePerson = Guid.Empty;
                selectedContractStatus = ContractStatus.FilterNoSelect;
                selectedContractPaymentStatus = ContractPaymentStatus.FilterNoSelect;
                Filter.SignedAtMin = null;
                Filter.SignedAtMax = null;
                (StartDate, EndDate) = await GetDefaultMonthDate();
            }
            else
            {
                (Filter.SignedAtMin, Filter.SignedAtMax) = GetDateTimeForApi(StartDate, EndDate);
            }

            if (!IsManagerRole() && !IsPartnerRole() && !IsSaleAdminRole())
            {
                selectedSalePerson = CurrentUser.Id.GetValueOrDefault();
            }

            var filter = Filter.Clone();

            // null value for select component UI
            filter.SalePersonId = selectedSalePerson == Guid.Empty ? null : selectedSalePerson;
            filter.ContractStatus = selectedContractStatus == ContractStatus.FilterNoSelect ? null : selectedContractStatus;
            filter.ContractPaymentStatus = selectedContractPaymentStatus == ContractPaymentStatus.FilterNoSelect ? null : selectedContractPaymentStatus;
            var contractBaseUrl = NavigationManager.ToAbsoluteUri("contracts");
            NavigationManager.NavigateTo($"{contractBaseUrl}?{filter.GetQueryString()}", false);
            ContractDtos = await ContractAppService.GetContractNavs(filter);
            if (_firstLoadList)
            {
                _firstLoadList = false;
                ContractDtos = ContractDtos.OrderByDescending(x => x.Contract.SignedAt).ToList();
            }

            GetSumContracts();
            await GetSumTransaction();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDate_Changed()
        {
            Filter.FilterText = string.Empty;
            await DoSearch();
        }

        private async Task OnDate_ChangedTransaction()
        {
            TransactionFilter.FilterText = string.Empty;
            await DoSearchTransaction();
        }

        private async Task OnSalePerson_Changed(Guid? value)
        {
            Filter.FilterText = string.Empty;
            selectedSalePerson = value;
            await DoSearch();
        }

        private async Task OnTransactionSalePerson_Changed(Guid? value)
        {
            TransactionFilter.FilterText = string.Empty;
            TransactionFilter.SalePersonId = value;
            await DoSearchTransaction();
        }

        private async Task OnContractStatus_Changed(ContractStatus value)
        {
            Filter.FilterText = string.Empty;
            selectedContractStatus = value;
            await DoSearch();
        }

        private async Task OnPaymentStatus_Changed(ContractPaymentStatus value)
        {
            Filter.FilterText = string.Empty;
            selectedContractPaymentStatus = value;
            await DoSearch();
        }


        private async Task OpenCreateContractModal()
        {
            NewContract = new CreateUpdateContractDto()
            {
                ContractType = ContractType.Service,
                VATPercent = (decimal) VAT.Percent8
            };
            await CreateContractSignAtChange(DateTime.UtcNow);
            DisablePartnerNewModal = false;
            await GetGroupCosts();
            NewContract.IsManualCost = GroupCosts.IsNullOrEmpty();
            CreateContractModal.Show();
        }

        private async Task CreateContractSignAtChange(DateTime? value)
        {
            if (value != null)
            {
                createSignAt = value;
                NewContract.SignedAt = await ConvertBrowserToUniversalDateTime(createSignAt.Value);
            }
        }

        private async Task CreateContractDueDateChange(DateTime? value)
        {
            if (value != null)
            {
                createDueDate = value;
                NewContract.PaymentDueDate = await ConvertBrowserToUniversalDateTime(createDueDate.Value);
            }
        }

        private void OnSelectCampaignNewModal(Guid? value)
        {
            NewContract.CampaignId = value;
            NewContract.PartnerId = Campaigns.FirstOrDefault(_ => _.Id == value)?.PartnerId;
            DisablePartnerNewModal = value != null;
        }

        private void CloseCreateContractModal()
        {
            CreateContractModal.Hide();
        }

        private async Task CreateContract()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await ContractAppService.CreateAsync(NewContract);
                    await DoSearch();
                    CreateContractModal.Hide();
                },
                L,
                true
            );
        }

        private async Task OpenEditContractModal(ContractWithNavigationPropertiesDto input)
        {
            var contract = input.Contract;
            EditingContract = new CreateUpdateContractDto();
            EditingContractId = contract.Id;
            EditingContract = ObjectMapper.Map<ContractDto, CreateUpdateContractDto>(contract);
            salePersonEditing = SalePersonsLookupDtos.Select
                (
                    _ => new SalePersonModel()
                    {
                        Id = _.Id,
                        DisplayName = _.DisplayName
                    }
                )
                .ToList();
            if (input.SalePerson is not null && !(salePersonEditing.Select(_ => _.Id).Contains(input.SalePerson.Id)))
                salePersonEditing.Add
                (
                    new SalePersonModel()
                    {
                        Id = input.SalePerson.Id,
                        DisplayName = $"{input.SalePerson.UserName} ({input.SalePerson.Email})",
                        IsHidden = true
                    }
                );
            editSignAt = EditingContract.SignedAt != null ? await ConvertUniversalToBrowserDateTime(EditingContract.SignedAt.Value) : null;
            editDueDate = EditingContract.PaymentDueDate != null ? await ConvertUniversalToBrowserDateTime(EditingContract.PaymentDueDate.Value) : null;
            await GetGroupCosts();
            // Get List GroupCost with contract group cost info and list contract group cost with db group cost
            // Example: GroupCosts = {(A, 10), (B, 15), (C, 20), (D, 29), (E, 14)} // EditingContract.GroupCostInfos = {(B, 13), (C, 25), (E, 14)}
            // =>       EditGroupCosts = {(A, 10), (B, 13), (C, 25), (D, 29), (E, 14)} // ChangedGroupCosts = {(B, 15), (C, 20)}
            EditGroupCosts = ActiveGroupCosts.Clone();
            foreach (var item in EditGroupCosts)
            {
                var contractGroupCostInfo = EditingContract.GroupCostInfos.FirstOrDefault(_ => _.Id == item.Id);
                if (contractGroupCostInfo is null) continue;
                ChangedGroupCosts.Add(item);
                item.Cost = contractGroupCostInfo.Cost;
            }

            foreach (var item in EditingContract.GroupCostInfos)
            {
                if (EditGroupCosts.Select(_ => _.Id).Contains(item.Id)) continue;
                var groupCost = GroupCosts.FirstOrDefault(_ => _.Id == item.Id);
                if (groupCost is null) continue;
                groupCost.Cost = item.Cost;
                EditGroupCosts.Add(groupCost);
            }

            EditContractGroupCostIds = EditingContract.GroupCostInfos.Select(_ => _.Id).ToList();
            DisablePartnerEditingModal = true;
            EditContractModal.Show();
            OnSelectCampaignEditingModal(EditingContract.CampaignId);
        }

        private void CloseEditContractModal()
        {
            EditContractModal.Hide();
        }

        private async Task UpdateContract()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await ContractAppService.UpdateAsync(EditingContractId, EditingContract);
                    await DoSearch();
                    EditContractModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private async Task DeleteContract(ContractDto input)
        {
            var confirmResult = await _uiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                var success = await Invoke
                (
                    async () =>
                    {
                        await ContractAppService.DeleteAsync(input.Id);
                        await DoSearch();
                        ConfirmationModal.Hide();
                    },
                    L,
                    true,
                    BlazorComponentBaseActionType.Delete
                );
            }
        }

        private async Task GetNullablePartnerLookupAsync(string newValue)
        {
            PartnersNullable = (await ContractAppService.GetPartnerLookup(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetCampaigns()
        {
            Campaigns = await ContractAppService.GetCampaigns();
        }

        private void OnSelectCampaignEditingModal(Guid? value)
        {
            EditingContract.CampaignId = value;
            if (EditingContract.CampaignId.IsNotNullOrEmpty())
            {
                EditingContract.PartnerId = Campaigns.FirstOrDefault(_ => _.Id == value)?.PartnerId;
            }

            DisablePartnerEditingModal = value != null;
        }

        private void ViewDetailsAsync(ContractDto input)
        {
            NavigationManager.NavigateTo($"/contracts/contract-details/{input.Id.ToString()}");
        }

        private async Task GetSalePersonsLookupAsync()
        {
            SalePersonsLookupDtos = await ContractAppService.GetAppUserLookupAsync(new GetMembersApiRequest {TeamName = TeamMemberConsts.Sale});
        }

        private string SetColorContractStatus(ContractStatus item)
        {
            return item switch
            {
                ContractStatus.ContractSigned => "text-success", ContractStatus.ContractSent => "text-info", _ => "text-black-50"
            };
        }

        private string SetColorContractPaymentStatus(ContractPaymentStatus item)
        {
            return item switch
            {
                ContractPaymentStatus.Paid => "text-success", ContractPaymentStatus.PartiallyPaid => "text-warning", ContractPaymentStatus.Unpaid => "text-danger", _ => "text-black-50"
            };
        }

        public void OnContractVATAmountChange(string amount)
        {
            NewContract.TotalValue = amount.ToDecimalOrDefault();
        }

        public void OnEditContractVATAmountChange(string amount)
        {
            EditingContract.TotalValue = amount.ToDecimalOrDefault();
        }

        private void GetSumContracts()
        {
            TotalContracts = ContractDtos.Sum(_ => _.Contract.TotalValue.ToNonVATAmount(_.Contract.VATPercent));
            TotalContractsVAT = ContractDtos.Sum(_ => _.Contract.TotalValue);
            PartialPaymentTotal = ContractDtos.Sum(_ => _.Contract.PartialPaymentValue);
            RemainingPaymentTotal = ContractDtos.Sum(_ => _.Contract.RemainingPaymentValue);
            CostTotal = ContractDtos.Sum(_ => _.Contract.Cost);
            TotalCount = ContractDtos.Count;
            TotalDebtByTerm = TotalContracts - ContractDtos.Sum(x => x.ContractTransactions.Where(y => y.CreationTime <= x.Contract.PaymentDueDate).Sum(z => z.PartialPaymentValue));
        }

        private async Task GetSumTransaction()
        {
            if (Filter.FilterText.IsNotNullOrEmpty())
            {
                TotalVatTransaction = ContractDtos.Sum(x => x.ContractTransactions.Sum(c => c.PartialPaymentValue));
                TotalTransaction = ContractDtos.Sum(x => x.ContractTransactions.Sum(c => c.PartialPaymentValue.ToNonVATAmount(c.VATPercent)));
            }
            else
            {
                var transactionFilter = new GetContractTransactionInput();
                (transactionFilter.PaymentDueDateMin, transactionFilter.PaymentDueDateMax) = GetDateTimeForApi(StartDate, EndDate);
                transactionFilter.SalePersonId = selectedSalePerson == Guid.Empty ? null : selectedSalePerson;
                var transactions = await ContractAppService.GetTransactions(transactionFilter);
                TotalVatTransaction = transactions.Sum(c => c.PartialPaymentValue);
                TotalTransaction = transactions.Sum(c => c.PartialPaymentValue.ToNonVATAmount(c.VATPercent));
            }
        }

        private void OpenConfirmationModal(ContractWithNavigationPropertiesDto contractDto)
        {
            DeletedContract = contractDto;
            ConfirmModalTitle = $"{contractDto.Contract.ContractCode}({contractDto.SalePerson.UserName})";
            ConfirmationModal.Show();
        }

        private void CloseConfirmationModal()
        {
            ConfirmationModal.Hide();
        }

        private async Task EditContractSignAtChange(DateTime? value)
        {
            if (value != null)
            {
                editSignAt = value;
                EditingContract.SignedAt = await ConvertBrowserToUniversalDateTime(editSignAt.Value);
            }
        }

        private async Task EditContractDueDateChange(DateTime? value)
        {
            if (value != null)
            {
                editDueDate = value;
                EditingContract.PaymentDueDate = await ConvertBrowserToUniversalDateTime(editDueDate.Value);
            }
        }

        private void OnNewContractTypeChanged(ContractType input)
        {
            NewContract.ContractType = input;
            NewContract.VATPercent = input switch
            {
                ContractType.Retail => (decimal) VAT.Percent0, ContractType.Service => (decimal) VAT.Percent8, _ => NewContract.VATPercent
            };
            OnContractVATAmountChange(NewAmountField.Text);
        }

        private string GetNonVATAmount(ContractWithNavigationPropertiesDto input)
        {
            if (input.Contract.ContractType == ContractType.Master)
            {
                var value = input.ContractTransactions.Sum(transaction => transaction.PartialPaymentValue.ToNonVATAmount(transaction.VATPercent));
                return value.ToVND();
            }

            return input.Contract.TotalValue.ToNonVATAmount(input.Contract.VATPercent).ToVND();
        }

        private void OnNewContractCostAmountChange(string amount)
        {
            NewContract.Cost = amount.ToDecimalOrDefault();
        }

        private void OnEditContractCostAmountChange(string amount)
        {
            EditingContract.Cost = amount.ToDecimalOrDefault();
        }

        private decimal GetTotalTransactionsAmount(IEnumerable<ContractTransactionDto> transactions)
        {
            if (!Filter.SignedAtMin.HasValue || !Filter.SignedAtMax.HasValue) return transactions.Sum(x => x.PartialPaymentValue);
            var filteredTransactions = transactions.Where(x => x.CreatedAt >= Filter.SignedAtMin && x.CreatedAt <= Filter.SignedAtMax);
            return filteredTransactions.Sum(x => x.PartialPaymentValue);
        }

        private string GetSaleName(AppUserDto user)
        {
            if (user is null) return string.Empty;
            var first = user.Surname.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            var last = user.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            return $"{first} {last}";
        }

        private void OnSelectedEditingGroupCost(object value)
        {
            var groupCostIds = ((IEnumerable<Guid>) value).ToList();

            // Todoo: Multiple select (Not test yet)
            // var groupCostIds = new List<Guid>();
            // // Example: groupCostIds = {B} EditGroupCosts = {(A, 10), (B, 13), (C, 25) // ChangedGroupCosts = {(B, 15), (C, 20)
            // // =>       changedGroupCost = (C, 20) // ChangedGroupCosts = {(B,15)} // EditGroupCosts = {(A, 10), (B, 13), (C, 20)
            var changedGroupCost = ChangedGroupCosts.FirstOrDefault(_ => !_.Id.IsIn(groupCostIds));
            if (changedGroupCost is not null)
            {
                EditGroupCosts = EditGroupCosts.Select
                    (
                        _ =>
                        {
                            if (_.Id == changedGroupCost.Id) _.Cost = changedGroupCost.Cost;
                            return _;
                        }
                    )
                    .ToList();
                ChangedGroupCosts.Remove(changedGroupCost);
            }

            var groupCosts = EditGroupCosts.Where(x => x.Id.IsIn(groupCostIds)).ToList();
            EditContractGroupCostIds = groupCostIds;
            EditingContract.GroupCostInfos = ObjectMapper.Map<List<GroupCostDto>, List<GroupCostInfoDto>>(groupCosts);
            EditingContract.Cost = groupCosts.Sum(item => item.Cost);
        }

        private void OnSelectedNewGroupCost(object value)
        {
            var groupCostIds = ((IEnumerable<Guid>) value).ToList();
            var groupCosts = ActiveGroupCosts.Where(x => x.Id.IsIn(groupCostIds)).ToList();
            NewContractGroupCostIds = groupCostIds;
            NewContract.GroupCostInfos = ObjectMapper.Map<List<GroupCostDto>, List<GroupCostInfoDto>>(groupCosts);
            NewContract.Cost = groupCosts.Sum(item => item.Cost);
        }

        private void OpenCreateGroupCostModal()
        {
            if (ActiveGroupCosts.IsNullOrEmpty())
            {
                ActiveGroupCosts.Add(new GroupCostDto());
            }

            CreateGroupCostsModal.Show();
        }

        private async Task SaveGroupCostItems()
        {
            if (await CheckGroupNamesExistence())
            {
                return;
            }

            var createdGroupCosts = ActiveGroupCosts.Where(_ => _.GroupName.IsNotNullOrEmpty()).ToList();
            if (createdGroupCosts.IsNotNullOrEmpty())
            {
                await ContractAppService.SaveGroupCosts(new GroupCostApiRequest() {GroupCosts = createdGroupCosts});
            }

            await GetGroupCosts();
            CloseGroupCostConfigModal();
        }

        private void CreateGroupCost()
        {
            ActiveGroupCosts.Add(new GroupCostDto());
        }

        private void CloseGroupCostConfigModal()
        {
            CreateGroupCostsModal.Hide();
        }

        private async Task GetGroupCosts()
        {
            GroupCosts = await ContractAppService.GetGroupCosts();
            ActiveGroupCosts = GroupCosts.Where(_ => !_.Disable).ToList();
        }

        private void RemoveGroupCost(GroupCostDto item)
        {
            ActiveGroupCosts.Remove(item);
        }

        private async Task<bool> CheckGroupNamesExistence()
        {
            var message = string.Empty;
            foreach (var items in ActiveGroupCosts.Where(x => x.GroupName.IsNotNullOrWhiteSpace()).GroupBy(x => x.GroupName.Trim()).Where(x => x.Count() > 1))
            {
                foreach (var item in items)
                {
                    message += $"{item.GroupName} ";
                    break;
                }
            }

            if (message.IsNotNullOrEmpty())
            {
                await _uiMessageService.Info($"{L["Contract.GroupCost.GroupNameExistence"]}: {message}");
                return true;
            }

            return false;
        }

        private void OnChangedNewManualCost(bool value)
        {
            NewContract.IsManualCost = value;
            if (value)
            {
                NewContractGroupCostIds = null;
                NewContract.GroupCostInfos = new List<GroupCostInfoDto>();
                NewContract.Cost = 0;
            }

            NewContract.Cost = 0;
        }

        private void OnChangedEditingManualCost(bool value)
        {
            EditingContract.IsManualCost = value;
            if (value)
            {
                EditContractGroupCostIds = null;
                EditingContract.GroupCostInfos = new List<GroupCostInfoDto>();
                EditingContract.Cost = 0;
            }

            EditingContract.Cost = 0;
        }

        private async Task DoSearchTransaction()
        {
            if (TransactionFilter.FilterText.IsNotNullOrWhiteSpace())
            {
                TransactionFilter.SalePersonId = Guid.Empty;
                TransactionFilter.PaymentDueDateMin = null;
                TransactionFilter.PaymentDueDateMax = null;
                (TransactionStartDate, TransactionEndDate) = await GetDefaultMonthDate();
            }
            else
            {
                (TransactionFilter.PaymentDueDateMin, TransactionFilter.PaymentDueDateMax) = GetDateTimeForApi(TransactionStartDate, TransactionEndDate);
            }

            await GetTransactions();
            GetTransactionStatistic();
            await InvokeAsync(StateHasChanged);
        }

        private void GetTransactionStatistic()
        {
            var contractTransactionDto = TransactionItemsWithNav.Select(x => x.ContractTransaction).ToList();
            TotalTransactionVATAmount = contractTransactionDto.Sum(x => x.PartialPaymentValue);
            TotalTransactionAmount = contractTransactionDto.Sum(x => x.PartialPaymentValue.ToNonVATAmount(x.VATPercent));
            TransactionCount = contractTransactionDto.Count;
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

        private Task OpenContractNavModal(Guid contractId)
        {
            ContractNav = TransactionItemsWithNav.FirstOrDefault(x => x.Contract.Id == contractId);
            if (ContractNav != null)
            {
                ContractGroupCostIds = ContractNav.Contract.GroupCostInfos.Select(x => x.Id);
            }

            ContractNavModal.Show();

            return Task.CompletedTask;
        }

        private void CloseEditTransactionModal()
        {
            EditContractTransaction = new CreateUpdateContractTransactionDto();
            EditTransactionModal.Hide();
        }

        private Task OnEditTransactionValueChanged(string transactionValue)
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

        private async Task GetTransactions()
        {
            PagedResultDto = await ContractAppService.GetTransactionsWithNav(TransactionFilter);
            TransactionItemsWithNav = PagedResultDto.Items.ToList();
            TransactionTotalCount = (int) PagedResultDto.TotalCount;
        }

        private void CloseContractNavModal()
        {
            ContractNavModal.Hide();
        }

        private async Task ExportTransactionAsync()
        {
            var rows = TransactionItemsWithNav.Select
                                               (
                                                item => new TransactionExportRow()
                                                {
                                                    ContractCode           = item.Contract.ContractCode,
                                                    SalePersonName         = item.SalePerson?.Name,
                                                    Description            = item.ContractTransaction.Description,
                                                    PartialPaymentVATValue = item.ContractTransaction.PartialPaymentValue.ToVND(),
                                                    PartialPaymentValue    = item.ContractTransaction.PartialPaymentValue.ToNonVATAmount(item.ContractTransaction.VATPercent).ToVND(),
                                                    VAT                    = $"{item.ContractTransaction.VATPercent}%",
                                                    Cost                   = item.ContractTransaction.Cost != 0 ? item.ContractTransaction.Cost.ToVND() : "",
                                                    PaymentDueDate         = BrowserDateTime.ConvertToBrowserTime(item.ContractTransaction.PaymentDueDate.GetValueOrDefault(), GlobalConsts.DateFormat),
                                                    CreatedAt              = BrowserDateTime.ConvertToBrowserTime(item.ContractTransaction.CreatedAt, GlobalConsts.DateFormat)
                                                }
                                               )
                                              .ToList();
            var stats = new TransactionStatsRow()
            {
                TotalTransactionVATAmount = TotalTransactionVATAmount.ToVND(),
                TotalTransactionAmount = TotalTransactionAmount.ToVND()
            };
            var excelBytes = ExportHelper.GenerateTransactionExcelBytes(L,rows,stats,"Transactions");
            var fileName = $"transactions_{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}.xlsx";
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
        }
    
}

    public record SalePersonModel()
    {
        public Guid? Id;
        public string DisplayName;
        public bool IsHidden;
    }
    
    public class TransactionExportRow
    {
        public string ContractCode { get; set; }
        public string SalePersonName { get; set; }
        public string Description { get; set; }
        public string PartialPaymentVATValue {get; set;}
        public string PartialPaymentValue {get; set;}
        public string VAT {get; set;}
        public string Cost { get; set; }
        public string PaymentDueDate { get; set; }
        public string CreatedAt { get; set; }
        
    }

    public class TransactionStatsRow
    {
        public string TotalTransactionVATAmount { get; set; }
        public string TotalTransactionAmount { get; set; }

    }

    
    
}