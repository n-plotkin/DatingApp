using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        //base(options) passes DbContextOptions options to the constructor of the parent DbContext class
        public DataContext(DbContextOptions options) : base(options) 
        {
            
        }
        //in a DbContext, a DbSet represents a table in the relational database.
        public DbSet<AppUser> Users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //we manually configure entity here
    
            //HasKey passes a UserLike instance k
            //The lambda returns a composite key of k's properties
            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.TargetUserId});

            //meaning each SourceUser in a UserLike has many liked users
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Meaning each TargetUser in a UserLike has many LikedByUsers
            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);



            builder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesRecieved)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}