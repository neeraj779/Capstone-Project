namespace Swar.API.Models.DBModels
{
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = default!;

        public string SongId { get; set; } = string.Empty;
    }
}
