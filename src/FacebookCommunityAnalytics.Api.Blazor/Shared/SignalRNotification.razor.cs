using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Blazor.MessageEvents;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Volo.Abp.AspNetCore.Components;

namespace FacebookCommunityAnalytics.Api.Blazor.Shared
{
    public partial class SignalRNotification
    {
        //private string _hubUrl;
        private HubConnection _hubConnection;

        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(100);
                
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri(BlazorNotificationHub.HubRoute), options =>
                {
                    options.SkipNegotiation = true;
                    options.Transports = HttpTransportType.WebSockets;
                })
                .Build();

            _hubConnection.On<ReceivedMessageEto>("ReceiveMessage", ReceiveMessage);

            await _hubConnection.StartAsync();
        }

        private async Task ReceiveMessage(ReceivedMessageEto messageEto)
        {
            await Notify.Success(messageEto.ReceivedText);
        }

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
