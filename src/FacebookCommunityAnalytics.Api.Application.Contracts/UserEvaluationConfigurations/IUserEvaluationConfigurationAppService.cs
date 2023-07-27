using System;  

using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public interface IUserEvaluationConfigurationAppService:IApplicationService
    {
        Task<UserEvaluationConfigurationDto> CreateAsync(UserEvaluationConfigurationCreateUpdateDto input);
        Task<UserEvaluationConfigurationDto> CreateOrUpdateRootConfig(UserEvaluationConfigurationDto input);
        Task<UserEvaluationConfigurationDto> CreateOrUpdateCustomConfig(UserEvaluationConfigurationDto input);
        Task<UserEvaluationConfigurationDto> UpdateUserEvalConfig(Guid appUserId, UserEvaluationConfigurationDto input);

        // Task<UserEvaluationConfigurationDto> UpdateSaleEvaluationConfigurationAsync(Guid id, SaleEvaluationConfigurationDto input);
        // Task<UserEvaluationConfigurationDto> UpdateTiktokEvaluationConfigurationAsync(Guid id, TiktokEvaluationConfigurationDto input);
        // Task<UserEvaluationConfigurationDto> UpdateContentEvaluationConfigurationAsync(Guid id, ContentEvaluationConfigurationDto input);
        // Task<UserEvaluationConfigurationDto> UpdateSeedingEvaluationConfigurationAsync(Guid id, SeedingEvaluationConfigurationDto input);
        // Task<UserEvaluationConfigurationDto> UpdateAffiliateEvaluationConfigurationAsync(Guid id, AffiliateEvaluationConfigurationDto input);

        // Task<UserEvaluationConfigurationDto> GetTeamEvaluationConfigForCurrentUser();
        Task<UserEvaluationConfigurationDto> GetEvalConfig(Guid appUserId);
        Task<List<UserEvaluationConfigurationDto>> GetUserEvaluationConfigurations(GetUserEvaluationConfigurationsInput request);
        Task UpdateAsync(Guid id, UserEvaluationConfigurationCreateUpdateDto input);
        Task DeleteAsync(Guid id);
    }
}