using Swar.API.Models.DTOs;

namespace Swar.API.Interfaces.Services
{
    public interface IPlaylistSongsService
    {
        /// <summary>
        /// Adds a song to a playlist.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <param name="songId">The ID of the song to add.</param>
        /// <returns>A DTO representing the playlist with the added song.</returns>
        public Task<PlaylistSongsReturnDTO> AddSongToPlaylist(int userId, AddSongToPlaylistDTO addSongToPlaylistDTO);

        /// <summary>
        /// Removes a song from a playlist.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <param name="songId">The ID of the song to remove.</param>
        /// <returns>A DTO representing the playlist after the song is removed.</returns>
        public Task<PlaylistSongsReturnDTO> RemoveSongFromPlaylist(int userId, int playlistId, string songId);

        /// <summary>
        /// Gets all songs in a specific playlist for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A collection of DTOs representing the songs in the playlist.</returns>
        public Task<PlaylistSongsDTO> GetAllSongsInUserPlaylist(int userId, int playlistId);

        /// <summary>
        /// Gets all songs in a specific playlist.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A collection of DTOs representing the songs in the playlist.</returns>
        public Task<IEnumerable<PlaylistSongsReturnDTO>> GetAllSongsInPlaylist();
    }
}
