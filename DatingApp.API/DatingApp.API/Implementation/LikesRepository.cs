﻿using DatingApp.API.DTOs;
using DatingApp.API.Pagination;
using DatingApp.DAL;
using DatingApp.DAL.Interfaces;
using DatingApp.Domain.Entities;
using DatingApp.Services.Extensions;
using DatingApp.Services.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Implementation
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
  
        }

        /// <summary>
        /// Finds the UserLike entity that mataches the primary key of the two ids
        /// </summary>
        /// <param name="sourceUserId"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        /// <summary>
        /// sourceUserId would return the list of users that are liked by the currently logged in user
        /// targetUserId would return the list of users that the user is liked by
        /// </summary>
        /// <param name="predicate">This can be the source user or target user</param>
        /// <param name="userId">This can be the sourceuserid or the targetuserid</param>
        /// <returns></returns>
        public async Task<PagedList<LikesDto>> GetUserLikes(LikesParams likesParams)
        {
            // This will start building a query that returns a list ordering the users by their username
            // AsQueryable means it has not been executed yet
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

            var likes = _context.Likes.AsQueryable(); 

            if(likesParams.Predicate == "liked")
            {
                // Get the likes that are liked by the currently logged in user
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                // This will get users that are liked
                users = likes.Select(like => like.TargetUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                // Get the likes that are liked by the currently logged in user
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            // executing the query
            // This will give us a list of users that are liked or likedBy
            var likedUsers = users.Select(user => new LikesDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            // This will return a paged list
            return await PagedList<LikesDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }
            
        /// <summary>
        /// This will allow us to check if a user has been liked by another user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}