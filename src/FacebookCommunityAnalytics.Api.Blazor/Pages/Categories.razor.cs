using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Categories
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<CategoryDto> CategoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateCategory { get; set; }
        private bool CanEditCategory { get; set; }
        private bool CanDeleteCategory { get; set; }
        private CategoryCreateDto NewCategory { get; set; }
        private Validations NewCategoryValidations { get; set; }
        private CategoryUpdateDto EditingCategory { get; set; }
        private Validations EditingCategoryValidations { get; set; }
        private Guid EditingCategoryId { get; set; }
        private Modal CreateCategoryModal { get; set; }
        private Modal EditCategoryModal { get; set; }
        private GetCategoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<CategoryDto> EntityActionsColumn { get; set; }

        public Categories()
        {
            NewCategory = new CategoryCreateDto();
            EditingCategory = new CategoryUpdateDto();
            Filter = new GetCategoriesInput
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
            await InitPage($"GDL - {L["Categories.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Categories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewCategory"],
                () =>
                {
                    OpenCreateCategoryModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Categories.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCategory = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Categories.Create);
            CanEditCategory = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Categories.Edit);
            CanDeleteCategory = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Categories.Delete);
        }

        private async Task GetCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CategoriesAppService.GetListAsync(Filter);
            CategoryList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CategoryDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateCategoryModal()
        {
            NewCategory = new CategoryCreateDto { };
            NewCategoryValidations.ClearAll();
            CreateCategoryModal.Show();
        }

        private void CloseCreateCategoryModal()
        {
            CreateCategoryModal.Hide();
        }

        private void OpenEditCategoryModal(CategoryDto input)
        {
            EditingCategoryId = input.Id;
            EditingCategory = ObjectMapper.Map<CategoryDto, CategoryUpdateDto>(input);
            EditingCategoryValidations.ClearAll();
            EditCategoryModal.Show();
        }

        private async Task DeleteCategoryAsync(CategoryDto input)
        {
            await CategoriesAppService.DeleteAsync(input.Id);
            await GetCategoriesAsync();
        }

        private async Task CreateCategoryAsync()
        {
            if (!NewCategoryValidations.ValidateAll()) return;

            await CategoriesAppService.CreateAsync(NewCategory);
            await GetCategoriesAsync();
            CreateCategoryModal.Hide();
        }

        private void CloseEditCategoryModal()
        {
            EditCategoryModal.Hide();
        }

        private async Task UpdateCategoryAsync()
        {
            if (!EditingCategoryValidations.ValidateAll()) return;

            await CategoriesAppService.UpdateAsync(EditingCategoryId, EditingCategory);
            await GetCategoriesAsync();
            EditCategoryModal.Hide();
        }
    }
}