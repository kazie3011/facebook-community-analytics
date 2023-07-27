using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using System.Linq;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IUserDomainService : IDomainService
    {
        Task<List<OrganizationUnit>> GetOrgUnitsByUser(Guid userId);

        Task SyncUserInfos();
        Task<UserInfoWithNavigationProperties> GetByCode(string userCode);
        Task<UserInfoWithNavigationProperties> GetByUserId(Guid userId);
        Task<AppUser> GetByUsername(string username);
        Task<List<AppUserDto>> GetByUsernames(List<string> usernames);
        Task<List<UserDetail>> GetUserDetails(ApiUserDetailsRequest request);
        Task<AppUser> Get(string seedingAccountFuid);
        Task<Dictionary<string, AppUser>> GetSeedingUserDic(List<string> fuids);
        Task<List<UserInfo>> GetUserInfos(List<Guid> userIds);
        Task<UserInfo> GetUserInfo(Guid userId);

        Task<Dictionary<Guid, List<string>>> GetUserSeedingAccounts();
        Task<bool> IsUserNoRole(IdentityUser identityUser);
        Task<bool> IsInRole(Guid userId, string roleConstant);
        Task<List<string>> DeactiveUserNoRole();
        Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input);
        Task<List<UserDetail>> GetMembers(GetMembersApiRequest input);
        Task<List<AppUser>> GetUsersByRole(Guid userId, string roleConstant);
        Task AssignRoleToUsers(AssignRoleRequest input);
        Task<List<Guid>> GetManagedUserIds(Guid userId);
        Task<List<AppUser>> GetTeamMembers(Guid teamId);
        Task<List<AppUser>> GetTeamMembers(List<Guid> teamIds);
        Task<List<UserInfo>> GetUsersByOrgNames(List<string> orgNames);
        Task AssignTeam(AssignTeamApiRequest apiRequest);
        Task UpdateMemberConfig(UpdateMemberConfigApiRequest apiRequest);
        Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input);
    }

    public class UserDomainService : BaseDomainService, IUserDomainService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IRepository<ApiConfiguration> _apiConfigurationRepository;
        private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly ITeamMemberDomainService _teamMemberDomainService;

        public UserDomainService(
            IdentityUserManager userManager,
            IUserInfoRepository userInfoRepository,
            IRepository<AppUser, Guid> userRepository,
            IRepository<ApiConfiguration> apiConfigurationRepository,
            IOrganizationDomainService organizationDomainService,
            IRepository<OrganizationUnit, Guid> organizationUnitRepository,
            IIdentityUserRepository identityUserRepository,
            IIdentityRoleRepository identityRoleRepository,
            ITeamMemberDomainService teamMemberDomainService)
        {
            _userManager = userManager;
            _userInfoRepository = userInfoRepository;
            _userRepository = userRepository;
            _apiConfigurationRepository = apiConfigurationRepository;
            _organizationDomainService = organizationDomainService;
            _organizationUnitRepository = organizationUnitRepository;
            _identityUserRepository = identityUserRepository;
            _identityRoleRepository = identityRoleRepository;
            _teamMemberDomainService = teamMemberDomainService;
        }


        public async Task SyncUserInfos()
        {
            await DoSyncUserInfos();
        }

        private async Task DoSyncUserInfos()
        {
            var apiConfiguration = await _apiConfigurationRepository.FirstOrDefaultAsync();
            if (apiConfiguration == null) { return; }

            var payrollConfig = apiConfiguration.PayrollConfiguration;

            var users = await _userRepository.GetListAsync();
            var baseSeedingMultiplier = payrollConfig.WaveMultiplier.Seeding_Staff_MultiplierBase;

            foreach (var user in users)
            {
                var existingUser = await _userInfoRepository.GetByUserIdAsync(user.Id);
                if (existingUser != null) continue;


                var identity = await _userManager.FindByIdAsync(user.Id.ToString());
                if (await _userManager.IsInRoleAsync(identity, RoleConsts.Leader))
                    baseSeedingMultiplier = payrollConfig.WaveMultiplier.Seeding_Leader_MultiplierBase;

                var newUserInfo = new UserInfo
                {
                    AppUserId = user.Id,
                    Code = (await _userInfoRepository.GetCurrentUserCode() + 1).ToString(),
                    ContentRoleType = ContentRoleType.Seeder,
                    JoinedDateTime = DateTime.UtcNow,
                    PromotedDateTime = DateTime.UtcNow,
                    SeedingMultiplier = (double) baseSeedingMultiplier,
                    AffiliateMultiplier = (double) payrollConfig.WaveMultiplier.AffiliateMultiplierBase,
                    IsActive = true,
                };

                await _userInfoRepository.InsertAsync(newUserInfo);
            }

            var toDeleteUserInfoIds = new List<Guid>();
            var existingUserInfos = await _userInfoRepository.GetListAsync();
            foreach (var userInfo in existingUserInfos)
            {
                if (userInfo.AppUserId == null) { toDeleteUserInfoIds.Add(userInfo.Id); }
                else
                {
                    var appUser = await _userRepository.FindAsync(userInfo.AppUserId.Value);
                    if (appUser == null || appUser.IsDeleted) { toDeleteUserInfoIds.Add(userInfo.Id); }
                }
            }

            if (toDeleteUserInfoIds.IsNotNullOrEmpty())
                await _userInfoRepository.DeleteManyAsync(toDeleteUserInfoIds, true);

            Trace.WriteLine($"================DONE {GetType().Name} at {DateTime.UtcNow}");
        }

        public async Task<UserInfoWithNavigationProperties> GetByCode(string userCode)
        {
            var userInfos = await _userInfoRepository.ToListAsync();
            var userInfo = userInfos.FirstOrDefault(info => info.Code.TrimStart('0').ToLower() == userCode.Trim().TrimStart('0').ToLower());
            if (userInfo?.AppUserId == null) return null;

            var user = await _userRepository.GetAsync(userInfo.AppUserId.Value);
            var entity = new UserInfoWithNavigationProperties
            {
                AppUser = user,
                UserInfo = userInfo
            };

            return entity;
        }

        public async Task<UserInfoWithNavigationProperties> GetByUserId(Guid userId)
        {
            var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
            if (userInfo?.AppUserId == null) return null;

            var user = await _userRepository.GetAsync(userInfo.AppUserId.Value);
            var entity = new UserInfoWithNavigationProperties
            {
                AppUser = user,
                UserInfo = userInfo
            };

            return entity;
        }

        public async Task<AppUser> GetByUsername(string username)
        {
            if (username.IsNullOrEmpty()) return null;

            var user = await _userRepository.FindAsync(x => x.UserName.ToLower() == username.ToLower());

            return user;
        }

        public async Task<List<AppUserDto>> GetByUsernames(List<string> usernames)
        {
            if (usernames.IsNullOrEmpty()) return default;

            var users = await _userRepository.GetListAsync(x => usernames.Contains(x.UserName));

            return ObjectMapper.Map<List<AppUser>, List<AppUserDto>>(users);
        }

        public async Task<List<UserDetail>> GetUserDetails(ApiUserDetailsRequest request)
        {
            var users = _userRepository.WhereIf(request.UserIds.IsNotNullOrEmpty(), u => request.UserIds.Contains(u.Id)).ToList();
            var ids = users.Select(_ => _.Id).ToList();
            var userInfos = _userInfoRepository.WhereIf(users.IsNotNullOrEmpty(), _ => ids.Contains(_.AppUserId.Value)).ToList();

            var userDetails = new List<UserDetail>();

            var orgUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest() {IsGDLNode = null});
            if (request.TeamId.IsNotNullOrEmpty()) orgUnits = orgUnits.Where(_ => _.Id == request.TeamId).ToList();

            if (request.GetSystemUsers.HasValue) userInfos = userInfos.Where(_ => _.IsSystemUser == request.GetSystemUsers).ToList();
            if (request.GetActiveUsers.HasValue) userInfos = userInfos.Where(_ => _.IsActive == request.GetActiveUsers).ToList();
            if (request.GetForPayrollCalculation.HasValue) userInfos = userInfos.Where(_ => _.EnablePayrollCalculation == request.GetForPayrollCalculation).ToList();

            foreach (var userInfo in userInfos)
            {
                var identity = await _userManager.FindByIdAsync(userInfo.AppUserId.ToString());
                if (identity == null) continue;

                var user = users.FirstOrDefault(_ => _.Id == userInfo.AppUserId);
                if (user == null) continue;

                var isValid = true;

                var orgIds = identity.OrganizationUnits.Select(_ => _.OrganizationUnitId).ToList();
                var teams = orgUnits.Where(_ => orgIds.Contains(_.Id)).ToList();

                if (request.GetTeamUsers.HasValue)
                {
                    isValid = request.GetTeamUsers == teams.IsNotNullOrEmpty();
                }

                if (isValid)
                {
                    userDetails.Add
                    (
                        new UserDetail
                        {
                            Identity = identity,
                            User = user,
                            Info = userInfo,
                            Teams = teams
                        }
                    );
                }
            }

            //Debug.WriteLine($"{GetType().Name}.GetUserDetails - {userDetails.Count}");

            return userDetails;
        }

        public async Task<AppUser> Get(string seedingAccountFuid)
        {
            if (seedingAccountFuid.IsNullOrWhiteSpace())
            {
                return null;
            }

            seedingAccountFuid = seedingAccountFuid.Trim();

            var userInfo = await _userInfoRepository.FirstOrDefaultAsync(x => x.Accounts != null && x.Accounts.Any(a => a.Fid.Contains(seedingAccountFuid)));
            if (userInfo == null) return null;

            return await _userRepository.FirstOrDefaultAsync(_ => _.Id == userInfo.AppUserId);
        }

        public async Task<Dictionary<string, AppUser>> GetSeedingUserDic(List<string> fuids)
        {
            if (fuids.IsNullOrEmpty()) return new Dictionary<string, AppUser>();

            fuids = fuids.Select(s => s.Trim()).Distinct().ToList();
            var seedingUserInfos = await _userInfoRepository.GetListAsync(x => x.Accounts != null);
            seedingUserInfos = seedingUserInfos
                .Where(ui => ui.Accounts.Any(acc => FacebookHelper.GetProfileFacebookId(acc.Fid).IsIn(fuids)))
                .ToList();
            var seedingUsers = (await _userRepository.GetListAsync())
                .Where(u => u?.Id != null && seedingUserInfos.Any(info => info.AppUserId == u.Id))
                .ToList();

            var dic = new Dictionary<string, AppUser>();
            foreach (var userInfo in seedingUserInfos)
            {
                var foundFuid = fuids.FirstOrDefault(s => userInfo.Accounts.Any(account => FacebookHelper.GetProfileFacebookId(account.Fid) == s));
                var user = seedingUsers.FirstOrDefault(appUser => appUser.Id == userInfo.AppUserId);

                if (foundFuid is not null && foundFuid.IsNotNullOrWhiteSpace() && user is not null && !dic.ContainsKey(foundFuid))
                {
                    dic.Add(foundFuid, user);
                }
            }

            return dic;
        }

        public async Task<Dictionary<Guid, List<string>>> GetUserSeedingAccounts()
        {
            var userinfos = await GetUserInfos();
            var dic = userinfos.ToDictionary(i => i.AppUserId.Value, i => i.Accounts.Select(acc => acc.Fid).ToList());

            return dic;
        }

        private async Task<List<UserInfo>> GetUserInfos(bool? isSystemUser = false)
        {
            var users = _userInfoRepository.WhereIf(isSystemUser != null, i => i.IsActive && i.IsSystemUser == isSystemUser).ToList();
            return (await _userInfoRepository.GetListAsync(i => i.IsActive && i.IsSystemUser == isSystemUser)).ToList();
        }

        public async Task<List<string>> DeactiveUserNoRole()
        {
            var users = await _userRepository.GetListAsync();
            var usersNoRole = new List<AppUser>();
            foreach (var appUser in users)
            {
                var identityUser = await _userManager.GetByIdAsync(appUser.Id);

                if (await IsUserNoRole(identityUser))
                {
                    var userInfo = await _userInfoRepository.FindAsync(u => u.Id == identityUser.Id);
                    if (userInfo == null
                        || !userInfo.IsActive
                        || userInfo.IsSystemUser) continue;

                    DateTime lockoutEndDate = DateTime.Now.AddYears(10);

                    await _userManager.SetLockoutEnabledAsync(identityUser, true);
                    await _userManager.SetLockoutEndDateAsync(identityUser, lockoutEndDate);
                    usersNoRole.Add(appUser);
                }
            }

            return usersNoRole != null ? usersNoRole.Select(u => u.UserName).ToList() : null;
        }

        public async Task<bool> IsUserNoRole(IdentityUser identityUser)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(identityUser);
                if (roles.IsNullOrEmpty()) return true;
            }
            catch (ArgumentNullException) { return true; }

            return false;
        }

        public async Task<bool> IsInRole(Guid userId, string roleConstant)
        {
            var identityUser = await _userManager.GetByIdAsync(userId);
            if (identityUser == null) return false;
            var roles = await _userManager.GetRolesAsync(identityUser);
            if (roles.IsNullOrEmpty()) return false;
            return roleConstant.IsIn(roles);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input)
        {
            var userDetails = await GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetSystemUsers = false,
                    GetActiveUsers = true,
                }
            );
            userDetails = userDetails.Where(u => u.Info.IsActive).ToList();
            var userIds = userDetails.Select(_ => _.User.Id).ToList();
            var users = await _userRepository.GetListAsync(_ => userIds.Contains(_.Id));

            users = users.WhereIf
                (
                    input.Filter.IsNotNullOrWhiteSpace(),
                    user => user.UserName.ToLower().RemoveDiacritics().Contains(input.Filter.ToLower().RemoveDiacritics())
                )
                .OrderBy(_ => _.UserName)
                .ToList();

            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = users.Count,
                Items = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(users)
            };
        }

        public async Task<List<OrganizationUnit>> GetOrgUnitsByUser(Guid userId)
        {
            var identityUser = await _userManager.FindByIdAsync(userId.ToString());
            if (identityUser?.OrganizationUnits == null) return new List<OrganizationUnit>();

            var orgUnits = await _userManager.GetOrganizationUnitsAsync(identityUser);
            return orgUnits.IsNullOrEmpty() ? new List<OrganizationUnit>() : orgUnits.DistinctBy(x => x.Id).ToList();
        }

        public async Task<List<UserDetail>> GetMembers(GetMembersApiRequest input)
        {
            var users = await GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetActiveUsers = input.IsActiveUser,
                    GetSystemUsers = false,
                }
            );
            if (users.IsNullOrEmpty()) return new List<UserDetail>();

            if (input.TeamId != null)
            {
                users = input.TeamId == Guid.Empty
                    ? users.Where(u => u.Teams.IsNullOrEmpty()).ToList()
                    : users.Where(u => u.Teams.Contains(t => t.Id == input.TeamId.Value)).ToList();
            }

            if (input.DepartmentId != null)
            {
                var organizations = await _organizationUnitRepository.GetListAsync(x => x.ParentId == input.DepartmentId.Value);
                users = users.Where(x => x.Team == null || organizations.Select(org => org.Id).Contains(x.Team.Id)).ToList();
            }

            if (input.FilterText.IsNotNullOrEmpty())
            {
                users = users.Where
                    (
                        _ => _.User.UserName.ToLower().Trim().Contains(input.FilterText.ToLower().Trim())
                             || (_.User.Name.IsNotNullOrEmpty() && _.User.Name.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.User.Surname.IsNotNullOrEmpty() && _.User.Surname.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.User.Email.IsNotNullOrEmpty() && _.User.Email.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.User.PhoneNumber.IsNotNullOrEmpty() && _.User.PhoneNumber.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.Info.Code.IsNotNullOrEmpty() && _.Info.Code.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.Info.UserPosition.ToString().ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                    )
                    .ToList();
            }

            return users;
        }

        /// <summary>
        /// Get users managed by "userId", usually userId = leader user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Guid>> GetManagedUserIds(Guid userId)
        {
            var identityUser = await _userManager.GetByIdAsync(userId);
            if (identityUser == null) return new List<Guid>();

            if (await _userManager.IsInRoleAsync(identityUser, RoleConsts.Leader))
            {
                var orgUnits = await _userManager.GetOrganizationUnitsAsync(identityUser);

                List<Guid> users = new();
                foreach (var organizationUnit in orgUnits)
                {
                    var u = await _userManager.GetUsersInOrganizationUnitAsync(organizationUnit, true);
                    users.AddRange(u.Select(x => x.Id));
                }

                users = users.Distinct().ToList();
                return users;
            }

            return new List<Guid> {userId};
        }

        /// <summary>
        /// Get users with specified role "roleConstant", in the same organization with "userId"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleConstant"></param>
        /// <returns></returns>
        public async Task<List<AppUser>> GetUsersByRole(Guid userId, string roleConstant)
        {
            var identityUser = await _userManager.FindByIdAsync(userId.ToString());
            if (identityUser == null) return new List<AppUser>();
            if (identityUser.OrganizationUnits == null) return new List<AppUser>();

            var orgUnits = await _userManager.GetOrganizationUnitsAsync(identityUser);
            if (orgUnits.IsNullOrEmpty()) return new List<AppUser>();

            List<AppUser> leaders = new();
            foreach (var organizationUnit in orgUnits)
            {
                var identityUsers = await _userManager.GetUsersInOrganizationUnitAsync(organizationUnit, true);
                foreach (var user in identityUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, roleConstant))
                    {
                        var u = await _userRepository.GetAsync(user.Id);
                        leaders.Add(u);
                    }
                }
            }

            return leaders;
        }

        public async Task<List<AppUser>> GetTeamMembers(List<Guid> teamIds)
        {
            var users = new List<AppUser>();
            foreach (var orgUnitId in teamIds)
            {
                users.AddRange(await GetTeamMembers(orgUnitId));
            }

            return users;
        }

        public async Task<List<UserInfo>> GetUsersByOrgNames(List<string> orgNames)
        {
            var results = new List<UserInfo>();
            foreach (var orgName in orgNames)
            {
                var orgUnit = await _organizationUnitRepository.GetAsync(_ => _.DisplayName == orgName);
                if (orgUnit == null) continue;

                var identityUsers = await _userManager.GetUsersInOrganizationUnitAsync(orgUnit);
                var identityUserIds = identityUsers.Select(_ => _.Id);

                results.AddRange(await _userInfoRepository.GetListAsync(x => x.AppUserId.HasValue && identityUserIds.Contains(x.AppUserId.Value)));
            }

            return results.DistinctBy(x => x.AppUserId).ToList();
        }

        public async Task AssignRoleToUsers(AssignRoleRequest request)
        {
            var users = await _userRepository.GetListAsync();
            var identityUsers = users.Select(async _ => await _userManager.FindByIdAsync(_.Id.ToString())).ToList();

            foreach (var u in identityUsers) { await _userManager.SetRolesAsync(await u, new[] {request.RoleConst}); }
        }

        public async Task AssignTeam(AssignTeamApiRequest apiRequest)
        {
            var identityUsers = await GetIdentityUsers(apiRequest.UserIds);
            if (identityUsers.IsNullOrEmpty()) return;
            
            //Check allow remove org 
            if (!await CheckAllowRemoveOrg(identityUsers))
            {
                throw new UserFriendlyException(LD["ApiDomain:UserRoleNotRemoveOrg"]);
            }

            var teamDictionary = GetTeamDictionary();
            foreach (var user in identityUsers)
            {
                var orgGuids = user.OrganizationUnits.Select(x => x.OrganizationUnitId).ToList();
                foreach (var orgGuid in orgGuids) { user.RemoveOrganizationUnit(orgGuid); }

                if (apiRequest.IsTeamAssigned
                    && apiRequest.OrganizationUnitId != null
                    && apiRequest.OrganizationUnitId.Value != Guid.Empty)
                {
                    user.AddOrganizationUnit(apiRequest.OrganizationUnitId.Value);
                    var organizationUnit = _organizationUnitRepository.FirstOrDefault(x => x.Id == apiRequest.OrganizationUnitId.Value);
                    var userInfo = await _userInfoRepository.GetByUserIdAsync(user.Id);
                    if (userInfo == null || organizationUnit == null) continue;
                    var teamType = teamDictionary.FirstOrDefault(x => x.Value.Contains(organizationUnit.DisplayName)).Key;
                    userInfo.UserPosition = GetUserPosition(teamType);
                    userInfo.MainTeamId = null;
                    await _userInfoRepository.UpdateAsync(userInfo);
                }
            }

            await _identityUserRepository.UpdateManyAsync(identityUsers);
        }

        private async Task<bool> CheckAllowRemoveOrg(List<IdentityUser> identityUsers)
        {
            foreach (var user in identityUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (RoleConsts.Manager.IsIn(roles) 
                    ||RoleConsts.Director.IsIn(roles)
                    || RoleConsts.Admin.IsIn(roles))
                {
                    return false;
                }
            }

            return true;
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

        public UserPosition GetUserPosition(string teamType)
        {
            var userPosition = UserPosition.Unknown;
            switch (teamType)
            {
                case "Sale":
                    userPosition = UserPosition.Sale;
                    break;
                case "Content":
                    userPosition = UserPosition.Content;
                    break;
                case "Tiktok":
                    userPosition = UserPosition.Tiktok;
                    break;
                case "Affiliate":
                    userPosition = UserPosition.CommunityAffiliateStaff;
                    break;
                case "Seeding":
                    userPosition = UserPosition.CommunitySeedingStaff;
                    break;
            }

            return userPosition;
        }

        private async Task<List<IdentityUser>> GetIdentityUsers(List<Guid> userIds)
        {
            if (userIds.IsNullOrEmpty()) return new List<IdentityUser>();
            var identityUsers = await _identityUserRepository.GetListAsync();
            return identityUsers.Where(_ => _.Id.IsIn(userIds)).ToList();
        }

        public async Task UpdateMemberConfig(UpdateMemberConfigApiRequest apiRequest)
        {
            var userInfos = await GetUserInfos(apiRequest.UserIds);
            if (userInfos.IsNullOrEmpty()) return;

            if (apiRequest.IsSystemUsers != null)
            {
                userInfos = userInfos.Select
                    (
                        _ =>
                        {
                            _.IsSystemUser = apiRequest.IsSystemUsers.Value;
                            return _;
                        }
                    )
                    .ToList();
            }

            if (apiRequest.IsGDLUser != null)
            {
                userInfos = userInfos.Select
                    (
                        _ =>
                        {
                            _.IsGDLStaff = apiRequest.IsGDLUser.Value;
                            return _;
                        }
                    )
                    .ToList();
            }

            if (apiRequest.IsCalculatePayrollUsers != null)
            {
                userInfos = userInfos.Select
                    (
                        _ =>
                        {
                            _.EnablePayrollCalculation = apiRequest.IsCalculatePayrollUsers.Value;
                            return _;
                        }
                    )
                    .ToList();
            }

            if (apiRequest.IsActiveUsers != null)
            {
                var users = await _identityUserRepository.GetListAsync(_ => apiRequest.UserIds.Contains(_.Id));
                var roles = await _identityRoleRepository.GetListAsync();
                var guestRoleId = roles.FirstOrDefault(_ => _.Name == RoleConsts.Guest)?.Id;
                foreach (var user in users)
                {
                    foreach (var org in user.OrganizationUnits.ToList())
                    {
                        user.RemoveOrganizationUnit(org.OrganizationUnitId);
                    }

                    foreach (var role in user.Roles.ToList())
                    {
                        user.RemoveRole(role.RoleId);
                    }

                    if (guestRoleId.IsNotNullOrEmpty()) user.AddRole((Guid) guestRoleId);
                }

                userInfos = userInfos.Select
                    (
                        _ =>
                        {
                            _.IsActive = apiRequest.IsActiveUsers.Value;
                            return _;
                        }
                    )
                    .ToList();
            }

            await _userInfoRepository.UpdateManyAsync(userInfos);
        }

        public async Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input)
        {
            return await _teamMemberDomainService.GetAppUserLookupAsync(input);
        }

        public async Task<List<UserInfo>> GetUserInfos(List<Guid> userIds)
        {
            if (userIds.IsNullOrEmpty()) return new List<UserInfo>();
            var userInfos = await _userInfoRepository.GetListWithNavigationPropertiesExtendAsync();
            return userInfos.Where(_ => _.AppUser.Id.IsIn(userIds)).Select(_ => _.UserInfo).ToList();
        }

        public async Task<UserInfo> GetUserInfo(Guid userId)
        {
            return await _userInfoRepository.FindAsync(x => x.AppUserId != null && x.AppUserId == userId);
        }

        public async Task<List<AppUser>> GetTeamMembers(Guid teamId)
        {
            var org = await _organizationUnitRepository.FindAsync(teamId);
            if (org == null) return new List<AppUser>();

            var identityUsers = await _userManager.GetUsersInOrganizationUnitAsync(org);
            var identityUserIds = identityUsers.Select(_ => _.Id);

            return await _userRepository.GetListAsync(_ => identityUserIds.Contains(_.Id));
        }
    }

    public class UserDetail
    {
        public OrganizationUnit Team
        {
            get
            {
                var team = Info.MainTeamId.IsNullOrEmpty() ? Teams.FirstOrDefault() : Teams.FirstOrDefault(_ => _.Id == Info.MainTeamId);
                return team ?? new OrganizationUnit();
            }
        }

        public List<OrganizationUnit> Teams { get; set; }
        public AppUser User { get; set; }
        public UserInfo Info { get; set; }
        public IdentityUser Identity { get; set; }

        public UserDetail()
        {
            Teams = new List<OrganizationUnit>();
        }
    }

    public class UserEvaluationDetail
    {
        public OrganizationUnit Team { get; set; }
        public AppUser User { get; set; }
        public UserInfo Info { get; set; }
        public IdentityUser Identity { get; set; }
    }
}