using Swar.API.Models.DTOs;

namespace Swar.API.Interfaces.Services
{
    public interface IUserService
    {
        public Task<LoginResultDTO> Login(UserLoginDTO user);
        public Task<RegisteredUserDTO> Register(UserRegisterDTO user);
        public Task<AccessTokenDTO> RefreshToken(int userId);
        public Task<RegisteredUserDTO> GetUserById(int userId);
        public Task<IEnumerable<RegisteredUserDTO>> GetAllUsers();
        public Task<RegisteredUserDTO> UpdateUser(int userId, UserUpdateDTO user);
        public Task<RegisteredUserDTO> DeleteUser(int userId);
        public Task<RegisteredUserDTO> UpdateUserPassword(int userId, UserPasswordUpdateDTO user);
        public Task<RegisteredUserDTO> ActivateUser(int userId, int adminId);
        public Task<RegisteredUserDTO> DeactivateUser(int userId, int adminId);
    }
}
