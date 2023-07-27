using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Controllers.UserCompensations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserCompensation")]
    [Route("api/app/user-compensations")]
    public class UserCompensationController : ApiController, IUserCompensationAppService
    {
        private readonly IUserCompensationAppService _userCompensationAppService;

        public UserCompensationController(IUserCompensationAppService userCompensationAppService)
        {
            _userCompensationAppService = userCompensationAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<UserCompensationDto> GetAsync(Guid id)
        {
            return await _userCompensationAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<UserCompensationDto>> GetListAsync(GetUserCompensationsInput input)
        {
            return await _userCompensationAppService.GetListAsync(input);
        }

        [HttpPost]
        public async Task<UserCompensationDto> CreateAsync(CreateUpdateUserCompensationDto input)
        {
            return await _userCompensationAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<UserCompensationDto> UpdateAsync(Guid id, CreateUpdateUserCompensationDto input)
        {
            return await _userCompensationAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _userCompensationAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("with-navigation-properties")]
        public Task<PagedResultDto<UserCompensationNavigationPropertiesDto>> GetListWithNavigationAsync(GetUserCompensationsInput input)
        {
            return _userCompensationAppService.GetListWithNavigationAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<UserCompensationNavigationPropertiesDto> GetWithNavigationAsync(Guid id)
        {
            return _userCompensationAppService.GetWithNavigationAsync(id);
        }

        [HttpGet]
        [Route("with-navigation-properties-by-user")]
        public Task<UserCompensationNavigationPropertiesDto> GetWithNavigationByUserAsync(Guid userId, int month, int year)
        {
            return _userCompensationAppService.GetWithNavigationByUserAsync(userId, month, year);
        }

        [HttpGet]
        [Route("calculate-compensations")]
        public Task CalculateCompensation(int month, int year,bool isHappyDay = false)
        {
            return _userCompensationAppService.CalculateCompensation(month, year, isHappyDay);
        }

        [HttpGet]
        [Route("export-compensation/{payrollId}")]
        public Task<byte[]> ExportCompensation(Guid payrollId)
        {
            return _userCompensationAppService.ExportCompensation(payrollId);
        }

        [HttpGet]
        [Route("compensation-detail/{payrollId}")]
        public  Task<CompensationDetailDto> GetCompensationDetailAsync(Guid payrollId)
        {
            return _userCompensationAppService.GetCompensationDetailAsync(payrollId);
        }

        [HttpGet]
        [Route("get-affiliate-conversions")]
        public Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId)
        {
            return _userCompensationAppService.GetAffiliateConversions(fromDate, toDate, userId);
        }
    }
}