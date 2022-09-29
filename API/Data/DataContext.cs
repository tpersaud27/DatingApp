using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    // This class is what we use to communicate with the database 
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // The type of DbSet is AppUser, where the name is the database will be users 
        // DbSet represents the collection of all entities in the context,
        // This would be the collection of all AppUser entities in the db 
        public DbSet<AppUser> Users { get; set; }

    }
}