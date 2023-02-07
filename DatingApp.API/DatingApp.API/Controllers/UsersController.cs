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
using System.Security.Claims;

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


        [HttpPut()]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // This 'User' is from System.Security.Claim.Claims. 
            // In here we will get access to our token, which we can get the userName from
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // When we retrieve this user, it is being tracked by EF
            // Any changes will be tracked by EF
            var user = await _userRepository.GetUserByUsernameAsync(userName);

            if (user == null) return NotFound();

            // We can use mapper to update the properties from out memberDto to our AppUser
            // This will override the properties in our user with those from memberDto
            // At this point nothing is saved
            _mapper.Map(memberUpdateDto, user);

            // This will return a status code of 204
            if (await _userRepository.SaveAllAsync()) return NoContent();

            // This would occur if the user did not make any changes 
            return BadRequest("Failed to update user");
        }

    }
}
