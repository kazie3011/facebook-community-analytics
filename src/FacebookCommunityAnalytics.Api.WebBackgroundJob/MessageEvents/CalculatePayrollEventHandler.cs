using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.MessageEvents
{
    public class CalculatePayrollEventHandler :
        IDistributedEventHandler<CalculatePayrollEto>,
        ITransientDependency
    {
        public Task HandleEventAsync(CalculatePayrollEto eto)
        {
            Hangfire.BackgroundJob.Enqueue<IPayrollDomainService>(_ => _.CalculateUserPayrolls(true, true, eto.IsHappyDay, eto.CurrentUserId));
            return Task.CompletedTask;
        }
    }
}
