using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.ApiNotifications
{
    public interface IApiNotificationAppService : IApplicationService
    {
        /// <summary>
        /// Notify using message queue
        /// </summary>
        /// <param name="input"></param>
        Task SendMessageAsync(SendMessageInput input);
        
        /// <summary>
        /// Only notify to same process request
        /// </summary>
        /// <param name="input"></param>
        Task SendMessageLocalAsync(SendMessageInput input);
    }
}
