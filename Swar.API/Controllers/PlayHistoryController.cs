using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Helpers;
using Swar.API.Interfaces.Services;
using Swar.API.Models;

namespace Swar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayHistoryController : ControllerBase
    {
        private readonly IPlayHistoryService _playHistoryService;

        public PlayHistoryController(IPlayHistoryService playHistoryService)
        {
            _playHistoryService = playHistoryService;
        }

        [HttpPost("LogSongHistory")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> LogSongHistory(string songId)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                await _playHistoryService.LogSongHistory(userId, songId);
                return Ok();
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

        [HttpGet("GetSongHistoryByUser")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetSongHistoryByUser(bool unique = true)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
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
