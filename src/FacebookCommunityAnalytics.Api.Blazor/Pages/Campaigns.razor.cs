using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.PieChart;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FluentDateTime;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Campaigns : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<CampaignWithNavigationPropertiesDto> CampaignList { get; set; }
        private IReadOnlyList<AuthorStatistic> AuthorStatistics { get; set; } = new List<AuthorStatistic>();
        private int PageSize { get; set; } = 100;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int AuthorPageSize { get; set; } = 10;
        private int AuthorCurrentPage { get; set; } = 1;
        private string AuthorCurrentSorting { get; set; }
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
        private GetCampaignsInput StatsFilter { get; set; }

        private DataGridEntityActionsColumn<CampaignWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private string _selectedCreateInfoTab = "InfoCampaignTab";
        private string _selectedEditInfoTab = "InfoCampaignTab";
        private DateTime? startDateTimeMin;
        private DateTime? startDateTimeMax;
        private DateTime? endDateTimeMin;
        private DateTime? endDateTimeMax;
        private CampaignTypeFilter campaignTypeFilter;
        private CampaignStatusFilter campaignStatusFilter = CampaignStatusFilter.Started;
        private bool firstLoad = true;

        private DateTime? createStartDateTime;
        private DateTime? createEndDateTime;
        private DateTime? editStartDateTime;
        private DateTime? editEndDateTime;
        private IReadOnlyList<CampaignDto> campKpiList { get; set; }
        

        private Chart _postGroupChart;
        private PieConfig _configPostGroupChart;

        private Chart _reactionGroupChart;
        private PieConfig _configReactionGroupChart;
        
        private string _chartFontTitleColor = "#037404";
        private string campaignTabName = "CampaignSearching";

        public Campaigns()
        {
            NewCampaign = new CampaignCreateDto();
            EditingCampaign = new CampaignUpdateDto();
            Filter = new GetCampaignsInput
            {
                Status = CampaignStatus.Started,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            
        }

        protected override async Task OnInitializedAsync()
        {
            ConfigChannelGroupChart();
            ConfigEngagementChart();
            BrowserDateTime = await GetBrowserDateTime();

            await GetAuthorStatistics();
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
                await LoadPostGroupStatsByCurrentMonth();
                await LoadPostEngagementStatsByCurrentMonth();
            }

            firstLoad = false;
        }

        private async Task LoadPostEngagementStatsByCurrentMonth()
        {
            _configReactionGroupChart.Data.Labels.Clear();
            _configReactionGroupChart.Data.Datasets.Clear();

            var (fromTime, endTime) = await GetBrowserDateTimeCurrentMonth();

            var chartData = await CampaignsAppService.GetReactionGroupsChart(fromTime.Value, endTime.Value);
            var dataItems = new List<double>();
            var color = new List<string>();
            var customTooltips = new List<string>();
            var i = 0;

            foreach (var item in chartData.Items)
            {
                _configReactionGroupChart.Data.Labels.Add(item.Label);
                color.Add(ChartColorHelper.GetColor(i));
                dataItems.Add(item.Value);
                customTooltips.Add($"{item.Value} ({item.ValuePercent}%)");
                i++;
            }

            var dataSet = new PieDataset<double>(dataItems, useDoughnutDefaults: false);
            dataSet.BackgroundColor = color.ToArray();
            _configReactionGroupChart.Data.Datasets.Add(dataSet);

            await _reactionGroupChart.Update();

            await JsRuntime.InvokeVoidAsync
            (
                "generalInterop.datalabelsConfig",
                _configReactionGroupChart.CanvasId,
                false,
                false,
                false,
                new List<string>(),
                new[] {0},
                false,
                customTooltips
            );
        }

        private async Task LoadPostGroupStatsByCurrentMonth()
        {
            _configPostGroupChart.Data.Labels.Clear();
            _configPostGroupChart.Data.Datasets.Clear();

            var (fromTime, endTime) = await GetBrowserDateTimeCurrentMonth();

            var chartData = await CampaignsAppService.GetPostCountGroupsChart(fromTime.Value, endTime.Value);
            var colors = new List<string>();
            var dataItems = new List<double>();
            var customTooltips = new List<string>();
            int i = 0;

            foreach (var item in chartData.Items)
            {
                _configPostGroupChart.Data.Labels.Add(item.Label);
                colors.Add(ChartColorHelper.GetColor(i));
                dataItems.Add(item.Value);
                customTooltips.Add($"{item.Value} ({item.ValuePercent}%)");
                i++;
            }
            
            var dataSet = new PieDataset<double>(dataItems, useDoughnutDefaults: false);
            dataSet.BackgroundColor = colors.ToArray();
            _configPostGroupChart.Data.Datasets.Add(dataSet);
            
            await _postGroupChart.Update();

            await JsRuntime.InvokeVoidAsync
            (       
                "generalInterop.datalabelsConfig",
                _configPostGroupChart.CanvasId,
                false,
                false,
                false,
                new List<string>(),
                new[] {0},
                false,
                customTooltips
            );
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
                L["NewCampaign"],
                () =>
                {
                    OpenCreateCampaignModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Campaigns.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCampaign = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Campaigns.Create);
            CanEditCampaign = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Campaigns.Edit);
            CanDeleteCampaign = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Campaigns.Delete);
            CanExport = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Campaigns.Export);
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
            if (startDateTimeMin != null && startDateTimeMax != null)
            {
                (filter.StartDateTimeMin, filter.StartDateTimeMax) = GetDateTimeForApi(startDateTimeMin, startDateTimeMax);
            }

            if (endDateTimeMin != null && endDateTimeMax != null)
            {
                (filter.EndDateTimeMin, filter.EndDateTimeMax) = GetDateTimeForApi(endDateTimeMin, endDateTimeMax);
            }

            filter.CampaignType = campaignTypeFilter == CampaignTypeFilter.NoSelect ? null : (CampaignType) Enum.ToObject(typeof(CampaignType), (Convert.ToInt32(campaignTypeFilter)));
            filter.Status = campaignStatusFilter == CampaignStatusFilter.NoSelect ? null : (CampaignStatus) Enum.ToObject(typeof(CampaignStatus), (Convert.ToInt32(campaignStatusFilter)));

            if (!firstLoad)
            {
                var campaignsBaseUrl = NavigationManager.ToAbsoluteUri("campaigns");
                NavigationManager.NavigateTo($"{campaignsBaseUrl}?{filter.GetQueryString()}", false);
            }

            var result = await CampaignsAppService.GetListAsync(filter);
            CampaignList = result.Items;
            TotalCount = (int) result.TotalCount;
            firstLoad = false;
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

        private async Task OpenCreateCampaignModal()
        {
            _selectedCreateInfoTab = "InfoCampaignTab";
            NewCampaign = new CampaignCreateDto
            {
                IsActive = true,
            };
            await CreateStartDateTimeChange(DateTime.UtcNow.Date);
            await CreateEndDateTimeChange(DateTime.UtcNow.Date.AddFluentTimeSpan(new TimeSpan(23, 59, 59)));
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

            editStartDateTime = EditingCampaign.StartDateTime != null ? await ConvertUniversalToBrowserDateTime(EditingCampaign.StartDateTime.Value) : null;
            editEndDateTime = EditingCampaign.EndDateTime != null ? await ConvertUniversalToBrowserDateTime(EditingCampaign.EndDateTime.Value) : null;

            EditCampaignModal.Show();
        }

        private async Task DeleteCampaignAsync(CampaignWithNavigationPropertiesDto input)
        {
            var resultConfirm = await _uiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (resultConfirm)
            {
                await CampaignsAppService.DeleteAsync(input.Campaign.Id);
                await DoSearch();
            }
        }

        private async Task SendEmail(Guid campaignId)
        {
            await CampaignsAppService.SendCampaignEmail(campaignId);
            await Message.Success(L["Message.SendingEmail"]);
        }

        private async Task CreateCampaignAsync()
        {
            //Pre Check exist
            if (NewCampaign.Name.IsNullOrEmpty())
            {
                await Message.Error(L["Campaign:NameIsRequired"]);
                return;
            }

            var campaigns = await CampaignsAppService.GetListAsync(new GetCampaignsInput {Code = NewCampaign.Code});
            if (campaigns.Items.IsNotNullOrEmpty())
            {
                await Message.Error(L["CampaignExist"]);
            }
            else
            {
                var success = await Invoke
                (
                    async () =>
                    {
                        if (NewCampaign.StartDateTime >= NewCampaign.EndDateTime)
                        {
                            NewCampaign.StartDateTime = NewCampaign.StartDateTime.Value.Date;
                            NewCampaign.EndDateTime = NewCampaign.EndDateTime.Value.Date.AddFluentTimeSpan(new TimeSpan(23, 59, 59));
                        }

                        await CampaignsAppService.CreateAsync(NewCampaign);
                        await DoSearch();
                        CreateCampaignModal.Hide();
                    },
                    L,
                    true
                );
            }
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

            var success = await Invoke
            (
                async () =>
                {
                    if (EditingCampaign.StartDateTime >= EditingCampaign.EndDateTime)
                    {
                        EditingCampaign.StartDateTime = EditingCampaign.StartDateTime.Value.Date;
                        EditingCampaign.EndDateTime = EditingCampaign.EndDateTime.Value.Date.AddFluentTimeSpan(new TimeSpan(23, 59, 59));
                    }

                    await CampaignsAppService.UpdateAsync(EditingCampaignId, EditingCampaign);
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
            PartnersNullable = (await CampaignsAppService.GetPartnerLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private async Task ExportPostsAsync(Guid? campaignId)
        {
            if (campaignId != null)
            {
                var data = await CampaignsAppService.ExportCampaign((Guid) campaignId);
                if (data.IsNullOrEmpty())
                {
                    await Message.Info(L["CampaignExport.NoPost"]);
                    return;
                }

                var campaign = await CampaignsAppService.GetByIdOrCode(campaignId.Value.ToString());
                var fileName = $"CP_{campaign.Name}";
                await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(data));
            }
        }

        private void OnNewNameChanged(string name)
        {
            NewCampaign.Name = name;
            NewCampaign.Code = name.IsNullOrWhiteSpace() ? "" : name.Replace(" ", "").Trim().RemoveDiacritics().ToLower();
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
                if (startDateTimeMax != null) await OnSelectFilter();
            }
        }

        private async Task OnStartDateTimeMax_Changed(DateTime? value)
        {
            if (value != null)
            {
                startDateTimeMax = value;
                if (startDateTimeMin != null) await OnSelectFilter();
            }
        }

        private async Task OnEndDateTimeMin_Changed(DateTime? value)
        {
            if (value != null)
            {
                endDateTimeMin = value;
                if (endDateTimeMax != null) await OnSelectFilter();
            }
        }

        private async Task OnEndDateTimeMax_Changed(DateTime? value)
        {
            if (value != null)
            {
                endDateTimeMax = value;
                if (endDateTimeMin != null) await OnSelectFilter();
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

        private async Task OnCampsByCurrentMonthGridAsync(DataGridReadDataEventArgs<CampaignDto> e)
        {
            await DoSearchCampStatisticsByCurrentMonth();
        }
        
        
        private async Task DoSearchCampStatisticsByCurrentMonth()
        {
            var (fromTime, endTime) = await GetBrowserDateTimeCurrentMonth();
            campKpiList = await CampaignsAppService.GetCampsByTime(fromTime.Value, endTime.Value);
        }
        

        private void ConfigChannelGroupChart()
        {
            _configPostGroupChart = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Campaign.PostByGroup"].Value,
                        FontSize = 20,
                        FontColor = _chartFontTitleColor,
                    },
                    Legend = new Legend()
                    {
                        Position = Position.Bottom
                    }
                }
                    
            };
        }

        private void ConfigEngagementChart()
        {
            _configReactionGroupChart = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = L["Campaign.PostEngagementByGroup"].Value,
                        FontSize = 20,
                        FontColor = _chartFontTitleColor,
                    },
                    Legend = new Legend()
                    {
                        Position = Position.Bottom
                    }
                },
            };
        }

        private async Task GetAuthorStatistics()
        {
            AuthorStatistics = await CampaignsAppService.GetAuthorStatistic();
        }

        private void OnSelectedTabChanged(string name)
        {
            campaignTabName = name;
        }

        private string GetPercentage(decimal value)
        {
            return value < 20 ? string.Empty : value > 100 ? "100%" :  $"{value.ToString(CultureInfo.InvariantCulture)}%";
        }
    }
}