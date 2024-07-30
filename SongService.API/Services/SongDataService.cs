using Newtonsoft.Json.Linq;
using SongService.API.Exceptions;
using SongService.API.Interfaces;
using System.Text.RegularExpressions;

namespace SongService.API.Services
{
    public class SongDataService : ISongDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ISongProcessingService _songProcessingService;

        public SongDataService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ISongProcessingService songProcessingService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _songProcessingService = songProcessingService ?? throw new ArgumentNullException(nameof(songProcessingService));
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient();
        }

        private async Task<string> FetchDataFromApi(string url)
        {
            using var client = CreateClient();
            return await client.GetStringAsync(url);
        }

        public async Task<JArray> SearchForSong(string query, bool lyrics, bool songData)
        {
            if (string.IsNullOrWhiteSpace(query) || query == "#")
                throw new InvalidQueryException();

            if (query.StartsWith("http") && query.Contains("saavn.com"))
            {
                var id = await GetSongId(query);
                var song = await GetSong(id, lyrics);
                return song != null ? new JArray { song } : new JArray();
            }

            var searchUrl = $"{_configuration["MusicAPIs:SearchBaseUrl"]}{query}";
            var response = await FetchDataFromApi(searchUrl);
            var jsonResponse = JObject.Parse(response);
            var songResponse = jsonResponse["songs"]["data"] as JArray ?? new JArray();

            if (!songData)
            {
                return songResponse;
            }

            var songs = new JArray();
            foreach (var song in songResponse)
            {
                var id = song["id"]?.ToString();
                if (id != null)
                {
                    var songDataResponse = await GetSong(id, lyrics);
                    if (songDataResponse != null)
                    {
                        songs.Add(songDataResponse);
                    }
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
            var response = await FetchDataFromApi(songDetailsUrl);
            var jsonResponse = JObject.Parse(response);
            var songData = jsonResponse[id];

            if (songData == null)
                return null;

            if (lyrics && songData["has_lyrics"]?.ToString() == "true")
            {
                songData["lyrics"] = await GetLyrics(id);
            }
            else
            {
                songData["lyrics"] = null;
            }

            return await _songProcessingService.FormatSong(songData);
        }

        public async Task<string> GetSongId(string url)
        {
            var response = await FetchDataFromApi(url);
            return ExtractIdFromResponse(response.ToString(), "\"pid\":\"", "\"id\":\"");
        }

        public async Task<JObject> GetAlbum(string albumId, bool lyrics)
        {
            var albumDetailsUrl = $"{_configuration["MusicAPIs:AlbumDetailsBaseUrl"]}{albumId}";
            var response = await FetchDataFromApi(albumDetailsUrl);
            var jsonResponse = JObject.Parse(response);
            int albumYear = jsonResponse["year"]?.Value<int>() ?? 0;
            if (albumYear == 0)
                throw new EntityNotFoundException("Album not found");

            return await _songProcessingService.FormatAlbum(jsonResponse);
        }

        public async Task<string> GetAlbumId(string inputUrl)
        {
            var response = await FetchDataFromApi(inputUrl);
            return ExtractIdFromResponse(response.ToString(), "\"album_id\":\"", "\"id\":\"");
        }

        public async Task<JObject> GetPlaylist(string listId, bool lyrics)
        {
            var playlistDetailsUrl = $"{_configuration["MusicAPIs:PlaylistDetailsBaseUrl"]}{listId}";
            var response = await FetchDataFromApi(playlistDetailsUrl);
            var jsonResponse = JObject.Parse(response);
            int listCount = jsonResponse["list_count"]?.Value<int>() ?? 0;

            if (listCount == 0)
                throw new EntityNotFoundException("Playlist not found");

            return await _songProcessingService.FormatPlaylist(jsonResponse);
        }

        public async Task<string> GetPlaylistId(string inputUrl)
        {
            var response = await FetchDataFromApi(inputUrl);
            return ExtractIdFromResponse(response.ToString(), "\"type\":\"playlist\",\"id\":\"", "\"id\":\"");
        }

        public async Task<string> GetLyrics(string id)
        {
            var lyricsBaseUrl = $"{_configuration["MusicAPIs:LyricsBaseUrl"]}{id}";
            var response = await FetchDataFromApi(lyricsBaseUrl);
            var jsonResponse = JObject.Parse(response);

            var lyrics = jsonResponse["lyrics"]?.ToString();
            if (string.IsNullOrEmpty(lyrics))
                throw new EntityNotFoundException("Lyrics not found");

            return lyrics;
        }

        private string ExtractIdFromResponse(string response, params string[] delimiters)
        {
            try
            {
                foreach (var delimiter in delimiters)
                {
                    var regexPattern = $"{Regex.Escape(delimiter)}(.*?)(?=\")";
                    var match = Regex.Match(response, regexPattern);
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidQueryException();
            }
            throw new InvalidQueryException();
        }
    }
}
