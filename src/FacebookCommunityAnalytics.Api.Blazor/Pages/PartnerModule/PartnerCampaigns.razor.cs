using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Extensions;
using Blazorise.RichTextEdit;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FluentDateTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class PartnerCampaigns
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<CampaignWithNavigationPropertiesDto> CampaignList { get; set; }
        private int PageSize { get; set; } = 100;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateCampaign { get; set; }
        private bool CanEditCampaign { get; set; }
        private bool CanDeleteCampaign { get; set; }
        private bool CanExport { get; set; }
        private CampaignCreateDto NewCampaign { get; set; }
        private CampaignUpdateDto EditingCampaign { get; set; }
        private Guid EditingCampaignId { get; set; }
        private Modal CreateCampaignModal { get; set; }
        private Modal EditCampaignModal { get; set; }
        private GetCampaignsInput Filter { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private string _selectedCreateInfoTab = "InfoCampaignTab";
        private string _selectedEditInfoTab = "InfoCampaignTab";
        private DateTime? startDateTimeMin;
        private DateTime? startDateTimeMax;
        private DateTime? endDateTimeMin;
        private DateTime? endDateTimeMax;
        private CampaignTypeFilter campaignTypeFilter = CampaignTypeFilter.NoSelect; 
        private CampaignStatusFilter campaignStatusFilter;
        private bool firstLoad = true;
        private DateTime? createStartDateTime;
        private DateTime? createEndDateTime;
        private DateTime? editStartDateTime;
        private DateTime? editEndDateTime;
        private RichTextEdit richTextNewRef;
        private RichTextEdit richTextEditRef;
        private DatePicker<DateTime?> newStartDatePicker;
        private DatePicker<DateTime?> newEndDatePicker;

        private DatePicker<DateTime?> editStartDatePicker;
        private DatePicker<DateTime?> editEndDatePicker;


        private List<CampaignWithNavigationPropertiesDto> selectedCampaigns { get; set; }
        private bool disable = true;
        public PartnerCampaigns()
        {
            NewCampaign = new CampaignCreateDto();
            EditingCampaign = new CampaignUpdateDto();
            Filter = new GetCampaignsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await InitFilter();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetNullablePartnerLookupAsync("");
        }

        private async Task InitFilter()
        {
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

            var queryStartDateTimeMin = NavigationManager.GetQueryParm("StartDateTimeMin");
            if (queryStartDateTimeMin.IsNotNullOrEmpty())
            {
                if (DateTime.TryParse(queryStartDateTimeMin, out var dateTime))
                {
                    startDateTimeMin = await ConvertUniversalToBrowserDateTime(dateTime);
                }
            }

            var queryStartDateTimeMax = NavigationManager.GetQueryParm("StartDateTimeMax");
            if (queryStartDateTimeMax.IsNotNullOrEmpty())
            {
                if (DateTime.TryParse(queryStartDateTimeMax, out var dateTime))
                {
                    startDateTimeMax = await ConvertUniversalToBrowserDateTime(dateTime);
                }
            }

            var queryEndDateTimeMin = NavigationManager.GetQueryParm("EndDateTimeMin");
            if (queryEndDateTimeMin.IsNotNullOrEmpty())
            {
                if (DateTime.TryParse(queryEndDateTimeMin, out var dateTime))
                {
                    endDateTimeMin = await ConvertUniversalToBrowserDateTime(dateTime);
                }
            }

            var queryEndDateTimeMax = NavigationManager.GetQueryParm("EndDateTimeMax");
            if (queryEndDateTimeMax.IsNotNullOrEmpty())
            {
                if (DateTime.TryParse(queryEndDateTimeMax, out var dateTime))
                {
                    endDateTimeMax = await ConvertUniversalToBrowserDateTime(dateTime);
                }
            }

            var campaignType = NavigationManager.GetQueryParm("CampaignType");
            if (campaignType.IsNotNullOrEmpty())
            {
                campaignTypeFilter = (CampaignTypeFilter) Enum.ToObject(typeof(CampaignType), (Convert.ToInt32(campaignType)));
                Filter.CampaignType = Convert.ToInt32(campaignType).ToEnum<CampaignType>();
            }

            var status = NavigationManager.GetQueryParm("Status");
            if (status.IsNotNullOrEmpty())
            {
                campaignStatusFilter = (CampaignStatusFilter) Enum.ToObject(typeof(CampaignStatus), (Convert.ToInt32(status)));
                Filter.Status = Convert.ToInt32(status).ToEnum<CampaignStatus>();
            }

            var filterText = NavigationManager.GetQueryParm("FilterText");
            if (filterText.IsNotNullOrEmpty())
            {
                Filter.FilterText = filterText;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["Campaigns.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Campaigns"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["Campaign.CreateButton"],
                async () =>
                {
                    await OpenCreateCampaignModal();
                },
                requiredPolicyName: ApiPermissions.PartnerModule.Default
            );

            // Toolbar.AddButton
            // (
            //     L["TriggerCrawler.Button"],
            //     async () =>
            //     {
            //         await  TriggerCrawlerAsync();
            //     },
            //     IconName.Bolt,
            //     Color.Warning,
            //     requiredPolicyName: ApiPermissions.PartnerModule.Default
            // );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCampaign = await AuthorizationService.IsGrantedAsync(ApiPermissions.PartnerModule.Default);
            CanEditCampaign = await AuthorizationService.IsGrantedAsync(ApiPermissions.PartnerModule.Default);
            CanDeleteCampaign = await AuthorizationService.IsGrantedAsync(ApiPermissions.PartnerModule.Default);
            CanExport = await AuthorizationService.IsGrantedAsync(ApiPermissions.PartnerModule.Default);
        }

        private async Task DoSearch()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            if (Filter.FilterText.IsNotNullOrWhiteSpace())
            {
                campaignTypeFilter = CampaignTypeFilter.NoSelect;
                campaignStatusFilter = CampaignStatusFilter.NoSelect;
                startDateTimeMin = null;
                startDateTimeMax = null;
                endDateTimeMin = null;
                endDateTimeMax = null;
            }
            
            var filter = Filter.Clone();
            if (endDateTimeMin != null && endDateTimeMax != null)
            {
                (filter.EndDateTimeMin, filter.EndDateTimeMax) = GetDateTimeForApi(endDateTimeMin, endDateTimeMax);
            }

            if (startDateTimeMin != null && startDateTimeMax != null)
            {
                (filter.StartDateTimeMin, filter.StartDateTimeMax) = GetDateTimeForApi(startDateTimeMin, startDateTimeMax);
            }
            
            filter.CampaignType = campaignTypeFilter == CampaignTypeFilter.NoSelect ? null : (CampaignType) Enum.ToObject(typeof(CampaignType), (Convert.ToInt32(campaignTypeFilter)));
            filter.Status = campaignStatusFilter == CampaignStatusFilter.NoSelect ? null : (CampaignStatus) Enum.ToObject(typeof(CampaignStatus), (Convert.ToInt32(campaignStatusFilter)));

            if (!firstLoad)
            {
                var contractBaseUrl = NavigationManager.ToAbsoluteUri("partnercampaigns");
                NavigationManager.NavigateTo($"{contractBaseUrl}?{filter.GetQueryString()}", false);
            }

            var result = await _partnerModuleAppService.GetCampaignNavs(filter);
            CampaignList = result.Items;
            TotalCount = (int) result.TotalCount;
            firstLoad = false;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CampaignWithNavigationPropertiesDto> e)
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

        private async Task TriggerCrawlerAsync()
        {
            if (selectedCampaigns.IsNullOrEmpty())
            {
                await Message.Error(LD["TriggerCrawler:CampaignIsRequired"]);
            }
            else
            {
                var numberPosts = await CrawlAppService.InitCampaignPosts(selectedCampaigns.Select(x => x.Campaign.Code).ToList());
                await Message.Success(string.Format(L["TriggerCrawler.NumberPost"], numberPosts));
            }
        }

        private async Task OpenCreateCampaignModal()
        {
            _selectedCreateInfoTab = "InfoCampaignTab";
            NewCampaign = new CampaignCreateDto
            {
                CampaignType = CampaignType.Seeding,
                IsActive = true,
            };
            await CreateStartDateTimeChange(DateTime.UtcNow.Date);
            await CreateEndDateTimeChange(DateTime.UtcNow.Date.AddMonths(1).AddFluentTimeSpan(new TimeSpan(23, 59, 59)));
            CreateCampaignModal.Show();
        }

        private void CloseCreateCampaignModal()
        {
            CreateCampaignModal.Hide();
        }

        private async Task OpenEditCampaignModal(CampaignWithNavigationPropertiesDto input)
        {
            _selectedEditInfoTab = "InfoCampaignTab";
            EditingCampaign = new CampaignUpdateDto();
            EditingCampaignId = input.Campaign.Id;
            EditingCampaign = ObjectMapper.Map<CampaignDto, CampaignUpdateDto>(input.Campaign);
            
            if (EditingCampaign.StartDateTime != null)
            {
                editStartDateTime = await ConvertUniversalToBrowserDateTime(EditingCampaign.StartDateTime.Value);
            }

            if (EditingCampaign.EndDateTime != null)
            {
                editEndDateTime = await ConvertUniversalToBrowserDateTime(EditingCampaign.EndDateTime.Value);
            }
            EditCampaignModal.Show();
        }

        private async Task DeleteCampaignAsync(CampaignWithNavigationPropertiesDto input)
        {
            var resultConfirm = await UiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (resultConfirm)
            {
                await _partnerModuleAppService.DeleteCampaign(input.Campaign.Id);
                await DoSearch();
            }
        }

        private async Task SendEmail(Guid campaignId)
        {
            await _partnerModuleAppService.SendCampaignEmail(campaignId);
            await Message.Success(L["Message.SendingEmail"]);
        }

        private async Task CreateCampaignAsync()
        {
            if (NewCampaign.Name.IsNullOrEmpty())
            {
                await Message.Error(L["Campaign:NameIsRequired"]);
                return;
            }

            if (!NewCampaign.PartnerId.HasValue)
            {
                await Message.Error(L["Campaign:PartnerIsRequired"]);
                return;
            }
            //Pre Check exist
            var campaigns = await _partnerModuleAppService.GetCampaigns(new GetCampaignsInput {Code = NewCampaign.Code});
            if (campaigns.IsNotNullOrEmpty())
            {
                await Message.Error(L["CampaignExist"]);
            }

            var success = await Invoke
            (
                async () =>
                {
                    if (NewCampaign.StartDateTime >= NewCampaign.EndDateTime)
                    {
                        NewCampaign.StartDateTime = NewCampaign.StartDateTime.Value.Date;
                        NewCampaign.EndDateTime = NewCampaign.EndDateTime.Value.Date.AddFluentTimeSpan(new TimeSpan(23, 59, 59));
                    }

                    await _partnerModuleAppService.CreateCampaign(NewCampaign);
                    await DoSearch();
                    CreateCampaignModal.Hide();
                },
                L,
                true
            );
        }

        private void CloseEditCampaignModal()
        {
            EditCampaignModal.Hide();
        }

        private async Task UpdateCampaignAsync()
        {
            if (EditingCampaign.Name.IsNullOrEmpty())
            {
                await Message.Error(L["Campaign:NameIsRequired"]);
                return;
            }
            
            if (!EditingCampaign.PartnerId.HasValue)
            {
                await Message.Error(L["Campaign:PartnerIsRequired"]);
                return;
            }

            var success = await Invoke
            (
                async () =>
                {
                    if (EditingCampaign.StartDateTime >= EditingCampaign.EndDateTime)
                    {
                        EditingCampaign.StartDateTime = EditingCampaign.StartDateTime.Value.Date;
                        EditingCampaign.EndDateTime = EditingCampaign.EndDateTime.Value.Date.AddFluentTimeSpan(new TimeSpan(23, 59, 59));
                    }

                    await _partnerModuleAppService.EditCampaign(EditingCampaignId, EditingCampaign);
                    await DoSearch();
                    EditCampaignModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private async Task GetNullablePartnerLookupAsync(string newValue)
        {
            PartnersNullable = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto {Filter = newValue});
        }

        private async Task ExportPostsAsync(Guid? campaignId)
        {
            if (campaignId != null)
            {
                var data = await _partnerModuleAppService.ExportCampaign((Guid) campaignId);
                if (data.IsNullOrEmpty())
                {
                    await Message.Info(L["CampaignExport.NoPost"]);
                    return;
                }

                var campaign = await _partnerModuleAppService.GetCampaign(campaignId.Value);
                var fileName = $"CP_{campaign.Name}";
                await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(data));
            }
        }

        private void ViewDetailsAsync(CampaignWithNavigationPropertiesDto input)
        {
            NavigationManager.NavigateTo($"/partner-campaign-details/{input.Campaign.Id.ToString()}");
        }

        private void OnNewNameChanged(string name)
        {
            NewCampaign.Name = name;
            NewCampaign.Code = name.IsNullOrWhiteSpace() ? "" : name.Replace(" ", "").Trim().RemoveDiacritics().ToLower();
        }

        private void ValidateName(ValidatorEventArgs e)
        {
            e.Status = string.IsNullOrEmpty(Convert.ToString(e.Value))
                ? ValidationStatus.Error
                : ValidationStatus.Success;
        }

        private async Task ValidateCodeAsync(ValidatorEventArgs e, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //Pre Check exist
            var campaigns = await _partnerModuleAppService.GetCampaigns(new GetCampaignsInput {Code = NewCampaign.Code});
            if (campaigns.IsNotNullOrEmpty())
            {
                await Message.Error(L["CampaignExist"]);
            }

            e.Status = campaigns.IsNotNullOrEmpty()
                ? ValidationStatus.Error
                : ValidationStatus.Success;
        }

        private void OnSelectedCreateInfoTabChanged(string name)
        {
            _selectedCreateInfoTab = name;
        }

        private void OnSelectedEditInfoTabChanged(string name)
        {
            _selectedEditInfoTab = name;
        }

        private async Task OnCampaignType_Changed(CampaignTypeFilter value)
        {
            campaignTypeFilter = value;
            await OnSelectFilter();
        }

        private async Task OnCampaignStatus_Changed(CampaignStatusFilter value)
        {
            campaignStatusFilter = value;
            await OnSelectFilter();
        }
        
        private async Task OnStartDateTimeMin_Changed(DateTime? value)
        {
            if (value != null)
            {
                startDateTimeMin = value;
                if(startDateTimeMax != null) await OnSelectFilter();
            }
        }
        
        private async Task OnStartDateTimeMax_Changed(DateTime? value)
        {
            if (value != null)
            {
                startDateTimeMax = value;
                if(startDateTimeMin != null) await OnSelectFilter();
            }
        }
        
        private async Task OnEndDateTimeMin_Changed(DateTime? value)
        {
            if (value != null)
            {
                endDateTimeMin = value;
                if(endDateTimeMax != null) await OnSelectFilter();
            }
        }
        
        private async Task OnEndDateTimeMax_Changed(DateTime? value)
        {
            if (value != null)
            {
                endDateTimeMax = value;
                if(endDateTimeMin != null) await OnSelectFilter();
            }
        }

        private async Task CreateStartDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                createStartDateTime = value;
                NewCampaign.StartDateTime = await ConvertBrowserToUniversalDateTime(createStartDateTime.Value);
            }
        }
        
        private async Task CreateEndDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                createEndDateTime = value;
                NewCampaign.EndDateTime = await ConvertBrowserToUniversalDateTime(createEndDateTime.Value);
            }
        }
        
        private async Task EditStartDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                editStartDateTime = value;
                EditingCampaign.StartDateTime = await ConvertBrowserToUniversalDateTime(editStartDateTime.Value);
            }
        }
        
        private async Task EditEndDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                editEndDateTime = value;
                EditingCampaign.EndDateTime = await ConvertBrowserToUniversalDateTime(editEndDateTime.Value);
            }
        }
        
        private async Task OnSelectFilter()
        {
            Filter.FilterText = string.Empty;
            await DoSearch();
        }
        public async Task OnDirectorReviewContentChanged()
        {
            NewCampaign.Description = await richTextNewRef.GetTextAsync();
        }
        
        public async Task OnDirectorReviewEditContent()
        {
            EditingCampaign.Description = await richTextEditRef.GetTextAsync();
        }

    }
}