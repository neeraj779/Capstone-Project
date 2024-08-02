using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Models.DBModels;
using Swar.API.Models.ENUMs;
using Swar.API.Repositories;
using Swar.API.Services;
namespace Swar.UnitTest.ServiceUnitTest
{
    [TestFixture]
    public class PlayHistoryServiceTests
    {
        private DbContextOptions<SwarContext> _dbContextOptions;
        private SwarContext _dbContext;
        private IRepository<int, PlayHistory> _playHistoryRepository;
        private IRepository<int, User> _userRepository;
        private PlayHistoryService _playHistoryService;
        private Mock<ILogger<PlayHistoryService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryPlayHistoryDb")
                .Options;
            _dbContext = new SwarContext(_dbContextOptions);

            _playHistoryRepository = new PlayHistoryRepository(_dbContext);
            _userRepository = new UserRepository(_dbContext);
            _loggerMock = new Mock<ILogger<PlayHistoryService>>();

            _playHistoryService = new PlayHistoryService(
                _playHistoryRepository,
                _userRepository,
                _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task LogSongHistory_ValidUser_LogsHistory()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            var songId = "song123";

            // Act
            await _playHistoryService.LogSongHistory(user.UserId, songId);

            // Assert
            var history = await _playHistoryRepository.GetAll();
            Assert.IsTrue(history.Any(h => h.UserId == user.UserId && h.SongId == songId));
        }

        [Test]
        public void LogSongHistory_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playHistoryService.LogSongHistory(999, "song123"));
        }

        [Test]
        public async Task LogSongHistory_UserInactive_ThrowsInactiveAccountException()
        {
            // Arrange
            var user = CreateUser(UserStatusEnum.UserStatus.Inactive);
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InactiveAccountException>(() => _playHistoryService.LogSongHistory(user.UserId, "song123"));
        }

        [Test]
        public async Task GetSongHistory_ValidUser_ReturnsHistory()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);


            PlayHistory p1 = new PlayHistory { UserId = user.UserId, SongId = "song1", PlayedAt = DateTime.Now.AddMinutes(-10) };
            PlayHistory p2 = new PlayHistory { UserId = user.UserId, SongId = "song2", PlayedAt = DateTime.Now.AddMinutes(-5) };
            await _playHistoryRepository.Add(p1);
            await _playHistoryRepository.Add(p2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playHistoryService.GetSongHistory(user.UserId, unique: false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Songs.Count, Is.EqualTo(2));
            Assert.Contains("song1", result.Songs);
            Assert.Contains("song2", result.Songs);
        }

        [Test]
        public void GetSongHistory_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playHistoryService.GetSongHistory(999, unique: false));
        }

        [Test]
        public async Task GetSongHistory_UserInactive_ThrowsInactiveAccountException()
        {
            // Arrange
            var user = CreateUser(UserStatusEnum.UserStatus.Inactive);
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InactiveAccountException>(() => _playHistoryService.GetSongHistory(user.UserId, unique: false));
        }

        [Test]
        public async Task GetSongHistory_UniqueTrue_ReturnsUniqueHistory()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);


            PlayHistory p1 = new PlayHistory { UserId = user.UserId, SongId = "song1", PlayedAt = DateTime.Now.AddMinutes(-10) };
            PlayHistory p2 = new PlayHistory { UserId = user.UserId, SongId = "song1", PlayedAt = DateTime.Now.AddMinutes(-5) };
            await _playHistoryRepository.Add(p1);
            await _playHistoryRepository.Add(p2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playHistoryService.GetSongHistory(user.UserId, unique: true);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Songs.Count, Is.EqualTo(1));
            Assert.That(result.Songs, Does.Contain("song1"));
        }

        [Test]
        public async Task GetAllUserHistory_ReturnsAllUsersHistory()
        {
            // Arrange
            var user1 = CreateUser();
            var user2 = CreateUser();
            await _userRepository.Add(user1);
            await _userRepository.Add(user2);

            PlayHistory p1 = new PlayHistory { UserId = user1.UserId, SongId = "song1", PlayedAt = DateTime.Now.AddMinutes(-10) };
            PlayHistory p2 = new PlayHistory { UserId = user2.UserId, SongId = "song2", PlayedAt = DateTime.Now.AddMinutes(-5) };

            await _playHistoryRepository.Add(p1);
            await _playHistoryRepository.Add(p2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playHistoryService.GetAllUserHistory();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.Any(r => r.UserId == user1.UserId && r.Songs.Contains("song1")));
            Assert.IsTrue(result.Any(r => r.UserId == user2.UserId && r.Songs.Contains("song2")));
        }

        [Test]
        public void GetAllUserHistory_NoHistory_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playHistoryService.GetAllUserHistory());
        }

        // Helper Methods
        private User CreateUser(UserStatusEnum.UserStatus status = UserStatusEnum.UserStatus.Active)
        {
            return new User
            {
                UserId = new Random().Next(1, 1000),
                UserStatus = status,
                PasswordHashKey = new byte[64],
                HashedPassword = new byte[64],
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now,
                Gender = "Male",
                Email = "user@gmail.com"
            };
        }
    }
}
