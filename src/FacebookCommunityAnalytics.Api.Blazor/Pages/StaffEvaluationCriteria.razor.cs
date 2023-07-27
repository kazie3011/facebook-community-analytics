using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class StaffEvaluationCriteria
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; set; } = new();
        private IReadOnlyList<StaffEvaluationCriteriaDto> StaffEvaluationCriteriaDtos { get; set; }
        private GetStaffEvaluationCriteriaInput Filter { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateStaffEvaluationCriteria { get; set; }
        private bool CanEditStaffEvaluationCriteria { get; set; }
        private bool CanDeleteStaffEvaluationCriteria { get; set; }
        private Modal CreateStaffEvaluationCriteriaModal { get; set; }
        private Modal EditStaffEvaluationCriteriaModal { get; set; }
        private CreateUpdateStaffEvaluationCriteriaDto NewStaffEvaluationCriteria { get; set; }
        private CreateUpdateStaffEvaluationCriteriaDto EditingStaffEvaluationCriteria { get; set; }
        private Guid EditingStaffEvaluationCriteriaId { get; set; }
        private DataGridEntityActionsColumn<StaffEvaluationCriteriaDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<OrganizationUnitDto> Teams { get; set; }

        public StaffEvaluationCriteria()
        {
            Filter = new GetStaffEvaluationCriteriaInput();
            Teams = new List<OrganizationUnitDto>();
            NewStaffEvaluationCriteria = new CreateUpdateStaffEvaluationCriteriaDto();
            EditingStaffEvaluationCriteria = new CreateUpdateStaffEvaluationCriteriaDto();
            StaffEvaluationCriteriaDtos = new List<StaffEvaluationCriteriaDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            Teams = await StaffEvaluationCriteriaAppService.GetTeams();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["StaffEvaluationCriteria.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:HR"]));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:StaffEvaluationCriteria"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewStaffEvaluationCriteria"],
                () =>
                {
                    OpenCreateStaffEvaluationCriteriaModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.StaffEvaluations.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateStaffEvaluationCriteria = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.StaffEvaluations.Create);
            CanEditStaffEvaluationCriteria = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.StaffEvaluations.Edit);
            CanDeleteStaffEvaluationCriteria = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.StaffEvaluations.Delete);
        }

        private async Task GetStaffEvaluationCriteriaAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            if (Filter.TeamId == Guid.Empty)
            {
                Filter.TeamId = null;
            }

            var result = await StaffEvaluationCriteriaAppService.GetListExtendAsync(Filter);
            StaffEvaluationCriteriaDtos = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<StaffEvaluationCriteriaDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetStaffEvaluationCriteriaAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateStaffEvaluationCriteriaModal()
        {
            CreateStaffEvaluationCriteriaModal.Show();
        }

        private void OpenEditStaffEvaluationModal(StaffEvaluationCriteriaDto input)
        {
            EditingStaffEvaluationCriteriaId = input.Id;
            EditingStaffEvaluationCriteria = ObjectMapper.Map<StaffEvaluationCriteriaDto, CreateUpdateStaffEvaluationCriteriaDto>(input);
            EditStaffEvaluationCriteriaModal.Show();
        }

        private async Task DeleteStaffEvaluation(StaffEvaluationCriteriaDto input)
        {
            await StaffEvaluationCriteriaAppService.DeleteAsync(input.Id);
        }

        private async Task CreateStaffEvaluationCriteriaAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await StaffEvaluationCriteriaAppService.CreateAsync(NewStaffEvaluationCriteria);
                    await GetStaffEvaluationCriteriaAsync();
                    CreateStaffEvaluationCriteriaModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Create
            );
        }

        private async Task UpdateStaffEvaluationCriteriaAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await StaffEvaluationCriteriaAppService.UpdateAsync(EditingStaffEvaluationCriteriaId, EditingStaffEvaluationCriteria);
                    await GetStaffEvaluationCriteriaAsync();
                    EditStaffEvaluationCriteriaModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private void CloseCreateStaffEvaluationCriteriaModal()
        {
            CreateStaffEvaluationCriteriaModal.Hide();
            NewStaffEvaluationCriteria = new CreateUpdateStaffEvaluationCriteriaDto();
        }

        private void CloseEditStaffEvaluationCriteriaModal()
        {
            EditStaffEvaluationCriteriaModal.Hide();
            EditingStaffEvaluationCriteria = new CreateUpdateStaffEvaluationCriteriaDto();
        }

        private void OnSelectedEvaluationType(EvaluationType? value)
        {
            Filter.EvaluationType = value == EvaluationType.FilterNoSelect ? null : value;
        }
    }
}