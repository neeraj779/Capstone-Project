namespace Swar.API.Models.DTOs
{
    public class LikedSongsReturnDTO
    {
        public int UserId { get; set; }
        public string SongId { get; set; } = string.Empty;

        public DateTime LikedDate { get; set; } = DateTime.Now;
    }
}
