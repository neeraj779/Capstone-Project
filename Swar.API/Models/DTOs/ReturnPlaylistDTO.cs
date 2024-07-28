namespace Swar.API.Models.DTOs
{
    public class ReturnPlaylistDTO
    {
        public int UserId { get; set; }
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
