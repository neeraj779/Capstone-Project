using Swar.API.Models.DTOs;

namespace Swar.API.Interfaces.Services
{
    public interface IUserService
    {
        public Task<LoginReturnDTO> Login(UserLoginDTO user);
        public Task<RegisteredUserDTO> Register(UserRegisterDTO user);
        public Task<AccessTokenDTO> RefreshToken(int userId);
        public Task<IEnumerable<RegisteredUserDTO>> GetAllUsers();
        public Task<RegisteredUserDTO> ActivateUser(int id);
        public Task<RegisteredUserDTO> DeactivateUser(int id);
    }
}
