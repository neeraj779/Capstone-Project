using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Interfaces.Repositories;
using Swar.API.Models.DBModels;

namespace Swar.API.Repositories
{
    public class UserRepository : AbstractRepository<int, User>, IUserRepository
    {
        public UserRepository(SwarContext context) : base(context)
        {
        }

        public async Task<User> GetByEmail(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
