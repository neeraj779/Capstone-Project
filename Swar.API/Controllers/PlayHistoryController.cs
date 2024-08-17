using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Services;
using Swar.API.Models;

namespace Swar.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlayHistoryController : ControllerBase
    {
        private readonly IPlayHistoryService _playHistoryService;

        public PlayHistoryController(IPlayHistoryService playHistoryService)
        {
            _playHistoryService = playHistoryService;
        }

        /// <summary>
        /// Logs the history of a song played by the user.
        /// </summary>
        /// <param name="songId">The ID of the song to log.</param>
        /// <returns>Returns a success message if the song history is logged successfully.</returns>
        [HttpPost("LogSongHistory")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> LogSongHistory([FromBody] string songId)
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int userId)
                    throw new EntityNotFoundException("User does not exist");
                await _playHistoryService.LogSongHistory(userId, songId);
                return Ok(new { Message = "Song history logged successfully." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (InactiveAccountException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Gets the song history for the current user.
        /// </summary>
        /// <param name="unique">Indicates whether to return only unique songs.</param>
        /// <returns>Returns a list of songs from the user's history.</returns>
        [HttpGet("GetSongHistoryByUser")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetSongHistoryByUser(bool unique = true)
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int userId)
                    throw new EntityNotFoundException("User does not exist");
                var result = await _playHistoryService.GetSongHistory(userId, unique);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (InactiveAccountException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Gets the play history for all users.
        /// </summary>
        /// <returns>Returns a list of all users' song history.</returns>
        [HttpGet("GetAllUserHistory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserHistory()
        {
            try
            {
                var result = await _playHistoryService.GetAllUserHistory();
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }
    }
}
