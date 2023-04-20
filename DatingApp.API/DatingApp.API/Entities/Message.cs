using DatingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.Pkcs;

namespace DatingApp.API.Entities
{
    public class Message
    {
        // This is the Id of the message
        // This will be the primary key
        // There will be two foreign keys associated, one is the SenderId and the other is the RecipientID
        public int Id { get; set; }
        // This is the sender Id
        public int SenderId { get; set; }
        // This is the username of the sender
        public string SenderUserName { get; set; }
        // This is the appuser entity for the sender
        // This is the related entity 
        public AppUser Sender { get; set; }

        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        
        // This is the message content
        public string Content { get; set; }

        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        
        // We will only delete the message from the database if both of these values are true
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }


    }
}
