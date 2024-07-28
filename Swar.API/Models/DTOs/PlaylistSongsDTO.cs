namespace Swar.API.Models.DTOs
{
    public class PlaylistSongsDTO
    {
        public int PlaylistId { get; set; }
        public List<string> Songs { get; set; } = new List<string>();
    }
}
