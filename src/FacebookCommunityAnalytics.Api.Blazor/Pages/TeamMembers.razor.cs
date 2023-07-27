using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Organizations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp.Identity;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Toastr;
using Volo.Abp.BlazoriseUI.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class TeamMembers
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<OrganizationUnitDto> Teams { get; set; }
        private DataGrid<TeamMemberDto> _dataGrid { get; set; }
        private IReadOnlyList<TeamMemberDto> Members { get; set; }
        private Guid CurrentTeamId { get; set; }
        private GetMembersApiRequest Filter { get; set; }
        private bool CanCreateTeam { get; set; }
        private bool CanEditTeam { get; set; }
        private bool CanDeleteTeam { get; set; }
        private bool _unRendered = true;
        private List<TeamMemberDto> SelectedMembers { get; set; }
        private bool? IsSystemUsers { get; set; }
        private bool? IsGDLUser { get; set; }
        private bool? IsCalculatePayrollUsers { get; set; }
        private bool? IsActiveUsers { get; set; }
        private Guid? NullableGuid { get; }
        private Guid EditingUserInfoId { get; set; }
        private Validations EditingUserInfoValidations { get; set; }
        private UserInfoUpdateDto EditingUserInfo { get; set; }
        private Modal UserInfoModal { get; set; }
        private Modal UserMemberEvalConfigModal { get; set; }

        private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private string _selectedTab = "TabSale";
        private bool _showLoading { get; set; }
        private UserEvaluationConfigurationDto _evalConfigDto = new();
        private string _selectedContentConfigTab = "TabContentTeam";
        private Guid MemberAppUserId { get; set; }
        private string MemberName { get; set; }
        private string GroupName { get; set; }

        public TeamMembers()
        {
            Filter = new GetMembersApiRequest();
            Teams = new List<OrganizationUnitDto>();
            SelectedMembers = new List<TeamMemberDto>();
            Members = new List<TeamMemberDto>();
            EditingUserInfo = new UserInfoUpdateDto();
        }
        
        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetNullableAppUserLookupAsync("");
            
            if (IsManagerRole())
            {
                Teams = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest());
            }
            else
            {
                Teams = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest()
                {
                    UserId = CurrentUser.Id.GetValueOrDefault()
                });
                if (Teams.Any())
                {
                    var teamId = Teams.FirstOrDefault()?.Id;
                    Filter.TeamId = teamId.GetValueOrDefault();
                }
            }
            await Search();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["TeamMember.PageTitle"].Value}");
                _unRendered = false;
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Teams"]));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:TeamMembers"]));
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateTeam = await AuthorizationService.IsGrantedAsync(ApiPermissions.TeamMembers.Create);
            CanEditTeam = await AuthorizationService.IsGrantedAsync(ApiPermissions.TeamMembers.Edit);
            CanDeleteTeam = await AuthorizationService.IsGrantedAsync(ApiPermissions.TeamMembers.Delete);
        }

        private async Task Search()
        {
            Members = await TeamMemberAppService.GetMembers(Filter);
        }

        private async Task AssignTeam(bool isTeamAssigned)
        {
            if (SelectedMembers.IsNullOrEmpty())
            {
                await Message.Error(L["NoRowSelected"]);
                return;
            }

            var success = await Invoke
            (
                async () =>
                {
                    await TeamMemberAppService.AssignTeam
                    (
                        new AssignTeamApiRequest
                        {
                            UserIds = SelectedMembers.Select(_ => _.Id).ToList(),
                            OrganizationUnitId = CurrentTeamId,
                            IsTeamAssigned = isTeamAssigned
                        }
                    );
                    await Search();

                    _dataGrid.SelectedRow = null;
                    _dataGrid.SelectedRows.Clear();
                    await _dataGrid.Reload();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private async Task ClearSelectedUsers()
        {
            await JSRuntime.InvokeVoidAsync("RemoveRowSelected");

            Filter.TeamId = null;
            CurrentTeamId = Guid.Empty;

            await Search();
            _dataGrid.SelectedRow = null;
            _dataGrid.SelectedRows.Clear();
            await _dataGrid.Reload();
        }

        private string GetTeamName(List<Guid> teamIds)
        {
            var orgNames = Teams.Where(_ => _.Id.IsIn(teamIds)).Select(x => x.DisplayName).ToList();
            return string.Join(',', orgNames);
        }

        private async Task ChangeAccountType()
        {
            if (SelectedMembers.IsNullOrEmpty())
            {
                await Message.Error(L["NoRowSelected"]);
                return;
            }

            if (IsSystemUsers == null && IsGDLUser == null && IsCalculatePayrollUsers == null && IsActiveUsers == null)
            {
                await Message.Error(L["NoTypeSelect"]);
                return;
            }

            var success = await Invoke
            (
                async () =>
                {
                    await TeamMemberAppService.UpdateMemberConfig
                    (
                        new UpdateMemberConfigApiRequest()
                        {
                            UserIds = SelectedMembers.Select(_ => _.Id).ToList(),
                            IsSystemUsers = IsSystemUsers,
                            IsGDLUser = IsGDLUser,
                            IsCalculatePayrollUsers = IsCalculatePayrollUsers,
                            IsActiveUsers = IsActiveUsers
                        }
                    );
                    await Search();

                    ClearUserProperty();

                    _dataGrid.SelectedRow = null;
                    _dataGrid.SelectedRows.Clear();
                    await _dataGrid.Reload();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private void ClearUserProperty()
        {
            IsSystemUsers = null;
            IsGDLUser = null;
            IsCalculatePayrollUsers = null;
            IsActiveUsers = null;
        }

        private void SystemPropertyChanged(bool? value)
        {
            IsSystemUsers = value;
        }

        private void IsGDLPropertyChanged(bool? value)
        {
            IsGDLUser = value;
        }

        private void CalculatePayrollPropertyChanged(bool? value)
        {
            IsCalculatePayrollUsers = value;
        }

        private void ActivePropertyChanged(bool? value)
        {
            IsActiveUsers = value;
        }

        private async Task OpenEditUserInfoModal(Guid userid)
        {
            var userInfo = await UserInfosAppService.GetByUserIdAsync(userid);
            EditingUserInfo = ObjectMapper.Map<UserInfoDto, UserInfoUpdateDto>(userInfo.UserInfo);
            EditingUserInfo.AppUser = userInfo.AppUser;
            EditingUserInfoValidations.ClearAll();
            UserInfoModal.Show();
        }

        private async Task GetNullableAppUserLookupAsync(string newValue)
        {
            UsersNullable = (await UserInfosAppService.GetAppUserLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private void CloseEditUserInfoModal()
        {
            UserInfoModal.Hide();
        }

        private async Task OnTeamFilter_Changed(Guid? TeamId)
        {
            Filter.TeamId = TeamId;

            await Search();
        }
        
        private async Task OnActiveFilter_Checked(bool value)
        {
            Filter.IsActiveUser = value;

            await Search();
        }

        public async Task TryGetTeamMembersByFilter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await Search();
            }
        }
    }
}
