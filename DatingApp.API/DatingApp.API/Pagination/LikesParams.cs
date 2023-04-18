namespace DatingApp.API.Pagination
{
    public class LikesParams: PaginationParams
    {

        // Here we want to add properties in addition to those in the pagination params

        public int UserId { get; set; }
        public string Predicate { get; set; }

    }
}
