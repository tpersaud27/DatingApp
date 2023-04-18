using DatingApp.API.Pagination;

namespace DatingApp.Services.Pagination
{
    public class UserParams: PaginationParams
    {

        // This is going to be used for filtering
        // We want to exclude the current user from the filtered results
        public string? CurrentUsername { get; set; }
        // We want to get the desired members with the gender the user wants to see
        public string? Gender { get; set; }
        // This is the min age of the memebers the users want to see
        public int MinAge { get; set; } = 18;
        // Max age
        public int MaxAge { get; set; } = 100;

        public string OrderBy { get; set; } = "lastActive";



    }
}
