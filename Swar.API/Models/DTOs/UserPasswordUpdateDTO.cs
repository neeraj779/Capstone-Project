using System.ComponentModel.DataAnnotations;

namespace Swar.API.Models.DTOs
{
    public class UserPasswordUpdateDTO
    {
        [Required(ErrorMessage = "Password is required.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
