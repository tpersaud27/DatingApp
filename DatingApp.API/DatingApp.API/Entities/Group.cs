using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Entities
{
    public class Group
    {
        public Group()
        {
            
        }
        public Group(string name)
        {
            Name = name;
        }

        // This will make sure the name is unique in the databse
        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();

    }
}