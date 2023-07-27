using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserPayrollCommissions.Default)]
    public class UserPayrollCommissionsAppService : ApplicationService, IUserPayrollCommissionsAppService
    {
        private readonly IUserPayrollCommissionRepository _userPayrollCommissionRepository;
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly IRepository<Payroll, Guid> _payrollRepository;

        public UserPayrollCommissionsAppService(IUserPayrollCommissionRepository userPayrollCommissionRepository, IRepository<AppUser, Guid> appUserRepository, IRepository<Payroll, Guid> payrollRepository)
        {
            _userPayrollCommissionRepository = userPayrollCommissionRepository; _appUserRepository = appUserRepository;
            _payrollRepository = payrollRepository;
        }

        public virtual async Task<PagedResultDto<UserPayrollCommissionWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollCommissionsInput input)
        {
            var totalCount = await _userPayrollCommissionRepository.GetCountAsync(input.FilterText, input.OrganizationId, input.Description, input.PayrollCommissionType, input.PayrollCommissionMin, input.PayrollCommissionMax, input.AmountMin, input.AmountMax, input.AppUserId, input.PayrollId);
            var items = await _userPayrollCommissionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.OrganizationId, input.Description, input.PayrollCommissionType, input.PayrollCommissionMin, input.PayrollCommissionMax, input.AmountMin, input.AmountMax, input.AppUserId, input.PayrollId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserPayrollCommissionWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserPayrollCommissionWithNavigationProperties>, List<UserPayrollCommissionWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserPayrollCommissionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserPayrollCommissionWithNavigationProperties, UserPayrollCommissionWithNavigationPropertiesDto>
                (await _userPayrollCommissionRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserPayrollCommissionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserPayrollCommission, UserPayrollCommissionDto>(await _userPayrollCommissionRepository.GetAsync(id));
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

        [Authorize(ApiPermissions.UserPayrollCommissions.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userPayrollCommissionRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.UserPayrollCommissions.Create)]
        public virtual async Task<UserPayrollCommissionDto> CreateAsync(UserPayrollCommissionCreateDto input)
        {

            var userPayrollCommission = ObjectMapper.Map<UserPayrollCommissionCreateDto, UserPayrollCommission>(input);
            userPayrollCommission.TenantId = CurrentTenant.Id;
            userPayrollCommission = await _userPayrollCommissionRepository.InsertAsync(userPayrollCommission, autoSave: true);
            return ObjectMapper.Map<UserPayrollCommission, UserPayrollCommissionDto>(userPayrollCommission);
        }

        [Authorize(ApiPermissions.UserPayrollCommissions.Edit)]
        public virtual async Task<UserPayrollCommissionDto> UpdateAsync(Guid id, UserPayrollCommissionUpdateDto input)
        {

            var userPayrollCommission = await _userPayrollCommissionRepository.GetAsync(id);
            ObjectMapper.Map(input, userPayrollCommission);
            userPayrollCommission = await _userPayrollCommissionRepository.UpdateAsync(userPayrollCommission);
            return ObjectMapper.Map<UserPayrollCommission, UserPayrollCommissionDto>(userPayrollCommission);
        }
    }
}