using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DBModels
{
    public class UserUploadedSong
    {
        [Key]
        public int SongId { get; set; }
        public string SongName { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public string Genre { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
