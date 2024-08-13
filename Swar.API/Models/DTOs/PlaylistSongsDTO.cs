namespace Swar.API.Models.DTOs
{
    public class PlaylistSongsDTO
    {
        public int PlaylistId { get; set; }
        public string PublicId { get; set; } = Guid.NewGuid().ToString("N");
        public string OwnerName { get; set; } = string.Empty;
        public string PlaylistName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SongsCount { get; set; }
        public List<string> Songs { get; set; } = new List<string>();
    }
}
