using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;

        }

        // This will return a action result that is of type AppUser
        // This will send a post request to register a new user
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            // Check to see if there is a matching user in the database
            if (await UserExists(registerDto.Username))
            {
                // we return a BadRequest http response (400 http status code)
                // We get this method from returning an action result
                return BadRequest("Username is taken!");
            }

            // The using keyword here triggers the IDispose method to dispose of this class once done
            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            // This tells entity framework we want to add a new user to the Users table in the database
            // This will just track the entity
            _context.Users.Add(user);
            // This will save the changes made to the content in the database
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // First thing we do when we login is get the user from the database
            // This will return the user from the database where the username matches the login entered
            var user = await _context.Users
                        .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null)
            {
                // Return a 401 http response code error
                return Unauthorized("Invalid Username");
            }

            // We need to get the user password from the db so we pass in the password salt (key)
            // This would give us the same computed hash because we are giving it the same key
            using var hmac = new HMACSHA512(user.PasswordSalt);
            // This is the hash that is generated when the user is trying to login
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Check if the hash that is generated when the user enters the password matches the hash in the db 
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password");
                }
            }

            // If the hash that is generated when the user enters the password matches the hash in the database
            // Return the user
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };


        }


        // This method will determine if a username already exists in the database
        // Remember Users is the name of the database table
        private async Task<bool> UserExists(string username)
        {
            // This will check if the user that is trying to register is using an existing username
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }


    }
}
