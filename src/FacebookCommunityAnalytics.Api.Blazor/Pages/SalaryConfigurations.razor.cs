using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.UserSalaryConfigurations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class SalaryConfigurations : BlazorComponentBase
    {
        private readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        private Guid? _selectedUserId { get; set; } = Guid.Empty;
        private List<LookupDto<Guid?>> UserLookupDtos { get; set; }
        private IReadOnlyList<OrganizationUnitDto> Teams { get; set; } = new List<OrganizationUnitDto>();
        private Guid? _selectedTeamId { get; set; } = Guid.Empty;
        private UserPosition _selectedUserPosition { get; set; }
        private List<UserPosition> ListPosition { get; set; } = new();
        private Dictionary<string, List<string>> _teamDictionary { get; set; }
        private UserSalaryConfigurationDto _createUpdateDto { get; set; } = new();
        private IReadOnlyList<UserSalaryConfigWithNavPropertiesDto> UserSalaryConfigList { get; set; }

        private int PageSize { get; set; } = 50;
        private int CurrentPage { get; set; } = 1;
        private int TotalCount { get; set; }
        private DataGridEntityActionsColumn<UserSalaryConfigWithNavPropertiesDto> EntityActionsColumn { get; set; }

        private GetUserSalaryConfigurationInput Filter { get; set; }
        private bool CanCreateUserSalaryConfig { get; set; }
        private bool CanEditUserSalaryConfig { get; set; }
        private bool CanDeleteUserSalaryConfig { get; set; }

        private UpdateUserSalaryConfigurationDto EditConfig { get; set; }
        private Guid EditingUserSalaryConfigId { get; set; }
        private Modal EditConfigModal { get; set; }

        public SalaryConfigurations()
        {
            EditConfig = new UpdateUserSalaryConfigurationDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            Teams = await StaffEvaluationAppService.GetEvaluationTeams();
            _teamDictionary = GetTeamDictionary();
            await SetBreadcrumbItemsAsync();
            await GetListUserSalaryConfig();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:HR"]));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:UserSalaryConfigurations"]));
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserSalaryConfig = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.UserSalaryConfiguration.Create);
            CanEditUserSalaryConfig = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.UserSalaryConfiguration.Edit);
            CanDeleteUserSalaryConfig = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.UserSalaryConfiguration.Delete);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserSalaryConfigWithNavPropertiesDto> e)
        {
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await GetListUserSalaryConfig();
        }

        private async Task OnSelectedValueTeam(Guid? value)
        {
            _selectedTeamId = value == Guid.Empty ? null : value;
            if (_selectedTeamId.IsNullOrEmpty()) return;

            UserLookupDtos = await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest { TeamId = _selectedTeamId });
            
            ListPosition = new List<UserPosition>();
            _selectedUserId = null;
            
            var teamName = Teams.FirstOrDefault(x => x.Id == _selectedTeamId.Value).DisplayName;
            var teamType = GetTeamType(teamName, GlobalConfiguration.TeamTypeMapping);

            switch (teamType)
            {
                case TeamType.Sale:
                    SetUserPositionTeamSale();
                    break;
                case TeamType.Content:
                    SetUserPositionTeamContent();
                    break;
                case TeamType.Affiliate:
                    SetUserPositionTeamAffiliate();
                    break;
                case TeamType.Seeding:
                    SetUserPositionTeamSeeding();
                    break;
                case TeamType.Tiktok:
                    SetUserPositionTeamTiktok();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private async Task OnSelectUserPositionChanged(UserPosition value)
        {
            _selectedUserPosition = value;
            _selectedUserId = null;
            if(value != UserPosition.Unknown) UserLookupDtos = await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest { TeamId = _selectedTeamId, UserPosition = value});
        }

        private async Task GetListUserSalaryConfig()
        {
            var result = await _userSalaryConfigurationAppService.GetListWithNavigationAsync(new GetUserSalaryConfigurationInput()
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize
            });
            
            UserSalaryConfigList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        private void SetUserPositionTeamSale()
        {
            ListPosition.Add(UserPosition.Sale);
        }

        private void SetUserPositionTeamAffiliate()
        {
            ListPosition.Add(UserPosition.CommunityAffiliateStaff);
            ListPosition.Add(UserPosition.CommunityAffiliateGroupLeader);
            ListPosition.Add(UserPosition.CommunityAffiliateLeader);
        }

        private void SetUserPositionTeamSeeding()
        {
            ListPosition.Add(UserPosition.CommunitySeedingStaff);
            ListPosition.Add(UserPosition.CommunitySeedingGroupLeader);
            ListPosition.Add(UserPosition.CommunitySeedingLeader);
            ListPosition.Add(UserPosition.CommunitySeedingLeader_ST1);
            ListPosition.Add(UserPosition.CommunitySeedingStaff_ST1);
        }

        private void SetUserPositionTeamContent()
        {
            ListPosition.Add(UserPosition.Content);
            ListPosition.Add(UserPosition.ContentIntern);
            ListPosition.Add(UserPosition.ContentStaff);
            ListPosition.Add(UserPosition.ContentExecutive);
            ListPosition.Add(UserPosition.ContentSeniorExecutive);
        }

        private void SetUserPositionTeamTiktok()
        {
            ListPosition.Add(UserPosition.Tiktok);
            ListPosition.Add(UserPosition.TiktokCreator);
            ListPosition.Add(UserPosition.TiktokCoordinator);
        }
        
        public Dictionary<string, List<string>> GetTeamDictionary()
        {
            return new Dictionary<string, List<string>>()
            {
                {"Sale", GlobalConfiguration.TeamTypeMapping.Sale},
                {"Content", GlobalConfiguration.TeamTypeMapping.Content},
                {"Tiktok", GlobalConfiguration.TeamTypeMapping.Tiktok},
                {"Affiliate", GlobalConfiguration.TeamTypeMapping.Affiliate},
                {"Seeding", GlobalConfiguration.TeamTypeMapping.Seeding}
            };
        }
        
        public TeamType GetTeamType(string teamName, TeamTypeMapping globalConfigurationTeamTypeMapping)
        {
            if (globalConfigurationTeamTypeMapping.Sale.Contains(teamName)) return TeamType.Sale;
            if (globalConfigurationTeamTypeMapping.Affiliate.Contains(teamName)) return TeamType.Affiliate;
            if (globalConfigurationTeamTypeMapping.Seeding.Contains(teamName)) return TeamType.Seeding;
            if (globalConfigurationTeamTypeMapping.Content.Contains(teamName)) return TeamType.Content;
            if (globalConfigurationTeamTypeMapping.Tiktok.Contains(teamName)) return TeamType.Tiktok;

            return TeamType.Unknown;
        }
        
        public Task OnSalaryChange(string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                _createUpdateDto.Salary = value.ToDecimalOrDefault();
            }

            return Task.CompletedTask;
        }
        
        public Task OnEditSalaryChange(string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                EditConfig.Salary = value.ToDecimalOrDefault();
            }

            return Task.CompletedTask;
        }

        private async Task CreateUserSalaryConfig()
        {
            if (_selectedTeamId.IsNullOrEmpty())
            {
                await Message.Error(L["UserSalaryConfig.PleaseChose.Team"]);
                return;   
            }
        
            if (_selectedUserId.IsNotNullOrEmpty() && _selectedUserPosition is UserPosition.Unknown or UserPosition.FilterNoSelect )
            {
                await Message.Error(L["UserSalaryConfig.PleaseChose.UserPosition"]);
                return;   
            }

            var newSalaryDto = _createUpdateDto.Clone();
            newSalaryDto.TeamId =  _selectedTeamId ?? Guid.Empty;
            newSalaryDto.UserPosition = _selectedUserPosition;
            if(_selectedUserId.IsNotNullOrEmpty())  newSalaryDto.UserId = _selectedUserId;
            newSalaryDto.Salary = _createUpdateDto.Salary;
            
            await _userSalaryConfigurationAppService.CreateOrUpdateSalaryConfig(newSalaryDto);
            _createUpdateDto = new UserSalaryConfigurationDto();
            await GetListUserSalaryConfig();
        }

        private async Task DeleteSalaryConfig(UserSalaryConfigurationDto input)
        {
            await _userSalaryConfigurationAppService.DeleteAsync(input.Id);
            await GetListUserSalaryConfig();
        }
        
        private void OpenEditUserSalaryConfig(UserSalaryConfigurationDto input)
        {
            EditingUserSalaryConfigId = input.Id;
            EditConfig = ObjectMapper.Map<UserSalaryConfigurationDto, UpdateUserSalaryConfigurationDto>(input);
            EditConfigModal.Show();
        }
        
        private void CloseEditConfigModal()
        {
            EditConfigModal.Hide();
        }
        
        private async Task UpdateUserSalaryConfig()
        {
            await _userSalaryConfigurationAppService.UpdateAsync(EditingUserSalaryConfigId, EditConfig);
            await GetListUserSalaryConfig();
            EditConfigModal.Hide();
        }
    }
}