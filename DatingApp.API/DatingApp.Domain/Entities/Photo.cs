using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Domain.Entities
{
    // The name of the tables will be called photos
    [Table("Photos")]
    public class Photo
    {

        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public int UserId { get; set; }

        // This is how we fully defined a relationship in entity framework
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }


    }
}