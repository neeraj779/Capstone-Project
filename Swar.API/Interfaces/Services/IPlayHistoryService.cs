using Swar.API.Models.DTOs;

namespace Swar.API.Interfaces.Services
{
    public interface IPlayHistoryService
    {
        public Task LogSongHistory(int userId, string songId);
        public Task<SongsListDTO> GetSongHistory(int userId, bool unique);
        public Task<IEnumerable<SongsListDTO>> GetAllUserHistory();
    }
}
