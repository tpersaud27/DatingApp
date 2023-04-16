namespace DatingApp.Services.Pagination
{
    public class UserParams
    {
        // This is the maximum number of elements per page
        private const int MaxPageSize = 50;
        // This is the page number the user will be on, by default it starts at 1
        public int PageNumber { get; set; } = 1;

        // Default page size
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        // This is going to be used for filtering
        // We want to exclude the current user from the filtered results
        public string? CurrentUsername { get; set; }
        // We want to get the desired members with the gender the user wants to see
        public string? Gender { get; set; }
        // This is the min age of the memebers the users want to see
        public int MinAge { get; set; } = 18;
        // Max age
        public int MaxAge { get; set; } = 100;



    }
}
