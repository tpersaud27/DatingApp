using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;

namespace DatingApp.API.Entities
{
    public class AppRole: IdentityRole<int>
    {

        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}
