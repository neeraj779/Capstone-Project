using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Interfaces.Repositories;
using Swar.API.Models.DBModels;

namespace Swar.API.Repositories
{
    public class PlaylistRepository : AbstractRepository<int, Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(SwarContext context) : base(context)
        {
        }

        public async Task<Playlist?> GetByPublicId(string publicId)
        {
            return await _context.Playlists.FirstOrDefaultAsync(p => p.PublicId == publicId);
        }
    }
}
