namespace DatingApp.API.Pagination
{
    public class PaginationParams
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
    }
}
