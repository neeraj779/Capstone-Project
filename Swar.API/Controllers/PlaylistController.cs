using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Helpers;
using Swar.API.Interfaces.Services;
using Swar.API.Models;
using Swar.API.Models.DTOs;

namespace Swar.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        /// <summary>
        /// Creates a new playlist.
        /// </summary>
        /// <param name="addPlaylistDTO">The playlist details.</param>
        /// <returns>Returns the created playlist.</returns>
        [HttpPost("CreatePlaylist")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CreatePlaylist(AddPlaylistDTO addPlaylistDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int userId = UserHelper.GetUserId(User);
                var result = await _playlistService.AddPlaylist(userId, addPlaylistDTO);

                return Ok(result);
            }
            catch (InactiveAccountException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorModel { Status = StatusCodes.Status403Forbidden, Message = ex.Message });
            }
            catch (MaxLimitException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorModel { Status = StatusCodes.Status400BadRequest, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a playlist by its ID.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to delete.</param>
        /// <returns>Returns the result of the deletion.</returns>
        [HttpDelete("DeletePlaylist/{playlistId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeletePlaylist(int playlistId)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                var result = await _playlistService.DeletePlaylist(userId, playlistId);

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
        /// Gets all playlists.
        /// </summary>
        /// <returns>Returns a list of all playlists.</returns>
        [HttpGet("GetAllPlaylists")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPlaylists()
        {
            try
            {
                var result = await _playlistService.GetAllPlaylists();
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
        /// Gets all playlists of a specific user.
        /// </summary>
        /// <returns>Returns a list of user's playlists.</returns>
        [HttpGet("GetAllPlaylistsByUserId")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetAllPlaylistsByUserId()
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                var result = await _playlistService.GetAllPlaylistsByUserId(userId);

                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status200OK, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific playlist by its ID.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>Returns the playlist details.</returns>
        [HttpGet("GetPlaylistById/{playlistId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetPlaylistById(int playlistId)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                var result = await _playlistService.GetPlaylistById(userId, playlistId);

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
        /// Updates a playlist by its ID.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to update.</param>
        /// <param name="updatePlaylistDTO">The updated playlist details.</param>
        /// <returns>Returns the updated playlist.</returns>
        [HttpPut("UpdatePlaylist/{playlistId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdatePlaylist(int playlistId, UpdatePlaylistDTO updatePlaylistDTO)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                var result = await _playlistService.UpdatePlaylist(userId, playlistId, updatePlaylistDTO);

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
        /// Updates the privacy of a playlist by its ID.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <param name="updatePlaylistPrivacyDTO">The updated privacy details.</param>
        /// <returns>Returns the updated playlist.</returns>
        [HttpPut("UpdatePlaylistPrivacy/{playlistId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdatePlaylistPrivacy(int playlistId, UpdatePlaylistPrivacyDTO updatePlaylistPrivacyDTO)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                var result = await _playlistService.UpdatePlaylistPrivacy(userId, playlistId, updatePlaylistPrivacyDTO.IsPrivate);

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
