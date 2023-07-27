// using Blazorise;
// using Blazorise.DataGrid;
// using FacebookCommunityAnalytics.Api.Blazor.Shared;
// using FacebookCommunityAnalytics.Api.Permissions;
// using FacebookCommunityAnalytics.Api.Shared;
// using FacebookCommunityAnalytics.Api.UserAffiliates;
// using Microsoft.AspNetCore.Authorization;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using FacebookCommunityAnalytics.Api.Core.Const;
// using FacebookCommunityAnalytics.Api.Core.Enums;
// using FacebookCommunityAnalytics.Api.Core.Extensions;
// using FacebookCommunityAnalytics.Api.Groups;
// using FacebookCommunityAnalytics.Api.Organizations;
// using Microsoft.AspNetCore.Components;
// using Microsoft.JSInterop;
// using Newtonsoft.Json;
// using NUglify.Helpers;
// using Volo.Abp.Application.Dtos;
// using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
// using Volo.Abp.BlazoriseUI.Components;
// using Volo.Abp.Identity;
//
// namespace FacebookCommunityAnalytics.Api.Blazor.Pages
// {
//     public partial class UserAffiliates : BlazorComponentBase
//     {
//         protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
//         protected PageToolbar Toolbar { get; } = new();
//         private IReadOnlyList<UserAffiliateWithNavigationPropertiesDto> UserAffiliateList { get; set; }
//         private int PageSize { get; set; } = LimitedResultRequestDto.DefaultMaxResultCount;
//         private int CurrentPage { get; set; } = 1;
//         private string CurrentSorting { get; set; }
//         private int TotalCount { get; set; }
//         private bool CanCreateUserAffiliate { get; set; }
//         private bool CanEditUserAffiliate { get; set; }
//         private bool CanDeleteUserAffiliate { get; set; }
//         private UserAffiliateCreateDto NewUserAffiliate { get; set; }
//         private Validations NewUserAffiliateValidations { get; set; }
//         private UserAffiliateUpdateDto EditingUserAffiliate { get; set; }
//         private Validations EditingUserAffiliateValidations { get; set; }
//
//         private Guid EditingUserAffiliateId { get; set; }
//
//         // private Modal CreateUserAffiliateModal { get; set; }
//         private Modal EditUserAffiliateModal { get; set; }
//         private GetUserAffiliatesInputExtend Filter { get; set; }
//
//         private DataGridEntityActionsColumn<UserAffiliateWithNavigationPropertiesDto> EntityActionsColumn { get; set; }
//         private IReadOnlyList<LookupDto<Guid?>> UsersNullable { get; set; } = new List<LookupDto<Guid?>>();
//         private IReadOnlyList<LookupDto<Guid?>> CategoriesNullable { get; set; } = new List<LookupDto<Guid?>>();
//         private IReadOnlyList<LookupDto<Guid?>> PartnersNullable { get; set; } = new List<LookupDto<Guid?>>();
//         private IReadOnlyList<LookupDto<Guid?>> CampaignsNullable { get; set; } = new List<LookupDto<Guid?>>();
//         private IReadOnlyList<LookupDto<Guid?>> GroupsNullable { get; set; } = new List<LookupDto<Guid?>>();
//
//         public UserAffiliateHasConversionFilter UserAffiliateHasConversionFilter { get; set; } = UserAffiliateHasConversionFilter.NoSelect;
//         
//         private IList<OrganizationUnitDto> _organizationUnitDtos = new List<OrganizationUnitDto>();
//         private IList<GroupDto> _groupDtos = new List<GroupDto>();
//         private ConversionModel _conversionModel = new();
//         private bool _disableAfterGetAffiliateUrl;
//         private bool _disableShortlinkButton;
//         private string _selectedConversionTab = string.Empty;
//         private string _selectedTab = string.Empty;
//
//         public UserAffiliates()
//         {
//             NewUserAffiliate = new UserAffiliateCreateDto();
//             EditingUserAffiliate = new UserAffiliateUpdateDto();
//             Filter = new GetUserAffiliatesInputExtend
//             {
//                 MaxResultCount = PageSize,
//                 SkipCount = (CurrentPage - 1) * PageSize,
//                 Sorting = CurrentSorting,
//                 RelativeDateTimeRange = RelativeDateTimeRange.Last7Days
//             };
//         }
//
//         protected override async Task OnInitializedAsync()
//         {
//             try
//             {
//                 Filter.ConversionOwnerFilter = ConversionOwnerFilter.NoSelect;
//                 await SetPermissionsAsync();
//                 await SetToolbarItemsAsync();
//                 await SetBreadcrumbItemsAsync();
//                 await GetNullableCategoryLookupAsync("");
//                 await GetNullablePartnerLookupAsync("");
//                 await GetNullableCampaignLookupAsync("");
//                 await GetNullableGroupLookupAsync("");
//                 await GetUserAffiliatesAsync();
//                 _groupDtos = await GroupExtendAppService.GetListAsync();
//
//                 if (CurrentUser is {IsAuthenticated: true} && IsManagerRole())
//                 {
//                     _organizationUnitDtos = await OrganizationsAppService.GetLeafNodes(new GetLeafNodesRequest());
//                     _organizationUnitDtos = _organizationUnitDtos.OrderBy(_ => _.DisplayName).ToList();
//                 }
//                 _selectedConversionTab = L["ConversionDataGrid"];
//                 _selectedTab = L["CreateAffiliateTab"];
//                 if (IsManagerRole())
//                 {
//                     _selectedTab = L["ConversionSumStats"];
//                 }
//                 
//             }
//             catch (Exception e)
//             {
//                 await JSRuntime.InvokeAsync<string>("console.log", e);
//                 throw;
//             }
//         }
//
//         protected virtual ValueTask SetBreadcrumbItemsAsync()
//         {
//             BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:UserAffiliates"]));
//             
//             return ValueTask.CompletedTask;
//         }
//
//         protected virtual ValueTask SetToolbarItemsAsync()
//         {
//             // Toolbar.AddButton(L["UserAffiliate.RefreshConversionButton"], () =>
//             // {
//             //     UpdateUserAffiliateConversion();
//             //     return Task.CompletedTask;
//             //     //await GetUserAffiliatesAsync();
//             // }, IconName.Add, requiredPolicyName: ApiPermissions.UserAffiliates.Edit);
//             if (_selectedTab == L["ConversionDetailsTab"])
//             {
//                 Toolbar.AddButton(L["UserAffiliate.ExportAffiliate"], async () =>
//                 {
//                     await ExportPostsAsync();
//                 }, IconName.Add, requiredPolicyName: ApiPermissions.UserAffiliates.Default);
//             }
//             return ValueTask.CompletedTask;
//         }
//
//         private async Task SetPermissionsAsync()
//         {
//             CanCreateUserAffiliate = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserAffiliates.Create);
//             CanEditUserAffiliate = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserAffiliates.Edit);
//             CanDeleteUserAffiliate = await AuthorizationService.IsGrantedAsync(ApiPermissions.UserAffiliates.Delete);
//         }
//
//         private async Task GetUserAffiliatesAsync()
//         {
//             Filter.MaxResultCount = PageSize;
//             Filter.SkipCount = (CurrentPage - 1) * PageSize;
//             Filter.Sorting = CurrentSorting;
//
//             if (Filter.FilterText.IsNotNullOrWhiteSpace())
//             {
//                 Filter.RelativeDateTimeRange = RelativeDateTimeRange.Unknown;
//             }
//
//             if (Filter.FilterText.IsNotNullOrWhiteSpace()) { Filter.RelativeDateTimeRange = RelativeDateTimeRange.Unknown; }
//
//             switch (UserAffiliateHasConversionFilter)
//             {
//                 case UserAffiliateHasConversionFilter.NoSelect:
//                     Filter.HasConversion = null;
//                     break;
//                 case UserAffiliateHasConversionFilter.HasConversion:
//                     Filter.HasConversion = true;
//                     break;
//                 case UserAffiliateHasConversionFilter.NoConversion:
//                     Filter.HasConversion = false;
//                     break;
//             }
//
//             Filter.CreatedAtMax = DateTime.UtcNow;
//             Filter.CreatedAtMin = Filter.RelativeDateTimeRange.ToDateTime();
//
//             var result = await UserAffiliateAppService.GetUserAffiliateWithNavigationProperties(Filter);
//             UserAffiliateList = result.Items;
//             TotalCount = (int) result.TotalCount;
//
//             var userAffiliates = await UserAffiliateAppService.GetListUserAffiliate(Filter);
//             _conversionModel.FromDate = Filter.RelativeDateTimeRange.ToDateTime();
//             _conversionModel.ToDate = DateTime.UtcNow;
//             _conversionModel.ClickCount = userAffiliates.Sum(_ => _.UserAffiliateConversion.ClickCount);
//             _conversionModel.ConversionCount = userAffiliates.Sum(_ => _.UserAffiliateConversion.ConversionCount);
//             _conversionModel.ConversionAmount = userAffiliates.Sum(_ => _.UserAffiliateConversion.ConversionAmount);
//             _conversionModel.CommissionAmount = userAffiliates.Sum(_ => _.UserAffiliateConversion.CommissionAmount);
//         }
//
//         #region SEARCH
//
//         protected virtual async Task SearchAsync()
//         {
//             CurrentPage = 1;
//             await GetUserAffiliatesAsync();
//             await InvokeAsync(StateHasChanged);
//         }
//
//         private void OnTeamSelectedValueChanged(Guid? value)
//         {
//             Filter.OrgUnitId = value == Guid.Empty ? null : value;
//         }
//
//         #endregion
//
//         private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserAffiliateWithNavigationPropertiesDto> e)
//         {
//             CurrentSorting = e.Columns
//                 .Where(c => c.SortDirection != SortDirection.None)
//                 .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
//                 .JoinAsString(",");
//             CurrentPage = e.Page;
//             PageSize = e.PageSize;
//             await GetUserAffiliatesAsync();
//             await InvokeAsync(StateHasChanged);
//         }
//
//         private void OpenEditUserAffiliateModal(UserAffiliateDto input)
//         {
//             EditingUserAffiliateId = input.Id;
//             EditingUserAffiliate = ObjectMapper.Map<UserAffiliateDto, UserAffiliateUpdateDto>(input);
//             EditingUserAffiliate.AppUserId = input.AppUserId;
//             EditingUserAffiliateValidations.ClearAll();
//             EditUserAffiliateModal.Show();
//         }
//
//         private async Task DeleteUserAffiliateAsync(UserAffiliateDto input)
//         {
//             await UserAffiliateAppService.DeleteAsync(input.Id);
//             await GetUserAffiliatesAsync();
//         }
//
//         private async Task CreateUserAffiliateAsync()
//         {
//             var success = await Invoke
//             (
//                 async () =>
//                 {
//                     NewUserAffiliate.CreatedAt = DateTime.UtcNow;
//                     NewUserAffiliate.AppUserId = CurrentUser.Id;
//
//                     await UserAffiliateAppService.CreateAsync(NewUserAffiliate);
//                     await GetUserAffiliatesAsync();
//
//                     // ClearUserAffiliateAsync();
//                     // NewUserAffiliate.AffiliateUrl = "";
//                     // NewUserAffiliate.Url = "";
//                     
//                     await JSRuntime.InvokeVoidAsync("clipboardCopy.resetButton");
//                     _disableShortlinkButton = false;
//                 },
//                 L,
//                 true,
//                 actionType: BlazorComponentBaseActionType.Create
//             );
//             await InvokeAsync(StateHasChanged);
//         }
//
//         private async Task ClearUserAffiliateAsync()
//         {
//             NewUserAffiliate = new UserAffiliateCreateDto {GroupId = Guid.Empty};
//             _disableAfterGetAffiliateUrl = false;
//             _disableShortlinkButton = false;
//             await JSRuntime.InvokeVoidAsync("clipboardCopy.resetButton");
//         }
//
//         private void CloseEditUserAffiliateModal()
//         {
//             EditUserAffiliateModal.Hide();
//             EditingUserAffiliate = new();
//         }
//
//         private async Task UpdateUserAffiliateAsync()
//         {
//             var success = await Invoke
//             (
//                 async () =>
//                 {
//                     await UserAffiliateAppService.UpdateAsync(EditingUserAffiliateId, EditingUserAffiliate);
//                     await GetUserAffiliatesAsync();
//                     EditUserAffiliateModal.Hide();
//                 },
//                 L,
//                 true,
//                 actionType: BlazorComponentBaseActionType.Update
//             );
//
//             await InvokeAsync(StateHasChanged);
//         }
//
//         private async Task GetNullableAppUserLookupAsync(string newValue)
//         {
//             UsersNullable = (await PostsExtendAppService.GetAppUserLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
//         }
//
//         private async Task GetNullableCategoryLookupAsync(string newValue)
//         {
//             CategoriesNullable = (await PostsExtendAppService.GetCategoryLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
//         }
//
//         private async Task GetNullablePartnerLookupAsync(string newValue)
//         {
//             PartnersNullable = (await PostsExtendAppService.GetPartnerLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
//         }
//
//         private async Task GetNullableCampaignLookupAsync(string newValue)
//         {
//             CampaignsNullable = (await PostsExtendAppService.GetCampaignLookupAsync(new LookupRequestDto {Filter = newValue})).Items;
//         }
//
//         private async Task GetNullableGroupLookupAsync(string newValue)
//         {
//             var groups = await PostsExtendAppService.GetGroupLookupAsync(new GroupLookupRequestDto {Filter = newValue});
//             GroupsNullable = groups.Items;
//         }
//
//         private async Task GetAffiliateUrlAsync(string url, bool isHappyDay)
//         {
//             if (CurrentUser.Id == null || url.HasNoValue()) return;
//
//             if (url.ToLower().Contains(MarketplaceType.Shopee.ToString().ToLower())) { NewUserAffiliate.MarketplaceType = MarketplaceType.Shopee; }
//             else if (url.ToLower().Contains(MarketplaceType.Lazada.ToString().ToLower())) { NewUserAffiliate.MarketplaceType = MarketplaceType.Lazada; }
//             else
//             {
//                 await Message.Error(L[ApiDomainErrorCodes.UserAffiliates.InvalidUrl]);
//                 return;
//             }
//
//             var userInfoWithNavigationProperties = await UserInfosAppService.GetByUserIdAsync(CurrentUser.Id.Value);
//             var groupFid = string.Empty;
//             if (NewUserAffiliate.GroupId != null) { groupFid = _groupDtos.FirstOrDefault(_ => _.Id == NewUserAffiliate.GroupId)?.Fid; }
//
//             var affiliate = await UserAffiliateAppService.GenerateAffiliate
//             (
//                 new GenerateAffiliateRequest
//                 {
//                     Url = url,
//                     UserCode = userInfoWithNavigationProperties.UserInfo.Code,
//                     MarketplaceType = NewUserAffiliate.MarketplaceType,
//                     GroupFid = groupFid,
//                     CampaignId = NewUserAffiliate.CampaignId?.ToString(),
//                     IsHappyDay = isHappyDay
//                 }
//             );
//             if (affiliate == null)
//             {
//                 await Message.Error(L[ApiDomainErrorCodes.UserAffiliates.InvalidUrl]);
//                 return;
//             }
//             else
//             {
//                 NewUserAffiliate.AffiliateUrl = affiliate.ShortUrl;
//                 NewUserAffiliate.Url = affiliate.ProductUrl;
//                 //await CopyToClipboard(affiliate.ShortUrl);
//                 _disableAfterGetAffiliateUrl = true;
//                 _disableShortlinkButton = true;
//
//                 await CreateUserAffiliateAsync();
//             }
//         }
//
//         // private async Task<bool> CopyShortlink()
//         // {
//         //    return  await CopyToClipboard(NewUserAffiliate.AffiliateUrl);
//         // }
//         
//         // IconName currentStateCopy = IconName.Save;
//         // private async Task<bool> CopyToClipboard(string value)
//         // {
//         //     if (value.IsNotNullOrWhiteSpace())
//         //     {
//         //         try
//         //         {
//         //             await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", value.Trim());
//         //         }
//         //         catch (Exception e)
//         //         {
//         //             await Message.Error(JsonConvert.SerializeObject(e));
//         //         }
//         //         // try
//         //         // {
//         //         //     await ClipboardService.WriteTextAsync(value.Trim());
//         //         // }
//         //         // catch (Exception e)
//         //         // {
//         //         //     await Message.Error(JsonConvert.SerializeObject(e));
//         //         // }
//         //         currentStateCopy = IconName.Check;
//         //         return true;
//         //     }
//         //     return false;
//         // }
//
//         private void OnSelectedValueCategory(Guid? value)
//         {
//             Filter.CategoryId = value == Guid.Empty ? null : value;
//         }
//
//         private void OnSelectedValuePartner(Guid? value)
//         {
//             Filter.PartnerId = value == Guid.Empty ? null : value;
//         }
//
//         private void OnSelectedValueCampaign(Guid? value)
//         {
//             Filter.CampaignId = value == Guid.Empty ? null : value;
//         }
//
//         // private void OnSelectedValueSearchGroup(Guid? value)
//         // {
//         //     if (value == Guid.Empty)
//         //     {
//         //         Filter.GroupId = null;
//         //     }
//         //     else
//         //     {
//         //         Filter.GroupId = value;
//         //     }
//         // }
//         //
//         // private void OnSelectedValueGroup(Guid? value)
//         // {
//         //     if (value == Guid.Empty)
//         //     {
//         //         NewUserAffiliate.GroupId = null;
//         //     }
//         //     else
//         //     {
//         //         NewUserAffiliate.GroupId = value;
//         //     }
//         // }
//
//         private string GetNameGroup(Guid? groupId)
//         {
//             var group = _groupDtos.FirstOrDefault(_ => _.Id == groupId);
//             if (group == null) return string.Empty;
//             return GroupConsts.GetGroupDisplayName(group.Title, group.GroupSourceType);
//         }
//
//         private void OnSelectedConversionTabChanged(string name)
//         {
//             _selectedConversionTab = name;
//         }
//
//         private async Task OnSelectedTabChanged(string name)
//         {
//             _selectedTab = name;
//             await ClearUserAffiliateAsync();
//         }
//         
//         private async Task ExportPostsAsync()
//         {
//             var excelBytes = await UserAffiliateAppService.ExportUserAffiliate(Filter);
//             if (excelBytes == null)
//             {
//                 await Message.Info(L["CampaignExport.NoAffiliates"]);
//                 return;
//             }
//             var fileName = "Affiliate-Shortlink";
//             await JSRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(excelBytes));
//         }
//     }
//
//     public class ConversionModel
//     {
//         public DateTime? FromDate { get; set; }
//         public DateTime? ToDate { get; set; }
//         public int ClickCount { get; set; }
//         public int ConversionCount { get; set; }
//         public decimal ConversionAmount { get; set; }
//         public decimal CommissionAmount { get; set; }
//     }
// }

