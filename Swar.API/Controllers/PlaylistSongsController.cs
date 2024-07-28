using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Services;
using Swar.API.Models;
using Swar.API.Models.DTOs;

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

        [HttpPost("AddSongToPlaylist")]
        public async Task<IActionResult> AddSongToPlaylist(AddSongToPlaylistDTO playlistSongsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = 100; // Todo: Get userId from token
                var result = await _playlistSongsService.AddSongToPlaylist(userId, playlistSongsDTO);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
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

        [HttpDelete("RemoveSongFromPlaylist/{playlistId}&{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, string songId)
        {
            try
            {
                var userId = 100; // Todo: Get userId from token
                var result = await _playlistSongsService.RemoveSongFromPlaylist(userId, playlistId, songId);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        [HttpGet("GetAllSongsInPlaylist")]
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

        [HttpGet("GetAllSongsInUserPlaylist")]
        public async Task<IActionResult> GetAllSongsInUserPlaylist(int playlistId)
        {
            try
            {
                var userId = 100; // Todo: Get userId from token
                var result = await _playlistSongsService.GetAllSongsInUserPlaylist(userId, playlistId);
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
    }
}
