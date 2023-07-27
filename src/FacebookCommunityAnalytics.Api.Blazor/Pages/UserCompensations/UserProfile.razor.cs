using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Blazorise;
using Blazorise.DataGrid;
using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserInfos;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.UserCompensations
{
    public partial class UserProfile
    {
        public readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        public UserProfileDto UserProfileDto;

        private IReadOnlyList<OrganizationUnitDto> OrganizationUnits { get; set; }
        public string PageTitle { get; set; }
        public string SelectedTab { get; set; } = "kpi";

        private IReadOnlyList<int> Months = GlobalConsts.MONTH_OF_YEAR;

        private IReadOnlyList<int> Years;

        // private int SelectedMonth = DateTime.Now.Month;
        // private int SelectedYear = DateTime.Now.Year;

        private Dictionary<string, DateRange> _dateRanges { get; set; }
        private DateTimeOffset? StartDate { get; set; }
        private DateTimeOffset? EndDate { get; set; }

        private int PageSize { get; set; } = 10;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private int AffiliateTotalCount { get; set; }
        private int AffiliatePageSize { get; set; } = 10;
        private int AffiliateCurrentPage { get; set; } = 1;
        private MediaDto ProfilePicture { get; set; }
        private List<LookupDto<Guid?>> UserLookupDtos { get; set; }
        private Guid? UserIdSelected { get; set; }

        private Guid? TeamIdSelected { get; set; }
        private List<UserCompensationNavigationPropertiesDto> UserPayrolls { get; set; }

        private List<CompensationAffiliateDto> UserCompensationAffiliates { get; set; }
        private List<CompensationAffiliateDto> DateGridAffiliates { get; set; }
        private List<StaffEvaluationWithNavigationPropertiesDto> StaffEvaluationWithNavigationProperties { get; set; }

        private IReadOnlyList<PostWithNavigationPropertiesDto> FacebookPosts { get; set; }
        private IReadOnlyList<ContractWithNavigationPropertiesDto> Contracts { get; set; }

        private DateTime? FromDateKPI { get; set; }
        private DateTime? ToDateKPI { get; set; }

        private bool _showLoading { get; set; } = true;
        private bool _showGridLoading;
        private int _invalidPostCount;

        private RadzenUpload _createContentUpload;
        private RadzenButton _buttonUploadAvatar { get; set; }
        private UserProfileChartResponse _chartStat { get; set; }

        public UserProfile()
        {
            UserProfileDto = new UserProfileDto();
            Years = Enumerable.Range(DateTime.UtcNow.Year - 5, 10).ToList();

            _chart_PostStats = new Chart();
            _chart_SaleStats = new Chart();
            _chart_TikTokStats = new Chart();
          
          UserPayrolls = new List<UserCompensationNavigationPropertiesDto>();
            UserCompensationAffiliates = new List<CompensationAffiliateDto>();
            StaffEvaluationWithNavigationProperties = new List<StaffEvaluationWithNavigationPropertiesDto>();

            FacebookPosts = new List<PostWithNavigationPropertiesDto>();
            Contracts = new List<ContractWithNavigationPropertiesDto>();

            UserLookupDtos = new List<LookupDto<Guid?>>();

            OrganizationUnits = new List<OrganizationUnitDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            _dateRanges = await GetDateRangePicker();
            var now = BrowserDateTime.Now;
            StartDate = now.Date.AddMonths(-6);
            EndDate = now.Date.AddDays(1).AddSeconds(-1);
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            OrganizationUnits = await TeamMemberAppService.GetTeams(new GetChildOrganizationUnitRequest());
            OrganizationUnits = OrganizationUnits.OrderBy(_ => _.DisplayName).ToList();

            if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue && IsNotManagerRole())
            {
                InitChart();
                UserIdSelected = CurrentUser.Id.Value;
                UserLookupDtos = new List<LookupDto<Guid?>>()
                {
                    new()
                    {
                        Id = CurrentUser.Id,
                        DisplayName = $"{CurrentUser.UserName} ({CurrentUser.Email})"
                    }
                };
                await ReloadAll();
                
            }
            _showLoading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                PageTitle = L["UserProfile.PageTitle"].Value;
                if (UserProfileDto.AppUser != null)
                {
                    PageTitle = PageTitle + " - " + UserProfileDto.AppUser.UserName;
                }
                await InitPage($"GDL - {PageTitle}");
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:UserProfile"]));

            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            // Toolbar.AddButton
            // (
            //     L["UserProfile.ExportExcel"],
            //     () =>
            //     {
            //         return Task.CompletedTask;
            //     },
            //     IconName.Download,
            //     requiredPolicyName: ApiPermissions.UserInfos.Default
            // );

            return ValueTask.CompletedTask;
        }

        private async Task ReloadAll()
        {
            _showLoading = true;
            UserProfileDto = new UserProfileDto();
            UserPayrolls = new List<UserCompensationNavigationPropertiesDto>();
            UserCompensationAffiliates = new List<CompensationAffiliateDto>();
            StaffEvaluationWithNavigationProperties = new List<StaffEvaluationWithNavigationPropertiesDto>();

            FacebookPosts = new List<PostWithNavigationPropertiesDto>();
            Contracts = new List<ContractWithNavigationPropertiesDto>();

            ProfilePicture = null;

            if (UserIdSelected.HasValue && UserIdSelected != Guid.Empty)
            {
                await LoadDataUserProfileKpi();
                await GetPayrollAsync();
            }

            _showLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task LoadDataUserProfileKpi()
        {
            _showLoading = true;
            if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue && UserIdSelected.HasValue)
            {
                UserProfileDto = await UserProfileAppService.GetUserProfileAsync(UserIdSelected.Value);
                if (UserProfileDto.UserInfo is {AvatarMediaId: { }})
                {
                    ProfilePicture = await MediaAppService.GetAsync(UserProfileDto.UserInfo.AvatarMediaId.Value);
                }
                if (!TeamIdSelected.HasValue)
                {
                    await LoadSelectUser();
                }
                StateHasChanged();
                if (TeamIdSelected.IsNotNullOrEmpty())
                {
                    _chartStat = await UserProfileAppService.GetChartStats(new UserProfileChartRequest()
                    {
                        TeamId = TeamIdSelected.Value,
                        UserId = UserIdSelected.Value,
                        FromDateTime = DateTime.UtcNow.AddDays(-7).Date,
                        ToDateTime = DateTime.UtcNow
                    });
                    if (_chartStat.CountPostsByTypeChartData.IsNotNullOrEmpty())
                    {
                        await RenderChart_PostChart();
                    }
                    if (_chartStat.SaleChartData.IsNotNullOrEmpty())
                    {
                        await RenderChart_SaleChart();
                    }
                    if (_chartStat.TikTokChartData.IsNotNullOrEmpty())
                    {
                        await RenderChart_TikTokChart();
                    }
                }
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    var staffEvaluation = await UserProfileAppService.GetStaffEvaluations
                    (
                        UserIdSelected.Value,
                        StartDate.Value.Year,
                        StartDate.Value.Month,
                        EndDate.Value.Year,
                        StartDate.Value.Month
                    );
                    (FromDateKPI, ToDateKPI) = GetDateTimeForApi(StartDate, EndDate);
                    StaffEvaluationWithNavigationProperties.Clear();
                    if (staffEvaluation != null)
                    {
                        StaffEvaluationWithNavigationProperties = staffEvaluation.OrderByDescending(_ => _.StaffEvaluation.Year).ThenByDescending(_ => _.StaffEvaluation.Month).ToList();
                    }
                    await SearchPostAsync();

                    Contracts = (await UserProfileAppService.GetContractsPagedResult
                    (
                        new GetContractsInput()
                        {
                            SalePersonId = UserIdSelected,
                            CreatedAtMin = FromDateKPI,
                            CreatedAtMax = ToDateKPI
                        }
                    )).Items;
                }
            }

            await ShowDataShortlink();
            _showLoading = false;
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PostWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;

            await SearchPostAsync();
        }
        
        private async Task OnAffiliateDataGridReadAsync(DataGridReadDataEventArgs<CompensationAffiliateDto> e)
        {
            AffiliateCurrentPage = e.Page;
            AffiliatePageSize = e.PageSize;
            DateGridAffiliates = UserCompensationAffiliates.Skip((AffiliateCurrentPage - 1) * AffiliatePageSize).Take(AffiliatePageSize).ToList();
        }

        private async Task SearchPostAsync()
        {
            _showGridLoading = true;
            var result = await UserProfileAppService.GetPostsPagedResult
            (
                new GetPostsInputExtend()
                {
                    AppUserId = UserIdSelected,
                    CreatedDateTimeMin = FromDateKPI,
                    CreatedDateTimeMax = ToDateKPI,
                    Sorting = CurrentSorting
                }
            );
            FacebookPosts = result.Items;
            _invalidPostCount = FacebookPosts.Count(_ => _.Post.IsNotAvailable);
            var skipCount = (CurrentPage - 1) * PageSize;
            FacebookPosts = FacebookPosts.Skip(skipCount).Take(PageSize).ToList();
            TotalCount = (int) result.TotalCount;
            _showGridLoading = false;
        }

        private Task OnSelectedTabChanged(string name)
        {
            SelectedTab = name;

            return Task.CompletedTask;
        }

        private async Task GetPayrollAsync()
        {
            _showGridLoading = true;
            if (UserPayrolls == null)
            {
                UserPayrolls = new List<UserCompensationNavigationPropertiesDto>();
            }
            else
            {
                UserPayrolls.Clear();
            }

            if (UserIdSelected.HasValue)
            {
                for (var i = 6; i >= 0; i--)
                {
                    var dateFilter = DateTime.UtcNow.AddMonths(i * -1);
                    var item = await UserProfileAppService.GetUserCompensationByUser(UserIdSelected.Value, dateFilter.Month, dateFilter.Year);
                    if (item != null)
                    {
                        UserPayrolls.Add(item);
                    }
                }
            }

            _showGridLoading = false;
        }

        private async Task ShowDataShortlink()
        {
            var fromDate = StartDate.Value.DateTime;
            var toDate = EndDate.Value.DateTime;
            if (UserIdSelected.HasValue)
            {
                UserCompensationAffiliates = await UserProfileAppService.GetAffiliateConversions(fromDate, toDate, UserIdSelected.Value);
                AffiliateTotalCount = UserCompensationAffiliates.Count;
            }
        }

        private async Task ExportUserStaffEvaluation(StaffEvaluationWithNavigationPropertiesDto input)
        {
            var posts = await UserProfileAppService.GetEvaluationPostDetailExportRow
            (
                new GetStaffEvaluationsInput
                {
                    AppUserId = input.AppUser.Id,
                    TeamId = input.OrganizationUnit.Id,
                    Year = input.StaffEvaluation.Year,
                    Month = input.StaffEvaluation.Month
                }
            );

            foreach (var post in posts)
            {
                post.IsNotAvailable = post.IsNotAvailable == PostConsts.True ? L["Post.IsNotAvailable"] : L["Post.IsAvailable"];
            }

            var fileName = $"{input.StaffEvaluation.Month}.{input.StaffEvaluation.Year}_{input.AppUser.UserName}_MonthlyStaffEvaluationDetails.xlsx";
            var excelBytes = ExportHelper.GeneratePostsExcelBytes(L, posts, fileName);

            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
        }

        private string GetPostUrl(PostDto post)
        {
            return post.Fid.IsNullOrWhiteSpace() ? L["PostUrl"] : $"Fid:{post.Fid.Substring(post.Fid.Length - 5)}";
        }

        private string SetColorContractStatus(ContractStatus item)
        {
            switch (item)
            {
                case ContractStatus.ContractSigned:
                    return "text-success";
                case ContractStatus.ContractSent:
                    return "text-info";
                default:
                    return "text-black-50";
            }
        }

        private string SetColorContractPaymentStatus(ContractPaymentStatus item)
        {
            switch (item)
            {
                case ContractPaymentStatus.Paid:
                    return "text-success";
                case ContractPaymentStatus.PartiallyPaid:
                    return "text-warning";
                case ContractPaymentStatus.Unpaid:
                    return "text-danger";
                default:
                    return "text-black-50";
            }
        }

        private async Task OnSelectedTeamIdChanged(Guid? TeamId)
        {
            TeamIdSelected = TeamId;

            UserIdSelected = null;

            UserLookupDtos = TeamId == Guid.Empty ? new List<LookupDto<Guid?>>() : await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest() { TeamId = TeamIdSelected });

            InitChart();
            await ReloadAll();
        }

        private async Task OnSelectedUserIdChanged(Guid? userId)
        {
            UserIdSelected = userId;
            
            if(userId != Guid.Empty) await ReloadAll();
        }

        public async Task SubmitUploadAvatarAsync()
        {
            _buttonUploadAvatar.Disabled = true;
            await _createContentUpload.Upload();
        }

        public async Task OnAvatarUploadComplete(UploadCompleteEventArgs args)
        {
            try
            {
                var results = JsonConvert.DeserializeObject<MediaDto>(args.RawResponse);
                if (results != null)
                {
                    ProfilePicture = await UserInfosAppService.UpdateAvatarAsync
                    (
                        new UserInfoUpdateAvatarDto
                        {
                            UserId = UserIdSelected,
                            MediaId = results.Id
                        }
                    );
                }

                _showLoading = false;
                await Message.Success(L["UserProfile.ChangeAvatar.Success"]);
            }
            catch (Exception){}

            _buttonUploadAvatar.Disabled = false;
        }

        private void StartDateChanged(DateTimeOffset? offset)
        {
            StartDate = offset;
        }

        private void EndDateChanged(DateTimeOffset? offset)
        {
            EndDate = offset;
        }
        
        private async Task LoadSelectUser()
        {
            TeamIdSelected = UserProfileDto.Team?.Id;
            if (TeamIdSelected.IsNotNullOrEmpty() && IsLeaderRole())
            {
                UserLookupDtos = await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest() { TeamId = TeamIdSelected });
            }
        }
    }
}