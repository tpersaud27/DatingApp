using AutoMapper;
using CloudinaryDotNet.Actions;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Implementation;
using DatingApp.API.Interfaces;
using DatingApp.API.Pagination;
using DatingApp.DAL.Implementation;
using DatingApp.DAL.Interfaces;
using DatingApp.Services.Extensions;
using DatingApp.Services.Pagination;
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


        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages));

            return messages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"> This is the username of the person the user would be clicking on</param>
        /// <returns></returns>
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            
            // Get the User
            var username = User.GetUsername();
            var user = _userRepository.GetUserByUsernameAsync(username);
            // Get the message based on the id
            var message = await _messageRepository.GetMessage(id);

            // We will need to check if both users have deleted the message on their ends
            // We will need to check if the user trying to delete the message is either the sender of receiver of the message
            // To do so we will just ensure the person trying to delete the message is not someone else
            if(message.SenderUserName != username || message.RecipientUsername != username)
            {
                return Unauthorized("Not authorized to delete this message");
            }
            
            // Set the deleted flag to be true
            if(message.SenderUserName == username)
            {
                message.SenderDeleted= true;
            }

            if(message.RecipientUsername == username)
            {
                message.RecipientDeleted= true;
            }

            // If both parties have the delete flag set to true then we delete
            if(message.SenderDeleted == true && message.RecipientDeleted == true)
            {
                // Delete the message
                 _messageRepository.DeleteMessage(message);
            }
            
            // Check if the changes were saved
            if(await _messageRepository.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("Something went wrong deleting message");
        }
    }
}
