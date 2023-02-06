using DatingApp.Services.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities
{
    /// <summary>
    /// This is the entity for the user
    /// </summary>
    public class AppUser
    {

        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        // This is when the user was created
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();

        /// <summary>
        /// This method will calculate the age of a user
        /// </summary>
        /// <returns>User age</returns>
        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}




    }
}
