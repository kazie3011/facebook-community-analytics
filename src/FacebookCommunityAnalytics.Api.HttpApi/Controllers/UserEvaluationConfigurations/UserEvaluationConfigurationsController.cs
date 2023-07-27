using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.UserEvaluationConfigurations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserEvaluationConfigurations")]
    [Route("api/app/user-evaluation-configs")]
    public class UserEvaluationConfigurationsController : AbpController, IUserEvaluationConfigurationAppService
    {
        private readonly IUserEvaluationConfigurationAppService _evaluationConfigurationAppService;

        public UserEvaluationConfigurationsController(IUserEvaluationConfigurationAppService evaluationConfigurationAppService)
        {
            _evaluationConfigurationAppService = evaluationConfigurationAppService;
        }

        [HttpPost]
        public Task<UserEvaluationConfigurationDto> CreateAsync(UserEvaluationConfigurationCreateUpdateDto input)
        {
            return _evaluationConfigurationAppService.CreateAsync(input);
        }

        [HttpPost("create-update")]
        public Task<UserEvaluationConfigurationDto> CreateOrUpdateRootConfig(UserEvaluationConfigurationDto input)
        {
            return _evaluationConfigurationAppService.CreateOrUpdateRootConfig(input);
        }

        [HttpPost("create-update-custom")]
        public Task<UserEvaluationConfigurationDto> CreateOrUpdateCustomConfig(UserEvaluationConfigurationDto input)
        {
            return _evaluationConfigurationAppService.CreateOrUpdateCustomConfig(input);
        }

        [HttpPost("update-user-config")]
        public Task<UserEvaluationConfigurationDto> UpdateUserEvalConfig(Guid appUserId , UserEvaluationConfigurationDto input)
        {
            return _evaluationConfigurationAppService.UpdateUserEvalConfig(appUserId ,input);
        }

        // [HttpPut("sale-config")]
        // public Task<UserEvaluationConfigurationDto> UpdateSaleEvaluationConfigurationAsync(Guid id, SaleEvaluationConfigurationDto input)
        // {
        //     return _evaluationConfigurationAppService.UpdateSaleEvaluationConfigurationAsync(id, input);
        // }
        //
        // [HttpPut("tiktok-config")]
        // public Task<UserEvaluationConfigurationDto> UpdateTiktokEvaluationConfigurationAsync(Guid id, TiktokEvaluationConfigurationDto input)
        // {
        //     return _evaluationConfigurationAppService.UpdateTiktokEvaluationConfigurationAsync(id, input);
        // }
        //
        // [HttpPut("content-config")]
        // public Task<UserEvaluationConfigurationDto> UpdateContentEvaluationConfigurationAsync(Guid id, ContentEvaluationConfigurationDto input)
        // {
        //     return _evaluationConfigurationAppService.UpdateContentEvaluationConfigurationAsync(id, input);
        // }
        //
        // [HttpPut("affiliate-config")]
        // public Task<UserEvaluationConfigurationDto> UpdateAffiliateEvaluationConfigurationAsync(Guid id, AffiliateEvaluationConfigurationDto input)
        // {
        //     return _evaluationConfigurationAppService.UpdateAffiliateEvaluationConfigurationAsync(id, input);
        // }
        //
        // [HttpPut("seeding-config")]
        // public Task<UserEvaluationConfigurationDto> UpdateSeedingEvaluationConfigurationAsync(Guid id, SeedingEvaluationConfigurationDto input)
        // {
        //     return _evaluationConfigurationAppService.UpdateSeedingEvaluationConfigurationAsync(id, input);
        // }

        // [HttpGet("current-team-config")]
        // public async Task<UserEvaluationConfigurationDto> GetTeamEvaluationConfigForCurrentUser()
        // {
        //    var result = await _evaluationConfigurationAppService.GetTeamEvaluationConfigForCurrentUser();
        //    return result;
        // }

        [HttpGet("eval-config")]
        public Task<UserEvaluationConfigurationDto> GetEvalConfig(Guid appUserId)
        {
            return _evaluationConfigurationAppService.GetEvalConfig(appUserId);
        }
        
        [HttpGet]
        public Task<List<UserEvaluationConfigurationDto>> GetUserEvaluationConfigurations(GetUserEvaluationConfigurationsInput request)
        {
            return _evaluationConfigurationAppService.GetUserEvaluationConfigurations(request);
        }
        
        [HttpPut]
        [Route("{id}")]
        public virtual Task UpdateAsync(Guid id, UserEvaluationConfigurationCreateUpdateDto input)
        {
            return _evaluationConfigurationAppService.UpdateAsync(id, input);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _evaluationConfigurationAppService.DeleteAsync(id);
        }
    }
}