// #nullable enable
// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using Blazorise;
// using FacebookCommunityAnalytics.Api.ApiConfigurations;
// using FacebookCommunityAnalytics.Api.Blazor.Shared;
// using FacebookCommunityAnalytics.Api.Configs;
// using FacebookCommunityAnalytics.Api.ApiConfigurations;
// using FacebookCommunityAnalytics.Api.Core.Enums;
// using FacebookCommunityAnalytics.Api.Groups;
// using FacebookCommunityAnalytics.Api.Shared;
// using FacebookCommunityAnalytics.Api.UserInfos;
// using FacebookCommunityAnalytics.Api.Users;
//
// namespace FacebookCommunityAnalytics.Api.Blazor.Pages
// {
//     public partial class PayrollsConfiguration : BlazorComponentBase
//     {
//         private string Seeding = "seeding";
//         private string Affiliate = "affiliate";
//
//         private PayrollConfiguration PayrollConfiguration { get; set; }
//         private IList<UserInfoWithNavigationPropertiesDto> _currentGroupModDtos = new List<UserInfoWithNavigationPropertiesDto>();
//         private IList<GroupDto> _currentBrandingGroupDtos = new List<GroupDto>();
//
//         private Guid? _groupModAppUserId;
//         private Guid? _groupId;
//
//         private CreateGroupModeratorModel _newGroupModerator = new();
//         private EditAndUpdateGroupModeratorModel _editAndDeleteGroupModerator = new();
//         private Modal BrandingGroupModal { get; set; }
//         private Modal CreateGroupModeratorModal { get; set; }
//         private Modal UpdateAndDeleteGroupModeratorModal { get; set; }
//
//         private IReadOnlyList<UserInfoWithNavigationPropertiesDto> _userInfoNavs = new List<UserInfoWithNavigationPropertiesDto>();
//
//         private IList<GroupDto> _typeGroupDtos = new List<GroupDto>();
//         private IList<GroupDto> _groupDtos = new List<GroupDto>();
//         private IList<GroupDto> _currentUserGroupDtos = new List<GroupDto>();
//         
//         public PayrollsConfiguration()
//         {
//             
//         }
//
//         private void OnSelectedTabChangedSeeding(string name)
//         {
//             Seeding = name;
//         }
//
//         private void OnSelectedTabChangedAffiliate(string name)
//         {
//             Affiliate = name;
//         }
//
//         protected override async Task OnInitializedAsync()
//         {
//             var results = await UserInfosAppService.GetListExtendAsync(new GetExtendUserInfosInput()
//             {
//                 MaxResultCount = 1000,
//                 ContentRoleType = ContentRoleType.Mod
//             });
//             _userInfoNavs = results.Items;
//             _groupDtos = await GroupExtendAppService.GetListAsync();
//             _typeGroupDtos = _groupDtos.Where(_ => _.GroupSourceType == GroupSourceType.Group).ToList();
//             await GetAsync();
//         }
//         
//         protected override async Task OnAfterRenderAsync(bool firstRender)
//         {
//             await JSRuntime.InvokeVoidAsync("setTitle", $"GDL - {L["PayrollConfigUI.PageTitle"].Value}");     
//         }
//
//         private async Task GetAsync()
//         {
//             try
//             {
//                 PayrollConfiguration = await ApiConfigurationAppService.GetPayrollConfiguration();
//                 _currentBrandingGroupDtos = PayrollConfiguration.BrandingGroupFids == null? new List<GroupDto>() : _typeGroupDtos.Where(_ => PayrollConfiguration.BrandingGroupFids.Contains(_.Fid)).ToList();
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 throw;
//             }
//          
//         }
//
//         private async Task UpdateAsync()
//         {
//             var success = await Invoke(
//                 async () => await ApiConfigurationAppService.UpdatePayrollConfiguration(PayrollConfiguration),
//                 L,
//                 true,
//                 BlazorComponentBaseActionType.Update);
//         }
//
//         //Branding Group Modal
//         private void OpenBrandingGroupModal()
//         {
//             BrandingGroupModal.Show();
//         }
//
//         private void UpdateBrandingGroup()
//         {
//             PayrollConfiguration.BrandingGroupFids = _currentBrandingGroupDtos.Select(_ => _.Fid).ToList();
//             CloseBrandingGroupModal();
//         }
//
//         private void CloseBrandingGroupModal()
//         {
//             _currentBrandingGroupDtos = PayrollConfiguration.BrandingGroupFids == null? new List<GroupDto>() : _typeGroupDtos.Where(_ => PayrollConfiguration.BrandingGroupFids.Contains(_.Fid)).ToList();
//             BrandingGroupModal.Hide();
//         }
//
//         //Create Group Moderator Modal
//         private void OpenCreateGroupModeratorModal()
//         {
//             _newGroupModerator = new();
//             CreateGroupModeratorModal.Show();
//         }
//
//         private async void CreateGroupModerator()
//         {
//             if (_newGroupModerator.UserId == null || _newGroupModerator.GroupId == null) return;
//
//             var group = await GroupsAppService.GetAsync((Guid) _newGroupModerator.GroupId);
//             if (group == null) return;
//
//             var userCodes = PayrollConfiguration.GroupModerators.Select(_ => _.UserCode).ToList();
//
//             var newUserInfo = await UserInfosAppService.GetByUserIdAsync((Guid) _newGroupModerator.UserId);
//             var newUserCode = newUserInfo.UserInfo.Code;
//
//             if (userCodes.Contains(newUserCode))
//             {
//                 var modGroups = PayrollConfiguration.GroupModerators
//                     .FirstOrDefault(_ => _.UserCode == newUserCode)?.GroupWave.Select(g => g.Key).ToList();
//
//                 if (modGroups != null && modGroups.Contains(group.Fid)) return;
//
//                 PayrollConfiguration.GroupModerators.FirstOrDefault(_ => _.UserCode == newUserCode)?.GroupWave
//                     .Add(group.Fid, _newGroupModerator.Wave);
//             }
//             else
//             {
//                 var groupMod = new GroupModerator()
//                 {
//                     UserCode = newUserInfo.UserInfo.Code,
//                     GroupWave = new Dictionary<string, decimal>()
//                 };
//                 groupMod.GroupWave.Add(group.Fid, _newGroupModerator.Wave);
//                 PayrollConfiguration.GroupModerators.Add(groupMod);
//             }
//
//             CloseCreateGroupModeratorModal();
//         }
//
//         private void CloseCreateGroupModeratorModal()
//         {
//             CreateGroupModeratorModal.Hide();
//         }
//
//         //Update And Delete Group Moderator Modal
//         private void OpenUpdateAndDeleteGroupModeratorModal()
//         {
//             _editAndDeleteGroupModerator = new EditAndUpdateGroupModeratorModel();
//             _groupModAppUserId = null;
//             _groupId = null;
//             var groupModUserCodes = PayrollConfiguration.GroupModerators.Select(_ => _.UserCode).ToList();
//             _currentGroupModDtos = _userInfoNavs.Where(_ => groupModUserCodes.Contains(_.UserInfo.Code)).ToList();
//             UpdateAndDeleteGroupModeratorModal.Show();
//         }
//
//         private void UpdateGroupModerator()
//         {
//             var dicGroupWave = GetDictionaryGroupWave();
//
//             var first = GetGroupModerator(out var index);
//
//             if (first != null)
//             {
//                 PayrollConfiguration.GroupModerators.Remove(first);
//                 first.GroupWave = dicGroupWave;
//                 PayrollConfiguration.GroupModerators.Insert(index, first);
//             }
//
//             CloseUpdateAndDeleteGroupModeratorModal();
//         }
//
//         private void DeleteGroupModerator()
//         {
//             var first = GetGroupModerator(out var index);
//
//             if (first != null)
//             {
//                 PayrollConfiguration.GroupModerators.Remove(first);
//             }
//
//             CloseUpdateAndDeleteGroupModeratorModal();
//         }
//
//         private void CloseUpdateAndDeleteGroupModeratorModal()
//         {
//             UpdateAndDeleteGroupModeratorModal.Hide();
//         }
//         
//         private void OnChangeEditingUserCode(Guid? userId)
//         {
//             _groupModAppUserId = userId;
//             var userCode = _currentGroupModDtos
//                 .FirstOrDefault(_ => _.AppUser.Id == _groupModAppUserId)
//                 ?.UserInfo.Code;
//             
//             _editAndDeleteGroupModerator.UserCode = userCode ?? string.Empty;
//
//             var groupFids = PayrollConfiguration.GroupModerators.FirstOrDefault(_ => _.UserCode == userCode)
//                 ?.GroupWave
//                 .Select(g => g.Key).ToList();
//             
//             _currentUserGroupDtos = groupFids == null ? new List<GroupDto>() : _groupDtos.Where(_ => groupFids.Contains(_.Fid)).ToList();
//             _groupId = null;
//             _editAndDeleteGroupModerator.GroupFid = string.Empty;
//             _editAndDeleteGroupModerator.Wave = 0;
//         }
//
//         private void OnChangeEditingGroup(Guid? id)
//         {
//             _groupId = id;
//             var userCode = _userInfoNavs
//                 .FirstOrDefault(_ => _.AppUser.Id == _groupModAppUserId)
//                 ?.UserInfo.Code;
//
//             var group = _currentUserGroupDtos.FirstOrDefault(_ => _.Id == id);
//             _editAndDeleteGroupModerator.GroupFid = group == null ? string.Empty : group.Fid;
//             var value = group == null
//                 ? 0
//                 : PayrollConfiguration.GroupModerators
//                     .FirstOrDefault(_ => _.UserCode == userCode)?.GroupWave
//                     .FirstOrDefault(g => g.Key == group.Fid).Value;
//
//             _editAndDeleteGroupModerator.Wave = value ?? 0;
//         }
//
//         private GroupModerator? GetGroupModerator(out int index)
//         {
//             GroupModerator? first = null;
//             index = 0;
//             foreach (var groupModerator in PayrollConfiguration.GroupModerators)
//             {
//                 if (groupModerator.UserCode == _editAndDeleteGroupModerator.UserCode)
//                 {
//                     first = groupModerator;
//                     index = PayrollConfiguration.GroupModerators.IndexOf(first);
//                     break;
//                 }
//             }
//
//             return first;
//         }
//
//         private Dictionary<string, decimal>? GetDictionaryGroupWave()
//         {
//             var dictionaryGroupWave = new Dictionary<string, decimal>();
//
//             if (!_editAndDeleteGroupModerator.GroupFid.IsNullOrWhiteSpace() &&
//                 !_editAndDeleteGroupModerator.UserCode.IsNullOrWhiteSpace())
//             {
//                 dictionaryGroupWave = PayrollConfiguration.GroupModerators
//                     .FirstOrDefault(_ => _.UserCode == _editAndDeleteGroupModerator.UserCode)?.GroupWave;
//
//                 if (dictionaryGroupWave != null)
//                 {
//                     if (dictionaryGroupWave.Keys.Contains(_editAndDeleteGroupModerator.GroupFid))
//                     {
//                         dictionaryGroupWave[_editAndDeleteGroupModerator.GroupFid] = _editAndDeleteGroupModerator.Wave;
//                     }
//
//                     return dictionaryGroupWave;
//                 }
//             }
//
//             return dictionaryGroupWave;
//         }
//         
//         //On Load Group Moderator Table
//         private string GetUserName(string userCode)
//         {
//             var userInfoWithNavigationPropertiesDto =
//                 _userInfoNavs.FirstOrDefault(_ => _.UserInfo.Code == userCode);
//
//             return userInfoWithNavigationPropertiesDto == null
//                 ? string.Empty
//                 : $"{userInfoWithNavigationPropertiesDto.AppUser.UserName}({userInfoWithNavigationPropertiesDto.AppUser.Email})";
//         }
//
//         private string GetGroupNameByFid(string groupFid)
//         {
//             var group = _groupDtos.FirstOrDefault(_ => _.Fid == groupFid);
//             return group == null ? string.Empty : $"{group.Title}({group.GroupSourceType})";
//         }
//
//         private void OnChangeNewUserId(Guid? value)
//         {
//             _newGroupModerator.UserId = value == Guid.Empty ? null : value;
//         }
//         
//         private void OnChangeNewGroupId(Guid? value)
//         {
//             _newGroupModerator.GroupId = value == Guid.Empty ? null : value;
//         }
//         
//     }
//
//     public class CreateGroupModeratorModel
//     {
//         public Guid? UserId { get; set; }
//         public Guid? GroupId { get; set; }
//         public decimal Wave { get; set; }
//     }
//
//     public class EditAndUpdateGroupModeratorModel
//     {
//         public string UserCode { get; set; }
//         public string GroupFid { get; set; }
//         public decimal Wave { get; set; }
//     }
// }

