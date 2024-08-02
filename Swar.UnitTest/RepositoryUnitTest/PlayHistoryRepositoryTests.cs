using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Models.DBModels;
using Swar.API.Repositories;

namespace Swar.UnitTest.RepositoryUnitTest
{
    [TestFixture]
    public class PlayHistoryRepositoryTests
    {
        private PlayHistoryRepository _repository;
        private SwarContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "PlayHistoryTestDatabase")
                .Options;

            _context = new SwarContext(options);
            _repository = new PlayHistoryRepository(_context);
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
            var playHistory = new PlayHistory
            {
                HistoryId = 1,
                SongId = "song123",
                PlayedAt = DateTime.Now,
                UserId = 1
            };

            var addedPlayHistory = await _repository.Add(playHistory);

            Assert.That(addedPlayHistory, Is.Not.Null);
            Assert.That(addedPlayHistory.HistoryId, Is.EqualTo(playHistory.HistoryId));
            Assert.That(addedPlayHistory.SongId, Is.EqualTo(playHistory.SongId));
            Assert.That(addedPlayHistory.PlayedAt, Is.EqualTo(playHistory.PlayedAt));
            Assert.That(addedPlayHistory.UserId, Is.EqualTo(playHistory.UserId));
        }

        [Test]
        public async Task Add_ShouldThrowExceptionWhenEntityAlreadyExists()
        {
            var playHistory = new PlayHistory
            {
                HistoryId = 1,
                SongId = "song123",
                PlayedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playHistory);

            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.Add(playHistory));
        }

        [Test]
        public async Task GetById_ShouldReturnEntity_WhenExists()
        {
            var playHistory = new PlayHistory
            {
                HistoryId = 1,
                SongId = "song123",
                PlayedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playHistory);

            var result = await _repository.GetById(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HistoryId, Is.EqualTo(playHistory.HistoryId));
            Assert.That(result.SongId, Is.EqualTo(playHistory.SongId));
            Assert.That(result.PlayedAt, Is.EqualTo(playHistory.PlayedAt));
            Assert.That(result.UserId, Is.EqualTo(playHistory.UserId));
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
            var playHistory = new PlayHistory
            {
                HistoryId = 1,
                SongId = "song123",
                PlayedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playHistory);

            playHistory.SongId = "updatedSong";
            await _repository.Update(playHistory);

            var updatedPlayHistory = await _repository.GetById(1);

            Assert.That(updatedPlayHistory, Is.Not.Null);
            Assert.That(updatedPlayHistory.SongId, Is.EqualTo("updatedSong"));
        }

        [Test]
        public void Update_ShouldThrowException_WhenEntityNotFound()
        {
            var playHistory = new PlayHistory
            {
                HistoryId = 999,
                SongId = "song123",
                PlayedAt = DateTime.Now,
                UserId = 1
            };

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.Update(playHistory));
        }

        [Test]
        public async Task Delete_ShouldRemoveEntity_WhenExists()
        {
            var playHistory = new PlayHistory
            {
                HistoryId = 1,
                SongId = "song123",
                PlayedAt = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(playHistory);
            await _repository.Delete(1);

            var deletedPlayHistory = await _repository.GetById(1);

            Assert.That(deletedPlayHistory, Is.Null);
        }

        [Test]
        public void Delete_ShouldThrowException_WhenEntityNotFound()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.Delete(999));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllEntities()
        {
            var playHistory1 = new PlayHistory
            {
                HistoryId = 1,
                SongId = "song123",
                PlayedAt = DateTime.Now,
                UserId = 1
            };

            var playHistory2 = new PlayHistory
            {
                HistoryId = 2,
                SongId = "song456",
                PlayedAt = DateTime.Now,
                UserId = 2
            };

            await _repository.Add(playHistory1);
            await _repository.Add(playHistory2);

            var allPlayHistories = await _repository.GetAll();

            Assert.That(allPlayHistories.Count(), Is.EqualTo(2));
            Assert.That(allPlayHistories, Does.Contain(playHistory1));
            Assert.That(allPlayHistories, Does.Contain(playHistory2));
        }
    }
}
