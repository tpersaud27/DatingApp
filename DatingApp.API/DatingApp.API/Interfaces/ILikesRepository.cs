using DatingApp.Domain.Entities;

namespace DatingApp.DAL.Interfaces
{
    public interface ILikesRepository
    {

        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)


    }
}
