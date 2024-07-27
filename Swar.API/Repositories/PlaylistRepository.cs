using Swar.API.Contexts;
using Swar.API.Models.DBModels;

namespace Swar.API.Repositories
{
    public class PlaylistRepository : AbstractRepository<int, Playlist>
    {
        public PlaylistRepository(SwarContext context) : base(context)
        {
        }
    }
}
