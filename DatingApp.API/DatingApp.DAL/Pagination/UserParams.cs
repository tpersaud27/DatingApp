namespace DatingApp.Services.Pagination
{
    public class UserParams
    {
        // This is the maximum number 
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        // Default page size
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }



    }
}
