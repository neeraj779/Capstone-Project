namespace Swar.API.Models.DTOs
{
    public class ReturnPlaylistDTO
    {
        public int UserId { get; set; }
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
