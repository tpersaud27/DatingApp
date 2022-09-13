using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseAPIController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        // This method will return a list of all users
        [HttpGet]
        [AllowAnonymous]
        // IEnumerable is one way of returning lists in dotnet. We can use lists too but we dont need all the method associated to the list
        // We use the IEnumerable of type AppUser, this will return us the list of users
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            // ToListAsync is a method from EntityFrameworkCore
            return await _context.Users.ToListAsync();


        }

        // This method will return a user based on the id
        // GET api/users/3
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            // Find is method from EntityFrameworkCore
            return await _context.Users.FindAsync(id);
        }



    }
}