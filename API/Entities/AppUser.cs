using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;

namespace API.Entities
{

    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        // The time the uses registers to the application is when their account was created
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        // This is a one to many relationship
        // One user can have many photos
        // Entity framework will associate one user to each Photo
        public ICollection<Photo> Photos { get; set; }

        // This is an extension method to calculate the User Age
        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }
    }
}


