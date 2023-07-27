using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.HealthChecks.Models;
using IdentityServer4.Models;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.HealthChecks
{
    public interface IHealthCheckDomainService : IDomainService
    {
        Task SendNotificationToSlack(SlackMessage input);
    }

    public class HealthCheckDomainService : BaseDomainService, IHealthCheckDomainService
    {
        public async Task SendNotificationToSlack(SlackMessage input)
        {
            var slackClient = new SlackMessageClient(GlobalConfiguration.SlackConfiguration.WebhookUrl);
            await slackClient.SendAsync(input);
        }
    }
}