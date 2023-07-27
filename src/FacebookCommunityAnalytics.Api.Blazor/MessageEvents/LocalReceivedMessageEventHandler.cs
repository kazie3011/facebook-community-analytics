using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace FacebookCommunityAnalytics.Api.Blazor.MessageEvents
{
    public class LocalReceivedMessageEventHandler :
        ILocalEventHandler<ReceivedMessageEto>,
        ITransientDependency
    {
        private readonly IHubContext<BlazorNotificationHub> _hubContext;

        public LocalReceivedMessageEventHandler(IHubContext<BlazorNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleEventAsync(ReceivedMessageEto eto)
        {
            if (eto.TargetUserId == Guid.Empty)
            {
                await _hubContext.Clients.Groups(ApiPermissions.Notification.ReceiveMessages).SendAsync("ReceiveMessage", eto);
            }
            else
            {
                await _hubContext.Clients
                    .User(eto.TargetUserId.ToString())
                    .SendAsync("ReceiveMessage", eto);
            }
        }
    }
}