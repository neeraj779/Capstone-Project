using Swar.API.Models.DBModels;

namespace Swar.API.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<int, User>
    {
        public Task<User> GetByEmail(string email);
    }
}
