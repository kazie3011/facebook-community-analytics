using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using FacebookCommunityAnalytics.Api.Blazor.Models;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Organizations;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class AffLinkDetails : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<UserAffiliateWithNavigationPropertiesDto> UserAffiliateList { get; set; }
        private int PageSize { get; set; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateUserAffiliate { get; set; }
        private bool CanEditUserAffiliate { get; set; }
        private bool CanDeleteUserAffiliate { get; set; }
        private UserAffiliateCreateDto NewUserAffiliate { get; set; }
        private UserAffiliateUpdateDto EditingUserAffiliate { get; set; }

        private Guid EditingUserAffiliateId { get; set; }

        // private Modal CreateUserAffiliateModal { get; set; }
        private Modal EditUserAffiliateModal { get; set; }
        private GetUserAffiliatesInputExtend Filter { get; set; }

        private DataGridEntityActionsColumn<UserAffiliateWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> CategoriesNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> CampaignsNullable { get; set; } = new List<LookupDto<Guid?>>();
        private IReadOnlyList<LookupDto<Guid?>> GroupsNullable { get; set; } = new List<LookupDto<Guid?>>();

        public UserAffiliateHasConversionFilter UserAffiliateHasConversionFilter { get; set; } = UserAffiliateHasConversionFilter.NoSelect;
        public AffiliateProviderType SelectedAffiliateProviderTypeFilter { get; set; }
        public Guid? SelectedTeamIdFilter { get; set; }
        public Guid? SelectedPartnerIdFilter { get; set; }
        public Guid? SelectedCampaignIdFilter { get; set; }

        private IList<OrganizationUnitDto> _organizationUnitDtos = new List<OrganizationUnitDto>();
        private IList<GroupDto> _groupDtos = new List<GroupDto>();
        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }

        private bool _showLoading;

        public AffLinkDetails()
        {
            NewUserAffiliate = new UserAffiliateCreateDto();
            EditingUserAffiliate = new UserAffiliateUpdateDto();
            Filter = new GetUserAffiliatesInputExtend
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = null
            };
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                BrowserDateTime = await GetBrowserDateTime();
                Filter.ConversionOwnerFilter = ConversionOwnerFilter.NoSelect;
                Filter.AffiliateProviderType = AffiliateProviderType.Shopiness;
                await SetPermissionsAsync();
                await SetToolbarItemsAsync();
                await SetBreadcrumbItemsAsync();
                await GetNullableCategoryLookupAsync("");
                await GetNullablePartnerLookupAsync("");
                await GetNullableCampaignLookupAsync("");
                await GetNullableGroupLookupAsync("");
                _groupDtos = await GroupExtendAppService.GetListAsync();

                if (CurrentUser is {IsAuthenticated: true} && IsManagerRole())
                {
                    _organizationUnitDtos = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest());
                    _organizationUnitDtos = _organizationUnitDtos.OrderBy(_ => _.DisplayName).ToList();
                }

                _dateRanges = await GetDateRangePicker();
            }
            catch (Exception e)
            {
                await JSRuntime.InvokeAsync<string>("console.log", e);
                throw;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                (StartDate, EndDate) = await GetDefaultDate();
                await InitPage($"GDL - {L["Aff.LinkDetails.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:AffLinkDetails"], "/linkdetails"));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            // Toolbar.AddButton(L["UserAffiliate.RefreshConversionButton"], () =>
            // {
            //     UpdateUserAffiliateConversion();
            //     return Task.CompletedTask;
            //     //await DoSearch();
            // }, IconName.Add, requiredPolicyName: ApiPermissions.UserAffiliates.Edit);
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserAffiliate = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserAffiliates.Create);
            CanEditUserAffiliate = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserAffiliates.Edit);
            CanDeleteUserAffiliate = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserAffiliates.Delete);
        }

        private async Task DoSearch()
        {
            _showLoading = false;
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            (Filter.CreatedAtMin, Filter.CreatedAtMax) = GetDateTimeForApi(StartDate, EndDate);
            if (Filter.FilterText.IsNotNullOrEmpty())
            {
                Filter.RelativeDateTimeRange = RelativeDateTimeRange.Unknown;
                Filter.CreatedAtMin = null;
                Filter.CreatedAtMax = null;
                (StartDate, EndDate) = await GetDefaultDate();
                SelectedAffiliateProviderTypeFilter = AffiliateProviderType.FilterNoSelect;
                SelectedTeamIdFilter = Guid.Empty;
                SelectedCampaignIdFilter = Guid.Empty;
                SelectedPartnerIdFilter = Guid.Empty;
                await JsRuntime.InvokeVoidAsync("setInputValue", "#GroupIdInputFilter input", "");
            }

            switch (UserAffiliateHasConversionFilter)
            {
                case UserAffiliateHasConversionFilter.NoSelect:
                    Filter.HasConversion = null;
                    break;
                case UserAffiliateHasConversionFilter.HasConversion:
                    Filter.HasConversion = true;
                    break;
                case UserAffiliateHasConversionFilter.NoConversion:
                    Filter.HasConversion = false;
                    break;
            }

            var filter = Filter.Clone();
            filter.AffiliateProviderType = SelectedAffiliateProviderTypeFilter == AffiliateProviderType.FilterNoSelect ? null : SelectedAffiliateProviderTypeFilter;
            filter.OrgUnitId = SelectedTeamIdFilter == Guid.Empty ? null : SelectedTeamIdFilter;
            filter.CampaignId = SelectedCampaignIdFilter == Guid.Empty ? null : SelectedCampaignIdFilter;
            filter.PartnerId = SelectedPartnerIdFilter == Guid.Empty ? null : SelectedPartnerIdFilter;
            var result = await UserAffiliateAppService.GetUserAffiliateWithNavigationProperties(filter);
            UserAffiliateList = result.Items;
            TotalCount = (int) result.TotalCount;
            _showLoading = true;
        }

        #region SEARCH

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnTeamSelectedValueChanged(Guid? value)
        {
            SelectedTeamIdFilter = value;
            await OnSelectFilter();
        }

        private void OnConversionOwnFilterCheckedChanged(bool value)
        {
            Filter.ConversionOwnerFilter = value ? ConversionOwnerFilter.Own : ConversionOwnerFilter.NoSelect;
        }

        #endregion

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserAffiliateWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenEditUserAffiliateModal(UserAffiliateDto input)
        {
            EditingUserAffiliateId = input.Id;
            EditingUserAffiliate = ObjectMapper.Map<UserAffiliateDto, UserAffiliateUpdateDto>(input);
            EditingUserAffiliate.AppUserId = input.AppUserId;
            EditUserAffiliateModal.Show();
        }

        private async Task DeleteUserAffiliateAsync(UserAffiliateDto input)
        {
            await UserAffiliateAppService.DeleteAsync(input.Id);
            await DoSearch();
        }

        private void CloseEditUserAffiliateModal()
        {
            EditUserAffiliateModal.Hide();
            EditingUserAffiliate = new();
        }

        private async Task UpdateUserAffiliateAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await UserAffiliateAppService.UpdateAsync(EditingUserAffiliateId, EditingUserAffiliate);
                    await DoSearch();
                    EditUserAffiliateModal.Hide();
                },
                L,
                true,
                actionType: BlazorComponentBaseActionType.Update
            );

            await InvokeAsync(StateHasChanged);
        }

        private async Task GetNullableAppUserLookupAsync(string newValue)
        {
            UsersNullable = (await PostsExtendAppService.GetAppUserLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullableCategoryLookupAsync(string newValue)
        {
            CategoriesNullable = (await PostsExtendAppService.GetCategoryLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullablePartnerLookupAsync(string newValue)
        {
            PartnersNullable = (await PostsExtendAppService.GetPartnerLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullableCampaignLookupAsync(string newValue)
        {
            CampaignsNullable = (await PostsExtendAppService.GetCampaignLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task GetNullableGroupLookupAsync(string newValue)
        {
            var groups = await PostsExtendAppService.GetGroupLookupAsync(new GroupLookupRequestDto {Filter = newValue});
            GroupsNullable = groups.Items;
            await OnSelectFilter();
        }


        // private async Task<bool> CopyShortlink()
        // {
        //    return  await CopyToClipboard(NewUserAffiliate.AffiliateUrl);
        // }

        // IconName currentStateCopy = IconName.Save;
        // private async Task<bool> CopyToClipboard(string value)
        // {
        //     if (value.IsNotNullOrWhiteSpace())
        //     {
        //         try
        //         {
        //             await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", value.Trim());
        //         }
        //         catch (Exception e)
        //         {
        //             await Message.Error(JsonConvert.SerializeObject(e));
        //         }
        //         // try
        //         // {
        //         //     await ClipboardService.WriteTextAsync(value.Trim());
        //         // }
        //         // catch (Exception e)
        //         // {
        //         //     await Message.Error(JsonConvert.SerializeObject(e));
        //         // }
        //         currentStateCopy = IconName.Check;
        //         return true;
        //     }
        //     return false;
        // }

        private async Task OnSelectedAffiliateProviderType(AffiliateProviderType value)
        {
            SelectedAffiliateProviderTypeFilter = value;
            await OnSelectFilter();
        }

        private async Task OnSelectFilter()
        {
            Filter.FilterText = string.Empty;
            await DoSearch();
        }

        private async Task OnSelectedValuePartner(Guid? value)
        {
            SelectedPartnerIdFilter = value;
            await OnSelectFilter();
        }

        private async Task OnSelectedValueCampaign(Guid? value)
        {
            SelectedCampaignIdFilter = value;
            await OnSelectFilter();
        }

        // private void OnSelectedValueSearchGroup(Guid? value)
        // {
        //     if (value == Guid.Empty)
        //     {
        //         Filter.GroupId = null;
        //     }
        //     else
        //     {
        //         Filter.GroupId = value;
        //     }
        // }
        //
        // private void OnSelectedValueGroup(Guid? value)
        // {
        //     if (value == Guid.Empty)
        //     {
        //         NewUserAffiliate.GroupId = null;
        //     }
        //     else
        //     {
        //         NewUserAffiliate.GroupId = value;
        //     }
        // }

        private string GetNameGroup(Guid? groupId)
        {
            var group = _groupDtos.FirstOrDefault(_ => _.Id == groupId);
            if (group == null) return string.Empty;
            return GroupConsts.GetGroupDisplayName(group.Title, group.GroupSourceType);
        }

        private async Task ExportPostsAsync()
        {
            var filter = Filter.Clone();
            
            (filter.CreatedAtMin, filter.CreatedAtMax) = GetDateTimeForApi(StartDate, EndDate);
            var excelBytes = await UserAffiliateAppService.GetUserAffiliateExcelAsync(filter);
            if (excelBytes == null)
            {
                await Message.Info(L["CampaignExport.NoAffiliates"]);
                return;
            }

            var fileName = "Affiliate-Shortlink";
            await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(excelBytes));
        }

        private void StartDateChanged(DateTimeOffset? offset)
        {
            StartDate = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            EndDate = offset;
        }
    }
}