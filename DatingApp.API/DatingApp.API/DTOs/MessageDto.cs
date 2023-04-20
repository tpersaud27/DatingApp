using DatingApp.Domain.Entities;

namespace DatingApp.API.DTOs
{
    public class MessageDto
    {

        // This is the Id of the message
        // This will be the primary key
        // There will be two foreign keys associated, one is the SenderId and the other is the RecipientID
        public int Id { get; set; }
        // This is the sender Id
        public int SenderId { get; set; }
        // This is the username of the sender
        public string SenderUserName { get; set; }
        // We want to display the photo of the user along side the message
        public string SenderPhotoUrl { get; set; }

        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }

        // This is the message content
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }

    }
}
