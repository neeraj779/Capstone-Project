using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Models.DBModels;
using Swar.API.Repositories;

namespace Swar.UnitTest.RepositoryUnitTest
{
    [TestFixture]
    public class PlaylistRepositoryTests
    {
        private PlaylistRepository _repository;
        private SwarContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "PlaylistTestDatabase")
                .Options;

            _context = new SwarContext(options);
            _repository = new PlaylistRepository(_context);
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
            var playlist = new Playlist
            {
                PlaylistId = 1,
                PlaylistName = "My Playlist",
                Description = "A great playlist",
                IsPrivate = false,
                CreatedAt = DateTime.Now,
            };

            var addedPlaylist = await _repository.Add(playlist);

            Assert.That(addedPlaylist, Is.Not.Null);
            Assert.That(addedPlaylist.PlaylistId, Is.EqualTo(playlist.PlaylistId));
            Assert.That(addedPlaylist.PlaylistName, Is.EqualTo(playlist.PlaylistName));
        }

        [Test]
        public async Task Add_ShouldThrowExceptionWhenEntityAlreadyExists()
        {
            var playlist = new Playlist
            {
                PlaylistId = 1,
                PlaylistName = "My Playlist",
                Description = "A great playlist",
                IsPrivate = false,
                CreatedAt = DateTime.Now,
            };

            await _repository.Add(playlist);

            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.Add(playlist));
        }

        [Test]
        public async Task GetById_ShouldReturnEntity_WhenExists()
        {
            var playlist = new Playlist
            {
                PlaylistId = 1,
                PlaylistName = "My Playlist",
                Description = "A great playlist",
                IsPrivate = false,
                CreatedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playlist);

            var result = await _repository.GetById(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PlaylistId, Is.EqualTo(playlist.PlaylistId));
            Assert.That(result.PlaylistName, Is.EqualTo(playlist.PlaylistName));
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenEntityNotFound()
        {
            var result = await _repository.GetById(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Update_ShouldModifyEntity()
        {
            var playlist = new Playlist
            {
                PlaylistId = 1,
                PlaylistName = "My Playlist",
                Description = "A great playlist",
                IsPrivate = false,
                CreatedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playlist);

            playlist.PlaylistName = "Updated Playlist";
            await _repository.Update(playlist);

            var updatedPlaylist = await _repository.GetById(1);

            Assert.That(updatedPlaylist, Is.Not.Null);
            Assert.That(updatedPlaylist.PlaylistName, Is.EqualTo("Updated Playlist"));
        }

        [Test]
        public void Update_ShouldThrowException_WhenEntityNotFound()
        {
            var playlist = new Playlist
            {
                PlaylistId = 999,
                PlaylistName = "Nonexistent Playlist",
                Description = "Doesn't exist",
                IsPrivate = false,
                CreatedAt = DateTime.Now,
                UserId = 1
            };

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.Update(playlist));
        }

        [Test]
        public async Task Delete_ShouldRemoveEntity_WhenExists()
        {
            var playlist = new Playlist
            {
                PlaylistId = 1,
                PlaylistName = "My Playlist",
                Description = "A great playlist",
                IsPrivate = false,
                CreatedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playlist);
            await _repository.Delete(1);

            var deletedPlaylist = await _repository.GetById(1);

            Assert.That(deletedPlaylist, Is.Null);
        }

        [Test]
        public void Delete_ShouldThrowException_WhenEntityNotFound()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.Delete(999));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllEntities()
        {
            var playlist1 = new Playlist
            {
                PlaylistId = 1,
                PlaylistName = "My Playlist",
                Description = "A great playlist",
                IsPrivate = false,
                CreatedAt = DateTime.Now,
                UserId = 1
            };

            var playlist2 = new Playlist
            {
                PlaylistId = 2,
                PlaylistName = "Another Playlist",
                Description = "Another great playlist",
                IsPrivate = true,
                CreatedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playlist1);
            await _repository.Add(playlist2);

            var allPlaylists = await _repository.GetAll();

            Assert.That(allPlaylists.Count(), Is.EqualTo(2));
            Assert.That(allPlaylists, Does.Contain(playlist1));
            Assert.That(allPlaylists, Does.Contain(playlist2));
        }
    }
}
