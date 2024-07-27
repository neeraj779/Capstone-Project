using Swar.API.Contexts;
using Swar.API.Models.DBModels;

namespace Swar.API.Repositories
{
    public class UserRepository : AbstractRepository<int, User>
    {
        public UserRepository(SwarContext context) : base(context)
        {
        }
    }
}
