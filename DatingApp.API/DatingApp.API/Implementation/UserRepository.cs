using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.DAL.Interfaces;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using DatingApp.Services.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.DAL.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<MemberDto> GetMemberByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<MemberDto> GetMemberByUsernameAsync(string userName)
        {
            return await _context.Users
                .Where(x => x.UserName == userName)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            // This will allow us to build up our query
            var query = _context.Users.AsQueryable();

            // This will exclude the currently logged in user from the query
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            // This will return all users with the specified gender
            query = query.Where(u => u.Gender == userParams.Gender);

            
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            // We will order the query results based on the time the account was created
            // Or the default case is when the user was last active
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive),
            };

            // This will return a paged list
            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider), 
                userParams.PageNumber, 
                userParams.PageSize);
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

        public void Update(AppUser user)
        {
            // This tells entity framework change tracking that something has changed with the user entity 
            // This might not be necessary but it will be included anyways. EF is automatically tracking when there is a change to an entity
             _context.Entry(user).State = EntityState.Modified;
        }
    }
}
