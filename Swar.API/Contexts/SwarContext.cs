using Microsoft.EntityFrameworkCore;
using Swar.API.Models.DBModels;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureRelationships(modelBuilder);
            ConfigureCompositeKeys(modelBuilder);
            ConfigureUniqueIndices(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Playlists)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.LikedSongs)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PlayHistories)
                .WithOne(ph => ph.User)
                .HasForeignKey(ph => ph.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.PlaylistSongs)
                .WithOne(ps => ps.Playlist)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<LikedSong>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.LikedSongs)
                .HasForeignKey(ul => ul.UserId);

            modelBuilder.Entity<PlayHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PlayHistories)
                .HasForeignKey(ph => ph.UserId);
        }

        private void ConfigureCompositeKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });
        }

        private void ConfigureUniqueIndices(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<LikedSong>()
                .HasIndex(ul => new { ul.UserId, ul.SongId })
                .IsUnique();

            modelBuilder.Entity<Playlist>()
                .HasIndex(p => p.PublicId)
                .IsUnique();
        }
    }
}
