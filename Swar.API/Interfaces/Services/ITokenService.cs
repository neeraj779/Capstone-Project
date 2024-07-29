using Swar.API.Models.DBModels;

namespace Swar.API.Interfaces.Services
{
    public interface ITokenService
    {
        public string GenerateJwtToken(User user, string token_type);
    }
}
