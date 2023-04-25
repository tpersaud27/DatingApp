using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        public static async Task SeedUsers(DataContext context)
        {
            // Check if any Users exist in DB
            // If there are, we just stop and return
            if (await context.Users.AnyAsync()) return;


            // Reading from the user seed data json file
            var userData = await File.ReadAllTextAsync("./UserSeedData/UserSeedData.json");
            // In case the data case is different from the properties
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            // We want to convert fron JSON to C# list
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);


            // We need to generate passwords for each user that we seeded because the seed data does not contain passwords
            foreach(var user in users)
            {
                // using var hmac = new HMACSHA512(); Covered using identity

                // We always store the username as lowercase in the DB
                user.UserName = user.UserName.ToLower();
                //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")); Covered using indentity
                //user.PasswordSalt = hmac.Key; Covered using identity

                // Signal EF to track the users we are adding
                context.Users.Add(user);
            }

            // Save the Users to the DB
            await context.SaveChangesAsync();

        }

    }
}
