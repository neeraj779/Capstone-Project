using Swar.API.Contexts;
using Swar.API.Models.DBModels;

namespace Swar.API.Repositories
{
    public class PlayHistoryRepository : AbstractRepository<int, PlayHistory>
    {
        public PlayHistoryRepository(SwarContext context) : base(context)
        {
        }
    }
}
