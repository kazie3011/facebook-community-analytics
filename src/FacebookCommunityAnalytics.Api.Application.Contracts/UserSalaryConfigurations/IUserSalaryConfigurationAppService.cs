using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public interface IUserSalaryConfigurationAppService:IApplicationService
    {
        Task CreateOrUpdateSalaryConfig(UserSalaryConfigurationDto newSalaryDto);
        Task<PagedResultDto<UserSalaryConfigWithNavPropertiesDto>> GetListWithNavigationAsync(GetUserSalaryConfigurationInput input);
        Task DeleteAsync(Guid id);
        Task<UserSalaryConfigurationDto> UpdateAsync(Guid id, UpdateUserSalaryConfigurationDto input);
    }
}