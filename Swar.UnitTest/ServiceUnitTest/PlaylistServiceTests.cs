using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Models.DBModels;
using Swar.API.Models.DTOs;
using Swar.API.Models.ENUMs;
using Swar.API.Repositories;
using Swar.API.Services;

namespace Swar.UnitTest.ServiceUnitTest
{
    [TestFixture]
    public class PlaylistServiceTests
    {
        private DbContextOptions<SwarContext> _dbContextOptions;
        private SwarContext _dbContext;
        private IRepository<int, Playlist> _playlistRepository;
        private IRepository<int, User> _userRepository;
        private PlaylistService _playlistService;
        private Mock<ILogger<PlaylistService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryPlaylistDb")
                .Options;
            _dbContext = new SwarContext(_dbContextOptions);

            _playlistRepository = new PlaylistRepository(_dbContext);
            _userRepository = new UserRepository(_dbContext);
            _loggerMock = new Mock<ILogger<PlaylistService>>();

            _playlistService = new PlaylistService(
                _playlistRepository,
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
        public async Task AddPlaylist_ValidInput_AddsPlaylist()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            var addPlaylistDto = new AddPlaylistDTO
            {
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false
            };

            // Act
            var result = await _playlistService.AddPlaylist(user.UserId, addPlaylistDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PlaylistName, Is.EqualTo(addPlaylistDto.PlaylistName));
            Assert.That(result.Description, Is.EqualTo(addPlaylistDto.Description));
            Assert.IsFalse(result.IsPrivate);
        }

        [Test]
        public async Task AddPlaylist_UserInactive_ThrowsInactiveAccountException()
        {
            // Arrange
            var user = CreateUser(UserStatusEnum.UserStatus.Inactive);
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            var addPlaylistDto = new AddPlaylistDTO
            {
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false
            };

            // Act & Assert
            Assert.ThrowsAsync<InactiveAccountException>(() => _playlistService.AddPlaylist(user.UserId, addPlaylistDto));
        }

        [Test]
        public async Task DeletePlaylist_ValidInput_DeletesPlaylist()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistService.DeletePlaylist(user.UserId, playlist.PlaylistId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PlaylistId, Is.EqualTo(playlist.PlaylistId));
            Assert.IsNull(await _playlistRepository.GetById(playlist.PlaylistId));
        }

        [Test]
        public void DeletePlaylist_PlaylistNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistService.DeletePlaylist(1, 999));
        }

        [Test]
        public async Task DeletePlaylist_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = CreateUser();
            var anotherUser = CreateUser();
            await _userRepository.Add(user);
            await _userRepository.Add(anotherUser);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _playlistService.DeletePlaylist(anotherUser.UserId, playlist.PlaylistId));
        }

        [Test]
        public async Task GetAllPlaylists_ReturnsPlaylists()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var playlist1 = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Playlist 1",
                Description = "Description 1",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            var playlist2 = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Playlist 2",
                Description = "Description 2",
                IsPrivate = true,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist1);
            await _playlistRepository.Add(playlist2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistService.GetAllPlaylists();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllPlaylists_NoPlaylists_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistService.GetAllPlaylists());
        }

        [Test]
        public async Task GetAllPlaylistsByUserId_ValidUser_ReturnsPlaylists()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "User Playlist",
                Description = "Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistService.GetAllPlaylistsByUserId(user.UserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().PlaylistId, Is.EqualTo(playlist.PlaylistId));
        }

        [Test]
        public async Task GetAllPlaylistsByUserId_NoPlaylistsForUser_ThrowsEntityNotFoundException()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistService.GetAllPlaylistsByUserId(user.UserId));
        }

        [Test]
        public async Task GetPlaylistById_ValidUser_ReturnsPlaylist()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistService.GetPlaylistById(user.UserId, playlist.PlaylistId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PlaylistId, Is.EqualTo(playlist.PlaylistId));
        }

        [Test]
        public void GetPlaylistById_PlaylistNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistService.GetPlaylistById(1, 999));
        }

        [Test]
        public async Task GetPlaylistById_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = CreateUser();
            var anotherUser = CreateUser();
            await _userRepository.Add(user);
            await _userRepository.Add(anotherUser);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _playlistService.GetPlaylistById(anotherUser.UserId, playlist.PlaylistId));
        }

        [Test]
        public async Task UpdatePlaylist_ValidInput_UpdatesPlaylist()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Old Playlist",
                Description = "Old Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            var updatePlaylistDto = new UpdatePlaylistDTO
            {
                PlaylistName = "Updated Playlist",
                Description = "Updated Description"
            };

            // Act
            var result = await _playlistService.UpdatePlaylist(user.UserId, playlist.PlaylistId, updatePlaylistDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PlaylistName, Is.EqualTo(updatePlaylistDto.PlaylistName));
            Assert.That(result.Description, Is.EqualTo(updatePlaylistDto.Description));
        }

        [Test]
        public void UpdatePlaylist_PlaylistNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistService.UpdatePlaylist(1, 999, new UpdatePlaylistDTO()));
        }

        [Test]
        public async Task UpdatePlaylist_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = CreateUser();
            var anotherUser = CreateUser();
            await _userRepository.Add(user);
            await _userRepository.Add(anotherUser);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            var updatePlaylistDto = new UpdatePlaylistDTO
            {
                PlaylistName = "Updated Playlist",
                Description = "Updated Description"
            };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _playlistService.UpdatePlaylist(anotherUser.UserId, playlist.PlaylistId, updatePlaylistDto));
        }

        [Test]
        public async Task UpdatePlaylistPrivacy_ValidInput_UpdatesPrivacy()
        {
            // Arrange
            var user = CreateUser();
            await _userRepository.Add(user);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistService.UpdatePlaylistPrivacy(user.UserId, playlist.PlaylistId, true);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsPrivate);
        }

        [Test]
        public void UpdatePlaylistPrivacy_PlaylistNotFound_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistService.UpdatePlaylistPrivacy(1, 999, true));
        }

        [Test]
        public async Task UpdatePlaylistPrivacy_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = CreateUser();
            var anotherUser = CreateUser();
            await _userRepository.Add(user);
            await _userRepository.Add(anotherUser);

            var playlist = new Playlist
            {
                UserId = user.UserId,
                PlaylistName = "Test Playlist",
                Description = "Test Description",
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _playlistService.UpdatePlaylistPrivacy(anotherUser.UserId, playlist.PlaylistId, true));
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
