using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DTOs
{
    public class AddSongToPlaylistDTO
    {
        [Required(ErrorMessage = "PlaylistId is required.")]
        public int PlaylistId { get; set; }

        [Required(ErrorMessage = "SongId is required.")]
        [StringLength(50, ErrorMessage = "SongId cannot be longer than 50 characters.")]
        public string SongId { get; set; } = string.Empty;
    }
}
