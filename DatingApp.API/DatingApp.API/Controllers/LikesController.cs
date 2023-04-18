using DatingApp.API.DTOs;
using DatingApp.DAL.Implementation;
using DatingApp.DAL.Interfaces;
using DatingApp.Domain.Entities;
using DatingApp.Services.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class LikesController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }


        // username is the route parameter. This is the person the user will be liking
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            // This is the sourceUserId that will be liking another user
            // We get this from the token claims (this is the logged in user)
            var sourceUserId = int.Parse(User.GetUserId());
            // This is the user we will be liking
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            // Getting the sourceUser
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
            
            // Defensive coding: checking if the likedUser does not exist
            if(likedUser == null) { return NotFound(); }

            if(sourceUser.UserName == username) { return BadRequest("You cannot like youself!"); }

            // This will return the entity based on the two ids
            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            // This should be null if the user has not been liked
            if(userLike != null) { return BadRequest("You already like this user"); }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id,

            };

            // This will create the entry in the Likes table
            sourceUser.LikedUsers.Add(userLike);

            // temp using the save all from userepository
            if(await _userRepository.SaveAllAsync()) { return Ok(); }

            return BadRequest("Failed to like user!");


        }

        /// <summary>
        /// This will return a list of LikesDto where it can either of the list of users that the current user likes
        /// Or it can be the list of users that likes current user (likedBy)
        /// </summary>
        /// <param name="predicate">this can be the user or liked by users</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikesDto>>> GetUserLikes(string predicate)
        {
            var users = await _likesRepository.GetUserLikes(predicate, int.Parse(User.GetUserId()));

            return Ok(users);
        }



    }
}
