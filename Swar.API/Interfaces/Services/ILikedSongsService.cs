using Swar.API.Models.DTOs;

namespace Swar.API.Interfaces.Services
{
    public interface ILikedSongsService
    {
        /// <summary>
        /// Adds a song to the liked songs list of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="songId">The ID of the song to add.</param>
        /// <returns>A DTO representing the liked songs list after the song is added.</returns>
        public Task<LikedSongsReturnDTO> AddSongToLikedSongs(int userId, string songId);

        /// <summary>
        /// Removes a song from the liked songs list of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="songId">The ID of the song to remove.</param>
        /// <returns>A DTO representing the liked songs list after the song is removed.</returns>
        public Task<LikedSongsReturnDTO> RemoveSongFromLikedSongs(int userId, string songId);

        /// <summary>
        /// Gets all liked songs of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of DTOs representing the liked songs of the user.</returns>
        public Task<LikedSongsDTO> GetAllLikedSongs(int userId);

        /// <summary>
        /// Checks if a song is in the liked songs list of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>"
        /// <returns> A boolean value indicating if the song is in the liked songs list.</returns>        
        public Task<bool> IsSongLikedByUser(int userId, string songId);
    }
}
