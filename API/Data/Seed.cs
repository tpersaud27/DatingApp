using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUser(DataContext context){
            // Check if users table contains any users
            if(await context.Users.AnyAsync()) return;

            // If we do not have any users
        }
    }
}