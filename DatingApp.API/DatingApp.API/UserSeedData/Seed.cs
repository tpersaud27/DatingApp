using DatingApp.API.Entities;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.DAL.UserSeedData
{
    public class Seed
    {

        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        { 
            // Check if we have any users, if we do we return
            if (await userManager.Users.AnyAsync()) return;

            // Reading from the user seed data json file
            var userData = await File.ReadAllTextAsync("./UserSeedData/UserSeedData.json");
            // In case the data case is different from the properties
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            // We want to convert fron JSON to C# list
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // Creating some roles
            var roles = new List<AppRole>
                {
                    new AppRole{Name = "Member" },
                    new AppRole{Name = "Admin" },
                    new AppRole{Name = "Moderator" }
                };

            // Using role manager to add some roles
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // We need to generate passwords for each user that we seeded because the seed data does not contain passwords
            foreach (var user in users)
            {
                // We always store the username as lowercase in the DB
                user.UserName = user.UserName.ToLower();

                // Create and save the user in the database
                await userManager.CreateAsync(user, "Pa$$w0rd");
                // The role to the user
                await userManager.AddToRoleAsync(user, "Member");
            }
            // Creating the admin user and adding roles
            var admin = new AppUser
            { UserName = "admin"};
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

        }

    }
}
