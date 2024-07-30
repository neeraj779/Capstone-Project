using Newtonsoft.Json.Linq;

namespace SongService.API.Interfaces
{
    public interface ISongProcessingService
    {
        public Task<JObject> FormatSong(JToken data);
        public Task<JObject> FormatAlbum(JToken data);
        public Task<JObject> FormatPlaylist(JToken data);
    }
}
