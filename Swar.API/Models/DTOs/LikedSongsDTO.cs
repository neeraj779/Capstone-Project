namespace Swar.API.Models.DTOs
{
    public class LikedSongsDTO
    {
        public int UserId { get; set; }
        public List<string> Songs { get; set; } = new List<string>();
    }
}
