using Swar.API.Models.DTOs;

namespace Swar.API.Interfaces.Services
{
    public interface IPlaylistService
    {
        public Task<ReturnPlaylistDTO> AddPlaylist(int userId, AddPlaylistDTO addPlaylistDTO);
        public Task<ReturnPlaylistDTO> GetPlaylistById(int userId, int playlistId);
        public Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylistsByUserId(int userId);
        public Task<ReturnPlaylistDTO> UpdatePlaylist(int userId, int playlistId, UpdatePlaylistDTO updatePlaylistDTO);
        public Task<ReturnPlaylistDTO> UpdatePlaylistPrivacy(int userId, int playlistId, bool IsPrivate);
        public Task<ReturnPlaylistDTO> DeletePlaylist(int userId, int playlistId);
        public Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylists();
    }
}
