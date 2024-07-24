using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DBModels
{
    public class PlayHistory
    {
        [Key]
        public int HistoryId { get; set; }
        public string SongId { get; set; } = string.Empty;
        public DateTime PlayedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
