using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using DatingApp.Services.Pagination;

namespace DatingApp.DAL.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string userName);

        Task<MemberDto> GetMemberByIdAsync(int id);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberByUsernameAsync(string userName);

    }
}
