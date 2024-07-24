using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DBModels
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public byte[] HashedPassword { get; set; } = Array.Empty<byte>();
        public byte[] PasswordHashKey { get; set; } = Array.Empty<byte>();
        public string Status { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime RegistrationDate { get; set; }

        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<LikedSong> Likes { get; set; }
        public ICollection<PlayHistory> PlayHistories { get; set; }
        public ICollection<UserUploadedSong> UploadedSongs { get; set; }
    }
}
