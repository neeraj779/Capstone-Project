namespace Swar.API.Models.DTOs
{
    public class AddSongToPlaylistDTO
    {
        public int PlaylistId { get; set; }
        public string SongId { get; set; } = string.Empty;
    }
}
