using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DBModels
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }
        public string PublicId { get; set; } = Guid.NewGuid().ToString("N");
        public string PlaylistName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public ICollection<PlaylistSong>? PlaylistSongs { get; set; }
    }
}
