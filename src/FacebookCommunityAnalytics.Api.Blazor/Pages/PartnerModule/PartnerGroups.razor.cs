using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule
{
    public partial class PartnerGroups : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<GroupDto> GroupList { get; set; }
        private int PageSize { get; set; } = 50;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private GroupCreateDto NewGroup { get; set; }
        private GroupUpdateDto EditingGroup { get; set; }
        private Guid EditingGroupId { get; set; }
        private Modal CreateGroupModal { get; set; }
        private Modal EditGroupModal { get; set; }
        private GetGroupsInput Filter { get; set; }
        private DataGridEntityActionsColumn<GroupDto> EntityActionsColumn { get; set; }

        private GroupSourceTypeFilter _groupSourceTypeFilter { get; set; } = GroupSourceTypeFilter.NoSelect;
        private GroupOwnershipTypeFilter _groupOwnershipTypeFilter { get; set; } = GroupOwnershipTypeFilter.NoSelect;
        
        private IReadOnlyList<LookupDto<Guid>> PartnerUsersLookupDtos { get; set; } = new List<LookupDto<Guid>>();

        private IEnumerable<Guid> CurrentPartUserIds = new List<Guid>() ;

        public PartnerGroups()
        {
            NewGroup = new GroupCreateDto { IsActive = true };
            EditingGroup = new GroupUpdateDto();
            Filter = new GetGroupsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting,
                IsActive = true
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await GetPartnerUsersLookupAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["Groups.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Groups"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewGroup"],
                () =>
                {
                    OpenCreateGroupModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.PartnerModule.Default
            );

            return ValueTask.CompletedTask;
        }


        private async Task GetGroupsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            OnGroupSourceTypeFilterSelectedValueChanged(_groupSourceTypeFilter);
            OnGroupOwnershipTypeFilterSelectedValueChanged(_groupOwnershipTypeFilter);

            var result = await _partnerModuleAppService.GetGroups(Filter);
            GroupList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetGroupsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<GroupDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await GetGroupsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateGroupModal()
        {
            NewGroup = new GroupCreateDto { IsActive = true };
            CreateGroupModal.Show();
        }
        
        private void CloseCreateGroupModal()
        {
            CreateGroupModal.Hide();
        }
        
        private void OpenEditGroupModal(GroupDto input)
        {
            EditingGroupId = input.Id;
            EditingGroup = ObjectMapper.Map<GroupDto, GroupUpdateDto>(input);
            
            EditGroupModal.Show();
        }
        
        private async Task CreateGroupAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await _partnerModuleAppService.CreateGroup(NewGroup);
                    await GetGroupsAsync();
                    CreateGroupModal.Hide();
                },
                L,
                true
            );
        }
        
        private void CloseEditGroupModal()
        {
            EditGroupModal.Hide();
        }
        
        private async Task UpdateGroupAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await _partnerModuleAppService.UpdateGroupAsync(EditingGroupId, EditingGroup);
                    await GetGroupsAsync();
                    EditGroupModal.Hide();
                },
                L,
                true
            );
        }

        private void OnGroupOwnershipTypeFilterSelectedValueChanged(GroupOwnershipTypeFilter value)
        {
            if (value == GroupOwnershipTypeFilter.NoSelect)
            {
                Filter.GroupOwnershipType = null;
            }
            else
            {
                Filter.GroupOwnershipType = Enum.Parse<GroupOwnershipType>(value.ToString());
            }
        }

        private void OnGroupSourceTypeFilterSelectedValueChanged(GroupSourceTypeFilter value)
        {
            if (value == GroupSourceTypeFilter.NoSelect)
            {
                Filter.GroupSourceType = null;
            }
            else
            {
                Filter.GroupSourceType = Enum.Parse<GroupSourceType>(value.ToString());
            }
        }
        
        private async Task GetPartnerUsersLookupAsync()
        {
            PartnerUsersLookupDtos = await  _partnerModuleAppService.GetPartnerUserLookup(new LookupRequestDto
            {
                MaxResultCount = 1000
            });
        }
        
    }
}