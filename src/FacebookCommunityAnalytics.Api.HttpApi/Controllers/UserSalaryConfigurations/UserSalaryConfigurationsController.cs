using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserSalaryConfigurations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.UserSalaryConfigurations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserSalaryConfigurations")]
    [Route("api/app/user-salary-configs")]
    public class UserSalaryConfigurationsController: AbpController, IUserSalaryConfigurationAppService
    {
        
        private readonly IUserSalaryConfigurationAppService _userSalaryConfigurationAppService;

        public UserSalaryConfigurationsController(IUserSalaryConfigurationAppService userSalaryConfigurationAppService)
        {
            _userSalaryConfigurationAppService = userSalaryConfigurationAppService;
        }

        [HttpPost]
        public Task CreateOrUpdateSalaryConfig(UserSalaryConfigurationDto newSalaryDto)
        {
            return _userSalaryConfigurationAppService.CreateOrUpdateSalaryConfig(newSalaryDto);
        }
        
        [HttpGet]
        public Task<PagedResultDto<UserSalaryConfigWithNavPropertiesDto>> GetListWithNavigationAsync(GetUserSalaryConfigurationInput input)
        {
            return _userSalaryConfigurationAppService.GetListWithNavigationAsync(input);
        }
        
        
        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userSalaryConfigurationAppService.DeleteAsync(id);
        }
        
        [HttpPost]
        [Route("update")]
        public Task<UserSalaryConfigurationDto> UpdateAsync(Guid id, UpdateUserSalaryConfigurationDto input)
        {
            return _userSalaryConfigurationAppService.UpdateAsync(id, input);
            
        }
    }
}