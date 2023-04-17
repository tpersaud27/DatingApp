namespace DatingApp.Domain.Entities
{
    /// <summary>
    /// This class will act as a join table between two AppUser entities
    /// This will be a many to many relationship between two AppUser entities
    /// </summary>
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser TargetUser { get; set; }
        public int TargetUserId { get; set; }
    }
}
