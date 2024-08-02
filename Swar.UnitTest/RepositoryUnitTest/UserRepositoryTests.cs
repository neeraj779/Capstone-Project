using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Models.DBModels;
using Swar.API.Models.ENUMs;
using Swar.API.Repositories;

namespace Swar.UnitTest.RepositoryUnitTest
{
    public class UserRepositoryTests
    {
        private UserRepository _repository;
        private SwarContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SwarContext>()
                .UseInMemoryDatabase(databaseName: "UserTestDatabase")
                .Options;

            _context = new SwarContext(options);
            _repository = new UserRepository(_context);
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
            var user = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Gender = "Male",
                HashedPassword = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 1, 2, 3 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            var addedUser = await _repository.Add(user);

            Assert.NotNull(addedUser);
            Assert.That(addedUser.UserId, Is.EqualTo(user.UserId));
        }

        [Test]
        public async Task Add_ShouldThrowExceptionWhenUserAlreadyExists()
        {
            var user = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Gender = "Male",
                HashedPassword = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 1, 2, 3 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            await _repository.Add(user);

            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.Add(user));
        }

        [Test]
        public async Task GetById_ShouldReturnEntity()
        {
            var user = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Gender = "Male",
                HashedPassword = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 1, 2, 3 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            await _repository.Add(user);
            var retrievedUser = await _repository.GetById(1);

            Assert.NotNull(retrievedUser);
            Assert.That(retrievedUser.UserId, Is.EqualTo(user.UserId));
        }

        [Test]
        public async Task GetById_ShouldReturnNullWhenEntityNotFound()
        {
            var retrievedUser = await _repository.GetById(999);

            Assert.Null(retrievedUser);
        }

        [Test]
        public async Task Update_ShouldModifyEntity()
        {
            var user = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Gender = "Male",
                HashedPassword = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 1, 2, 3 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            await _repository.Add(user);

            user.Name = "Jane Doe";
            await _repository.Update(user);
            var updatedUser = await _repository.GetById(1);

            Assert.NotNull(updatedUser);
            Assert.That(updatedUser.Name, Is.EqualTo("Jane Doe"));
        }

        [Test]
        public void Update_ShouldThrowExceptionWhenEntityNotFound()
        {
            var user = new User
            {
                UserId = 999,
                Name = "Nonexistent User",
                Email = "nonexistent@example.com",
                Gender = "Unknown",
                HashedPassword = new byte[] { 0 },
                PasswordHashKey = new byte[] { 0 },
                UserStatus = UserStatusEnum.UserStatus.Inactive,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.Update(user));
        }

        [Test]
        public async Task Delete_ShouldRemoveEntity()
        {
            var user = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Gender = "Male",
                HashedPassword = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 1, 2, 3 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            await _repository.Add(user);
            await _repository.Delete(1);
            var deletedUser = await _repository.GetById(1);

            Assert.Null(deletedUser);
        }

        [Test]
        public void Delete_ShouldThrowExceptionWhenEntityNotFound()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.Delete(999));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllEntities()
        {
            var user1 = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Gender = "Male",
                HashedPassword = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 1, 2, 3 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            var user2 = new User
            {
                UserId = 2,
                Name = "Jane Doe",
                Email = "jane.doe@example.com",
                Gender = "Female",
                HashedPassword = new byte[] { 4, 5, 6 },
                PasswordHashKey = new byte[] { 4, 5, 6 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            await _repository.Add(user1);
            await _repository.Add(user2);

            var allUsers = await _repository.GetAll();

            Assert.That(allUsers.Count(), Is.EqualTo(2));
            Assert.Contains(user1, allUsers.ToList());
            Assert.Contains(user2, allUsers.ToList());
        }

        [Test]
        public async Task GetByEmail_ShouldReturnUser()
        {
            var user = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Gender = "Male",
                HashedPassword = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 1, 2, 3 },
                UserStatus = UserStatusEnum.UserStatus.Active,
                Role = UserRoleEnum.UserRole.User,
                RegistrationDate = DateTime.Now
            };

            await _repository.Add(user);
            var retrievedUser = await _repository.GetByEmail("john.doe@example.com");

            Assert.NotNull(retrievedUser);
            Assert.That(retrievedUser.UserId, Is.EqualTo(user.UserId));
        }

        [Test]
        public async Task GetByEmail_ShouldReturnNullWhenEmailNotFound()
        {
            var retrievedUser = await _repository.GetByEmail("nonexistent@example.com");

            Assert.Null(retrievedUser);
        }
    }
}
