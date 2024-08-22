using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Services;
using Swar.API.Models;
using System.Security.Claims;

namespace Swar.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LikedSongsController : ControllerBase
    {
        private readonly ILikedSongsService _likedSongsService;

        public LikedSongsController(ILikedSongsService likedSongsService)
        {
            _likedSongsService = likedSongsService;
        }

        /// <summary>
        /// Adds a song to the user's liked songs.
        /// </summary>
        /// <param name="songId">The ID of the song to like.</param>
        /// <returns>Returns the result of the operation.</returns>
        [HttpPost("LikeSong")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> LikeSong([FromBody] string songId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _likedSongsService.AddSongToLikedSongs(userId, songId);

                return Ok(result);
            }
            catch (InactiveAccountException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorModel { Status = StatusCodes.Status403Forbidden, Message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (EntityAlreadyExistsException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorModel { Status = StatusCodes.Status400BadRequest, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Removes a song from the user's liked songs.
        /// </summary>
        /// <param name="songId">The ID of the song to unlike.</param>
        /// <returns>Returns the result of the operation.</returns>
        [HttpDelete("UnlikeSong/{songId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UnlikeSong(string songId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _likedSongsService.RemoveSongFromLikedSongs(userId, songId);

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

        /// <summary>
        /// Gets all liked songs of the user.
        /// </summary>
        /// <returns>Returns a list of liked songs.</returns>
        [HttpGet("GetLikedSongs")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetLikedSongs()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _likedSongsService.GetAllLikedSongs(userId);

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

        /// <summary>
        /// Checks if a song is liked by the user.
        /// </summary>
        /// <param name="songId">The ID of the song to check.</param>
        /// <returns>Returns true if the song is liked, otherwise false.</returns>
        [HttpGet("IsSongLikedByUser/{songId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> IsSongLikedByUser(string songId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _likedSongsService.IsSongLikedByUser(userId, songId);

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
    }
}
