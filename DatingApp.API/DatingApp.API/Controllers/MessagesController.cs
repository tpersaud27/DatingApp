using AutoMapper;
using CloudinaryDotNet.Actions;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Implementation;
using DatingApp.API.Interfaces;
using DatingApp.DAL.Implementation;
using DatingApp.DAL.Interfaces;
using DatingApp.Services.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class MessagesController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            // Get the username from the claism
            var username  = User.GetUsername();
            // Check if the person is trying to send a message to themselves
            if(username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }
            // Get the user entity
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            // Get the recipient entity
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            // Check if the recipient the user is trying to send a message to exists
            if(recipient== null)
            {
                return NotFound("User not found");
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
            // Add the message
            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync())
            {
                // Return the MessageDto after 
                // Note technically since we are creating a resouce here we should be returning a created at resource
                // This can be done as shown in the users controller
                return Ok(_mapper.Map<MessageDto>(message));
            }
            // Otherwise return bad request
            return BadRequest("Failed to send message");
        }

    }
}
