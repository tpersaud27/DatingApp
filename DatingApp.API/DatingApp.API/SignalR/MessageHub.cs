using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.DAL.Interfaces;
using DatingApp.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.API.SignalR
{
    [Authorize]
    public class MessageHub: Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper) { 
            
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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

            await AddToGroup(groupName);

            var messages = await _messageRepository
                           .GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceivedMessageThread", messages);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
              // Get the username from the claism
            var username  = Context.User.GetUsername();
            // Check if the person is trying to send a message to themselves
            if(username == createMessageDto.RecipientUsername.ToLower())
            {
                // Note: Exceptions are more expensive on our server as compared to http responses
                // since we are not in a controller we have to throw an exception here
                throw new HubException("You cannot send messages to your self");
            }
            // Get the user entity
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            // Get the recipient entity
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            // Check if the recipient the user is trying to send a message to exists
            if(recipient== null)
            {
                throw new HubException("Not found user");
            }

            // Create out new message entity
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUsername = recipient.UserName,
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content,
            };

            // Get the groupName
            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupName);

            // Check if we have a matching recipient name
            if(group.Connections.Any(x =>x.Username == recipient.UserName))
            {
                // If we have a matching recipient then we set the time read to now
                message.DateRead = DateTime.UtcNow;
            }

            // Add the message
            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }

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

        // This method will be used to add a group to the database
        private async Task<bool> AddToGroup(string groupName)
        {
            // Get the group first
            var group = await _messageRepository.GetMessageGroup(groupName);
            // Create a new connection
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            // First we should check if the group does exist in the databse
            if (group == null)
            {
                // If it is null we should be creating a new group
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            // Add the conneciton to the group, either the one we have created or the one existing
            group.Connections.Add(connection);
            // Save the changes
            return await _messageRepository.SaveAllAsync();
        }

        // All we do here is removing the connection from the database
        // When we disconnect, signaR will remove the connection from the hub
        private async Task RemoveFromMessageGroup()
        {
            var connection = await _messageRepository.GetConnection(Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            await _messageRepository.SaveAllAsync();
        }

    }
}
