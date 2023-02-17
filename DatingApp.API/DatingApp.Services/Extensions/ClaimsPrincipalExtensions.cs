using System.Security.Claims;

namespace DatingApp.Services.Extensions
{
    // Extension methods always have static class
    public static class ClaimsPrincipalExtensions
    {

        public static string GetUsername(this ClaimsPrincipal user)
        {
            // This 'User' is from System.Security.Claim.Claims. 
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

    }
}
