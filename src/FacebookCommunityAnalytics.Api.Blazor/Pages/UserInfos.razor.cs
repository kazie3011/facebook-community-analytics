using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Organizations;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Microsoft.JSInterop;
using Volo.Abp.Http.Client;
using Volo.Abp.Identity;


namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class UserInfos : BlazorComponentBase
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<UserInfoWithNavigationPropertiesDto> UserInfoList { get; set; }
        private int PageSize { get; set; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateUserInfo { get; set; }
        private bool CanEditUserInfo { get; set; }
        private bool CanDeleteUserInfo { get; set; }
        private UserInfoCreateDto NewUserInfo { get; set; }
        private UserInfoUpdateDto EditingUserInfo { get; set; }
        private Guid EditingUserInfoId { get; set; }
        private Modal CreateUserInfoModal { get; set; }
        private Modal EditUserInfoModal { get; set; }
        private Modal UserInfoAccountsModal { get; set; }

        private IReadOnlyList<OrganizationUnitDto> OrganizationUnits { get; set; }
        
        private bool _showLoading;
        private GetExtendUserInfosInput Filter { get; set; }
        private DataGridEntityActionsColumn<UserInfoWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
        private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
        private UserInfoAccount _userInfoAccount = new();
        private List<OrganizationUnitDto> _teams = new();
        
        private DateTime? createUserDateOfBirth;
        private DateTime? createUserJoinedDateTime;
        private DateTime? createUserPromotedDateTime;
        private DateTime? editUserDateOfBirth;
        private DateTime? editUserJoinedDateTime;
        private DateTime? editUserPromotedDateTime;
        
        private bool? IsSystemUsers { get; set; }
        private bool? IsGDLUser { get; set; }
        private bool? IsCalculatePayrollUsers { get; set; }
        private bool? IsActiveUsers { get; set; }
        private bool? HasMainTeam { get; set; }
        private UserPosition? _userPosition;
        
        public UserInfos()
        {
            OrganizationUnits = new List<OrganizationUnitDto>();
            NewUserInfo = new UserInfoCreateDto();
            EditingUserInfo = new UserInfoUpdateDto();

            Filter = new GetExtendUserInfosInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = null
            };
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetPermissionsAsync();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await GetNullableAppUserLookupAsync("");

            OrganizationUnits = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage($"GDL - {L["UserInfos.PageTitle"].Value}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:HRUserInfos"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["SyncUserInfo"],
                async () =>
                {
                    await UserInfosAppService.SyncUserInfos();
                    await GetUserInfosAsync();
                },
                requiredPolicyName: ApiPermissions.UserInfos.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserInfo = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserInfos.Create);
            CanEditUserInfo = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserInfos.Edit);
            CanDeleteUserInfo = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserInfos.Delete);
        }
        
        private async Task GetUserInfosAsync()
        {
            _showLoading = false;
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            
            Filter.IsSystemUser = IsSystemUsers;
            Filter.IsGDLStaff = IsGDLUser;
            Filter.EnablePayrollCalculation = IsCalculatePayrollUsers;
            Filter.IsActive = IsActiveUsers;
            Filter.HasMainTeam = HasMainTeam;
            
            var result = await UserInfosAppService.GetListExtendAsync(Filter);
            UserInfoList = result.Items;
            TotalCount = (int) result.TotalCount;
            _showLoading = true;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetUserInfosAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserInfoWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await GetUserInfosAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void CloseCreateUserInfoModal()
        {
            CreateUserInfoModal.Hide();
        }

        private void OpenUserInfoAccountsModal(UserInfoWithNavigationPropertiesDto input)
        {
            _userInfoAccount = new UserInfoAccount();
            EditingUserInfoId = input.UserInfo.Id;
            EditingUserInfo = ObjectMapper.Map<UserInfoDto, UserInfoUpdateDto>(input.UserInfo);
            EditingUserInfo.AppUser = input.AppUser;
            UserInfoAccountsModal.Show();
        }

        private async Task UpdateUserInfoAccountsAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await UserInfosAppService.AddSeedingAccount(EditingUserInfoId, _userInfoAccount);
                    EditingUserInfo.Accounts.Add(_userInfoAccount);
                    await GetUserInfosAsync();
                    _userInfoAccount = new UserInfoAccount();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );

            // TODOO VuN: remove later
            // var success = await Invoke
            // (
            //     async () =>
            //     {
            //         if (EditingUserInfo.Accounts.IsNotNullOrEmpty() && EditingUserInfo.Accounts.Select(_ => _.Fid).Contains(_userInfoAccount.Fid))
            //         {
            //             await Message.Error(L["DuplicateFacebookId"]);
            //             return;
            //         }
            //
            //         //Use Facebook helper to get personal facebook id from Fid
            //         //Check if Fid valid here
            //         //UserInfosAppService.SeedingAccountExisted => true/false
            //
            //         var fId = FacebookHelper.GetPersonalFacebookId(_userInfoAccount.Fid);
            //         var isSeedingAccountExisted = await UserInfosAppService.SeedingAccountExisted(fId);
            //         if (isSeedingAccountExisted)
            //         {
            //             await Message.Error(L["DuplicateFacebookId"]);
            //             return;
            //         }
            //
            //         EditingUserInfo.Accounts.Add(_userInfoAccount);
            //         await UserInfosAppService.UpdateAsync(EditingUserInfoId, EditingUserInfo);
            //         _userInfoAccount = new UserInfoAccount();
            //         await GetUserInfosAsync();
            //     },
            //     L,
            //     true,
            //     BlazorComponentBaseActionType.Update
            // );
        }

        private void CloseUserInfoAccountsModal()
        {
            UserInfoAccountsModal.Hide();
        }

        private async Task DeleteUserInfoAccountsAsync(UserInfoAccount input)
        {
            EditingUserInfo.Accounts.Remove(input);
            await UserInfosAppService.UpdateAsync(EditingUserInfoId, EditingUserInfo);
            await GetUserInfosAsync();
        }

        private async Task OpenEditUserInfoModal(UserInfoWithNavigationPropertiesDto input, List<OrganizationUnitDto> teams)
        {
            _teams = teams;
            EditingUserInfoId = input.UserInfo.Id;
            EditingUserInfo = ObjectMapper.Map<UserInfoDto, UserInfoUpdateDto>(input.UserInfo);
            editUserDateOfBirth = EditingUserInfo.DateOfBirth != null ? await ConvertUniversalToBrowserDateTime(EditingUserInfo.DateOfBirth.Value) : null;
            editUserJoinedDateTime = EditingUserInfo.JoinedDateTime != null ? await ConvertUniversalToBrowserDateTime(EditingUserInfo.JoinedDateTime.Value) : null;
            editUserPromotedDateTime = EditingUserInfo.PromotedDateTime != null ? await ConvertUniversalToBrowserDateTime(EditingUserInfo.PromotedDateTime.Value) : null;

            EditingUserInfo.AppUser = input.AppUser;
            EditUserInfoModal.Show();
        }

        private async Task DeleteUserInfoAsync(UserInfoWithNavigationPropertiesDto input)
        {   var confirmResult = await UiMessageService.Confirm(L["UserInfos.DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                var success = await Invoke
                (
                    async () =>
                    {
                        await UserInfosAppService.DeleteAsync(input.UserInfo.Id);
                        await GetUserInfosAsync();
                    },
                    L,
                    true,
                    BlazorComponentBaseActionType.Delete
                );
            }
        }

        private async Task CreateUserInfoAsync()
        {
            if (NewUserInfo.AppUserId.HasValue)
            {
                var userInfo = await UserInfosAppService.GetByUserIdAsync(NewUserInfo.AppUserId.Value);
                if (userInfo != null)
                {
                    await Message.Error(L["UserInfoExisted"]);
                    return;
                }
            }
            await UserInfosAppService.CreateAsync(NewUserInfo);
            await GetUserInfosAsync();
            CreateUserInfoModal.Hide();
        }

        private void CloseEditUserInfoModal()
        {
            _teams = new List<OrganizationUnitDto>();
            EditUserInfoModal.Hide();
        }

        private async Task UpdateUserInfoAsync()
        {
            await UserInfosAppService.UpdateAsync(EditingUserInfoId, EditingUserInfo);
            await GetUserInfosAsync();
            EditUserInfoModal.Hide();
        }

        private async Task GetNullableAppUserLookupAsync(string newValue)
        {
            UsersNullable = (await UserInfosAppService.GetAppUserLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
        }

        private void ViewPayslip(UserInfoWithNavigationPropertiesDto dto)
        {
            NavigationManager.NavigateTo($"/payslip/{dto.UserInfo.Code}");
        }
        
        private async Task CreateUserDateOfBirthChange(DateTime? value)
        {
            if (value != null)
            {
                createUserDateOfBirth = value;
                NewUserInfo.DateOfBirth = await ConvertBrowserToUniversalDateTime(createUserDateOfBirth.Value);
            }
        }
        
        private async Task CreateUserJoinedDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                createUserJoinedDateTime = value;
                NewUserInfo.JoinedDateTime = await ConvertBrowserToUniversalDateTime(createUserJoinedDateTime.Value);
            }
        }
        
        private async Task CreateUserPromotedDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                createUserPromotedDateTime = value;
                NewUserInfo.PromotedDateTime = await ConvertBrowserToUniversalDateTime(createUserPromotedDateTime.Value);
            }
        }
        
        private async Task EditUserDateOfBirthChange(DateTime? value)
        {
            if (value != null)
            {
                editUserDateOfBirth = value;
                EditingUserInfo.DateOfBirth = await ConvertBrowserToUniversalDateTime(editUserDateOfBirth.Value);
            }
        }
        
        private async Task EditUserJoinedDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                editUserJoinedDateTime = value;
                EditingUserInfo.JoinedDateTime = await ConvertBrowserToUniversalDateTime(editUserJoinedDateTime.Value);
            }
        }
        
        private async Task EditUserPromotedDateTimeChange(DateTime? value)
        {
            if (value != null)
            {
                editUserPromotedDateTime = value;
                EditingUserInfo.PromotedDateTime = await ConvertBrowserToUniversalDateTime(editUserPromotedDateTime.Value);
            }
        }

        private async Task OnSelectedUserPosition(UserPosition? value)
        {
            _userPosition = value;
            Filter.UserPosition = _userPosition == UserPosition.FilterNoSelect ? null : _userPosition;
            await GetUserInfosAsync();
        }
    }
}