using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DBModels
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}
