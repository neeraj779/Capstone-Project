using Swar.API.Contexts;
using Swar.API.Models.DBModels;

namespace Swar.API.Repositories
{
    public class LikedSongsRepository : AbstractRepository<int, LikedSong>
    {
        public LikedSongsRepository(SwarContext context) : base(context)
        {
        }
    }
}
