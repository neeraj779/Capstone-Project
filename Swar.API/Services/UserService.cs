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
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepo, ITokenService tokenService, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginResultDTO> Login(UserLoginDTO loginDTO)
        {
            var user = await _userRepo.GetByEmail(loginDTO.Email);
            ValidateUser(user);

            if (user.HashedPassword == null || user.PasswordHashKey == null)
                throw new ExternalServiceLoginException();

            if (!IsPasswordCorrect(loginDTO.Password, user.PasswordHashKey, user.HashedPassword))
                throw new InvalidCredentialsException();

            _logger.LogInformation("Login attempt for user ID: {UserId}", user.UserId);

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
            {
                _logger.LogWarning("Registration attempt for already registered userid {UserId}", user.UserId);
                throw new EntityAlreadyExistsException("You are already registered. Please login", user.UserId);
            }

            if (string.IsNullOrEmpty(userDTO.ExternalId))
            {
                if (string.IsNullOrEmpty(userDTO.Password))
                    throw new ArgumentException("Password is required for internal registration.");

                if (!IsPasswordStrong(userDTO.Password))
                    throw new WeakPasswordException();
            }

            User newUser = await CreateUser(userDTO, UserRoleEnum.UserRole.User);

            return MapUserToReturnDTO(newUser);
        }

        public async Task<AccessTokenDTO> RefreshToken(int userId)
        {
            var user = await _userRepo.GetById(userId);
            ValidateUser(user);

            _logger.LogInformation("Token refresh for user ID: {UserId}", user.UserId);

            return new AccessTokenDTO
            {
                AccessToken = _tokenService.GenerateJwtToken(user, "access"),
                TokenType = "Bearer"
            };
        }

        private async Task<User> CreateUser(UserRegisterDTO user, UserRoleEnum.UserRole role)
        {
            User newUser = new User();

            if (!string.IsNullOrEmpty(user.Password))
            {
                HMACSHA512 hMACSHA = new HMACSHA512();
                newUser.PasswordHashKey = hMACSHA.Key;
                newUser.HashedPassword = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            }

            newUser.ExternalId = user.ExternalId;
            newUser.Name = user.Name;
            newUser.Email = user.Email;
            newUser.UserStatus = UserStatusEnum.UserStatus.Active;
            newUser.Role = role;
            newUser.RegistrationDate = DateTime.UtcNow;

            await _userRepo.Add(newUser);

            return newUser;
        }

        public async Task<RegisteredUserDTO> GetUserById(int userId)
        {
            User user = await _userRepo.GetById(userId);
            if (user == null)
                throw new EntityNotFoundException("User not found");

            _logger.LogInformation("User details fetched for user ID: {UserId}", user.UserId);

            return MapUserToReturnDTO(user);

        }

        public async Task<IEnumerable<RegisteredUserDTO>> GetAllUsers()
        {
            var users = await _userRepo.GetAll();

            if (users.Count() == 0)
            {
                _logger.LogWarning("No users found during GetAllUsers");
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
                {
                    _logger.LogWarning("Error during user update. Email already used for user ID: {UserId}", user.UserId);
                    throw new EntityAlreadyExistsException("An account with this email already exists");
                }
            }

            UpdateUserProperties(user, userUpdateDto);

            _logger.LogInformation("User details updated for user ID: {UserId}", user.UserId);

            User updatedUser = await _userRepo.Update(user);
            return MapUserToReturnDTO(updatedUser);

        }

        public async Task<RegisteredUserDTO> UpdateUserPassword(int userId, UserPasswordUpdateDTO userPasswordUpdateDto)
        {
            User user = await _userRepo.GetById(userId);
            ValidateUser(user);

            if (!IsPasswordCorrect(userPasswordUpdateDto.OldPassword, user.PasswordHashKey, user.HashedPassword))
            {
                _logger.LogWarning("Error during user password update. Old password is incorrect for user ID: {UserId}", user.UserId);
                throw new InvalidCredentialsException("Old password is incorrect");
            }

            if (!IsPasswordStrong(userPasswordUpdateDto.NewPassword))
                throw new WeakPasswordException();

            HMACSHA512 hMACSHA = new HMACSHA512();
            user.PasswordHashKey = hMACSHA.Key;
            user.HashedPassword = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userPasswordUpdateDto.NewPassword));

            await _userRepo.Update(user);

            _logger.LogInformation("User password updated for user ID: {UserId}", user.UserId);
            return MapUserToReturnDTO(user);
        }

        public async Task<RegisteredUserDTO> DeleteUser(int userId)
        {
            User user = await _userRepo.GetById(userId);
            if (user == null)
                throw new EntityNotFoundException("User not found");

            await _userRepo.Delete(userId);
            _logger.LogInformation("User deleted for user ID: {UserId}", user.UserId);

            return MapUserToReturnDTO(user);
        }


        public async Task<RegisteredUserDTO> ActivateUser(int id, int adminId)
        {
            ValidateUser(await _userRepo.GetById(adminId), true);

            var user = await _userRepo.GetById(id);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                throw new EntityNotFoundException("User not found");
            }

            user.UserStatus = UserStatusEnum.UserStatus.Active;
            await _userRepo.Update(user);

            _logger.LogInformation("User activated for user ID: {UserId}", user.UserId);
            return MapUserToReturnDTO(user);
        }

        public async Task<RegisteredUserDTO> DeactivateUser(int id, int adminId)
        {
            ValidateUser(await _userRepo.GetById(adminId), true);

            var user = await _userRepo.GetById(id);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                throw new EntityNotFoundException("User not found");
            }

            user.UserStatus = UserStatusEnum.UserStatus.Inactive;
            await _userRepo.Update(user);

            _logger.LogInformation("User deactivated for user ID: {UserId}", user.UserId);
            return MapUserToReturnDTO(user);
        }

        public async Task<int?> GetUserIdByEmail(string email)
        {
            User user = await _userRepo.GetByEmail(email);
            return user?.UserId;
        }

        private void ValidateUser(User user, bool isAdmin = false)
        {
            string userType = isAdmin ? "Admin" : "User";
            if (user == null)
            {
                _logger.LogWarning("{UserType} does not exist", userType);
                throw new EntityNotFoundException($"{userType} does not exist");
            }

            if (user.UserStatus == UserStatusEnum.UserStatus.Inactive)
            {
                _logger.LogWarning("{UserType} account is inactive", userType);
                throw new InactiveAccountException($"{userType} account is inactive");
            }
        }

        private bool IsPasswordStrong(string password)
        {
            const int minLength = 8;
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return password.Length >= minLength && hasUpperCase && hasLowerCase && hasDigit;
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
        }


        public RegisteredUserDTO MapUserToReturnDTO(User user)
        {
            RegisteredUserDTO registeredUserDTO = new RegisteredUserDTO
            {
                UserId = user.UserId,
                ExternalId = user.ExternalId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Status = user.UserStatus.ToString(),
                RegistrationDate = user.RegistrationDate
            };
            return registeredUserDTO;
        }
    }
}
