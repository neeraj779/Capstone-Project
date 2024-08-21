using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DTOs
{
    public class UserRegisterDTO
    {
        public string? ExternalId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }
    }
}
