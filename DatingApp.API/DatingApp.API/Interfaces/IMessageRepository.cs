using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Pagination;
using DatingApp.Services.Pagination;

namespace DatingApp.API.Interfaces
{
    public interface IMessageRepository
    {

        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);

        // Returns the list of messages between two users
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientName);

        Task<bool> SaveAllAsync();


    }
}
