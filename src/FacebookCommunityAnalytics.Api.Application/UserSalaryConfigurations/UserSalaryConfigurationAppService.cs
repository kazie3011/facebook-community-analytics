using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserInfos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.BoD.Default)]
    [Authorize(ApiPermissions.UserSalaryConfiguration.Default)]
    public class UserSalaryConfigurationAppService : ApplicationService, IUserSalaryConfigurationAppService
    {
        private readonly IUserSalaryConfigurationRepository _userSalaryConfigurationRepository;

        public UserSalaryConfigurationAppService(IUserSalaryConfigurationRepository userSalaryConfigurationRepository)
        {
            _userSalaryConfigurationRepository = userSalaryConfigurationRepository;
        }
        
        public async Task CreateOrUpdateSalaryConfig(UserSalaryConfigurationDto input)
        {
            var salaryConfig = await _userSalaryConfigurationRepository.FirstOrDefaultAsync(x => x.TeamId == input.TeamId && x.UserId == input.UserId && x.UserPosition ==  input.UserPosition);
            if (salaryConfig != null)
            {
                salaryConfig.Salary = input.Salary;
                salaryConfig.Description = input.Description;

                await _userSalaryConfigurationRepository.UpdateAsync(salaryConfig);
            }
            else {
                var newUserSalaryConfig = new UserSalaryConfiguration
                {
                    UserId = input.UserId,
                    TeamId = input.TeamId,
                    UserPosition = input.UserPosition,
                    Salary = input.Salary,
                    Description = input.Description
                };
                await _userSalaryConfigurationRepository.InsertAsync(newUserSalaryConfig);
            }
        }

        public async Task<PagedResultDto<UserSalaryConfigWithNavPropertiesDto>> GetListWithNavigationAsync(GetUserSalaryConfigurationInput input)
        {
            var userSalaryConfig = await _userSalaryConfigurationRepository.GetListWithNavigationPropertiesAsync
            (
                input.TeamId,
                input.UserId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            var count = await _userSalaryConfigurationRepository.GetCountAsync(input.TeamId, input.UserId);

            return new PagedResultDto<UserSalaryConfigWithNavPropertiesDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<UserSalaryConfigurationNavigationProperties>, List<UserSalaryConfigWithNavPropertiesDto>>(userSalaryConfig)
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            await _userSalaryConfigurationRepository.DeleteAsync(id);
        }

        public async Task<UserSalaryConfigurationDto> UpdateAsync(Guid id, UpdateUserSalaryConfigurationDto input)
        {
            var config = await _userSalaryConfigurationRepository.GetAsync(id);
            ObjectMapper.Map(input, config);
            config =  await _userSalaryConfigurationRepository.UpdateAsync(config);
            return ObjectMapper.Map<UserSalaryConfiguration, UserSalaryConfigurationDto>(config);
        }
    }
}