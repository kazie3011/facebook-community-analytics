using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public interface IUserPayrollsAppService : IApplicationService
    {
        Task<PagedResultDto<UserPayrollWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollsInput input);

        Task<UserPayrollWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserPayrollDto> GetAsync(Guid id);
        Task<UserPayrollWithNavigationPropertiesDto> GenerateUserPayroll(UserPayrollRequest userPayrollRequest);

        Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserPayrollDto> CreateAsync(UserPayrollCreateDto input);

        Task<UserPayrollDto> UpdateAsync(Guid id, UserPayrollUpdateDto input);

    }
}