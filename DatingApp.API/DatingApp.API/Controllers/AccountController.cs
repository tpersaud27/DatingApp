using DatingApp.DAL;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.API.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        // Note: Its the frameworks job to look at which implementation is being used for the interfaces being injected
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }


        /// <summary>
        /// Allows user to register
        /// Endpoint: api/Account/register
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {

            // Check if the userName is already in use
            if(await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken.");
            }

            // We want to hash the password
            // When we are done using this class we want to dispose of it. This is why we are using the 'using' keyword here.
            using var hmac = new HMACSHA512();

            // We need to map the Dto to the AppUser entity
            var user = new AppUser{

                // When the user first registers we want to store the username in lowercase in the DB
                UserName = registerDto.Username.ToLower(),
                // We had to convert the string to a byte array
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                // HMAC class comes with a randomly key, this will be used as our PasswordSalt
                PasswordSalt = hmac.Key
            };

            // This will track out user in memory
            _context.Users.Add(user);
            // This will save the user to the db
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        /// <summary>
        /// This endpoint validates a users credentials - allowing them to login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>UserDto of the user loggin in</returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Retrieve the user
            // Note: We use first or default because default will return null
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

            // Check if the user exists
            if(user == null)
            {
                // Return unauthorized status code, indicating user does not exist
                return Unauthorized("Invalid Username.");
            }

            // Check the password
            // To do this we need to get the same hash. To do this, we pass in the Key into the ComputeHash function. This should produce an identical hash that we have stored in the db
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Compare each element in the byte array and compare this with the hashed password
            for( int i =0; i< computedHash.Length; i++ ) 
            {
                // If any single element does not match we return unauthorized because the password entered in incorrect.
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password.");
                }
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        /// <summary>
        /// Checks if the user exists in the database
        /// </summary>
        /// <returns>True if user exists, false otherwise</returns>
        private async Task<bool> UserExists(string userName)
        {
            // This will return true if the username exists in the Users table
            return await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }

    }
}
