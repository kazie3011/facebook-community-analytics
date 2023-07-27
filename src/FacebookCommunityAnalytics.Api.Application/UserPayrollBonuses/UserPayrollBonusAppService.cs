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
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserPayrollBonuses.Default)]
    public class UserPayrollBonusesAppService : ApplicationService, IUserPayrollBonusesAppService
    {
        private readonly IUserPayrollBonusRepository _userPayrollBonusRepository;
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly IRepository<Payroll, Guid> _payrollRepository;

        public UserPayrollBonusesAppService(IUserPayrollBonusRepository userPayrollBonusRepository, IRepository<AppUser, Guid> appUserRepository, IRepository<Payroll, Guid> payrollRepository)
        {
            _userPayrollBonusRepository = userPayrollBonusRepository; _appUserRepository = appUserRepository;
            _payrollRepository = payrollRepository;
        }

        public virtual async Task<PagedResultDto<UserPayrollBonusWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollBonusesInput input)
        {
            var totalCount = await _userPayrollBonusRepository.GetCountAsync(input.FilterText, input.PayrollBonusType, input.AmountMin, input.AmountMax, input.Description, input.AppUserId, input.PayrollId);
            var items = await _userPayrollBonusRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.PayrollBonusType, input.AmountMin, input.AmountMax, input.Description, input.AppUserId, input.PayrollId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserPayrollBonusWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserPayrollBonusWithNavigationProperties>, List<UserPayrollBonusWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserPayrollBonusWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserPayrollBonusWithNavigationProperties, UserPayrollBonusWithNavigationPropertiesDto>
                (await _userPayrollBonusRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserPayrollBonusDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserPayrollBonus, UserPayrollBonusDto>(await _userPayrollBonusRepository.GetAsync(id));
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

        [Authorize(ApiPermissions.UserPayrollBonuses.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userPayrollBonusRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.UserPayrollBonuses.Create)]
        public virtual async Task<UserPayrollBonusDto> CreateAsync(UserPayrollBonusCreateDto input)
        {

            var userPayrollBonus = ObjectMapper.Map<UserPayrollBonusCreateDto, UserPayrollBonus>(input);
            userPayrollBonus.TenantId = CurrentTenant.Id;
            userPayrollBonus = await _userPayrollBonusRepository.InsertAsync(userPayrollBonus, autoSave: true);
            return ObjectMapper.Map<UserPayrollBonus, UserPayrollBonusDto>(userPayrollBonus);
        }

        [Authorize(ApiPermissions.UserPayrollBonuses.Edit)]
        public virtual async Task<UserPayrollBonusDto> UpdateAsync(Guid id, UserPayrollBonusUpdateDto input)
        {

            var userPayrollBonus = await _userPayrollBonusRepository.GetAsync(id);
            ObjectMapper.Map(input, userPayrollBonus);
            userPayrollBonus = await _userPayrollBonusRepository.UpdateAsync(userPayrollBonus);
            return ObjectMapper.Map<UserPayrollBonus, UserPayrollBonusDto>(userPayrollBonus);
        }
    }
}