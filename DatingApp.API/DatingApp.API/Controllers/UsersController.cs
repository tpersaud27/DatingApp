using DatingApp.DAL;
using DatingApp.DAL.Implementation;
using DatingApp.DAL.Interfaces;
using DatingApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository){
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _userRepository.GetUsersAsync());
        }

        [HttpGet("id/{id}")] //api/users/id/{id}
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            return Ok(await _userRepository.GetUserByIdAsync(id));
            
        }

        [HttpGet("username/{username}")] //api/users/{username}
        public async Task<ActionResult<AppUser>> GetUserByUsername(string userName)
        {
            return Ok(await _userRepository.GetUserByUsernameAsync(userName));
        }



    }
}
