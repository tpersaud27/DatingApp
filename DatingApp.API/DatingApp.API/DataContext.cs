using DatingApp.API.Entities;
using DatingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.DAL
{
    /// <summary>
    /// This will be used as a service in the project. 
    /// </summary>
    public class DataContext: IdentityDbContext<
        AppUser,
        AppRole, 
        int,
        IdentityUserClaim<int>,
        AppUserRole, 
        IdentityUserLogin<int>, 
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
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
        /// 
        /// 
        /// Note: IdentityDbContent already has a table for users, so we will be using that one
        /// 
        /// </summary>
        /// 
        ///public DbSet<AppUser> Users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // this provide the AppUser with many relationships of userRoles, associated to one user
            // where the foreign key is the user id and must be required (not null)
            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            // this provide the AppRole with many relationships of UserRoles, associated to one role
            // where the foreign key is the role id and must be required (not null)
            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

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
