using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Controllers.UserCompensations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserSalary")]
    [Route("api/app/user-salaries")]
    public class UserSalaryController : ApiController, IUserSalaryAppService
    {
        private readonly IUserSalaryAppService _userSalaryAppService;

        public UserSalaryController(IUserSalaryAppService userSalaryAppService)
        {
            _userSalaryAppService = userSalaryAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<UserSalaryDto> GetAsync(Guid id)
        {
            return await _userSalaryAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<UserSalaryDto>> GetListAsync(GetUserSalariesInput input)
        {
            return await _userSalaryAppService.GetListAsync(input);
        }

        [HttpPost]
        public async Task<UserSalaryDto> CreateAsync(CreateUpdateUserSalaryDto input)
        {
            return await _userSalaryAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<UserSalaryDto> UpdateAsync(Guid id, CreateUpdateUserSalaryDto input)
        {
            return await _userSalaryAppService.UpdateAsync(id, input);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _userSalaryAppService.DeleteAsync(id);
        }
    }
}