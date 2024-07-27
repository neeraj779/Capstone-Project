using Microsoft.EntityFrameworkCore;
using Swar.API.Models.DBModels;
using Swar.API.Models.ENUMs;
using System.Security.Cryptography;
using System.Text;

namespace Swar.API.Contexts
{
    public class SwarContext : DbContext
    {
        public SwarContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<LikedSong> LikedSongs { get; set; }
        public DbSet<PlayHistory> PlayHistories { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<UserUploadedSong> UserUploadedSongs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var adminUser = new User
            {
                UserId = 100,
                UserName = "admin",
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.Admin,
                RegistrationDate = DateTime.UtcNow
            };

            HMACSHA512 hMACSHA = new HMACSHA512();
            adminUser.PasswordHashKey = hMACSHA.Key;
            adminUser.HashedPassword = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes("admin"));


            modelBuilder.Entity<User>().HasData(adminUser);

            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.PlaylistSongs)
                .WithOne(ps => ps.Playlist)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Playlists)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Likes)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PlayHistories)
                .WithOne(ph => ph.User)
                .HasForeignKey(ph => ph.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UploadedSongs)
                .WithOne(us => us.User)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId);


            modelBuilder.Entity<LikedSong>()
                .HasIndex(ul => new { ul.UserId, ul.SongId })
                .IsUnique();
        }
    }
}
