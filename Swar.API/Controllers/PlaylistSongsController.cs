using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Services;
using Swar.API.Models;
using Swar.API.Models.DTOs;
using System.Security.Claims;

namespace Swar.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlaylistSongsController : ControllerBase
    {
        private readonly IPlaylistSongsService _playlistSongsService;

        public PlaylistSongsController(IPlaylistSongsService playlistSongsService)
        {
            _playlistSongsService = playlistSongsService;
        }

        /// <summary>
        /// Adds a song to a playlist.
        /// </summary>
        /// <param name="playlistSongsDTO">Details of the song and playlist.</param>
        /// <returns>Returns the result of adding the song to the playlist.</returns>
        [HttpPost("AddSongToPlaylist")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> AddSongToPlaylist(AddSongToPlaylistDTO playlistSongsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _playlistSongsService.AddSongToPlaylist(userId, playlistSongsDTO);

                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorModel { Status = StatusCodes.Status403Forbidden, Message = ex.Message });
            }
            catch (EntityAlreadyExistsException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ErrorModel { Status = StatusCodes.Status409Conflict, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Removes a song from a playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <param name="songId">The ID of the song.</param>
        /// <returns>Returns the result of removing the song from the playlist.</returns>
        [HttpDelete("RemoveSongFromPlaylist/{playlistId}/{songId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, string songId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _playlistSongsService.RemoveSongFromPlaylist(userId, playlistId, songId);

                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorModel { Status = StatusCodes.Status403Forbidden, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Gets all songs in all playlists.
        /// </summary>
        /// <returns>Returns a list of all songs in all playlists.</returns>
        [HttpGet("GetAllSongsInPlaylist")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSongsInPlaylist()
        {
            try
            {
                var result = await _playlistSongsService.GetAllSongsInPlaylist();
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status200OK, new ErrorModel { Status = StatusCodes.Status200OK, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Gets all songs in a specific user’s playlist.
        /// </summary>
        /// <param name="publicId">The ID of the playlist.</param>
        /// <returns>Returns a list of songs in the specified user's playlist.</returns>
        [HttpGet("GetAllSongsInUserPlaylist/{publicId}")]
        public async Task<IActionResult> GetAllSongsInUserPlaylist(string publicId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _playlistSongsService.GetAllSongsInUserPlaylist(userId, publicId);

                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorModel { Status = StatusCodes.Status403Forbidden, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }
    }
}
