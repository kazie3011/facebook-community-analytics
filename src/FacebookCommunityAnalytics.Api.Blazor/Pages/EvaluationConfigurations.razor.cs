using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using JetBrains.Annotations;
using Volo.Abp.Identity;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class EvaluationConfigurations : BlazorComponentBase
    {
        private bool _showLoading { get; set; }
        private readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        private string _selectedTab = "TabSale";

        private UserEvaluationConfigurationDto _evalConfigDto = new();
        private UserEvaluationConfigurationCreateUpdateDto EditingEvaluationConfiguration { get; set; } = new();
        private Guid EditingEvaluationConfigurationId { get; set; }

        private GetStaffEvaluationsInput Filter { get; set; } = new GetStaffEvaluationsInput();

        private IReadOnlyList<int> Months = Enumerable.Range(1, 12).ToList();
        private IReadOnlyList<int> Years = Enumerable.Range(DateTime.UtcNow.Year - 5, 10).ToList();
        private IReadOnlyList<OrganizationUnitDto> Teams { get; set; } = new List<OrganizationUnitDto>();
        private IReadOnlyList<OrganizationUnitDto> FilterTeams { get; set; } = new List<OrganizationUnitDto>();
        private List<UserPosition> ListPosition { get; set; } = new();
        private List<UserEvaluationConfigurationDto> UserEvaluations { get; set; } = new List<UserEvaluationConfigurationDto>();
        private UserPosition _selectedUserPosition { get; set; }
        private Guid? _selectedTeamId { get; set; } = Guid.Empty;
        private Guid? _selectedUserId { get; set; } = Guid.Empty;

        private bool isLoading { get; set; }
        private Modal EditSaleModal { get; set; }
        private Modal EditTiktokModal { get; set; }
        private Modal EditContentModal { get; set; }
        private Modal EditSeedingAndAffiliateModal { get; set; }
        private Dictionary<string, List<string>> _teamDictionary { get; set; }
        private Modal ConfirmationModal { get; set; }
        private string ConfirmModalTitle { get; set; }
        private List<LookupDto<Guid?>> UserLookupDtos { get; set; }

        public EvaluationConfigurations()
        {
            UserLookupDtos = new List<LookupDto<Guid?>>();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            _evalConfigDto = await UserEvaluationConfigurationAppService.GetEvalConfig(Guid.Empty);

            await SetBreadcrumbItemsAsync();
            await SetSelectedTab();
            Teams = await StaffEvaluationAppService.GetEvaluationTeams();
            FilterTeams = Teams.Where(x => GlobalConfiguration.TeamTypeMapping.Sale.Contains(x.DisplayName)).ToList();
            var teamIds = FilterTeams.Select(x => x.Id).ToList();
            await ReloadCustomConfigTable(teamIds);
            ListPosition.Add(UserPosition.Unknown);
            ListPosition.Add(UserPosition.Sale);
            _teamDictionary = GetTeamDictionary();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage();
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:HR"]));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:EvaluationConfigurations"]));
            return ValueTask.CompletedTask;
        }

        private async Task ReloadCustomConfigTable(List<Guid> teamIds)
        {
            var request = new GetUserEvaluationConfigurationsInput();
            if (_selectedTeamId.IsNotNullOrEmpty())
            {
                request.TeamId = _selectedTeamId; 
            }
            else
            {
                request.TeamIds = teamIds;
                UserLookupDtos = await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest {TeamIds = teamIds});
            }

            UserEvaluations = await UserEvaluationConfigurationAppService.GetUserEvaluationConfigurations(request);
        }

        private async Task SaveRootEvalConfig()
        {
            _showLoading = true;
            _evalConfigDto = await UserEvaluationConfigurationAppService.CreateOrUpdateRootConfig(_evalConfigDto);

            var teamIds = FilterTeams.Select(x => x.Id).ToList();
            await ReloadCustomConfigTable(teamIds);
            _showLoading = false;
        }

        private async Task CreateCustomEvalConfig()
        {
            if (_selectedTeamId.IsNullOrEmpty())
            {
                await Message.Error(L["StaffEvaluations.PleaseChoseTeam"]);
                return;
            }

            _showLoading = true;
            var newEvalConfigDto = _evalConfigDto.Clone();
            newEvalConfigDto.Id = Guid.Empty;
            newEvalConfigDto.TeamId = _selectedTeamId;
            newEvalConfigDto.UserPosition = _selectedUserPosition;
            newEvalConfigDto.AppUserId = _selectedUserId;
            _evalConfigDto = await UserEvaluationConfigurationAppService.CreateOrUpdateCustomConfig(newEvalConfigDto);
            
            var teamIds = FilterTeams.Select(x => x.Id).ToList();
            await ReloadCustomConfigTable(teamIds);
            
            _showLoading = false;
        }

        public async Task UpdateCustomEvalConfig()
        {
            await UserEvaluationConfigurationAppService.UpdateAsync(EditingEvaluationConfigurationId, EditingEvaluationConfiguration);

            var teamIds = FilterTeams.Select(x => x.Id).ToList();
            await ReloadCustomConfigTable(teamIds);
            
            EditSaleModal.Hide();
            EditTiktokModal.Hide();
            EditContentModal.Hide();
            EditSeedingAndAffiliateModal.Hide();
        }

        private async Task OnSelectedTabChanged(string name)
        {
            _selectedTab = name;
            _selectedTeamId = Guid.Empty;
            _selectedUserId = Guid.Empty;
            FilterTeams = new List<OrganizationUnitDto>();
            ListPosition = new List<UserPosition>();
            UserEvaluations = new List<UserEvaluationConfigurationDto>();
            UserLookupDtos = new List<LookupDto<Guid?>>();

            switch (name)
            {
                case "TabSale":
                    FilterTeams = Teams.Where(x => _teamDictionary["Sale"].Contains(x.DisplayName)).ToList();
                    SetUserPositionTeamSale();
                    var teamSaleIds = FilterTeams.Select(x => x.Id).ToList();
                    await ReloadCustomConfigTable(teamSaleIds);
                    break;
                case "TabTiktok":
                    FilterTeams = Teams.Where(x => _teamDictionary["Tiktok"].Contains(x.DisplayName)).ToList();
                    SetUserPositionTeamTiktok();
                    var teamTiktokIds = FilterTeams.Select(x => x.Id).ToList();
                    await ReloadCustomConfigTable(teamTiktokIds);

                    break;
                case "TabContent":
                    SetUserPositionTeamContent();
                    FilterTeams = Teams.Where(x => _teamDictionary["Content"].Contains(x.DisplayName)).ToList();
                    var teamContentIds = FilterTeams.Select(x => x.Id).ToList();
                    await ReloadCustomConfigTable(teamContentIds);

                    break;
                case "TabCommunityConfiguration":

                    FilterTeams = Teams.Where(x => _teamDictionary["Affiliate"].Contains(x.DisplayName) || _teamDictionary["Seeding"].Contains(x.DisplayName)).ToList();
                    if (_selectedTeamId.IsNullOrEmpty()) SetUserPositionTeamAffiliate();
                    var teamCommunityIds = FilterTeams.Select(x => x.Id).ToList();
                    await ReloadCustomConfigTable(teamCommunityIds);

                    break;
                default:
                    break;
            }
        }


        private async Task SetSelectedTab()
        {
            if (await PermissionChecker.IsGrantedAsync(ApiPermissions.UserEvaluationConfigurations.ConfigSale))
            {
                _selectedTab = "TabSale";
            }
            else if (await PermissionChecker.IsGrantedAsync(ApiPermissions.UserEvaluationConfigurations.ConfigTiktok))
            {
                _selectedTab = "TabTiktok";
            }
            else if (await PermissionChecker.IsGrantedAsync(ApiPermissions.UserEvaluationConfigurations.ConfigContent))
            {
                _selectedTab = "TabContent";
            }
            else if (await PermissionChecker.IsGrantedAsync(ApiPermissions.UserEvaluationConfigurations.ConfigCommunity))
            {
                _selectedTab = "TabCommunityConfiguration";
            }
        }

        public Task OnContractAmountChange(string contractAmount)
        {
            if (contractAmount.IsNotNullOrEmpty())
            {
                _evalConfigDto.Sale.ContractAmountKPI = contractAmount.ToDecimalOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnEditContractAmountChange(string contractAmount)
        {
            if (contractAmount.IsNotNullOrEmpty())
            {
                EditingEvaluationConfiguration.Sale.ContractAmountKPI = contractAmount.ToDecimalOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnEditPaidContractAmountChange(string paidContractAmountKPI)
        {
            if (paidContractAmountKPI.IsNotNullOrEmpty())
            {
                EditingEvaluationConfiguration.Sale.PaidContractAmountKPI = paidContractAmountKPI.ToDecimalOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnPaidContractAmountKPIChange(string paidContractAmount)
        {
            if (paidContractAmount.IsNotNullOrEmpty())
            {
                _evalConfigDto.Sale.PaidContractAmountKPI = paidContractAmount.ToDecimalOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnTiktokVideoPerMonthChange(string tiktokVideoPerMonth)
        {
            if (tiktokVideoPerMonth.IsNotNullOrEmpty())
            {
                _evalConfigDto.Tiktok.TiktokVideoPerMonth = tiktokVideoPerMonth.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnTiktokAverageVideoViewChange(string tiktokAverageView)
        {
            if (tiktokAverageView.IsNotNullOrEmpty())
            {
                _evalConfigDto.Tiktok.TiktokAverageVideoView = tiktokAverageView.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnEditTiktokVideoPerMonthChange(string tiktokVideoPerMonth)
        {
            if (tiktokVideoPerMonth.IsNotNullOrEmpty())
            {
                EditingEvaluationConfiguration.Tiktok.TiktokVideoPerMonth = tiktokVideoPerMonth.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnEditTiktokAverageVideoViewChange(string tiktokAverageView)
        {
            if (tiktokAverageView.IsNotNullOrEmpty())
            {
                EditingEvaluationConfiguration.Tiktok.TiktokAverageVideoView = tiktokAverageView.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnContentMinimumPostReactionsChange(string MinimumPostInteractions)
        {
            if (MinimumPostInteractions.IsNotNullOrEmpty())
            {
                _evalConfigDto.Content.MinimumPostReactions = MinimumPostInteractions.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnEditMinimumPostInteractionsChange(string MinimumPostInteractions)
        {
            if (MinimumPostInteractions.IsNotNullOrEmpty())
            {
                EditingEvaluationConfiguration.Content.MinimumPostReactions = MinimumPostInteractions.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnEditSeedingMinimumPostInteractionsChange(string postInteractions)
        {
            if (postInteractions.IsNotNullOrEmpty())
            {
                EditingEvaluationConfiguration.Seeding.MinimumPostReactions = postInteractions.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnEditAffiliateConversionCountChange(string conversionCount)
        {
            if (conversionCount.IsNotNullOrEmpty())
            {
                EditingEvaluationConfiguration.Affiliate.MinConversionCount = conversionCount.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnMinimumPostReactionsChange(string minimumPostReactions)
        {
            if (minimumPostReactions.IsNotNullOrEmpty())
            {
                _evalConfigDto.Seeding.MinimumPostReactions = minimumPostReactions.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnMinConversionChange(string MinConversionCount)
        {
            if (MinConversionCount.IsNotNullOrEmpty())
            {
                _evalConfigDto.Affiliate.MinConversionCount = MinConversionCount.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnSeedingMinimumPostReactionsChange(string minimumPostInteractions)
        {
            if (minimumPostInteractions.IsNotNullOrEmpty())
            {
                _evalConfigDto.Seeding.MinimumPostReactions = minimumPostInteractions.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public Task OnAffiliateMinConversionCountChange(string affiliateMinConversion)
        {
            if (affiliateMinConversion.IsNotNullOrEmpty())
            {
                _evalConfigDto.Affiliate.MinConversionCount = affiliateMinConversion.ToIntOrDefault();
            }

            return Task.CompletedTask;
        }

        public string GetTeamName(Guid? teamId)
        {
            return Teams.FirstOrDefault(x => x.Id == teamId)?.DisplayName;
        }

        private void SetUserPositionTeamSale()
        {
            ListPosition.Add(UserPosition.Unknown);
            ListPosition.Add(UserPosition.Sale);
        }

        private void SetUserPositionTeamAffiliate()
        {
            ListPosition.Add(UserPosition.Unknown);
            ListPosition.Add(UserPosition.CommunityAffiliateStaff);
            ListPosition.Add(UserPosition.CommunityAffiliateGroupLeader);
            ListPosition.Add(UserPosition.CommunityAffiliateLeader);
        }

        private void SetUserPositionTeamSeeding()
        {
            ListPosition.Add(UserPosition.Unknown);
            ListPosition.Add(UserPosition.CommunitySeedingStaff);
            ListPosition.Add(UserPosition.CommunitySeedingGroupLeader);
            ListPosition.Add(UserPosition.CommunitySeedingLeader);
            ListPosition.Add(UserPosition.CommunitySeedingLeader_ST1);
            ListPosition.Add(UserPosition.CommunitySeedingStaff_ST1);
        }

        private void SetUserPositionTeamContent()
        {
            ListPosition.Add(UserPosition.Unknown);
            ListPosition.Add(UserPosition.Content);
            ListPosition.Add(UserPosition.ContentIntern);
            ListPosition.Add(UserPosition.ContentStaff);
            ListPosition.Add(UserPosition.ContentExecutive);
            ListPosition.Add(UserPosition.ContentSeniorExecutive);
        }

        private void SetUserPositionTeamTiktok()
        {
            ListPosition.Add(UserPosition.Unknown);
            ListPosition.Add(UserPosition.Tiktok);
            ListPosition.Add(UserPosition.TiktokCreator);
            ListPosition.Add(UserPosition.TiktokCoordinator);
        }

        private async Task OnSelectedValueTeam(Guid? value)
        {
            _selectedTeamId = value == Guid.Empty ? null : value;
            if (_selectedTeamId.IsNotNullOrEmpty())
            {
                UserEvaluations = await UserEvaluationConfigurationAppService.GetUserEvaluationConfigurations(new GetUserEvaluationConfigurationsInput {TeamId = _selectedTeamId});
            }
            else
            {
                var teamIds = FilterTeams.Select(x => x.Id).ToList();
                await ReloadCustomConfigTable(teamIds);
            }

            if (_selectedTeamId.IsNullOrEmpty()) return;

            UserLookupDtos = await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest {TeamId = _selectedTeamId});
        }

        private async Task OnSelectedCommunityTeam(Guid? value)
        {
            _selectedTeamId = value == Guid.Empty ? null : value;
            if (_selectedTeamId.IsNotNullOrEmpty())
            {
                UserEvaluations = await UserEvaluationConfigurationAppService.GetUserEvaluationConfigurations(new GetUserEvaluationConfigurationsInput {TeamId = _selectedTeamId});
            }
            else
            {
                var teamIds = FilterTeams.Select(x => x.Id).ToList();
                await ReloadCustomConfigTable(teamIds);
            }

            if (_selectedTeamId.IsNullOrEmpty()) return;
            ListPosition = new List<UserPosition>();

            var teamName = Teams.FirstOrDefault(x => x.Id == _selectedTeamId)?.DisplayName;
            var teamType = _teamDictionary.FirstOrDefault(x => x.Value.Contains(teamName)).Key;

            switch (teamType)
            {
                case "Affiliate":
                    SetUserPositionTeamAffiliate();
                    UserLookupDtos = await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest {TeamId = _selectedTeamId});

                    break;
                case "Seeding":
                    SetUserPositionTeamSeeding();
                    UserLookupDtos = await TeamMemberAppService.GetAppUserLookupAsync(new GetMembersApiRequest {TeamId = _selectedTeamId});

                    break;
            }
        }

        private void OpenRemoveModal(UserEvaluationConfigurationDto userEvaluationConfiguration)
        {
            ConfirmModalTitle = $"{GetTeamName(userEvaluationConfiguration.TeamId)} - {userEvaluationConfiguration.UserPosition} ";
            ConfirmationModal.Show();
            EditingEvaluationConfigurationId = userEvaluationConfiguration.Id;
        }

        private void CloseConfirmationModal()
        {
            ConfirmationModal.Hide();
        }

        private async Task DeleteEvalConfig()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await UserEvaluationConfigurationAppService.DeleteAsync(EditingEvaluationConfigurationId);
                    var request = new GetUserEvaluationConfigurationsInput();
                    request.TeamId = _selectedTeamId;
                    UserEvaluations = await UserEvaluationConfigurationAppService.GetUserEvaluationConfigurations(request);
                    ConfirmationModal.Hide();
                },
                L,
                true,
                BlazorComponentBaseActionType.Delete
            );
        }

        private void OpenSaleEvalConfigModal(UserEvaluationConfigurationDto input)
        {
            EditingEvaluationConfigurationId = input.Id;
            EditingEvaluationConfiguration = ObjectMapper.Map<UserEvaluationConfigurationDto, UserEvaluationConfigurationCreateUpdateDto>(input);
            EditSaleModal.Show();
        }

        private void CloseSaleEvalConfigModal()
        {
            EditingEvaluationConfigurationId = Guid.Empty;
            EditSaleModal.Hide();
        }

        private void OpenTiktokEvalConfigModal(UserEvaluationConfigurationDto input)
        {
            EditingEvaluationConfigurationId = input.Id;
            EditingEvaluationConfiguration = ObjectMapper.Map<UserEvaluationConfigurationDto, UserEvaluationConfigurationCreateUpdateDto>(input);
            EditTiktokModal.Show();
        }

        private void CloseTiktokEvalConfigModal()
        {
            EditingEvaluationConfigurationId = Guid.Empty;
            EditTiktokModal.Hide();
        }

        private void OpenContentEvalConfigModal(UserEvaluationConfigurationDto input)
        {
            EditingEvaluationConfigurationId = input.Id;
            EditingEvaluationConfiguration = ObjectMapper.Map<UserEvaluationConfigurationDto, UserEvaluationConfigurationCreateUpdateDto>(input);
            EditContentModal.Show();
        }

        private void CloseContentEvalConfigModal()
        {
            EditingEvaluationConfigurationId = Guid.Empty;
            EditContentModal.Hide();
        }

        private void OpenSeedingAndAffiliateEvalConfigModal(UserEvaluationConfigurationDto input)
        {
            EditingEvaluationConfigurationId = input.Id;
            EditingEvaluationConfiguration = ObjectMapper.Map<UserEvaluationConfigurationDto, UserEvaluationConfigurationCreateUpdateDto>(input);
            EditSeedingAndAffiliateModal.Show();
        }

        private void CloseAffiliateEvalConfigModal()
        {
            EditingEvaluationConfigurationId = Guid.Empty;
            EditSeedingAndAffiliateModal.Hide();
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

        public string GetAppUserName(Guid id)
        {
            return UserLookupDtos.FirstOrDefault(x => x.Id == id)?.DisplayName;
        }

        public async Task EvaluateTeamSale()
        {
            isLoading = false;
            var request = new StaffEvaluationRequest
            {
                Month = Filter.Month,
                Year = Filter.Year,
                TeamType = TeamType.Sale
            };
            await StaffEvaluationAppService.EvaluateStaffs(request);
            isLoading = true;
        }

        public async Task EvaluateTeamTiktok()
        {
            isLoading = false;
            var request = new StaffEvaluationRequest
            {
                Month = Filter.Month,
                Year = Filter.Year,
                TeamType = TeamType.Tiktok
            };
            await StaffEvaluationAppService.EvaluateStaffs(request);
            isLoading = true;
        }

        public async Task EvaluateTeamContent()
        {
            isLoading = false;
            var request = new StaffEvaluationRequest
            {
                Month = Filter.Month,
                Year = Filter.Year,
                TeamType = TeamType.Content
            };
            await StaffEvaluationAppService.EvaluateStaffs(request);
            isLoading = true;
        }

        public async Task EvaluateTeamSeeding()
        {
            isLoading = false;
            var request = new StaffEvaluationRequest
            {
                Month = Filter.Month,
                Year = Filter.Year,
                TeamType = TeamType.Seeding
            };
            await StaffEvaluationAppService.EvaluateStaffs(request);
            isLoading = true;
        }

        public async Task EvaluateTeamAffiliate()
        {
            isLoading = false;
            var request = new StaffEvaluationRequest
            {
                Month = Filter.Month,
                Year = Filter.Year,
                TeamType = TeamType.Affiliate
            };
            await StaffEvaluationAppService.EvaluateStaffs(request);
            isLoading = true;
        }
    }
}