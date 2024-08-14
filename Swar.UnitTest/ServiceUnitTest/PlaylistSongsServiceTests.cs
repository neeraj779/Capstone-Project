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
    public class PlaylistSongsServiceTests
    {
        private DbContextOptions<SwarContext> _dbContextOptions;
        private SwarContext _dbContext;
        private IPlaylistSongsRepository _playlistSongRepository;
        private IPlaylistRepository _playlistRepository;
        private IRepository<int, User> _userRepository;
        private PlaylistSongsService _playlistSongsService;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryPlaylistSongsDb")
                .Options;
            _dbContext = new SwarContext(_dbContextOptions);

            _playlistSongRepository = new PlaylistSongsRepository(_dbContext);
            _playlistRepository = new PlaylistRepository(_dbContext);
            _userRepository = new UserRepository(_dbContext);
            var logger = new Mock<ILogger<PlaylistSongsService>>().Object;

            _playlistSongsService = new PlaylistSongsService(
                _playlistSongRepository,
                _playlistRepository,
                _userRepository,
                logger);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task AddSongToPlaylist_ValidInput_AddsSongToPlaylist()
        {
            // Arrange
            var user = CreateUser();
            user = await _userRepository.Add(user);

            var playlist = CreatePlaylist(user.UserId);
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            var addSongDto = new AddSongToPlaylistDTO
            {
                PlaylistId = playlist.PlaylistId,
                SongId = "song1"
            };

            // Act
            var result = await _playlistSongsService.AddSongToPlaylist(user.UserId, addSongDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PlaylistId, Is.EqualTo(addSongDto.PlaylistId));
            Assert.That(result.SongId, Is.EqualTo(addSongDto.SongId));
        }

        [Test]
        public async Task AddSongToPlaylist_SongAlreadyExists_ThrowsEntityAlreadyExistsException()
        {
            // Arrange
            var user = CreateUser();
            user = await _userRepository.Add(user);
            var playlist = CreatePlaylist(user.UserId);
            await _playlistRepository.Add(playlist);
            await _playlistSongRepository.Add(new PlaylistSong
            {
                PlaylistId = playlist.PlaylistId,
                SongId = "song1"
            });
            await _dbContext.SaveChangesAsync();

            var addSongDto = new AddSongToPlaylistDTO
            {
                PlaylistId = playlist.PlaylistId,
                SongId = "song1"
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _playlistSongsService.AddSongToPlaylist(user.UserId, addSongDto));
        }

        [Test]
        public async Task GetAllSongsInPlaylist_ReturnsAllSongs()
        {
            // Arrange
            var user = CreateUser();
            var playlist = CreatePlaylist(user.UserId);
            await _userRepository.Add(user);
            await _playlistRepository.Add(playlist);
            await _playlistSongRepository.Add(new PlaylistSong
            {
                PlaylistId = playlist.PlaylistId,
                SongId = "song1"
            });
            await _playlistSongRepository.Add(new PlaylistSong
            {
                PlaylistId = playlist.PlaylistId,
                SongId = "song2"
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistSongsService.GetAllSongsInPlaylist();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllSongsInPlaylist_NoSongs_ThrowsEntityNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistSongsService.GetAllSongsInPlaylist());
        }

        [Test]
        public async Task GetAllSongsInUserPlaylist_ValidUser_ReturnsPlaylistSongsDTO()
        {
            // Arrange
            var user = CreateUser();
            user = await _userRepository.Add(user);
            var playlist = CreatePlaylist(user.UserId);
            await _playlistRepository.Add(playlist);
            await _playlistSongRepository.Add(new PlaylistSong
            {
                PlaylistId = playlist.PlaylistId,
                SongId = "song1"
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistSongsService.GetAllSongsInUserPlaylist(user.UserId, playlist.PublicId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PlaylistInfo.PlaylistId, Is.EqualTo(playlist.PlaylistId));
            Assert.Contains("song1", result.Songs);
        }

        [Test]
        public async Task GetAllSongsInUserPlaylist_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = CreateUser();
            user = await _userRepository.Add(user);
            var playlist = CreatePlaylist(5);
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _playlistSongsService.GetAllSongsInUserPlaylist(1, playlist.PublicId));
        }

        [Test]
        public async Task RemoveSongFromPlaylist_ValidInput_RemovesSong()
        {
            // Arrange
            var user = CreateUser();
            var playlist = CreatePlaylist(user.UserId);
            await _userRepository.Add(user);
            await _playlistRepository.Add(playlist);
            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlist.PlaylistId,
                SongId = "song1"
            };
            await _playlistSongRepository.Add(playlistSong);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _playlistSongsService.RemoveSongFromPlaylist(user.UserId, playlist.PlaylistId, "song1");

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PlaylistId, Is.EqualTo(playlistSong.PlaylistId));
            Assert.That(result.SongId, Is.EqualTo(playlistSong.SongId));
            var removedSong = await _playlistSongRepository.GetByCompositeKey(playlist.PlaylistId, "song1");
            Assert.IsNull(removedSong);
        }

        [Test]
        public async Task RemoveSongFromPlaylist_SongNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var user = CreateUser();
            var playlist = CreatePlaylist(user.UserId);
            await _userRepository.Add(user);
            await _playlistRepository.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _playlistSongsService.RemoveSongFromPlaylist(user.UserId, playlist.PlaylistId, "nonexistentSong"));
        }

        private User CreateUser()
        {
            return new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "user@example.com",
                PasswordHashKey = new byte[64],
                HashedPassword = new byte[64],
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };
        }

        private Playlist CreatePlaylist(int userId)
        {
            return new Playlist
            {
                UserId = userId,
                IsPrivate = true,
                PlaylistName = "Test Playlist"
            };
        }
    }
}
