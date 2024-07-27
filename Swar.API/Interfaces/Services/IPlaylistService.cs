using Swar.API.Models.DTOs;

namespace Swar.API.Interfaces.Services
{
    public interface IPlaylistService
    {
        public Task<ReturnPlaylistDTO> AddPlaylist(int userId, AddPlaylistDTO addPlaylistDTO);
        public Task<ReturnPlaylistDTO> GetPlaylistById(int userId, int playlistId);
        public Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylistsByUserId(int userId);
        public Task<ReturnPlaylistDTO> UpdatePlaylistName(int userId, int playlistId, string playlistName);
        public Task<ReturnPlaylistDTO> UpdatePlaylistPrivacy(int userId, int playlistId, bool isPublic);
        public Task<ReturnPlaylistDTO> DeletePlaylist(int userId, int playlistId);
        public Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylists();
    }
}
