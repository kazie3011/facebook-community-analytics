using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Microsoft.JSInterop;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.UserCompensations
{
    public partial class UserCompensations : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();

        protected GetUserCompensationsInput Filter = new();
        private int PageSize { get; set; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = UserCompensationConsts.GetDefaultSorting(true);
        private int TotalCount { get; set; }
        private IReadOnlyList<UserCompensationNavigationPropertiesDto> UserCompensationDtos = new List<UserCompensationNavigationPropertiesDto>();
        
        public UserCompensations()
        {
            // NewUserInfo = new UserInfoCreateDto();
            // EditingUserInfo = new UserInfoUpdateDto();
            Filter = new GetUserCompensationsInput()
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            //await SetPermissionsAsync();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            //await GetNullableAppUserLookupAsync("");
        }

        private async Task GetUserCompensationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserCompensationAppService.GetListWithNavigationAsync(Filter);
            UserCompensationDtos = result.Items;
            TotalCount = (int) result.TotalCount;
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["UserCompensations.PageTitle"].Value}");
        } 
        
        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:UserCompensations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            //Toolbar.AddButton(L["NewUserInfo"], () =>
            //{
            //    OpenCreateUserInfoModal();
            //    return Task.CompletedTask;
            //}, IconName.Add, requiredPolicyName: ApiPermissions.UserInfos.Create);

            Toolbar.AddButton
            (
                L["SyncUserCompensations"],
                async () =>
                {
                    //TODO Sync UserCompensation
                    await GetUserCompensationsAsync();
                },
                IconName.Sync,
                requiredPolicyName: ApiPermissions.UserCompensations.Create
            );

            return ValueTask.CompletedTask;
        }
        
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserCompensationNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            
            await GetUserCompensationsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
}