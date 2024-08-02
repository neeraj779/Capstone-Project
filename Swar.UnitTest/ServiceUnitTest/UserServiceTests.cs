using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using Swar.API.Models.DTOs;
using Swar.API.Models.ENUMs;
using Swar.API.Repositories;
using Swar.API.Services;
using System.Security.Cryptography;
using System.Text;

namespace Swar.API.Tests.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private DbContextOptions<SwarContext> _dbContextOptions;
        private SwarContext _dbContext;
        private IUserRepository _userRepository;
        private Mock<ITokenService> _tokenService;
        private Mock<ILogger<UserService>> _loggerMock;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryUserDb")
                .Options;
            _dbContext = new SwarContext(_dbContextOptions);

            _userRepository = new UserRepository(_dbContext);
            _tokenService = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _userService = new UserService(_userRepository, _tokenService.Object, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsLoginResultDTO()
        {
            // Arrange
            var password = "Password123!";
            var user = CreateUser("user@example.com", password);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var loginDTO = new UserLoginDTO
            {
                Email = user.Email,
                Password = password
            };

            // Act
            var result = await _userService.Login(loginDTO);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Login_InvalidCredentials_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var loginDTO = new UserLoginDTO
            {
                Email = user.Email,
                Password = "WrongPassword!"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.Login(loginDTO));
        }

        [Test]
        public async Task Login_InactiveUser_ThrowsInactiveUserException()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            user.UserStatus = UserStatusEnum.UserStatus.Inactive;

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var loginDTO = new UserLoginDTO
            {
                Email = user.Email,
                Password = "Password123!"
            };

            // Act & Assert
            Assert.ThrowsAsync<InactiveAccountException>(() => _userService.Login(loginDTO));
        }

        [Test]
        public async Task Register_ValidUser_ReturnsRegisteredUserDTO()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDTO
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "StrongPassword1!",
                Gender = "Male"
            };

            // Act
            var result = await _userService.Register(userRegisterDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo(userRegisterDto.Email));
            var userInDb = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == userRegisterDto.Email);
            Assert.IsNotNull(userInDb);
        }

        [Test]
        public void Register_UserAlreadyExists_ThrowsEntityAlreadyExistsException()
        {
            // Arrange
            var existingUser = CreateUser("existing@example.com", "Password123!");
            _dbContext.Users.Add(existingUser);
            _dbContext.SaveChanges();

            var userRegisterDto = new UserRegisterDTO
            {
                Name = "New User",
                Email = existingUser.Email,
                Password = "StrongPassword1!",
                Gender = "Male"
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _userService.Register(userRegisterDto));
        }

        [Test]
        public void Register_WeakPassword_ThrowsWeakPasswordException()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDTO
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "weak",
                Gender = "Male"
            };

            // Act & Assert
            Assert.ThrowsAsync<WeakPasswordException>(() => _userService.Register(userRegisterDto));
        }

        [Test]
        public async Task RefreshToken_ValidUser_ReturnsAccessTokenDTO()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.RefreshToken(user.UserId);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void RefreshToken_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.RefreshToken(999));
        }

        [Test]
        public async Task GetUserById_ValidUser_ReturnsRegisteredUserDTO()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserById(user.UserId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }

        [Test]
        public void GetUserById_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetUserById(999));
        }

        [Test]
        public async Task GetAllUsers_UsersExist_ReturnsListOfRegisteredUserDTO()
        {
            // Arrange
            var user1 = CreateUser("user1@example.com", "Password123!");
            var user2 = CreateUser("user2@example.com", "Password123!");
            await _dbContext.Users.AddRangeAsync(user1, user2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetAllUsers();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllUsers_NoUsersFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetAllUsers());
        }

        [Test]
        public async Task UpdateUser_ValidUser_ReturnsUpdatedUserDTO()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var userUpdateDto = new UserUpdateDTO
            {
                Name = "Updated Name",
                Email = "test@gmail.com",
                Gender = "Male"
            };

            // Act
            var result = await _userService.UpdateUser(user.UserId, userUpdateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo(userUpdateDto.Name));
        }

        [Test]
        public void UpdateUser_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userUpdateDto = new UserUpdateDTO { Name = "Updated Name" };

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.UpdateUser(999, userUpdateDto));
        }

        [Test]
        public void UpdateUser_EmailAlreadyExists_ThrowsEntityAlreadyExistsException()
        {
            // Arrange
            var user1 = CreateUser("user1@example.com", "Password123!");
            var user2 = CreateUser("user2@example.com", "Password123!");

            _dbContext.Users.AddRange(user1, user2);
            _dbContext.SaveChanges();
            var userUpdateDto = new UserUpdateDTO
            {
                Name = "Updated Name",
                Email = user2.Email
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _userService.UpdateUser(user1.UserId, userUpdateDto));
        }

        [Test]
        public async Task UpdateUserPasword_ValidUser_ReturnsUpdatedUserDTO()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var userUpdateDto = new UserPasswordUpdateDTO
            {
                OldPassword = "Password123!",
                NewPassword = "NewPassword123!"
            };

            // Act
            var result = await _userService.UpdateUserPassword(user.UserId, userUpdateDto);


            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void UpdateUserPassword_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userUpdateDto = new UserPasswordUpdateDTO
            {
                OldPassword = "Password123!",
                NewPassword = "NewPassword123!"
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.UpdateUserPassword(999, userUpdateDto));
        }

        [Test]
        public void UpdateUserPassword_InvalidOldPassword_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            _dbContext.Users.Add(user);

            var userUpdateDto = new UserPasswordUpdateDTO
            {
                OldPassword = "WrongPassword!",
                NewPassword = "NewPassword123!"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.UpdateUserPassword(user.UserId, userUpdateDto));
        }


        [Test]
        public void UpdateUserPassword_WeakNewPassword_ThrowsWeakPasswordException()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            _dbContext.Users.Add(user);

            var userUpdateDto = new UserPasswordUpdateDTO
            {
                OldPassword = "Password123!",
                NewPassword = "weak"
            };

            // Act & Assert
            Assert.ThrowsAsync<WeakPasswordException>(() => _userService.UpdateUserPassword(user.UserId, userUpdateDto));
        }

        [Test]
        public async Task DeleteUser_ValidUserId_DeletesUser()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.DeleteUser(user.UserId);

            // Assert
            var deletedUser = await _dbContext.Users.FindAsync(user.UserId);
            Assert.IsNull(deletedUser);
        }

        [Test]
        public void DeleteUser_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.DeleteUser(999));
        }

        [Test]
        public async Task ActivateUser_ValidUserId_ActivatesUser()
        {
            // Arrange
            var user = CreateUser("testuser@example.com", "Password123!");
            user.UserStatus = UserStatusEnum.UserStatus.Inactive;
            await _dbContext.Users.AddAsync(user);

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.ActivateUser(user.UserId);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Active"));
        }

        [Test]
        public void ActivateUser_UserNotFound_ThrowsEntityNotFoundException()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.ActivateUser(999));
        }


        [Test]
        public async Task DeactivateUser_ValidUserId_DeactivatesUser()
        {
            // Arrange
            var user = CreateUser("user@example.com", "Password123!");
            user.UserStatus = UserStatusEnum.UserStatus.Active;

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.DeactivateUser(user.UserId);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Inactive"));
        }


        [Test]
        public void DeactivateUser_UserNotFound_ThrowsEntityNotFoundException()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.DeactivateUser(999));
        }



        private User CreateUser(string email, string password)
        {
            using var hmac = new HMACSHA512();
            return new User
            {
                Email = email,
                PasswordHashKey = hmac.Key,
                HashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                Name = "Test User",
                RegistrationDate = DateTime.Now
            };
        }
    }
}
