namespace Swar.API.Models.DTOs
{
    public class PlaylistInfoDTO
    {
        public int PlaylistId { get; set; }
        public string PublicId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string PlaylistName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SongsCount { get; set; }
    }
}
