using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Models.DBModels;
using Swar.API.Repositories;

namespace Swar.UnitTest.RepositoryUnitTest
{
    [TestFixture]
    public class PlaylistSongsRepositoryTests
    {
        private PlaylistSongsRepository _repository;
        private SwarContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "PlaylistSongsTestDatabase")
                .Options;

            _context = new SwarContext(options);
            _repository = new PlaylistSongsRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_ShouldAddEntity()
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song1"
            };

            var addedPlaylistSong = await _repository.Add(playlistSong);

            Assert.NotNull(addedPlaylistSong);
            Assert.That(addedPlaylistSong.PlaylistId, Is.EqualTo(playlistSong.PlaylistId));
            Assert.That(addedPlaylistSong.SongId, Is.EqualTo(playlistSong.SongId));
        }

        [Test]
        public async Task Add_ShouldThrowExceptionWhenEntityAlreadyExists()
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song1"
            };

            await _repository.Add(playlistSong);

            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.Add(playlistSong));
        }

        [Test]
        public async Task GetById_ShouldReturnEntity_WhenExists()
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song1"
            };

            await _repository.Add(playlistSong);

            var result = await _repository.GetByCompositeKey(1, "song1");

            Assert.NotNull(result);
            Assert.That(result.PlaylistId, Is.EqualTo(playlistSong.PlaylistId));
            Assert.That(result.SongId, Is.EqualTo(playlistSong.SongId));
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenEntityNotFound()
        {
            var result = await _repository.GetByCompositeKey(999, "nonexistent");

            Assert.Null(result);
        }


        [Test]
        public void Update_ShouldThrowException_WhenEntityNotFound()
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = 999,
                SongId = "nonexistent"
            };

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.Update(playlistSong));
        }

        [Test]
        public async Task Delete_ShouldRemoveEntity_WhenExists()
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song1"
            };

            await _repository.Add(playlistSong);
            await _repository.DeletePlaylistSong(playlistSong.PlaylistId, playlistSong.SongId);

            var deletedPlaylistSong = await _repository.GetByCompositeKey(playlistSong.PlaylistId, playlistSong.SongId);

            Assert.Null(deletedPlaylistSong);
        }

        [Test]
        public void Delete_ShouldThrowException_WhenEntityNotFound()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.DeletePlaylistSong(999, "test"));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllEntities()
        {
            var playlistSong1 = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song1"
            };

            var playlistSong2 = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song2"
            };

            await _repository.Add(playlistSong1);
            await _repository.Add(playlistSong2);

            var allPlaylistSongs = await _repository.GetAll();

            Assert.That(allPlaylistSongs.Count(), Is.EqualTo(2));
            Assert.Contains(playlistSong1, allPlaylistSongs.ToList());
            Assert.Contains(playlistSong2, allPlaylistSongs.ToList());
        }

        [Test]
        public async Task GetByCompositeKey_ShouldReturnEntity_WhenExists()
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song1"
            };

            await _repository.Add(playlistSong);

            var result = await _repository.GetByCompositeKey(1, "song1");

            Assert.NotNull(result);
            Assert.That(result.PlaylistId, Is.EqualTo(playlistSong.PlaylistId));
            Assert.That(result.SongId, Is.EqualTo(playlistSong.SongId));
        }

        [Test]
        public async Task GetByCompositeKey_ShouldReturnNull_WhenEntityNotFound()
        {
            var result = await _repository.GetByCompositeKey(999, "nonexistent");

            Assert.Null(result);
        }

        [Test]
        public async Task DeletePlaylistSong_ShouldRemoveEntity_WhenExists()
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = 1,
                SongId = "song1"
            };

            await _repository.Add(playlistSong);

            var deletedPlaylistSong = await _repository.DeletePlaylistSong(1, "song1");

            Assert.NotNull(deletedPlaylistSong);
            Assert.That(deletedPlaylistSong.PlaylistId, Is.EqualTo(playlistSong.PlaylistId));
            Assert.That(deletedPlaylistSong.SongId, Is.EqualTo(playlistSong.SongId));

            var result = await _repository.GetByCompositeKey(1, "song1");
            Assert.Null(result);
        }

        [Test]
        public void DeletePlaylistSong_ShouldThrowException_WhenEntityNotFound()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.DeletePlaylistSong(999, "nonexistent"));
        }
    }
}
