using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Partners
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; set; } = new();
        private IReadOnlyList<PartnerDto> PartnerList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
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
        private PartnerType selectedPartnerType;
        private DataGridEntityActionsColumn<PartnerDto> EntityActionsColumn { get; set; }

        private IReadOnlyList<LookupDto<Guid>> PartnerUsersLookupDtos { get; set; } = new List<LookupDto<Guid>>();

        private IEnumerable<Guid> CurrentPartUserIds = new List<Guid>();

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
            await GetPartnerUsersLookupAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["Partners.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Partners"]));
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
                requiredPolicyName: ApiPermissions.Partners.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePartner = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Partners.Create);
            CanEditPartner = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Partners.Edit);
            CanDeletePartner = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Partners.Delete);
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

            var result = await PartnersAppService.GetListAsync(filter);
            PartnerList = result.Items;
            TotalCount = (int) result.TotalCount;
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
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreatePartnerModal()
        {
            NewPartner = new PartnerCreateDto {IsActive = true};

            CurrentPartUserIds = new List<Guid>();
            CreatePartnerModal.Show();
        }

        private void CloseCreatePartnerModal()
        {
            CreatePartnerModal.Hide();
        }

        private void OpenEditPartnerModal(PartnerDto input)
        {
            CurrentPartUserIds = input.PartnerUserIds;
            EditingPartnerId = input.Id;
            EditingPartner = ObjectMapper.Map<PartnerDto, PartnerUpdateDto>(input);
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
                        await PartnersAppService.DeleteAsync(input.Id);
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
            await Invoke
            (
                async () =>
                {
                    NewPartner.Code = NewPartner.Name.IsNullOrWhiteSpace() ? "" : NewPartner.Name.Replace(" ", "").Trim().RemoveDiacritics().ToLower();
                    await PartnersAppService.CreateAsync(NewPartner);
                    await DoSearch();
                    CreatePartnerModal.Hide();
                },
                L,
                true
            );
        }

        private void CloseEditPartnerModal()
        {
            EditPartnerModal.Hide();
        }

        private async Task UpdatePartnerAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await PartnersAppService.UpdateAsync(EditingPartnerId, EditingPartner);
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

        private async Task GetPartnerUsersLookupAsync()
        {
            PartnerUsersLookupDtos = await PartnersAppService.GetPartnerUserLookupAsync(new LookupRequestDto {MaxResultCount = 1000});
        }

        private Task PartnerUsersSelectedValuesChanged(object value)
        {
            if (value != null)
            {
                var ids = value is IEnumerable<Guid> guids ? guids.ToList() : new List<Guid> {(Guid) value};
                NewPartner.PartnerUserIds = ids;
                EditingPartner.PartnerUserIds = ids;
            }
            else
            {
                NewPartner.PartnerUserIds = new List<Guid>();
                EditingPartner.PartnerUserIds = new List<Guid>();
            }


            return Task.CompletedTask;
        }

        private async Task OnPartnerType_Changed(PartnerType value)
        {
            selectedPartnerType = value;
            Filter.FilterText = string.Empty;
            await DoSearch();
        }
    }
}