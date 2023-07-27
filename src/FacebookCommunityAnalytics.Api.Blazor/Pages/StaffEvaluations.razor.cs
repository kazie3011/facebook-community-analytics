using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Blazorise.RichTextEdit;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Identity;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class StaffEvaluations
    {
        private readonly IReadOnlyList<int> Months = new List<int>
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12
        };

        private IReadOnlyList<int> Years = new List<int>();
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        private bool CanExportExcel;
        private bool CanDeleteStaffEvaluation;

        private RichTextEdit richTextEditRef;
        private RichTextEdit richTextTaskEditRef;
        protected PageToolbar Toolbar { get; set; } = new();
        private IReadOnlyList<StaffEvaluationWithNavigationPropertiesDto> StaffEvalWithNavs { get; set; }
        private IReadOnlyList<StaffEvaluationWithNavigationPropertiesDto> TikTokChannelEvalWithNavs { get; set; }
        private GetStaffEvaluationsInput Filter { get; }
        private GetStaffEvaluationsInput ChannelFilter { get; set; }

        private int PageSize { get; set; } = 100;
        private int CurrentPage { get; set; } = 1;
        private int PageChannelSize { get; set; } = 100;
        private int CurrentChannelPage { get; set; } = 1;
        private int TotalCount { get; set; }
        private int TotalChannelCount { get; set; }
        private Modal EditStaffEvaluationModal { get; set; }
        private Modal ChannelEvaluationModal { get; set; }
        private Modal DeleteEvaluationModal { get; set; }
        private Guid EditingStaffEvaluationId { get; set; }
        private CreateUpdateStaffEvaluationDto EditingStaffEvaluation { get; set; }
        private IReadOnlyList<IdentityUserDto> Users { get; set; }
        private IReadOnlyList<OrganizationUnitDto> Teams { get; set; }
        private string userName { get; set; }
        private string modName { get; set; }
        private string CurrentSorting { get; set; }
        private Guid? _selectedTeamId;
        private string ReviewContentEdit { get; set; }
        private string AssignedContentEdit { get; set; }
        private bool _isNotChannelEval = true;

        public StaffEvaluations()
        {
            StaffEvalWithNavs = new List<StaffEvaluationWithNavigationPropertiesDto>();
            TikTokChannelEvalWithNavs = new List<StaffEvaluationWithNavigationPropertiesDto>();
            EditingStaffEvaluation = new CreateUpdateStaffEvaluationDto();
            Users = new List<IdentityUserDto>();
            Teams = new List<OrganizationUnitDto>();
            Filter = new GetStaffEvaluationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize
            };
            ChannelFilter = new GetStaffEvaluationsInput
            {
                MaxResultCount = PageChannelSize,
                SkipCount = (CurrentChannelPage - 1) * PageChannelSize
            };
        }
        
        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            Teams = await StaffEvaluationAppService.GetEvaluationTeams();
            Users = await userInfosAppService.GetUsers();
            Years = await StaffEvaluationAppService.GetEvaluationYears();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["StaffEvaluations.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:HR"]));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:StaffEvaluation"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["StaffEvaluations.GenerateButton"],
                async () =>
                {
                    await StaffEvaluationAppService.GenerateStaffEvaluations();
                    await GetStaffEvaluationsAsync();
                },
                requiredPolicyName: ApiPermissions.StaffEvaluations.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanExportExcel = IsManagerRole();
            CanDeleteStaffEvaluation = await AuthorizationService
                    .IsGrantedAsync(ApiPermissions.StaffEvaluations.Delete);
        }

        private async Task GetStaffEvaluationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;
            var result = await StaffEvaluationAppService.GetListExtendAsync(Filter);
            StaffEvalWithNavs = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        private async Task GetChannelEvaluationsAsync(Guid appUserId)
        {
            var user = Users.FirstOrDefault(x => x.Id == appUserId);
            if (user != null)
            {
                modName = GetFullName(user);
            }

            ChannelFilter.MaxResultCount = PageChannelSize;
            ChannelFilter.SkipCount = (CurrentChannelPage - 1) * PageChannelSize;
            ChannelFilter.IsTikTokEvaluation = true;
            ChannelFilter.AppUserId = appUserId;
            var result = await StaffEvaluationAppService.GetListExtendAsync(ChannelFilter);
            TikTokChannelEvalWithNavs = result.Items;
            TotalChannelCount = (int) result.TotalCount;
        }


        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<StaffEvaluationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await GetStaffEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenEditStaffEvaluationModal(StaffEvaluationDto input, bool isNotChannelEval = true)
        {
            _isNotChannelEval = isNotChannelEval;
            EditingStaffEvaluationId = input.Id;
            //richTextEditRef.ClearAsync();
            var user = Users.FirstOrDefault(x => x.Id == input.AppUserId);
            if (user != null)
            {
                userName = GetFullName(user);
            }

            EditingStaffEvaluation = ObjectMapper.Map<StaffEvaluationDto, CreateUpdateStaffEvaluationDto>(input);
            ReviewContentEdit = EditingStaffEvaluation.DirectorReview;
            AssignedContentEdit = EditingStaffEvaluation.AssignedTasks;
            EditStaffEvaluationModal.Show();
        }

        private async Task UpdateStaffEvaluationAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    EditingStaffEvaluation.DirectorReview = ReviewContentEdit;
                    EditingStaffEvaluation.AssignedTasks = AssignedContentEdit;
                    EditingStaffEvaluation.TotalPoint = EditingStaffEvaluation.QuantityKPI + EditingStaffEvaluation.QualityKPI + EditingStaffEvaluation.ReviewPoint;
                    await StaffEvaluationAppService.UpdateAsync(EditingStaffEvaluationId, EditingStaffEvaluation);
                    await GetStaffEvaluationsAsync();
                    EditStaffEvaluationModal.Hide();
                    ChannelEvaluationModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Update
            );
        }

        private void CloseEditStaffEvaluationModal()
        {
            EditStaffEvaluationModal.Hide();
            EditingStaffEvaluation = new CreateUpdateStaffEvaluationDto();
        }

        private async Task OnSelectedStaffEvaluationStatus(StaffEvaluationStatus? value)
        {
            Filter.StaffEvaluationStatus = value == StaffEvaluationStatus.FilterNoSelect ? null : value;
            await GetStaffEvaluationsAsync();
        }

        private async Task ExportExcelAsync()
        {
            var results = await StaffEvaluationAppService.GetExportRows(Filter);
            var isSaleTeam = false;
            if (Filter.Month > 0)
            {
                var team = string.Empty;
                if (Teams.Count == 1)
                {
                    team = $"_{Teams.FirstOrDefault()?.DisplayName}";
                    isSaleTeam = await StaffEvaluationAppService.ContainSaleTeam(team);
                }

                if (team.IsNullOrEmpty())
                {
                    team = Filter.TeamId.IsNotNullOrEmpty() ? $"_{Teams.FirstOrDefault(_ => _.Id == Filter.TeamId)?.DisplayName}" : string.Empty;
                    isSaleTeam = await StaffEvaluationAppService.ContainSaleTeam(results.Select(x => x.Team).Distinct().ToArray());
                }

                var fileName = $"{Filter.Month}.{Filter.Year}{team}_MonthlyStaffEvaluation.xlsx";
                var excelBytes = ExportHelper.GenerateStaffEvalExcelBytes(L, results.OrderBy(_ => _.Team).ToList(), fileName, isSaleTeam);

                await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
            }
        }

        private async Task ExportSeedingUserStaffEvaluation(StaffEvaluationWithNavigationPropertiesDto input)
        {
            var (fromDateTime, toDateTime) = GetPayrollDateTime(input.StaffEvaluation.Year, input.StaffEvaluation.Month);
            if (input.AppUser != null)
            {
                var posts = await StaffEvaluationAppService.GetEvaluationSeedingPostExport
                (
                    new GetPostEvaluationRequest()
                    {
                        AppUserId = input.AppUser.Id,
                        FromDateTime = fromDateTime,
                        ToDateTime = toDateTime
                    }
                );
                if (Filter.Month > 0)
                {
                    foreach (var post in posts)
                    {
                        post.IsNotAvailable = post.IsNotAvailable == PostConsts.True ? L["Post.IsNotAvailable"] : L["Post.IsAvailable"];
                    }

                    var team = string.Empty;
                    if (Teams.Count == 1)
                    {
                        team = $"{Teams.FirstOrDefault()?.DisplayName}";
                    }

                    if (team.IsNullOrEmpty())
                    {
                        team = Filter.TeamId.IsNotNullOrEmpty() ? $"{Teams.FirstOrDefault(_ => _.Id == Filter.TeamId)?.DisplayName}" : string.Empty;
                    }

                    var fileName = $"{Filter.Month}.{Filter.Year}_{team}_{input.AppUser.UserName}_MonthlyStaffEvaluationDetails.xlsx";
                    var excelBytes = ExportHelper.GeneratePostsExcelBytes(L, posts, fileName);

                    await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
                }
            }
        }
        
        private async Task ExportAffiliateUserStaffEvaluation(StaffEvaluationWithNavigationPropertiesDto input)
        {
            var (fromDateTime, toDateTime) = GetPayrollDateTime(input.StaffEvaluation.Year, input.StaffEvaluation.Month);
            if (input.AppUser != null)
            {
                var posts = await StaffEvaluationAppService.GetAffiliatesEvaluationExport
                (
                    new GetAffiliateEvaluationRequest()
                    {
                        UserId = input.AppUser.Id,
                        FromDateTime = fromDateTime,
                        ToDateTime = toDateTime
                    }
                );
                if (Filter.Month > 0)
                {
                    var team = string.Empty;
                    if (Teams.Count == 1)
                    {
                        team = $"{Teams.FirstOrDefault()?.DisplayName}";
                    }

                    if (team.IsNullOrEmpty())
                    {
                        team = Filter.TeamId.IsNotNullOrEmpty() ? $"{Teams.FirstOrDefault(_ => _.Id == Filter.TeamId)?.DisplayName}" : string.Empty;
                    }

                    var fileName = $"{Filter.Month}.{Filter.Year}_{team}_{input.AppUser.UserName}_MonthlyStaffEvaluationDetails.xlsx";

                    await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(posts));
                }
            }
        }

        private async Task ExportTiktokUserStaffEvaluation(StaffEvaluationWithNavigationPropertiesDto input)
        {
            if (input.AppUser != null && input.OrganizationUnit != null)
            {
                var tikTokChannelEvaluations = await StaffEvaluationAppService.GetEvaluationTiktokChannelExport(new ExportTiktokEvaluationRequest()
                {
                    Year = input.StaffEvaluation.Year,
                    Month = input.StaffEvaluation.Month,
                    UserId = input.AppUser.Id,
                    TeamName = input.OrganizationUnit.DisplayName
                });

                var fileName = $"{input.StaffEvaluation.Month}.{input.StaffEvaluation.Year}_{input.OrganizationUnit.DisplayName}_{input.AppUser.UserName}_MonthlyStaffEvaluationDetails.xlsx";
                var excelBytes = ExportHelper.GenerateTiktokStaffEvalExcelBytes(L, tikTokChannelEvaluations, fileName);

                await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
            }
        }
        
        private async Task ExportContractUserStaffEvaluation(StaffEvaluationWithNavigationPropertiesDto input)
        {
            if (input.AppUser != null)
            {
                var (fromTime, toTime) = GetPayrollDateTime(input.StaffEvaluation.Year, input.StaffEvaluation.Month);
                var contractEvaluationExports = await StaffEvaluationAppService.GetContractEvaluationExport(new GetContractEvaluationRequest()
                {
                    SalePersonId = input.AppUser.Id,
                    FromDateTime = fromTime,
                    ToDateTime = toTime,
                });

                var fileName = $"{input.StaffEvaluation.Month}.{input.StaffEvaluation.Year}_{input.OrganizationUnit?.DisplayName}_{input.AppUser.UserName}_MonthlyStaffEvaluationDetails.xlsx";
                var excelBytes = ExportHelper.GenerateTiktokStaffEvalExcelBytes(L, contractEvaluationExports, fileName); //Todoo Long

                await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(excelBytes));
            }
        }
        
        public async Task OnDirectorReviewContentChanged()
        {
            ReviewContentEdit = await richTextEditRef.GetTextAsync();
        }

        public async Task OnAssignedTasksContentChanged()
        {
            AssignedContentEdit = await richTextTaskEditRef.GetTextAsync();
        }

        private string GetFullName([CanBeNull] IdentityUserDto user)
        {
            return user == null ? string.Empty : $"{user.Surname} {user.Name} ({user.UserName})";
        }

        private async Task OnSelectedYearChanged(int value)
        {
            Filter.Year = value;
            await GetStaffEvaluationsAsync();
        }

        private async Task OnSelectedMonthChanged(int value)
        {
            Filter.Month = value;
            await GetStaffEvaluationsAsync();
        }

        private async Task OnSelectedTeamChanged(Guid? value)
        {
            _selectedTeamId = value;
            Filter.TeamId = _selectedTeamId.IsNullOrEmpty() ? null : _selectedTeamId;


            await GetStaffEvaluationsAsync();
        }

        private Task OpenDeleteStaffEvaluation(Guid id)
        {
            DeleteEvaluationModal.Show();
            EditingStaffEvaluationId = id;
            return Task.CompletedTask;
        }

        private async Task DeleteStaffEvaluation()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await StaffEvaluationAppService.DeleteAsync(EditingStaffEvaluationId);
                    await GetStaffEvaluationsAsync();
                    ChannelEvaluationModal.Hide();
                    EditStaffEvaluationModal.Hide();
                    DeleteEvaluationModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Delete
            );
        }

        public Task OnBonusAmountChange(string bonusAmount)
        {
            EditingStaffEvaluation.BonusAmount = bonusAmount.ToDecimalOrDefault();
            return Task.CompletedTask;
        }  
        public Task OnFinesAmountChange(string FinesAmount)
        {
            EditingStaffEvaluation.FinesAmount = FinesAmount.ToDecimalOrDefault();
            return Task.CompletedTask;
        }
        
        public void OnSaleKPIAmountChange(string amount)
        {
            if (amount.IsNotNullOrEmpty())
            {
                EditingStaffEvaluation.SaleKPIAmount = amount.ToDecimalOrDefault();
            }
        }

        private async Task OpenChannelEvaluation(Guid appUserId)
        {
            await GetChannelEvaluationsAsync(appUserId);
            ChannelEvaluationModal.Show();
        }

        public void CloseChannelEvaluationModal()
        {
            TikTokChannelEvalWithNavs = new List<StaffEvaluationWithNavigationPropertiesDto>();
            modName = String.Empty;
            ChannelEvaluationModal.Hide();
        }
        
        public void CloseDeleteEvaluationModal()
        {
            DeleteEvaluationModal.Hide();
        }
    
    }
}