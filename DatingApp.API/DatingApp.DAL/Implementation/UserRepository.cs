using DatingApp.DAL.Interfaces;
using DatingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.DAL.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Returns user based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user =  await _context.Users.FindAsync(id);

            if(user == null) {
                return null;
            }

            return user;
        }

        public async Task<AppUser> GetUserByUsernameAsync(string userName)
        {
            var user = await _context.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if(user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            // Note: This is known as eager loading. This will include the related data with the users. In this case it will include the photos as well.
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        /// <summary>
        /// We will true if something is saves. False is nothing is saved
        /// </summary>
        /// <returns>True is something is saved. False otherwise</returns>
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async void Update(AppUser user)
        {
            // This tells entity framework change tracking that something has changed with the user entity 
            // This might not be necessary but it will be included anyways. EF is automatically tracking when there is a change to an entity
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
