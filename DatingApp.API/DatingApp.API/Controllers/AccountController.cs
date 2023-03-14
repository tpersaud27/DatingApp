using AutoMapper;
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
        private readonly IMapper _mapper;

        // Note: Its the frameworks job to look at which implementation is being used for the interfaces being injected
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
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

            // Here we map the user values from the registerDto to the AppUser
            // At this point all values passed in from the request body are mapped
            var user = _mapper.Map<AppUser>(registerDto);

            // We want to hash the password
            // When we are done using this class we want to dispose of it. This is why we are using the 'using' keyword here.
            using var hmac = new HMACSHA512();

            // Note: For the code below we are this new user which is mapped into the AppUser will ...
            // contain the values that are passed in from the body of the request
            // when the user first registers we want to store the username in lowercase in the DB
            user.UserName = registerDto.Username.ToLower();
            // We had to convert the string to a byte array
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            // HMAC class comes with a randomly key, this will be used as our PasswordSalt
            user.PasswordSalt = hmac.Key;

            // This will track out user in memory
            _context.Users.Add(user);
            // This will save the user to the db
            await _context.SaveChangesAsync();

            // We return a UserDto because the only information we want to expose is the JWT, Username, and KnownAs
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
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
            var user = await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

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
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
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
