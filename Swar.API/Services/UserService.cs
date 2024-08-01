using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using Swar.API.Models.DTOs;
using Swar.API.Models.ENUMs;
using System.Security.Cryptography;
using System.Text;

namespace Swar.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepo, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _userRepo = userRepo;
        }

        public async Task<LoginResultDTO> Login(UserLoginDTO loginDTO)
        {
            var user = await _userRepo.GetByEmail(loginDTO.Email);
            ValidateUser(user);

            if (!IsPasswordCorrect(loginDTO.Password, user.PasswordHashKey, user.HashedPassword))
                throw new InvalidCredentialsException();

            return new LoginResultDTO
            {
                AccessToken = _tokenService.GenerateJwtToken(user, "access"),
                RefreshToken = _tokenService.GenerateJwtToken(user, "refresh"),
                Role = user.Role.ToString(),
                TokenType = "Bearer",
            };
        }

        public async Task<RegisteredUserDTO> Register(UserRegisterDTO userDTO)
        {
            var user = await _userRepo.GetByEmail(userDTO.Email);

            if (user != null)
                throw new EntityAlreadyExistsException("You are already registered. Please login");

            if (!IsPasswordStrong(userDTO.Password))
                throw new WeakPasswordException();

            User newUser = await CreateUser(userDTO, UserRoleEnum.UserRole.User);

            return MapUserToReturnDTO(newUser);
        }

        public async Task<AccessTokenDTO> RefreshToken(int userId)
        {
            var user = await _userRepo.GetById(userId);
            ValidateUser(user);

            return new AccessTokenDTO
            {
                AccessToken = _tokenService.GenerateJwtToken(user, "access"),
                TokenType = "Bearer"
            };
        }

        private async Task<User> CreateUser(UserRegisterDTO user, UserRoleEnum.UserRole role)
        {
            User newUser = new User();

            HMACSHA512 hMACSHA = new HMACSHA512();
            newUser.Name = user.Name;
            newUser.Email = user.Email;
            newUser.Gender = user.Gender;
            newUser.PasswordHashKey = hMACSHA.Key;
            newUser.HashedPassword = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            newUser.UserStatus = UserStatusEnum.UserStatus.Inactive;
            newUser.Role = role;
            newUser.RegistrationDate = DateTime.Now;

            await _userRepo.Add(newUser);

            return newUser;
        }

        public async Task<RegisteredUserDTO> GetUserById(int userId)
        {
            User user = await _userRepo.GetById(userId);
            if (user == null)
                throw new EntityNotFoundException("User not found");

            return MapUserToReturnDTO(user);

        }

        public async Task<IEnumerable<RegisteredUserDTO>> GetAllUsers()
        {
            var users = await _userRepo.GetAll();

            if (users.Count() == 0)
            {
                throw new EntityNotFoundException("No users found");
            }

            List<RegisteredUserDTO> userReturnDTOs = new List<RegisteredUserDTO>();
            foreach (var user in users)
                userReturnDTOs.Add(MapUserToReturnDTO(user));

            return userReturnDTOs;
        }

        public async Task<RegisteredUserDTO> UpdateUser(int userId, UserUpdateDTO userUpdateDto)
        {
            User user = await _userRepo.GetById(userId);
            ValidateUser(user);

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Email))
            {
                var isEmailExist = await _userRepo.GetByEmail(userUpdateDto.Email);
                if (isEmailExist != null && isEmailExist.UserId != userId)
                    throw new EntityAlreadyExistsException("An account with this email already exists");
            }

            UpdateUserProperties(user, userUpdateDto);

            User updatedUser = await _userRepo.Update(user);
            return MapUserToReturnDTO(updatedUser);

        }

        public async Task<RegisteredUserDTO> UpdateUserPassword(int userId, UserPasswordUpdateDTO userPasswordUpdateDto)
        {
            User user = await _userRepo.GetById(userId);
            ValidateUser(user);

            if (!IsPasswordCorrect(userPasswordUpdateDto.OldPassword, user.PasswordHashKey, user.HashedPassword))
                throw new InvalidCredentialsException("Old password is incorrect");

            if (!IsPasswordStrong(userPasswordUpdateDto.NewPassword))
                throw new WeakPasswordException();

            HMACSHA512 hMACSHA = new HMACSHA512();
            user.PasswordHashKey = hMACSHA.Key;
            user.HashedPassword = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userPasswordUpdateDto.NewPassword));

            await _userRepo.Update(user);
            return MapUserToReturnDTO(user);
        }

        public async Task<RegisteredUserDTO> DeleteUser(int userId)
        {
            User user = await _userRepo.GetById(userId);
            if (user == null)
                throw new EntityNotFoundException("User not found");

            await _userRepo.Delete(userId);
            return MapUserToReturnDTO(user);
        }


        public async Task<RegisteredUserDTO> ActivateUser(int id)
        {
            var user = await _userRepo.GetById(id);
            ValidateUser(user);

            user.UserStatus = UserStatusEnum.UserStatus.Active;
            await _userRepo.Update(user);

            return MapUserToReturnDTO(user);
        }

        public async Task<RegisteredUserDTO> DeactivateUser(int id)
        {
            var user = await _userRepo.GetById(id);
            ValidateUser(user);

            user.UserStatus = UserStatusEnum.UserStatus.Inactive;
            await _userRepo.Update(user);

            return MapUserToReturnDTO(user);
        }

        private void ValidateUser(User user)
        {
            if (user == null)
                throw new EntityNotFoundException("You are not registered");

            if (user.UserStatus == UserStatusEnum.UserStatus.Inactive)
                throw new InactiveAccountException();
        }

        private bool IsPasswordStrong(string password)
        {
            const int minLength = 8;
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return password.Length >= minLength && hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        private bool IsPasswordCorrect(string password, byte[] passwordHashKey, byte[] storedPasswordHash)
        {
            using var hmac = new HMACSHA512(passwordHashKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedPasswordHash);
        }

        private void UpdateUserProperties(User user, UserUpdateDTO userUpdateDto)
        {
            if (!string.IsNullOrWhiteSpace(userUpdateDto.Name))
            {
                user.Name = userUpdateDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Email))
            {
                user.Email = userUpdateDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Gender))
            {
                user.Gender = userUpdateDto.Gender;
            }
        }


        public RegisteredUserDTO MapUserToReturnDTO(User user)
        {
            RegisteredUserDTO registeredUserDTO = new RegisteredUserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender,
                Role = user.Role.ToString(),
                Status = user.UserStatus.ToString(),
                RegistrationDate = user.RegistrationDate
            };
            return registeredUserDTO;
        }
    }
}
