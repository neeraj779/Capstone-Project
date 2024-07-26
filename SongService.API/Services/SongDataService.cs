using Newtonsoft.Json.Linq;
using SongService.API.Exceptions;
using SongService.API.Interfaces;
using SongService.API.Utils;

namespace SongService.API.Services
{
    public class SongDataService : ISongDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SongDataService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<JArray> SearchForSong(string query, bool lyrics, bool songData)
        {
            if (query == "#")
                throw new InvalidQueryException();

            if (query.StartsWith("http") && query.Contains("saavn.com"))
            {
                var id = await GetSongId(query);
                var song = await GetSong(id, lyrics);
                return song != null ? new JArray { song } : new JArray();
            }

            var searchUrl = $"{_configuration["MusicAPIs:SearchBaseUrl"]}{query}";
            var response = await _httpClient.GetStringAsync(searchUrl);
            var jsonResponse = JObject.Parse(response);
            var songResponse = jsonResponse["songs"]["data"] as JArray;

            if (songResponse == null)
            {
                return new JArray();
            }

            if (!songData)
            {
                return songResponse;
            }

            var songs = new JArray();
            foreach (var song in songResponse)
            {
                var id = song["id"].ToString();
                var songDataResponse = await GetSong(id, lyrics);
                if (songDataResponse != null)
                {
                    songs.Add(songDataResponse);
                }
            }
            if (songs.Count == 0)
            {
                throw new EntityNotFoundException("Songs not found");
            }
            return songs;
        }

        public async Task<JObject> GetSong(string id, bool lyrics)
        {
            var songDetailsUrl = $"{_configuration["MusicAPIs:SongDetailsBaseUrl"]}{id}";
            var response = await _httpClient.GetStringAsync(songDetailsUrl);
            var jsonResponse = JObject.Parse(response);
            if (jsonResponse[id] == null)
                return null;
            return SongsDataUtils.FormatSong(jsonResponse[id], lyrics);
        }

        public async Task<string> GetSongId(string url)
        {
            var response = await _httpClient.GetStringAsync(url);
            try
            {
                return response.Split(new[] { "\"pid\":\"" }, StringSplitOptions.None)[1].Split('\"')[0];
            }
            catch (IndexOutOfRangeException)
            {
                return response.Split(new[] { "\"song\":{\"type\":\"" }, StringSplitOptions.None)[1].Split(new[] { "\",\"image\":\"" }, StringSplitOptions.None)[0].Split(new[] { "\"id\":\"" }, StringSplitOptions.None)[1];
            }
        }

        public async Task<JObject> GetAlbum(string albumId, bool lyrics)
        {
            try
            {
                string AlbumDetailsBaseUrl = _configuration["MusicAPIs:AlbumDetailsBaseUrl"];
                var response = await _httpClient.GetAsync(AlbumDetailsBaseUrl + albumId);
                if (response.IsSuccessStatusCode)
                {
                    var songsJson = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(songsJson);
                    return SongsDataUtils.FormatAlbum(jsonResponse, lyrics);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public async Task<string> GetAlbumId(string inputUrl)
        {
            var response = "";
            try
            {
                response = await _httpClient.GetStringAsync(inputUrl);
                return response.Split(new[] { "\"album_id\":\"" }, StringSplitOptions.None)[1].Split('\"')[0];
            }
            catch (IndexOutOfRangeException)
            {
                return response.Split(new[] { "\"page_id\",\"" }, StringSplitOptions.None)[1].Split('\"')[0];
            }
            catch (InvalidOperationException)
            {
                throw new InvalidQueryException();
            }
        }

        public async Task<JObject> GetPlaylist(string listId, bool lyrics)
        {
            try
            {
                string PlaylistDetailsBaseUrl = _configuration["MusicAPIs:PlaylistDetailsBaseUrl"];
                var response = await _httpClient.GetAsync(PlaylistDetailsBaseUrl + listId);
                if (response.IsSuccessStatusCode)
                {
                    var songsJson = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(songsJson);
                    return SongsDataUtils.FormatPlaylist(jsonResponse, lyrics);
                }
            }
            catch (Exception e)
            {
                throw new EntityNotFoundException("Playlist not found");
            }

            return null;
        }

        public async Task<string> GetPlaylistId(string inputUrl)
        {
            var response = "";
            try
            {
                response = await _httpClient.GetStringAsync(inputUrl);
                return response.Split(new[] { "\"type\":\"playlist\",\"id\":\"" }, StringSplitOptions.None)[1].Split('\"')[0];
            }
            catch (IndexOutOfRangeException)
            {
                return response.Split(new[] { "\"page_id\",\"" }, StringSplitOptions.None)[1].Split('\"')[0];
            }
            catch (InvalidOperationException)
            {
                throw new InvalidQueryException();
            }
        }

        public async Task<string> GetLyrics(string id)
        {
            try
            {
                string LyricsBaseUrl = _configuration["MusicAPIs:LyricsBaseUrl"];
                var response = await _httpClient.GetStringAsync(LyricsBaseUrl + id);
                var lyricsJson = JObject.Parse(response);
                return lyricsJson["lyrics"].ToString();
            }

            catch (Exception e)
            {
                throw new EntityNotFoundException("Lyrics not found");
            }
        }
    }
}
