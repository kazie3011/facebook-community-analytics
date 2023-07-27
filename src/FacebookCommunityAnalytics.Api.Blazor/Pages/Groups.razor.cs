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
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Groups : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<GroupDto> GroupList { get; set; }
        private int PageSize { get; set; } = 50;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateGroup { get; set; }
        private bool CanEditGroup { get; set; }
        private bool CanDeleteGroup { get; set; }
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
        private IReadOnlyList<LookupDto<Guid>> StaffUsersLookupDtos { get; set; } = new List<LookupDto<Guid>>();

        private IEnumerable<Guid> CurrentPartUserIds = new List<Guid>();
        private IEnumerable<Guid> ModeratorUserIds = new List<Guid>();

        private string selectedTab = "info";

        public Groups()
        {
            NewGroup = new GroupCreateDto {IsActive = true};
            EditingGroup = new GroupUpdateDto();
            Filter = new GetGroupsInput
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
            await GetStaffUsersLookupAsync();
            Filter.IsActive = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["Groups.PageTitle"].Value}");
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
                requiredPolicyName: ApiPermissions.Groups.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateGroup = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Groups.Create);
            CanEditGroup = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Groups.Edit);
            CanDeleteGroup = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Groups.Delete);
        }

        private async Task GetGroupsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            OnGroupSourceTypeFilterSelectedValueChanged(_groupSourceTypeFilter);
            OnGroupOwnershipTypeFilterSelectedValueChanged(_groupOwnershipTypeFilter);

            var result = await GroupsAppService.GetListAsync(Filter);
            GroupList = result.Items;
            TotalCount = (int) result.TotalCount;
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
            NewGroup = new GroupCreateDto {IsActive = true};
            ModeratorUserIds = new List<Guid>();
            CreateGroupModal.Show();
        }

        private void CloseCreateGroupModal()
        {
            CreateGroupModal.Hide();
        }

        private void OpenEditGroupModal(GroupDto input)
        {
            EditingGroupId = input.Id;
            ModeratorUserIds = new List<Guid>();
            EditingGroup = ObjectMapper.Map<GroupDto, GroupUpdateDto>(input);
            if (EditingGroup.ModeratorIds != null) ModeratorUserIds = EditingGroup.ModeratorIds;

            EditGroupModal.Show();
        }

        private async Task DeleteGroupAsync(GroupDto input)
        {
            await GroupsAppService.DeleteAsync(input.Id);
            await GetGroupsAsync();
        }

        private async Task CreateGroupAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await GroupsAppService.CreateAsync(NewGroup);
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
                    await GroupsAppService.UpdateAsync(EditingGroupId, EditingGroup);
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
            PartnerUsersLookupDtos = (await GroupsAppService.GetPartnerUserLookupAsync(new LookupRequestDto {MaxResultCount = 1000}));
        }

        private Task OnSelectedTabChanged(string name)
        {
            selectedTab = name;

            return Task.CompletedTask;
        }

        private async Task GetStaffUsersLookupAsync()
        {
            StaffUsersLookupDtos = (await GroupsAppService.GetStaffUserLookupAsync(new LookupRequestDto {MaxResultCount = 1000}));
        }

        private Task EditModeratorIdsSelectedValuesChanged(object value)
        {
            if (value != null)
            {
                var ids = value is IEnumerable<Guid> guids ? guids.ToList() : new List<Guid> {(Guid) value};
                EditingGroup.ModeratorIds = ids;
            }
            else
            {
                EditingGroup.ModeratorIds = new List<Guid>();
            }

            return Task.CompletedTask;
        }

        private Task CreateModeratorIdsSelectedValuesChanged(object value)
        {
            if (value != null)
            {
                var ids = value is IEnumerable<Guid> guids ? guids.ToList() : new List<Guid> {(Guid) value};
                NewGroup.ModeratorIds = ids;
            }
            else
            {
                NewGroup.ModeratorIds = new List<Guid>();
            }

            return Task.CompletedTask;
        }
    }
}