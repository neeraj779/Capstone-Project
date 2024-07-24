using Newtonsoft.Json.Linq;

namespace SongService.API.Interfaces
{
    public interface ISongDataService
    {
        Task<JArray> SearchForSong(string query, bool lyrics, bool songData);
        Task<JObject> GetSong(string id, bool lyrics);
        Task<string> GetSongId(string url);
        Task<JObject> GetAlbum(string albumId, bool lyrics);
        Task<string> GetAlbumId(string inputUrl);
        Task<JObject> GetPlaylist(string listId, bool lyrics);
        Task<string> GetPlaylistId(string inputUrl);
        Task<string> GetLyrics(string id);
    }
}
