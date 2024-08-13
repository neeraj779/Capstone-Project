namespace Swar.API.Models.DTOs
{
    public class PlaylistSongsDTO
    {
        public PlaylistInfoDTO PlaylistInfo { get; set; } = new PlaylistInfoDTO();
        public List<string> Songs { get; set; } = new List<string>();
    }
}
