using DatingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.DAL
{
    /// <summary>
    /// This will be used as a service in the project. 
    /// </summary>
    public class DataContext: DbContext
    {

        public DataContext()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">This is what we use to configure the class. Specifically the connection string</param>
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// This is the Users table
        /// The columns of the table will be that of AppUser( Id and UserName)
        /// </summary>
        public DbSet<AppUser> Users { get; set; }
    }
}
