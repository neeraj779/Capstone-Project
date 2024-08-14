using Swar.API.Models.DBModels;

namespace Swar.API.Interfaces.Repositories
{
    public interface IPlaylistRepository : IRepository<int, Playlist>
    {
        public Task<Playlist?> GetByPublicId(string publicId);
    }
}
