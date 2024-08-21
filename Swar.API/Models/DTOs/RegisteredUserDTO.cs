namespace Swar.API.Models.DTOs
{
    public class RegisteredUserDTO
    {
        public int UserId { get; set; }
        public string? ExternalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
    }
}
