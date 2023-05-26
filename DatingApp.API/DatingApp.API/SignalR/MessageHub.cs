using DatingApp.API.Interfaces;
using DatingApp.Services.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.API.SignalR
{
    public class MessageHub: Hub
    {
        private readonly IMessageRepository _messageRepository;

        public MessageHub(IMessageRepository messageRepository) { 
            
            _messageRepository = messageRepository;
                
        
        }

        public override async Task OnConnectedAsync()
        {
            // Getting access to the http request when we connect to signal r
            // This will allow us to send something up through query strings in the http request
            var httpContext = Context.GetHttpContext();
            // This will be the query string for the user's username
            var otherUser = httpContext.Request.Query["user"];

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageRepository
                           .GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceivedMessageThread", messages);
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


        // This will sort the users name by alphabetical order
        // Caller is the person sending the message to the other user
        // This is so that no matter who connects first, the group name will always be the same
        private string GetGroupName(string caller, string other)
        {
            // Originally stringCompare returns a integer, we can do the comparison here to make it return a boolean value
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }



    }
}
