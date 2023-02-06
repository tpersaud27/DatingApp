using AutoMapper;
using DatingApp.DAL;
using DatingApp.DAL.Implementation;
using DatingApp.DAL.Interfaces;
using DatingApp.Domain.DTOs;
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
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper){
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);

        }

        [HttpGet("id/{id}")] //api/users/id/{id}
        public async Task<ActionResult<MemberDto>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(userToReturn);

            
        }

        [HttpGet("username/{username}")] //api/users/{username}
        public async Task<ActionResult<MemberDto>> GetUserByUsername(string userName)
        {
            return await _userRepository.GetMemberByUsernameAsync(userName);
        }





    }
}
