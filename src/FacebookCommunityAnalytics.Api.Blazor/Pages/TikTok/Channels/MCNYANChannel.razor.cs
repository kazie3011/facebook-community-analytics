using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Channels
{
    public partial class MCNYANChannel
    {
        private GetGroupsInput Filter { get; set; }
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        public PageToolbar Toolbar { get; } = new();
        private GroupOwnershipTypeFilter _ownershipFilter { get; set; } = GroupOwnershipTypeFilter.NoSelect;
        private GroupCategoryType _categoryFilter { get; set; } = GroupCategoryType.FilterNoSelect;
        private DataGridEntityActionsColumn<GroupDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<LookupDto<Guid>> StaffUsersLookupDtos { get; set; } = new List<LookupDto<Guid>>();
        private IEnumerable<Guid> ModeratorUserIds = new List<Guid>();
        private GroupUpdateDto EditingGroup { get; set; }
        private IReadOnlyList<GroupDto> GroupList { get; set; }
        private Modal EditChannelModal { get; set; }
        private Guid EditingGroupId { get; set; }

        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 50;
        private string selectedTab = "info";

        Validations editingValidations;

        public MCNYANChannel()
        {
            EditingGroup = new GroupUpdateDto();
            Filter = new GetGroupsInput()
            {
                IsActive = true,
                GroupSourceType = GroupSourceType.Tiktok,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetStaffUsersLookupAsync();
            }
        }

        private async Task GetStaffUsersLookupAsync()
        {
            StaffUsersLookupDtos = (await GroupsAppService.GetStaffUserLookupAsync(new LookupRequestDto {MaxResultCount = 1000}));
        }

        public async Task DoSearch()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.TikTokMcnType = TikTokMCNType.MCNGdl;
            Filter.GroupOwnershipType = GroupOwnershipType.YAN;
            var result = await GroupsExtendAppService.GetListTikTokAsync(Filter);
            GroupList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        // private async Task OnChanged_Ownership(GroupOwnershipTypeFilter value)
        // {
        //     _ownershipFilter = value;
        //     if (value == GroupOwnershipTypeFilter.NoSelect)
        //     {
        //         Filter.GroupOwnershipType = null;
        //     }
        //     else
        //     {
        //         Filter.GroupOwnershipType = Enum.Parse<GroupOwnershipType>(value.ToString());
        //     }
        //
        //     await DoSearch();
        // }
        private async Task OnChanged_Category(GroupCategoryType value)
        {
            _categoryFilter = value;   
            if (value == GroupCategoryType.FilterNoSelect)
            {
                Filter.GroupCategoryType = null;
            }
            else
            {
                Filter.GroupCategoryType = Enum.Parse<GroupCategoryType>(value.ToString());
            }
            
            await DoSearch();
        }

        private async Task OnChecked_Active(bool value)
        {
            Filter.IsActive = value;
            await DoSearch();
        }
        
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<GroupDto> e)
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

        void OpenEditGroupModal(GroupDto input)
        {
            EditingGroupId = input.Id;
            ModeratorUserIds = new List<Guid>();
            EditingGroup = ObjectMapper.Map<GroupDto, GroupUpdateDto>(input);
            if (EditingGroup.ModeratorIds != null) ModeratorUserIds = EditingGroup.ModeratorIds;
            EditChannelModal.Show();
        }

        private async Task DeleteGroupAsync(GroupDto input)
        {
            var resultConfirm = await UiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (resultConfirm)
            {
                await GroupsAppService.DeleteAsync(input.Id);
                await DoSearch();
            }
        }
        private async Task DeactivatedGroupAsync(GroupDto input)
        {
            var resultConfirm = await UiMessageService.Confirm(L["DeactivatedConfirmationMessage"]);
            if (resultConfirm)
            {
                await GroupsAppService.DeactivatedAsync(input.Id);
                await DoSearch();
            }
        }
        
        private async Task ActivatedGroupAsync(GroupDto input)
        {
            var resultConfirm = await UiMessageService.Confirm(L["ActivatedConfirmationMessage"]);
            if (resultConfirm)
            {
                await GroupsAppService.ActivatedAsync(input.Id);
                await DoSearch();
            }
        }

        private Task OnSelectedTabChanged(string name)
        {
            selectedTab = name;
            return Task.CompletedTask;
        }

        private void CloseEditGroupModal()
        {
            EditChannelModal.Hide();
        }

        private async Task UpdateChannel()
        {
            var success = await Invoke
            (
                async () =>
                {
                    if (editingValidations.ValidateAll())
                    {
                        EditingGroup.Name = EditingGroup.Fid;
                        await GroupsAppService.UpdateAsync(EditingGroupId, EditingGroup);
                        await DoSearch();
                        EditChannelModal.Hide();
                    }
                },
                L,
                true
            );
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
    }
}