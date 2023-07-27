using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.ApiNotifications
{
    [RemoteService(IsEnabled = false)]
    //[Authorize(ApiPermissions.Notification.Default)]
    public class ApiNotificationAppService :  ApplicationService, IApiNotificationAppService
    {
        private readonly IApiNotificationDomainService _notifyDomainService;
        public ApiNotificationAppService(IApiNotificationDomainService notifyDomainService)
        {
            _notifyDomainService = notifyDomainService;
        }

        /// <summary>
        /// Notify using message queue
        /// </summary>
        /// <param name="input"></param>
        public async Task SendMessageAsync(SendMessageInput input)
        {
            await _notifyDomainService.NotifyToWebUser(input);
        }

        /// <summary>
        /// Only notify to same process request
        /// </summary>
        /// <param name="input"></param>
        public async Task SendMessageLocalAsync(SendMessageInput input)
        {

            await _notifyDomainService.LocalNotifyToWebUser(input);
        }
    }
}
