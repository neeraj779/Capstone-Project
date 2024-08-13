using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DTOs
{
    public class UpdatePlaylistDTO
    {
        [Required(ErrorMessage = "PlaylistName is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "PlaylistName must be between 1 and 100 characters.")]
        public string PlaylistName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
