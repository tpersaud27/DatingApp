using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.Pagination;
using DatingApp.DAL;
using DatingApp.Services.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Implementation
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            // Adding the group to the db
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            // Getting the connection from the db based on the connection id
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public Task<PagedList<MessageDto>> GetMessageForUser()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all messages from the user
        /// </summary>
        /// <param name="messageParams"></param>
        /// <returns></returns>
        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            // We want the most recent first
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUserName == messageParams.Username && u.SenderDeleted == false),
                // Default case is unread
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.DateRead == null 
                        && u.RecipientDeleted == false)
            };

            // Map the query into out messageDto
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            // Finding group based on groupName (loading the connections with it)
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        /// <summary>
        /// Returns messages between two parties
        /// </summary>
        /// <param name="currentUsername"></param>
        /// <param name="recipientUsername"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            // This will allow us to retrieve messages between the two users
            // These are messages that either the current user or recipient as sent to one another
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    m => m.RecipientUsername == currentUsername 
                    && m.RecipientDeleted == false
                    && m.SenderUserName == recipientUsername
                    || m.RecipientUsername == recipientUsername 
                    && m.SenderDeleted == false
                    && m.SenderUserName == currentUsername
                )
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            // We will not go to the database to get these unread messages. We will get the from the query above
            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Any())
            {
                // Loop through the unread messages and make then read
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
