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
    public class LikedSongsServiceTests
    {
        private DbContextOptions<SwarContext> _dbContextOptions;
        private SwarContext _dbContext;
        private IRepository<int, User> _userRepository;
        private IRepository<int, LikedSong> _likedSongRepository;
        private LikedSongsService _likedSongsService;
        private Mock<ILogger<LikedSongsService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryLikedSongsDb")
                .Options;
            _dbContext = new SwarContext(_dbContextOptions);

            _userRepository = new UserRepository(_dbContext);
            _likedSongRepository = new LikedSongsRepository(_dbContext);
            _loggerMock = new Mock<ILogger<LikedSongsService>>();

            _likedSongsService = new LikedSongsService(
                _userRepository,
                _likedSongRepository,
                _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task AddSongToLikedSongs_ValidUser_AddsSong()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            var songId = "song123";

            // Act
            var result = await _likedSongsService.AddSongToLikedSongs(user.UserId, songId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.UserId, Is.EqualTo(user.UserId));
            Assert.That(result.SongId, Is.EqualTo(songId));
            Assert.That(result.LikedDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public void AddSongToLikedSongs_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _likedSongsService.AddSongToLikedSongs(999, "song123"));
        }

        [Test]
        public async Task AddSongToLikedSongs_UserInactive_ThrowsInactiveAccountException()
        {
            // Arrange
            var user = CreateUser(UserStatusEnum.UserStatus.Inactive);
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InactiveAccountException>(() => _likedSongsService.AddSongToLikedSongs(user.UserId, "song123"));
        }

        [Test]
        public async Task AddSongToLikedSongs_SongAlreadyLiked_ThrowsEntityAlreadyExistsException()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var likedSong = new LikedSong
            {
                UserId = user.UserId,
                SongId = "song123",
                LikedDate = DateTime.UtcNow
            };
            await _likedSongRepository.Add(likedSong);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _likedSongsService.AddSongToLikedSongs(user.UserId, "song123"));
        }

        [Test]
        public async Task RemoveSongFromLikedSongs_ValidUser_RemovesSong()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var likedSong = new LikedSong
            {
                UserId = user.UserId,
                SongId = "song123",
                LikedDate = DateTime.UtcNow
            };
            await _likedSongRepository.Add(likedSong);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _likedSongsService.RemoveSongFromLikedSongs(user.UserId, "song123");

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.UserId, Is.EqualTo(user.UserId));
            Assert.That(result.SongId, Is.EqualTo("song123"));
            Assert.That(result.LikedDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public async Task RemoveSongFromLikedSongs_SongNotLiked_ThrowsEntityNotFoundException()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _likedSongsService.RemoveSongFromLikedSongs(user.UserId, "song123"));
        }

        [Test]
        public async Task GetAllLikedSongs_ValidUser_ReturnsLikedSongs()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);


            LikedSong l1 = new LikedSong { UserId = user.UserId, SongId = "song1", LikedDate = DateTime.Now.AddMinutes(-10) };
            LikedSong l2 = new LikedSong { UserId = user.UserId, SongId = "song2", LikedDate = DateTime.Now.AddMinutes(-5) };
            await _likedSongRepository.Add(l1);
            await _likedSongRepository.Add(l2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _likedSongsService.GetAllLikedSongs(user.UserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Songs.Count, Is.EqualTo(2));
            Assert.Contains("song1", result.Songs);
            Assert.Contains("song2", result.Songs);
        }

        [Test]
        public void GetAllLikedSongs_UserNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _likedSongsService.GetAllLikedSongs(999));
        }

        [Test]
        public async Task IsSongLikedByUser_SongLiked_ReturnsTrue()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var likedSong = new LikedSong
            {
                UserId = user.UserId,
                SongId = "song123",
                LikedDate = DateTime.UtcNow
            };
            await _likedSongRepository.Add(likedSong);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _likedSongsService.IsSongLikedByUser(user.UserId, "song123");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsSongLikedByUser_SongNotLiked_ReturnsFalse()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _likedSongsService.IsSongLikedByUser(user.UserId, "song123");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsSongLikedByUser_UserNotFound_ReturnsFalse()
        {
            // Act
            var result = await _likedSongsService.IsSongLikedByUser(999, "song123");

            // Assert
            Assert.IsFalse(result);
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
                Email = "user@gmail.com"
            };
        }
    }
}
