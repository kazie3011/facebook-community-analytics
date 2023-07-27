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
using FacebookCommunityAnalytics.Api.UserWaves;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserWaves.Default)]
    public class UserWavesAppService : ApplicationService, IUserWavesAppService
    {
        private readonly IUserWaveRepository _userWaveRepository;
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly IRepository<Payroll, Guid> _payrollRepository;

        public UserWavesAppService(IUserWaveRepository userWaveRepository, IRepository<AppUser, Guid> appUserRepository, IRepository<Payroll, Guid> payrollRepository)
        {
            _userWaveRepository = userWaveRepository; _appUserRepository = appUserRepository;
            _payrollRepository = payrollRepository;
        }

        public virtual async Task<PagedResultDto<UserWaveWithNavigationPropertiesDto>> GetListAsync(GetUserWavesInput input)
        {
            var totalCount = await _userWaveRepository.GetCountAsync(input.FilterText, input.WaveType, input.TotalPostCountMin, input.TotalPostCountMax, input.TotalReactionCountMin, input.TotalReactionCountMax, input.LikeCountMin, input.LikeCountMax, input.CommentCountMin, input.CommentCountMax, input.ShareCountMin, input.ShareCountMax, input.AmountMin, input.AmountMax, input.Description, input.AppUserId, input.PayrollId);
            var items = await _userWaveRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.WaveType, input.TotalPostCountMin, input.TotalPostCountMax, input.TotalReactionCountMin, input.TotalReactionCountMax, input.LikeCountMin, input.LikeCountMax, input.CommentCountMin, input.CommentCountMax, input.ShareCountMin, input.ShareCountMax, input.AmountMin, input.AmountMax, input.Description, input.AppUserId, input.PayrollId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserWaveWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserWaveWithNavigationProperties>, List<UserWaveWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserWaveWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserWaveWithNavigationProperties, UserWaveWithNavigationPropertiesDto>
                (await _userWaveRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserWaveDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserWave, UserWaveDto>(await _userWaveRepository.GetAsync(id));
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

        [Authorize(ApiPermissions.UserWaves.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userWaveRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.UserWaves.Create)]
        public virtual async Task<UserWaveDto> CreateAsync(UserWaveCreateDto input)
        {

            var userWave = ObjectMapper.Map<UserWaveCreateDto, UserWave>(input);
            userWave.TenantId = CurrentTenant.Id;
            userWave = await _userWaveRepository.InsertAsync(userWave, autoSave: true);
            return ObjectMapper.Map<UserWave, UserWaveDto>(userWave);
        }

        [Authorize(ApiPermissions.UserWaves.Edit)]
        public virtual async Task<UserWaveDto> UpdateAsync(Guid id, UserWaveUpdateDto input)
        {

            var userWave = await _userWaveRepository.GetAsync(id);
            ObjectMapper.Map(input, userWave);
            userWave = await _userWaveRepository.UpdateAsync(userWave);
            return ObjectMapper.Map<UserWave, UserWaveDto>(userWave);
        }
    }
}