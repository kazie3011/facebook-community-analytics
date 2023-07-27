using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class Contracts
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<ContractWithNavigationPropertiesDto> ContractDtos { get; set; }
        private CreateUpdateContractDto NewContract { get; set; }
        private CreateUpdateContractDto EditingContract { get; set; }
        
        private Guid EditingContractId { get; set; }
        
        private Modal CreateContractModal { get; set; }
        
        private Modal EditContractModal { get; set; }
        private GetContractsInput Filter { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<CampaignDto> Campaigns { get; set; } = new List<CampaignDto>();
        private DataGridEntityActionsColumn<ContractWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateContract { get; set; }
        private bool CanEditContract { get; set; }
        private bool CanDeleteContract { get; set; }
        private bool DisablePartnerNewModal { get; set; }
        private bool DisablePartnerEditingModal { get; set; }
        private int TextBerBer { get; set; }
        
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }

        public Contracts()
        {
            Filter = new GetContractsInput();
            ContractDtos = new List<ContractWithNavigationPropertiesDto>();
            NewContract = new CreateUpdateContractDto();
            EditingContract = new CreateUpdateContractDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetContracts();
            await GetCampaigns();
            await GetPartnerLookupAsync();
            BrowserDateTime = await GetBrowserDateTime();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                (StartDate, EndDate) = await GetDefaultMonthDate();
                await InitPage($"GDL - {L["Campaigns.PageTitle"].Value}");
            }
        }
        
        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Contracts"]));
            return ValueTask.CompletedTask;
        }
        
        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["Contract.NewContract.Button"], () =>
            {
                OpenCreateContractModal();
                return Task.CompletedTask;
            }, IconName.Add, requiredPolicyName: ApiPermissions.PartnerModule.Default);

            return ValueTask.CompletedTask;
        }
        
        private async Task SetPermissionsAsync()
        {
            CanCreateContract = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.PartnerModule.Default);
            CanEditContract = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.PartnerModule.Default);
            CanDeleteContract = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.PartnerModule.Default);
        }

        private async Task GetContracts()
        {
            if (!IsManagerRole() && !IsPartnerRole())
            {
                Filter.SalePersonId = CurrentUser.Id;
            }

            var filter = Filter.Clone();
            if (StartDate.HasValue && EndDate.HasValue)
            {
                (filter.CreatedAtMin, filter.CreatedAtMax) = GetDateTimeForApi(StartDate, EndDate);
            }
            
            var result = await _partnerModuleAppService.GetContractNavs(filter);
            ContractDtos = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private void OpenCreateContractModal()
        {
            NewContract = new CreateUpdateContractDto();
            DisablePartnerNewModal = false;
            CreateContractModal.Show();
        }

        private void CloseCreateContractModal()
        {
            CreateContractModal.Hide();
        }

        private async Task CreateContract()
        {
            if (NewContract.TotalValue < NewContract.PartialPaymentValue)
            {
                await Message.Error(Lv[ApiDomainErrorCodes.Contract.InvalidValue]);
                return;
            }

            if (NewContract.ContractCode.IsNullOrEmpty())
            {
                await Message.Error(Lv[ApiDomainErrorCodes.Contract.InvalidContractCode]);
                return;
            }

            var checkCode = await _partnerModuleAppService.ContractExist(NewContract.ContractCode);
            if (checkCode)
            {
                await Message.Error(L["ContractCodeExist"]);
                return;
            }
            
            if (NewContract.PartnerId.IsNullOrEmpty())
            {
                await Message.Error(Lv[ApiDomainErrorCodes.Contract.PartnerIsRequired]);
                return;
            }
            
            var success = await Invoke
            (
                async () =>
                {
                   
                    NewContract.CreatedAt = DateTime.UtcNow;
                    
                    await _partnerModuleAppService.CreateContract(NewContract);
                    await GetContracts();
                    CreateContractModal.Hide();
                },
                L,
                true
            );
        }

        private void OpenEditContractModal(ContractDto input)
        {
            EditingContract = new CreateUpdateContractDto();
            EditingContractId = input.Id;
            EditingContract = ObjectMapper.Map<ContractDto, CreateUpdateContractDto>(input);
            DisablePartnerEditingModal = false;
            EditContractModal.Show();
            OnSelectCampaignEditingModal(EditingContract.CampaignId);
        }

        private void CloseEditContractModal()
        {
            EditContractModal.Hide();
        }

        private async Task UpdateContract()
        {
            if (EditingContract.TotalValue < EditingContract.PartialPaymentValue)
            {
                await Message.Error(Lv[ApiDomainErrorCodes.Contract.InvalidValue]);
                return;
            }
            if (EditingContract.ContractCode.IsNullOrEmpty())
            {
                await Message.Error(Lv[ApiDomainErrorCodes.Contract.InvalidContractCode]);
                return;
            }
            
            var success = await Invoke
            (
                async () =>
                {
                    await _partnerModuleAppService.EditContract(EditingContractId, EditingContract);
                    await GetContracts();
                    EditContractModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private async Task DeleteContract(ContractDto input)
        {
            var success = await Invoke
            (
                async () =>
                {
                    await _partnerModuleAppService.DeleteContract(input.Id);
                    await GetContracts();
                },
                L,
                true,
                BlazorComponentBaseActionType.Delete
            );
        }
        
        // void ValidateCode( ValidatorEventArgs e)
        // {
        //     var code = Convert.ToString(e.Value);
        //
        //     if (code.IsNullOrEmpty())
        //     {
        //         e.Status = ValidationStatus.Error;
        //     }
        //     else
        //     {
        //         var existContract = PartnerModule.CheckExistContract(code);
        //         if (existContract)
        //         {
        //             e.Status = ValidationStatus.Error;
        //             e.ErrorText = L["ContractCodeExist"];
        //         }
        //         else
        //         {
        //             e.Status = ValidationStatus.Success;
        //         }
        //     }
        // }
        
        private async Task GetPartnerLookupAsync()
        {
            PartnersNullable = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto());
        }
        
        private async Task GetCampaigns()
        {
            Campaigns = (await _partnerModuleAppService.GetCampaigns(new GetCampaignsInput()));
        }
        
        private void OnContractStatusSelectedValueChanged(ContractStatus? value)
        {
            Filter.ContractStatus = value == ContractStatus.FilterNoSelect ? null : value;
        }
        
        private void OnContractPaymentStatusSelectedValueChanged(ContractPaymentStatus? value)
        {
            Filter.ContractPaymentStatus = value == ContractPaymentStatus.FilterNoSelect ? null : value;
        }

        private void OnSelectCampaignNewModal(Guid? value)
        {
            NewContract.CampaignId = value;
            NewContract.PartnerId = Campaigns.FirstOrDefault(_ => _.Id == value)?.PartnerId;
            DisablePartnerNewModal = value != null;
        }
        
        private void OnSelectCampaignEditingModal(Guid? value)
        {
            EditingContract.CampaignId = value;
            EditingContract.PartnerId = Campaigns.FirstOrDefault(_ => _.Id == value)?.PartnerId;
            DisablePartnerEditingModal = value != null;
        }
        
        private void ViewDetailsAsync(ContractDto input)
        {
            NavigationManager.NavigateTo($"/contracts/contract-details/{input.Id.ToString()}");
        }
        
    }
}