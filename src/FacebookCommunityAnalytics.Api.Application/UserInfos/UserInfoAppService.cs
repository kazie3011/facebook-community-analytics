using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Medias;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Volo.Abp.BlobStoring;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserInfos.Default)]
    public class UserInfosAppService : ApiAppService, IUserInfosAppService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;

        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IUserDomainService _userDomainService;
        private readonly IdentityUserManager _userManager;

        private readonly IRepository<UserSalary, Guid> _userSalaryRepository;
        private readonly IRepository<Media, Guid> _mediaRepository;

        private readonly IStaffEvaluationRepository _staffEvaluationRepository;
        private readonly IBlobContainer _blobContainer;

        public UserInfosAppService(
            IUserInfoRepository userInfoRepository,
            IRepository<AppUser, Guid> userRepository,
            IOrganizationDomainService organizationDomainService,
            IUserDomainService userDomainService,
            IdentityUserManager userManager,
            IRepository<UserSalary, Guid> userSalaryRepository,
            IStaffEvaluationRepository staffEvaluationRepository,
            IBlobContainer blobContainer,
            IRepository<Media, Guid> mediaRepository)
        {
            _userInfoRepository = userInfoRepository;
            _userRepository = userRepository;
            _organizationDomainService = organizationDomainService;
            _userDomainService = userDomainService;
            _userManager = userManager;
            _userSalaryRepository = userSalaryRepository;
            _staffEvaluationRepository = staffEvaluationRepository;
            _blobContainer = blobContainer;
            _mediaRepository = mediaRepository;
        }

        public virtual async Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListExtendAsync(
            GetExtendUserInfosInput input)
        {
            if (IsManagerRole())
            {
                // do nothing
            }
            else if (IsLeaderRole())
            {
                input.AppUserIds = await _userDomainService.GetManagedUserIds(CurrentUser.GetId());
            }
            else if (IsStaffRole())
            {
                input.AppUserId = CurrentUser.GetId();
            }

            //var totalCount = await _userInfoRepository.GetCountExtendAsync(input.FilterText, input.Code, input.IdentityNumber, input.Facebook, input.DateOfBirthMin, input.DateOfBirthMax, input.Fuid1, input.Fname1, input.Fuid2, input.Fname2, input.JoinedDateTimeMin, input.JoinedDateTimeMax, input.PromotedDateTimeMin, input.PromotedDateTimeMax, input.AffiliateMultiplierMin, input.AffiliateMultiplierMax, input.SeedingMultiplierMin, input.SeedingMultiplierMax, input.ContentRoleType, input.AppUserId, input.AppUserIds);
            var items = await _userInfoRepository.GetListWithNavigationPropertiesExtendAsync
            (
                input.FilterText,
                input.Code,
                input.IdentityNumber,
                input.Facebook,
                input.DateOfBirthMin,
                input.DateOfBirthMax,
                input.JoinedDateTimeMin,
                input.JoinedDateTimeMax,
                input.PromotedDateTimeMin,
                input.PromotedDateTimeMax,
                input.AffiliateMultiplierMin,
                input.AffiliateMultiplierMax,
                input.SeedingMultiplierMin,
                input.SeedingMultiplierMax,
                input.ContentRoleType,
                input.UserPosition,
                input.IsGDLStaff,
                input.IsSystemUser,
                input.EnablePayrollCalculation,
                input.IsActive,
                input.AppUserId,
                input.AppUserIds,
                input.Sorting,
                int.MaxValue,
                0
            );
            if (input.HasMainTeam.HasValue)
            {
                items = input.HasMainTeam.Value ? items.Where(_ => _.UserInfo.MainTeamId.IsNotNullOrEmpty()).ToList() : items.Where(_ => _.UserInfo.MainTeamId.IsNullOrEmpty()).ToList();
            }
            var totalCount = 0;
            // can't find user info, fallback to find user
            if (input.FilterText.IsNotNullOrWhiteSpace())
            {
                var filterText = input.FilterText.ToLower().Trim();
                var users = _userRepository
                    .WhereIf(input.AppUserIds.IsNotNullOrEmpty(), _ => input.AppUserIds.Contains(_.Id))
                    .WhereIf(input.AppUserId.IsNotNullOrEmpty(), _ => input.AppUserId == _.Id)
                    .Where(_ => _.Name.ToLower().Contains(filterText)
                                || _.Surname.ToLower().Contains(filterText)
                                || _.Email.ToLower().Contains(filterText)
                                || _.UserName.ToLower().Contains(filterText)
                                || _.PhoneNumber.ToLower().Contains(filterText))
                    .ToList();

                var userInfos = await _userInfoRepository.Get(users.Select(_ => _.Id).ToArray());
                foreach (var userInfoNav in userInfos.Where(userInfoNav => !items.Contains(_ => _.UserInfo.Id == userInfoNav.UserInfo.Id)))
                {
                    items.Add(userInfoNav);
                    //totalCount++;
                }
            }

            items = items.DistinctBy(_ => _.UserInfo.Id).ToList();
            totalCount = items.Count;
            items = items.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            var results = ObjectMapper
                .Map<List<UserInfoWithNavigationProperties>, List<UserInfoWithNavigationPropertiesDto>>(items);
            foreach (var result in results)
            {
                result.OrgUnits = await GetOrgUnitsByUser(result.AppUser.Id);
            }

            return new PagedResultDto<UserInfoWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = results
            };
        }

        public virtual async Task<UserInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserInfoWithNavigationProperties, UserInfoWithNavigationPropertiesDto>
                (await _userInfoRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserInfoDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserInfo, UserInfoDto>(await _userInfoRepository.GetAsync(id));
        }

        public async Task<List<AppUserDto>> GetUsers()
        {
            var users = await _userDomainService.GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetTeamUsers = true,
                    GetSystemUsers = false,
                    GetActiveUsers = true,
                }
            );
            return ObjectMapper.Map<List<AppUser>, List<AppUserDto>>(users.Select(x => x.User).ToList());
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            var query = _userRepository.AsQueryable()
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                )
                .OrderBy(x => x.UserName);

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        public async Task<List<LookupDto<Guid?>>> GetPartnerUserLookupAsync(LookupRequestDto input)
        {
            var userIds = (await _userManager.GetUsersInRoleAsync(RoleConsts.Partner)).Select(x => x.Id).ToList();
            var query = _userRepository.AsQueryable()
                .Where(x => userIds.Contains(x.Id))
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                )
                .OrderBy(x => x.UserName);

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            return ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(lookupData);
        }

        [Authorize(ApiPermissions.UserInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userInfoRepository.DeleteAsync(id);
        }


        private static void AugmentAccount(UserInfo userInfo)
        {
            if (userInfo.Accounts.IsNullOrEmpty()) { userInfo.Accounts = new List<UserInfoAccount>(); }

            foreach (var account in userInfo.Accounts)
            {
                account.Fid = account.Fid.Trim().Trim('/').Replace("\t", "");
                var isFid_A_Url = Flurl.Url.IsValid(account.Fid);
                if (isFid_A_Url)
                {
                    var fid = new Flurl.Url(account.Fid).QueryParams.ToList().FirstOrDefault(_ => _.Name.ToLower() == "id");
                    if (fid.Value != null && fid.Value.ToString().IsNotNullOrWhiteSpace()) { account.Fid = fid.Value.ToString(); }
                }
            }
        }

        [Authorize(ApiPermissions.UserInfos.Create)]
        public virtual async Task<UserInfoDto> CreateAsync(UserInfoCreateDto input)
        {
            if (input.AppUserId == default)
            {
                throw new ApiException(L["The {0} field is required.", L["AppUser"]]);
            }

            var userInfo = ObjectMapper.Map<UserInfoCreateDto, UserInfo>(input);
            userInfo.TenantId = CurrentTenant.Id;
            AugmentAccount(userInfo);

            userInfo = await _userInfoRepository.InsertAsync(userInfo, autoSave: true);
            await UpdateStaffEvaluation(userInfo);

            return ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
        }

        [Authorize(ApiPermissions.UserInfos.Edit)]
        public virtual async Task<UserInfoDto> UpdateAsync(Guid id, UserInfoUpdateDto input)
        {
            if (input.AppUserId == default)
            {
                throw new ApiException(L["The {0} field is required.", L["AppUser"]]);
            }

            var user = await _userManager.GetByIdAsync(input.AppUserId.Value);
            if (input.IsActive == false)
            {
                DateTime lockoutEndDate = DateTime.Now.AddYears(10);
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, false);
            }

            var userInfo = await _userInfoRepository.GetAsync(id);
            AugmentAccount(userInfo);
            ObjectMapper.Map(input, userInfo);
            userInfo = await _userInfoRepository.UpdateAsync(userInfo);
            await UpdateStaffEvaluation(userInfo);

            return ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
        }

        private async Task UpdateStaffEvaluation(UserInfo userInfo)
        {
            if (userInfo.MainTeamId.IsNotNullOrEmpty())
            {
                var evalStartDay = GlobalConfiguration.GlobalPayrollConfiguration.PayrollEndDay;
                var date = DateTime.UtcNow;
                if (date.Day >= evalStartDay) date = date.AddMonths(1);
                var year = date.Year;
                var month = date.Month;
                var userEvaluation = await _staffEvaluationRepository.FindAsync(_ => _.Month == month && _.Year == year && _.AppUserId == userInfo.AppUserId);
                if (userEvaluation != null)
                {
                    userEvaluation.TeamId = userInfo.MainTeamId;
                    await _staffEvaluationRepository.UpdateAsync(userEvaluation);
                }
            }
        }

        public async Task AddSeedingAccount(Guid id, UserInfoAccount account)
        {
            var existed = await SeedingAccountExisted(account.Fid);
            if (existed)
            {
                throw new ApiException(LD[ApiDomainErrorCodes.UserInfo.DuplicatedSeedingAccountFid]);
            }

            var userInfo = await _userInfoRepository.GetAsync(id);
            if (userInfo != null)
            {
                if (userInfo.Accounts.IsNullOrEmpty()) userInfo.Accounts = new List<UserInfoAccount>();

                account.CreatedAt = DateTime.UtcNow;
                userInfo.Accounts.Add(account);
                await _userInfoRepository.UpdateAsync(userInfo);
            }
        }

        public async Task<bool> SeedingAccountExisted(string fid)
        {
            var altFid = FacebookHelper.GetProfileFacebookId(fid);
            return await _userInfoRepository.AnyAsync(x => x.Accounts != null && x.Accounts.Any(y => y.Fid.Contains(fid) || y.Fid.Contains(altFid)));
        }

        public async Task<UserInfoDto> GetUserInfoByUserId(Guid userId)
        {
            var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
            var result = ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
            return result;
        }

        public async Task<UserInfoWithNavigationPropertiesDto> GetByCodeAsync(string userCode)
        {
            var entity = await _userDomainService.GetByCode(userCode);

            return ObjectMapper.Map<UserInfoWithNavigationProperties, UserInfoWithNavigationPropertiesDto>(entity);
        }

        public virtual async Task<UserInfoWithNavigationPropertiesDto> GetByUserIdAsync(Guid userId)
        {
            var entity = await _userDomainService.GetByUserId(userId);

            return ObjectMapper.Map<UserInfoWithNavigationProperties, UserInfoWithNavigationPropertiesDto>(entity);
        }

        [Authorize(ApiPermissions.UserInfos.Create)]
        public async Task SyncUserInfos()
        {
            await _userDomainService.SyncUserInfos();
        }


        public async Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListAsync(GetUserInfosInput input)
        {
            var totalCount = await _userInfoRepository.GetCountAsync
            (
                input.FilterText,
                input.Code,
                input.IdentityNumber,
                input.Facebook,
                input.DateOfBirthMin,
                input.DateOfBirthMax,
                input.JoinedDateTimeMin,
                input.JoinedDateTimeMax,
                input.PromotedDateTimeMin,
                input.PromotedDateTimeMax,
                input.AffiliateMultiplierMin,
                input.AffiliateMultiplierMax,
                input.SeedingMultiplierMin,
                input.SeedingMultiplierMax,
                input.ContentRoleType,
                input.IsGDLStaff,
                input.IsSystemUser,
                input.IsActive,
                input.EnablePayrollCalculation,
                input.AppUserId
            );
            var items = await _userInfoRepository.GetListWithNavigationPropertiesAsync
            (
                input.FilterText,
                input.Code,
                input.IdentityNumber,
                input.Facebook,
                input.DateOfBirthMin,
                input.DateOfBirthMax,
                input.JoinedDateTimeMin,
                input.JoinedDateTimeMax,
                input.PromotedDateTimeMin,
                input.PromotedDateTimeMax,
                input.AffiliateMultiplierMin,
                input.AffiliateMultiplierMax,
                input.SeedingMultiplierMin,
                input.SeedingMultiplierMax,
                input.ContentRoleType,
                input.IsGDLStaff,
                input.IsSystemUser,
                input.IsActive,
                input.EnablePayrollCalculation,
                input.AppUserId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            // can't find user info, fallback to find user
            if (input.FilterText.IsNotNullOrWhiteSpace())
            {
                var filterText = input.FilterText.ToLower().Trim();
                var users = await _userRepository.GetListAsync
                (
                    _ => _.Name.ToLower().Contains(filterText)
                         || _.Surname.ToLower().Contains(filterText)
                         || _.Email.ToLower().Contains(filterText)
                         || _.UserName.ToLower().Contains(filterText)
                         || _.PhoneNumber.ToLower().Contains(filterText)
                );

                var userInfos = await _userInfoRepository.Get(users.Select(_ => _.Id).ToArray());
                foreach (var userInfoNav in userInfos.Where(userInfoNav => !items.Contains(_ => _.UserInfo.Id == userInfoNav.UserInfo.Id)))
                {
                    items.Add(userInfoNav);
                    totalCount++;
                }
            }

            items = items.DistinctBy(_ => _.UserInfo.Id).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<UserInfoWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper
                    .Map<List<UserInfoWithNavigationProperties>, List<UserInfoWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input)
        {
            return await _userDomainService.GetUserLookupAsync(input);
        }

        public async Task<MediaDto> UpdateAvatarAsync(UserInfoUpdateAvatarDto input)
        {
            if (input.MediaId != Guid.Empty)
            {
                var userInfo = await _userInfoRepository.GetAsync(x => x.AppUserId == input.UserId);
                if (userInfo != null)
                {
                    userInfo.AvatarMediaId = input.MediaId;
                    await _userInfoRepository.UpdateAsync(userInfo);
                }

                return ObjectMapper.Map<Media, MediaDto>(await _mediaRepository.GetAsync(input.MediaId));
            }

            return null;
        }

        private async Task<List<OrganizationUnitDto>> GetOrgUnitsByUser(Guid userId)
        {
            var teams = await _userDomainService.GetOrgUnitsByUser(userId);
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(teams);
        }
    }
}