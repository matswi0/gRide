using gRide.DAL;
using gRide.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace gRide.Data
{
    public class gRideDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        private readonly IWebHostEnvironment _env;

        public gRideDbContext(DbContextOptions options, IWebHostEnvironment env) : base(options)
        {
            _env = env;
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<AppUserEvent> AppUsersEvents { get; set; }
        public DbSet<AppUserFriends> AppUsersFriends { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUserEvent>().HasKey(ue => new { ue.UserId, ue.EventId });
            builder.Entity<AppUserEvent>()
                .HasOne(u => u.User)
                .WithMany(ue => ue.EventsLinked)
                .HasForeignKey(ue => ue.UserId);
            builder.Entity<AppUserEvent>()
                .HasOne(u => u.Event)
                .WithMany(ue => ue.UsersLinked)
                .HasForeignKey(ue => ue.EventId);

            builder.Entity<AppUserFriends>().HasKey(uf => new { uf.UserId, uf.FriendId });
            builder.Entity<AppUserFriends>()
                .HasOne(uf => uf.User)
                .WithMany()
                .HasForeignKey(uf => uf.UserId);
            builder.Entity<AppUserFriends>()
                .HasOne(uf => uf.FriendUser)
                .WithMany(uf => uf.Friends)
                .HasForeignKey(uf => uf.FriendId);

            builder.Seed(_env);
        }
    }
}
