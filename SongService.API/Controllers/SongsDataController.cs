using Microsoft.AspNetCore.Mvc;
using SongService.API.Exceptions;
using SongService.API.Interfaces;
using SongService.API.Models;

namespace SongService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsDataController : ControllerBase
    {
        private readonly ISongDataService _songService;

        public SongsDataController(ISongDataService songService)
        {
            _songService = songService;
        }

        [HttpGet("SearchSong")]
        public async Task<ActionResult> SearchForSong(string query, bool lyrics = false, bool songData = true)
        {
            try
            {
                var songs = await _songService.SearchForSong(query, lyrics, songData);
                return Ok(songs.ToString());
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (InvalidQueryException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorModel { Status = StatusCodes.Status400BadRequest, Message = ex.Message });
            }
        }

        [HttpGet("GetSongById")]
        public async Task<ActionResult> GetSong(string id, bool lyrics = false)
        {
            try
            {
                var song = await _songService.GetSong(id, lyrics);
                return Ok(song.ToString());
            }
            catch (InvalidSongIdException)
            {
                return NotFound();
            }
        }

        [HttpGet("GetAlbum")]
        public async Task<IActionResult> GetAlbum(string albumId, bool lyrics = false)
        {
            var album = await _songService.GetAlbum(albumId, lyrics);
            return Ok(album);
        }

        [HttpGet("GetPlaylist")]
        public async Task<IActionResult> GetPlaylist(string listId, bool lyrics = false)
        {
            var playlist = await _songService.GetPlaylist(listId, lyrics);
            return Ok(playlist);
        }

        [HttpGet("GetLyrics")]
        public async Task<IActionResult> GetLyrics(string id)
        {
            try
            {
                var lyrics = await _songService.GetLyrics(id);
                return Ok(lyrics);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
        }
    }
}
