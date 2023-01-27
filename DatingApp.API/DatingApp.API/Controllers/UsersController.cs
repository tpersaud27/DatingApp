using DatingApp.DAL;
using DatingApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")] //api/Users
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context){
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            // This will return a list of all users in the db
            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")] //api/users/{id}
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<AppUser> AddNewUser(AppUser appUser)
        {

            await _context.Users.AddAsync(appUser);
            await _context.SaveChangesAsync();

            return appUser;

        }



    }
}
