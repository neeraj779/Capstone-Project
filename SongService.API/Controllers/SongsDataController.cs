using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SongService.API.Exceptions;
using SongService.API.Interfaces;
using SongService.API.Models;

namespace SongService.API.Controllers
{
    /// <summary>
    /// Controller for managing song data operations.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SongsDataController : ControllerBase
    {
        private readonly ISongDataService _songService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongsDataController"/> class.
        /// </summary>
        /// <param name="songService">The service for handling song data.</param>
        public SongsDataController(ISongDataService songService)
        {
            _songService = songService;
        }

        /// <summary>
        /// Searches for songs based on a query.
        /// </summary>
        /// <param name="query">The search query for songs.</param>
        /// <param name="lyrics">Whether to include lyrics in the search results.</param>
        /// <param name="songData">Whether to include song data in the search results.</param>
        /// <returns>An <see cref="ActionResult"/> with the search results or an error message.</returns>
        [HttpGet("GetSongsByQuery")]
        [Authorize]
        public async Task<ActionResult> SearchForSong(string query, bool lyrics = false, bool songData = true)
        {
            try
            {
                var songs = await _songService.SearchForSong(query, lyrics, songData);
                return Ok(songs.ToString());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidQueryException ex)
            {
                return BadRequest(new ErrorModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves a song by its ID.
        /// </summary>
        /// <param name="id">The ID of the song to retrieve.</param>
        /// <param name="lyrics">Whether to include lyrics in the response.</param>
        /// <returns>An <see cref="ActionResult"/> with the song data or an error message.</returns>
        [HttpGet("GetSongById")]
        public async Task<ActionResult> GetSongById(string id, bool lyrics = false)
        {
            try
            {
                var song = await _songService.GetSong(id, lyrics);
                if (song == null)
                {
                    return NotFound(new ErrorModel
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Song not found"
                    });
                }
                return Ok(song.ToString());
            }
            catch (Exception ex)
            {
                // Log exception details here
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves an album by its ID.
        /// </summary>
        /// <param name="albumId">The ID of the album to retrieve.</param>
        /// <param name="lyrics">Whether to include lyrics in the response.</param>
        /// <returns>An <see cref="IActionResult"/> with the album data.</returns>
        [HttpGet("GetAlbumById")]
        [Authorize]
        public async Task<IActionResult> GetAlbumById(string albumId, bool lyrics = false)
        {
            try
            {
                var album = await _songService.GetAlbum(albumId, lyrics);
                return Ok(album.ToString());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves an album using its URL.
        /// </summary>
        /// <param name="inputUrl">The URL of the album.</param>
        /// <param name="lyrics">Whether to include lyrics in the response.</param>
        /// <returns>An <see cref="IActionResult"/> with the album data or an error message.</returns>
        [HttpGet("GetAlbumByLink")]
        [Authorize]
        public async Task<IActionResult> GetAlbumByLink(string inputUrl, bool lyrics = false)
        {
            try
            {
                var albumId = await _songService.GetAlbumId(inputUrl);
                var album = await _songService.GetAlbum(albumId, lyrics);
                return Ok(album.ToString());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves a playlist by its ID.
        /// </summary>
        /// <param name="listId">The ID of the playlist to retrieve.</param>
        /// <param name="lyrics">Whether to include lyrics in the response.</param>
        /// <returns>An <see cref="IActionResult"/> with the playlist data.</returns>
        [HttpGet("GetPlaylistById")]
        [Authorize]
        public async Task<IActionResult> GetPlaylistById(string listId, bool lyrics = false)
        {
            try
            {
                var playlist = await _songService.GetPlaylist(listId, lyrics);
                return Ok(playlist.ToString());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves a playlist using its URL.
        /// </summary>
        /// <param name="inputUrl">The URL of the playlist.</param>
        /// <param name="lyrics">Whether to include lyrics in the response.</param>
        /// <returns>An <see cref="IActionResult"/> with the playlist data or an error message.</returns>
        [HttpGet("GetPlaylistByLink")]
        [Authorize]
        public async Task<IActionResult> GetPlaylistByLink(string inputUrl, bool lyrics = false)
        {
            try
            {
                var listId = await _songService.GetPlaylistId(inputUrl);
                var playlist = await _songService.GetPlaylist(listId, lyrics);
                return Ok(playlist.ToString());
            }
            catch (InvalidQueryException ex)
            {
                return BadRequest(new ErrorModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves lyrics for a song by its ID.
        /// </summary>
        /// <param name="id">The ID of the song whose lyrics are to be retrieved.</param>
        /// <returns>An <see cref="IActionResult"/> with the lyrics or an error message.</returns>
        [HttpGet("GetLyricsById")]
        [Authorize]
        public async Task<IActionResult> GetLyrics(string id)
        {
            try
            {
                var lyrics = await _songService.GetLyrics(id);
                return Ok(lyrics.ToString());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
        }
    }
}
