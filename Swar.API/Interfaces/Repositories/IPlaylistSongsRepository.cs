using Swar.API.Models.DBModels;

namespace Swar.API.Interfaces.Repositories
{
    public interface IPlaylistSongsRepository : IRepository<int, PlaylistSong>
    {
        Task<PlaylistSong> GetByCompositeKey(int playlistId, string songId);
        Task<PlaylistSong> DeletePlaylistSong(int playlistId, string songId);
    }
}
