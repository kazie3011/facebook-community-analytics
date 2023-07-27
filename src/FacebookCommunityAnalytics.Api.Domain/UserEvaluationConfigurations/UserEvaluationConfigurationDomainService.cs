using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public interface IUserEvaluationConfigurationDomainService:IDomainService
    {
        Task<UserEvaluationConfigurationDto> GetEvalConfigByTeamId(Guid teamId);
    }
    
    public class UserEvaluationConfigurationDomainService:BaseDomainService,IUserEvaluationConfigurationDomainService
    {
        private readonly IUserEvaluationConfigurationRepository _evaluationConfigurationRepository;

        public UserEvaluationConfigurationDomainService(IUserEvaluationConfigurationRepository evaluationConfigurationRepository)
        {
            _evaluationConfigurationRepository = evaluationConfigurationRepository;
        }

        public async Task<UserEvaluationConfigurationDto> GetEvalConfigByTeamId(Guid teamId)
        {
            var evalConfig = await _evaluationConfigurationRepository.GetByUserId(teamId);
            return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(evalConfig);
        }
    }
}