using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.UserCompensations
{
    public partial class UserBonusConfigs : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        protected GetUserBonusConfigsInput Filter;
        private int PageSize { get; set; } = 1000;
        private int CurrentPage { get; set; } = 1;
        private int TotalCount { get; set; }
        private IReadOnlyList<UserBonusConfigDto> UserBonusConfigsDtos = new List<UserBonusConfigDto>();
        private DataGridEntityActionsColumn<UserBonusConfigDto> EntityActionsColumn { get; set; }
        
        private bool CanEdit { get; set; }
        private bool CanCreate { get; set; }
        
        
        private Modal CreateModal { get; set; }
        private Modal EditModal { get; set; }

        private CreateUpdateUserBonusConfigDto CreateUserBonusConfig { get; set; }
        private CreateUpdateUserBonusConfigDto EditUserBonusConfig { get; set; }

        private Guid EditUserBonusId { get; set; }
        public UserBonusConfigs()
        {
            CreateUserBonusConfig = new CreateUpdateUserBonusConfigDto();
            EditUserBonusConfig = new CreateUpdateUserBonusConfigDto();
            Filter = new GetUserBonusConfigsInput()
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize
            };
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
        }
        private async Task SetPermissionsAsync()
        {
            CanCreate = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.UserBonusConfigs.Create);
            CanEdit = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.UserBonusConfigs.Edit);
        }
        private async Task GetUserBonusConfigsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;

            var result = await UserBonusConfigAppService.GetListAsync(Filter);
            UserBonusConfigsDtos = result.Items;
            TotalCount = (int) result.TotalCount;
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["UserBonusConfigs.PageTitle"].Value}");
        } 
        
        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:UserBonusConfigs"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["CreateBonus"],
                () =>
                {
                    OpenCreateModal();
                   return Task.CompletedTask;
                },
                requiredPolicyName: ApiPermissions.UserCompensations.Create
            );

            return ValueTask.CompletedTask;
        }
        
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserBonusConfigDto> e)
        {
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            
            await GetUserBonusConfigsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task CreateUserBonusConfigAsync()
        {
            await UserBonusConfigAppService.CreateAsync(CreateUserBonusConfig);
            await GetUserBonusConfigsAsync();
            
            CreateModal.Hide();
        }
        private async Task UpdateUserBonusConfigAsync()
        {
            await UserBonusConfigAppService.UpdateAsync(EditUserBonusId,EditUserBonusConfig);
            await GetUserBonusConfigsAsync();
            
            CloseEditModal();
        }
        private void CloseCreateModal()
        {
            CreateModal.Hide();
        }
        private void CloseEditModal()
        {
            EditModal.Hide();
        }

        private void OpenCreateModal()
        {
            CreateModal.Show();
        }
        private void OpenEditModal(UserBonusConfigDto item)
        {
            EditUserBonusConfig = ObjectMapper.Map<UserBonusConfigDto,CreateUpdateUserBonusConfigDto>(item);
            EditUserBonusId = item.Id;
            EditModal.Show();
        }
    }
}