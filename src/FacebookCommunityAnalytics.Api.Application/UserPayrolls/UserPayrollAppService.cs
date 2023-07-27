using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.UserInfos;
using Newtonsoft.Json;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserPayrolls.Default)]
    public class UserPayrollsAppService : ApiAppService, IUserPayrollsAppService
    {
        private readonly IdentityUserManager _identityUserManager;

        private readonly IUserPayrollRepository _userPayrollRepository;
        private readonly IRepository<Payroll, Guid> _payrollRepository;
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IPayrollDomainService _payrollDomainService;

        public UserPayrollsAppService(IUserPayrollRepository userPayrollRepository, IRepository<Payroll, Guid> payrollRepository, IRepository<AppUser, Guid> appUserRepository, IPayrollDomainService payrollDomainService, IdentityUserManager identityUserManager, IUserInfoRepository userInfoRepository)
        {
            _userPayrollRepository = userPayrollRepository; _payrollRepository = payrollRepository;
            _appUserRepository = appUserRepository;
            _payrollDomainService = payrollDomainService;
            _identityUserManager = identityUserManager;
            _userInfoRepository = userInfoRepository;
        }

        public virtual async Task<PagedResultDto<UserPayrollWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollsInput input)
        {
            var totalCount = await _userPayrollRepository.GetCountAsync(input.FilterText, input.Code, input.OrganizationId, input.ContentRoleType, input.AffiliateMultiplierMin, input.AffiliateMultiplierMax, input.SeedingMultiplierMin, input.SeedingMultiplierMax, input.Description, input.WaveAmountMin, input.WaveAmountMax, input.BonusAmountMin, input.BonusAmountMax, input.TotalAmountMin, input.TotalAmountMax, input.PayrollId, input.AppUserId);
            var items = await _userPayrollRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Code, input.OrganizationId, input.ContentRoleType, input.AffiliateMultiplierMin, input.AffiliateMultiplierMax, input.SeedingMultiplierMin, input.SeedingMultiplierMax, input.Description, input.WaveAmountMin, input.WaveAmountMax, input.BonusAmountMin, input.BonusAmountMax, input.TotalAmountMin, input.TotalAmountMax, input.PayrollId, input.AppUserId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserPayrollWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserPayrollWithNavigationProperties>, List<UserPayrollWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserPayrollWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserPayrollWithNavigationProperties, UserPayrollWithNavigationPropertiesDto>
                (await _userPayrollRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserPayrollDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserPayroll, UserPayrollDto>(await _userPayrollRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input)
        {
            var query = _payrollRepository.AsQueryable()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Code != null &&
                         x.Code.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Payroll>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Payroll>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            var query = _appUserRepository.AsQueryable()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.UserName != null &&
                         x.UserName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        [Authorize(ApiPermissions.UserPayrolls.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userPayrollRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.UserPayrolls.Create)]
        public virtual async Task<UserPayrollDto> CreateAsync(UserPayrollCreateDto input)
        {

            var userPayroll = ObjectMapper.Map<UserPayrollCreateDto, UserPayroll>(input);
            userPayroll.TenantId = CurrentTenant.Id;
            userPayroll = await _userPayrollRepository.InsertAsync(userPayroll, autoSave: true);
            return ObjectMapper.Map<UserPayroll, UserPayrollDto>(userPayroll);
        }

        [Authorize(ApiPermissions.UserPayrolls.Edit)]
        public virtual async Task<UserPayrollDto> UpdateAsync(Guid id, UserPayrollUpdateDto input)
        {

            var userPayroll = await _userPayrollRepository.GetAsync(id);
            ObjectMapper.Map(input, userPayroll);
            userPayroll = await _userPayrollRepository.UpdateAsync(userPayroll);
            return ObjectMapper.Map<UserPayroll, UserPayrollDto>(userPayroll);
        }

        [Authorize(ApiPermissions.PaySlip.Default)]
        public async Task<UserPayrollWithNavigationPropertiesDto> GenerateUserPayroll(UserPayrollRequest userPayrollRequest)
        {
            var userInfo = await _userInfoRepository.FindAsync(_ => _.Code.ToLower() == userPayrollRequest.UserCode.ToLower().Trim());
            if (userInfo != null)
            {
                var user = await _appUserRepository.GetAsync(userInfo.AppUserId.Value);
                var identityUser = await _identityUserManager.GetByIdAsync(userInfo.AppUserId.Value);
                if (await _identityUserManager.IsInRoleAsync(identityUser, RoleConsts.Staff))
                {
                    var payslip = await _payrollDomainService.GetUserPayslip(userPayrollRequest);
                    var userPayroll =  ObjectMapper.Map<UserPayroll, UserPayrollDto>(payslip);
                    var result = new UserPayrollWithNavigationPropertiesDto
                    {
                        UserPayroll = userPayroll,
                        Payroll = new PayrollDto(),
                        AppUser = ObjectMapper.Map<AppUser, AppUserDto>(user)
                    };

                    return result;
                }
            }

            return null;
        }
    }
}