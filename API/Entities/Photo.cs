using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        // Used for photo storage
        public string PublicId { get; set; }
        // This is how we fully define our relationship between the appuser and the photo
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }

    }
}