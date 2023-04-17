using DatingApp.API.DTOs;
using DatingApp.Domain.Entities;

namespace DatingApp.DAL.Interfaces
{
    public interface ILikesRepository
    {

        /// <summary>
        /// Returns the UserLike entity containing informtation of the two parties
        /// </summary>
        /// <param name="sourceUserId">This is the user liking the other person</param>
        /// <param name="targetUserId">This is the other person</param>
        /// <returns></returns>
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);


        Task<AppUser> GetUserWithLikes(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"> This can either be the user likes or the users who liked the user</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<LikesDto>> GetUserLikes(string predicate, int userId);



    }
}
