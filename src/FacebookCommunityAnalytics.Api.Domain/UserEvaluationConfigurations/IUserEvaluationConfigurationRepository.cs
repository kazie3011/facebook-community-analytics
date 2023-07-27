using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public interface IUserEvaluationConfigurationRepository : IRepository<UserEvaluationConfiguration, Guid>
    {
        Task<UserEvaluationConfiguration> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
        Task<UserEvaluationConfiguration> GetByTeamId(Guid teamId, CancellationToken cancellationToken = default);
    }
}