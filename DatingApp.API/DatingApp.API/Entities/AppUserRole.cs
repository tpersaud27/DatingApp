using DatingApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Entities
{
    /// <summary>
    /// This class will serve as the join table between the AppUser and AppRole
    /// </summary>
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }

    }
}
