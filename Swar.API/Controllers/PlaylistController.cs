using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
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

        [HttpPost("CreatePlaylist")]
        public async Task<IActionResult> CreatePlaylist(AddPlaylistDTO addPlaylistDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = 100; // Todo: Get userId from token
                var result = await _playlistService.AddPlaylist(userId, addPlaylistDTO);
                return Ok(result);
            }

            catch (InactiveAccountException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorModel { Status = StatusCodes.Status403Forbidden, Message = ex.Message });

            }
            catch (MaxPlaylistLimitException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorModel { Status = StatusCodes.Status400BadRequest, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        [HttpDelete("DeletePlaylist/{playlistId}")]
        public async Task<IActionResult> DeletePlaylist(int playlistId)
        {
            try
            {
                var userId = 100; // Todo: Get userId from token
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

        [HttpGet("GetAllPlaylists")]
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

        [HttpGet("GetAllPlaylistsByUserId")]
        public async Task<IActionResult> GetAllPlaylistsByUserId()
        {
            try
            {
                var userId = 100; // Todo: Get userId from token
                var result = await _playlistService.GetAllPlaylistsByUserId(userId);
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

        [HttpGet("GetPlaylistById")]
        public async Task<IActionResult> GetPlaylistById(int playlistId)
        {
            try
            {
                var userId = 100; // Todo: Get userId from token
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

        [HttpPut("UpdatePlaylist/{playlistId}")]
        public async Task<IActionResult> UpdatePlaylist(int playlistId, UpdatePlaylistDTO updatePlaylistDTO)
        {
            try
            {
                var userId = 100; // Todo: Get userId from token
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

        [HttpPut("UpdatePlaylistPrivacy/{playlistId}")]
        public async Task<ActionResult> UpdatePlaylistPrivacy(int playlistId, UpdatePlaylistPrivacyDTO updatePlaylistPrivacyDTO)
        {
            try
            {
                var userId = 100; // Todo: Get userId from token
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
