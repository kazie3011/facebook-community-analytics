using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;

namespace FacebookCommunityAnalytics.Api.ApiNotifications
{
    public interface IApiNotificationDomainService
    {
        Task NotifyToWebUser(SendMessageInput input);
        Task LocalNotifyToWebUser(SendMessageInput input);
    }
    
    public class ApiNotificationDomainService : BaseDomainService, IApiNotificationDomainService
    {
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly ILocalEventBus _localEventBus;
        public ApiNotificationDomainService(IDistributedEventBus distributedEventBus, ILocalEventBus localEventBus)
        {
            _distributedEventBus = distributedEventBus;
            _localEventBus = localEventBus;
        }

        public async Task NotifyToWebUser(SendMessageInput input)
        {
            var message = input.IsLocalized ? L[input.Message] : input.Message;
            await _distributedEventBus.PublishAsync(new ReceivedMessageEto(input.SenderId, input.SenderName, message));
        }

        public async Task LocalNotifyToWebUser(SendMessageInput input)
        {
            var message = input.IsLocalized ? L[input.Message] : input.Message;
            await _localEventBus.PublishAsync(new ReceivedMessageEto(input.SenderId, input.SenderName, message));
        }
    }
}
