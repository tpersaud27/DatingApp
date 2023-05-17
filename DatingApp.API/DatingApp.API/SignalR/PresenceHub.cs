using DatingApp.Services.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.API.SignalR
{
    // This will be used to display if a user is online / the presence of a user
    public class PresenceHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            // This will notify other users that someone is online (this is everyone else but the client that is connecting)
            // Everyone will receive the username of the user that has just connected
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Notify online users that the current user has logged off
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            await base.OnDisconnectedAsync(exception);
        }


    }
}