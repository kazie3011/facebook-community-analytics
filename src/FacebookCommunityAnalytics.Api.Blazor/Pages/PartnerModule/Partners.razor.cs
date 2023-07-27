using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class Partners : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; set; } = new();
        private IReadOnlyList<PartnerDto> PartnerList { get; set; }
        private int PageSize { get; set; } = 50;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreatePartner { get; set; }
        private bool CanEditPartner { get; set; }
        private bool CanDeletePartner { get; set; }
        private PartnerCreateDto NewPartner { get; set; }
        private PartnerUpdateDto EditingPartner { get; set; }
        private Guid EditingPartnerId { get; set; }
        private Modal CreatePartnerModal { get; set; }
        private Modal EditPartnerModal { get; set; }
        private GetPartnersInput Filter { get; set; }
        private PartnerType SelectedPartnerType { get; set; }
        private DataGridEntityActionsColumn<PartnerDto> EntityActionsColumn { get; set; }
        private Validations NewPartnerValidations { get; set; }
        private Validations EditPartnerValidations { get; set; }
        private PartnerType selectedPartnerType;


        private List<PartnerDto> selectedPartners;
        public Partners()
        {
            NewPartner = new PartnerCreateDto();
            EditingPartner = new PartnerUpdateDto();
            Filter = new GetPartnersInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["Partners.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Partner.List"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewPartner"],
                () =>
                {
                    OpenCreatePartnerModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.PartnerModule.Default
            );

            // Toolbar.AddButton
            // (
            //     L["TriggerCrawler.Button"],
            //     async () =>
            //     {
            //        await  TriggerCrawlerAsync();
            //     },
            //     IconName.Bolt,
            //     Color.Warning,
            //     requiredPolicyName: ApiPermissions.PartnerModule.Default
            // );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePartner = await AuthorizationService.IsGrantedAsync(ApiPermissions.PartnerModule.Default);
            CanEditPartner = await AuthorizationService.IsGrantedAsync(ApiPermissions.PartnerModule.Default);
            CanDeletePartner = await AuthorizationService.IsGrantedAsync(ApiPermissions.PartnerModule.Default);
        }

        private async Task DoSearch()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            
            if (Filter.FilterText.IsNotNullOrWhiteSpace())
            {
                selectedPartnerType = PartnerType.FilterNoSelect;
            }
            var filter = Filter.Clone();
            filter.PartnerType = selectedPartnerType == PartnerType.FilterNoSelect ? null : selectedPartnerType;

            var result = await _partnerModuleAppService.GetPartners(filter);
            PartnerList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PartnerDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await DoSearch();
            //await InvokeAsync(StateHasChanged);
        }

        private void OpenCreatePartnerModal()
        {
            NewPartner = new PartnerCreateDto { IsActive = true };

            NewPartnerValidations.ClearAll();
            CreatePartnerModal.Show();
        }

        private void CloseCreatePartnerModal()
        {
            CreatePartnerModal.Hide();
        }

        private void OpenEditPartnerModal(PartnerDto input)
        {
            EditingPartnerId = input.Id;
            EditingPartner = ObjectMapper.Map<PartnerDto, PartnerUpdateDto>(input); 
            EditPartnerValidations.ClearAll();
            EditPartnerModal.Show();
        }

        private async Task DeletePartnerAsync(PartnerDto input)
        {
            var confirmResult = await UiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                var success = await Invoke
                (
                    async () =>
                    {
                        await _partnerModuleAppService.DeletePartner(input.Id);
                        await DoSearch();
                    },
                    L,
                    true,
                    BlazorComponentBaseActionType.Delete
                );
            }
        }

        private async Task CreatePartnerAsync()
        {
            if (!NewPartnerValidations.ValidateAll())
            {
                return;
            }

            var success = await Invoke
            (
                async () =>
                {
                    NewPartner.Code = NewPartner.Name.IsNullOrWhiteSpace() ? "" : NewPartner.Name.Replace(" ", "").Trim().RemoveDiacritics().ToLower();
                    await _partnerModuleAppService.CreatePartner(NewPartner);
                    await DoSearch();
                    CreatePartnerModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Create
            );
        }

        private void CloseEditPartnerModal()
        {
            EditPartnerModal.Hide();
        }

        private async Task UpdatePartnerAsync()
        {
            if (!EditPartnerValidations.ValidateAll())
            {
                return;
            }

            var success = await Invoke
            (
                async () =>
                {
                    await _partnerModuleAppService.EditPartner(EditingPartnerId, EditingPartner);
                    await DoSearch();
                    EditPartnerModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private void ViewDetailsAsync(PartnerDto input)
        {
            NavigationManager.NavigateTo($"/partner-details/{input.Id.ToString()}");
        }
        
        private async Task OnPartnerType_Changed(PartnerType value)
        {
            selectedPartnerType = value;
            Filter.FilterText = string.Empty;
            await DoSearch();
        }
    }
}