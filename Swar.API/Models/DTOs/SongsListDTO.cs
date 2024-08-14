namespace Swar.API.Models.DTOs
{
    public class SongsListDTO
    {
        public int UserId { get; set; }
        public int songsCount { get; set; }
        public List<string> Songs { get; set; } = new List<string>();
    }
}
