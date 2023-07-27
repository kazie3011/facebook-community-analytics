using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.CmsSites;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.Cms
{
    public partial class Sites
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new();
        private IReadOnlyList<CmsSiteDto> CmsSites { get; set; }
        private int PageSize { get; set; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        
        private Modal CreateModal { get; set; }
        private Modal EditModal { get; set; }
        private GetCmsSitesInputDto Filter { get; set; }

        
        private DataGridEntityActionsColumn<CmsSiteDto> EntityActionsColumn { get; set; }
        private CreateUpdateCmsSiteDto CreateUpdateCmsSiteDto { get; set; }
        
        private Guid EditingId { get; set; }
        private bool CanCreate { get; set; }
        private bool CanEdit { get; set; }
        private bool CanDelete { get; set; }
        public Sites()
        {
            CreateUpdateCmsSiteDto = new CreateUpdateCmsSiteDto()
            {
            };
            Filter = new GetCmsSitesInputDto
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
            await JsRuntime.InvokeVoidAsync("HiddenMenuOnMobile");
        }
        
        private async Task SetPermissionsAsync()
        {
            CanCreate = await AuthorizationService
                .IsGrantedAsync(CmsPermissions.Sites.Create);
            CanEdit = await AuthorizationService
                .IsGrantedAsync(CmsPermissions.Sites.Edit);
            CanDelete = await AuthorizationService
                .IsGrantedAsync(CmsPermissions.Sites.Delete);
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:CmsSites"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["NewCmsSite"], () =>
            {
                OpenCreateModal();
                return Task.CompletedTask;
            }, IconName.Add, requiredPolicyName: CmsPermissions.Sites.Create);

            return ValueTask.CompletedTask;
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CmsSiteDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await GetCmsSitesAsync();
            await InvokeAsync(StateHasChanged);
        }
        private async Task GetCmsSitesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CmsSiteAppService.GetListAsync(Filter);
            CmsSites = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        
        private void OpenCreateModal()
        {
            CreateUpdateCmsSiteDto = new CreateUpdateCmsSiteDto();
            CreateModal.Show();
        }

        private void CloseCreateModal()
        {
            CreateModal.Hide();
        }
        
        private async Task OpenEditModal(CmsSiteDto e)
        {
            EditingId = e.Id;
            
            var cmsSiteDto = await CmsSiteAppService.GetAsync(e.Id);
            if (cmsSiteDto != null)
            {
                CreateUpdateCmsSiteDto = ObjectMapper.Map<CmsSiteDto, CreateUpdateCmsSiteDto>(cmsSiteDto);
            }
            EditModal.Show();
        }
        private void CloseEditModal()
        {
            EditModal.Hide();
        }
        private async Task CreateAsync()
        {
            await CmsSiteAppService.CreateAsync(CreateUpdateCmsSiteDto);
            await GetCmsSitesAsync();
            CreateModal.Hide();
        }

        private async Task UpdateAsync()
        {
            await CmsSiteAppService.UpdateAsync(EditingId,CreateUpdateCmsSiteDto);
            await GetCmsSitesAsync();
            EditModal.Hide();
        }
        
        public async Task DeleteAsync(CmsSiteDto e)
        {
            await CmsSiteAppService.DeleteAsync(e.Id);
            await GetCmsSitesAsync();
        }
        
    }
}