using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Models.DBModels;
using Swar.API.Repositories;

namespace Swar.UnitTest.RepositoryUnitTest
{
    [TestFixture]
    public class LikedSongsRepositoryTests
    {
        private LikedSongsRepository _repository;
        private SwarContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "LikedSongsTestDatabase")
                .Options;

            _context = new SwarContext(options);
            _repository = new LikedSongsRepository(_context);
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
            var likedSong = new LikedSong
            {
                LikeId = 1,
                SongId = "song123",
                LikedDate = DateTime.Now,
                UserId = 1
            };

            var addedLikedSong = await _repository.Add(likedSong);

            Assert.That(addedLikedSong, Is.Not.Null);
            Assert.That(addedLikedSong.LikeId, Is.EqualTo(likedSong.LikeId));
            Assert.That(addedLikedSong.SongId, Is.EqualTo(likedSong.SongId));
            Assert.That(addedLikedSong.LikedDate, Is.EqualTo(likedSong.LikedDate));
            Assert.That(addedLikedSong.UserId, Is.EqualTo(likedSong.UserId));
        }

        [Test]
        public async Task Add_ShouldThrowExceptionWhenEntityAlreadyExists()
        {
            var likedSong = new LikedSong
            {
                LikeId = 1,
                SongId = "song123",
                LikedDate = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(likedSong);

            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.Add(likedSong));
        }

        [Test]
        public async Task GetById_ShouldReturnEntity_WhenExists()
        {
            var likedSong = new LikedSong
            {
                LikeId = 1,
                SongId = "song123",
                LikedDate = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(likedSong);

            var result = await _repository.GetById(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.LikeId, Is.EqualTo(likedSong.LikeId));
            Assert.That(result.SongId, Is.EqualTo(likedSong.SongId));
            Assert.That(result.LikedDate, Is.EqualTo(likedSong.LikedDate));
            Assert.That(result.UserId, Is.EqualTo(likedSong.UserId));
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
            var likedSong = new LikedSong
            {
                LikeId = 1,
                SongId = "song123",
                LikedDate = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(likedSong);

            likedSong.SongId = "updatedSong";
            await _repository.Update(likedSong);

            var updatedLikedSong = await _repository.GetById(1);

            Assert.That(updatedLikedSong, Is.Not.Null);
            Assert.That(updatedLikedSong.SongId, Is.EqualTo("updatedSong"));
        }

        [Test]
        public void Update_ShouldThrowException_WhenEntityNotFound()
        {
            var likedSong = new LikedSong
            {
                LikeId = 999,
                SongId = "song123",
                LikedDate = DateTime.Now,
                UserId = 1
            };

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.Update(likedSong));
        }

        [Test]
        public async Task Delete_ShouldRemoveEntity_WhenExists()
        {
            var likedSong = new LikedSong
            {
                LikeId = 1,
                SongId = "song123",
                LikedDate = DateTime.Now,
                UserId = 1
            };

            await _repository.Add(likedSong);
            await _repository.Delete(1);

            var deletedLikedSong = await _repository.GetById(1);

            Assert.That(deletedLikedSong, Is.Null);
        }

        [Test]
        public void Delete_ShouldThrowException_WhenEntityNotFound()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.Delete(999));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllEntities()
        {
            var likedSong1 = new LikedSong
            {
                LikeId = 1,
                SongId = "song123",
                LikedDate = DateTime.Now,
                UserId = 1
            };

            var likedSong2 = new LikedSong
            {
                LikeId = 2,
                SongId = "song456",
                LikedDate = DateTime.Now,
                UserId = 2
            };

            await _repository.Add(likedSong1);
            await _repository.Add(likedSong2);

            var allLikedSongs = await _repository.GetAll();

            Assert.That(allLikedSongs.Count(), Is.EqualTo(2));
            Assert.That(allLikedSongs, Does.Contain(likedSong1));
            Assert.That(allLikedSongs, Does.Contain(likedSong2));
        }
    }
}
