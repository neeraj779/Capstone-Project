using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Models.DBModels;

namespace Swar.API.Repositories
{
    public class PlaylistSongsRepository : AbstractRepository<int, PlaylistSong>, IPlaylistSongsRepository
    {
        public PlaylistSongsRepository(SwarContext context) : base(context)
        {
        }

        public async Task<PlaylistSong> GetByCompositeKey(int playlistId, string songId)
        {
            var result = await _dbSet
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            return result;
        }

        public async Task<PlaylistSong> DeletePlaylistSong(int playlistId, string songId)
        {
            var result = await GetByCompositeKey(playlistId, songId);
            if (result == null)
            {
                throw new EntityNotFoundException();
            }
            _dbSet.Remove(result);
            await _context.SaveChangesAsync();
            return result;
        }
    }
}
