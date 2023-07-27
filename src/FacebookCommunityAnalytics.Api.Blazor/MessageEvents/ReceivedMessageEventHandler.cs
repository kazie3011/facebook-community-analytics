using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace FacebookCommunityAnalytics.Api.Blazor.MessageEvents
{
    public class ReceivedMessageEventHandler :
        IDistributedEventHandler<ReceivedMessageEto>,
        ITransientDependency
    {
        private readonly IHubContext<BlazorNotificationHub> _hubContext;

        public ReceivedMessageEventHandler(IHubContext<BlazorNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleEventAsync(ReceivedMessageEto eto)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", eto);
            //if (eto.TargetUserId == Guid.Empty)
            //{
            //    await _hubContext.Clients.Groups(ApiPermissions.Notification.ReceiveMessages).SendAsync("ReceiveMessage", eto);
            //}
            //else
            //{
            //    await _hubContext.Clients
            //        .User(eto.TargetUserId.ToString())
            //        .SendAsync("ReceiveMessage", eto);
            //}
        }
    }
}
