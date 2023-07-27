using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.Blazor.MessageEvents
{
    //[Authorize]
    [HubRoute("/blazor-notification-hub")]
    public class BlazorNotificationHub : AbpHub
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrentUser _currentUser;
        public const string HubRoute = "/blazor-notification-hub";
        public BlazorNotificationHub(ICurrentUser currentUser, IAuthorizationService authorizationService)
        {
            _currentUser = currentUser;
            _authorizationService = authorizationService;
        }
        

        //public override async Task OnConnectedAsync()
        //{
        //    var check = await _authorizationService.AuthorizeAsync(ApiPermissions.Notification.ReceiveMessages);
        //    if (check.Succeeded)
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, ApiPermissions.Notification.ReceiveMessages);
        //    }
        //    await base.OnConnectedAsync();
        //}
    }
}
