using DatingApp.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.API.SignalR
{

    // This will be used to display if a user is online / the presence of a user
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            // This is adding the user to the presenceTracker
            await _presenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            // This will notify other users that someone is online (this is everyone else but the client that is connecting)
            // Everyone will receive the username of the user that has just connected
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            // This will send all users a list of current users
            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await _presenceTracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            // Notify online users that the current user has logged off
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            // This will send all users a list of current users
            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);

        }


    }
}