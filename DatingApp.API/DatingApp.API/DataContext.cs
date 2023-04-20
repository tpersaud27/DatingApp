using DatingApp.API.Entities;
using DatingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.DAL
{
    /// <summary>
    /// This will be used as a service in the project. 
    /// </summary>
    public class DataContext: DbContext
    {

        public DataContext()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">This is what we use to configure the class. Specifically the connection string</param>
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// This is the Users table
        /// The columns of the table will be that of AppUser( Id and UserName)
        /// </summary>
        public DbSet<AppUser> Users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(primaryKey => new { primaryKey.SourceUserId, primaryKey.TargetUserId });

            // Specify that the SourceUser can like many other users
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                // Have one recipient
                .HasOne(u => u.Recipient)
                // With many messages received
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                // Have one sender
                .HasOne(u => u.Sender)
                // With many messages sent
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
