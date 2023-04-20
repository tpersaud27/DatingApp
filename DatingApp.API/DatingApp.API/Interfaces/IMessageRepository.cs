using DatingApp.API.Entities;
using DatingApp.Services.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Interfaces
{
    public interface IMessageRepository
    {


        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>> GetMessageForUser();

        // Returns the list of messages between two users
        Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId);



    }
}
