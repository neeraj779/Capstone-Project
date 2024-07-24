﻿using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SongService.API.Utils
{
    public class SongsDataUtils
    {
        private const string DefaultImageSize = "150x150";
        private const string TargetImageSize = "500x500";
        private const string HighQualitySuffix = "_320.mp4";
        private const string MediumQualitySuffix = "_160.mp4";
        private const string PreviewSuffix = "_96_p.mp4";

        public static JObject FormatSong(JToken data, bool includeLyrics)
        {
            try
            {
                string encryptedMediaUrl = data["encrypted_media_url"]?.ToString();
                if (!string.IsNullOrEmpty(encryptedMediaUrl))
                {
                    data["media_url"] = DecryptUrl(encryptedMediaUrl);
                }

                string quality = data["320kbps"]?.ToString();
                string mediaUrl = data["media_url"]?.ToString();
                if (mediaUrl != null)
                {
                    if (quality != "true")
                    {
                        mediaUrl = mediaUrl.Replace(HighQualitySuffix, MediumQualitySuffix);
                    }
                    data["media_url"] = mediaUrl;
                    data["media_preview_url"] = GeneratePreviewUrl(mediaUrl, quality);
                }
            }
            catch (Exception)
            {
                string mediaPreviewUrl = data["media_preview_url"]?.ToString();
                if (mediaPreviewUrl != null)
                {
                    data["media_url"] = RestoreUrlFromPreview(mediaPreviewUrl, data["320kbps"]?.ToString());
                }
            }

            FormatBasicSongInfo(data);

            if (includeLyrics && data["has_lyrics"]?.ToString() == "true")
            {
                data["lyrics"] = GetLyrics(data["id"]?.ToString());
            }
            else
            {
                data["lyrics"] = null;
            }

            FormatCopyrightText(data);

            return (JObject)data;
        }

        public static JObject FormatAlbum(JToken data, bool includeLyrics)
        {
            data["image"] = data["image"]?.ToString().Replace(DefaultImageSize, TargetImageSize);
            data["name"] = FormatString(data["name"]?.ToString());
            data["primary_artists"] = FormatString(data["primary_artists"]?.ToString());
            data["title"] = FormatString(data["title"]?.ToString());

            foreach (var song in data["songs"] ?? Enumerable.Empty<JToken>())
            {
                FormatSong(song, includeLyrics);
            }

            return (JObject)data;
        }

        public static JObject FormatPlaylist(JToken data, bool includeLyrics)
        {
            data["firstname"] = FormatString(data["firstname"]?.ToString());
            data["listname"] = FormatString(data["listname"]?.ToString());

            foreach (var song in data["songs"] ?? Enumerable.Empty<JToken>())
            {
                FormatSong(song, includeLyrics);
            }

            return (JObject)data;
        }

        private static void FormatBasicSongInfo(JToken data)
        {
            data["song"] = FormatString(data["song"]?.ToString());
            data["music"] = FormatString(data["music"]?.ToString());
            data["singers"] = FormatString(data["singers"]?.ToString());
            data["starring"] = FormatString(data["starring"]?.ToString());
            data["album"] = FormatString(data["album"]?.ToString());
            data["primary_artists"] = FormatString(data["primary_artists"]?.ToString());
            data["image"] = data["image"]?.ToString().Replace(DefaultImageSize, TargetImageSize);
        }

        private static string? FormatString(string input)
        {
            return input?.Replace("&quot;", "'")
                         .Replace("&amp;", "&")
                         .Replace("&#039;", "'");
        }

        private static string GeneratePreviewUrl(string mediaUrl, string quality)
        {
            return mediaUrl
                .Replace(HighQualitySuffix, PreviewSuffix)
                .Replace(MediumQualitySuffix, PreviewSuffix)
                .Replace("//aac.", "//preview.");
        }

        private static string RestoreUrlFromPreview(string previewUrl, string quality)
        {
            string restoredUrl = previewUrl.Replace("preview", "aac");

            if (quality == "true")
            {
                restoredUrl = restoredUrl.Replace(PreviewSuffix, HighQualitySuffix);
            }
            else
            {
                restoredUrl = restoredUrl.Replace(PreviewSuffix, MediumQualitySuffix);
            }

            return restoredUrl;
        }

        private static string DecryptUrl(string url)
        {
            using var desCipher = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes("38346591"),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var encUrl = Convert.FromBase64String(url.Trim());
            using var decryptor = desCipher.CreateDecryptor();
            using var ms = new MemoryStream(encUrl);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            var decryptedUrl = reader.ReadToEnd();
            return decryptedUrl.Replace("_96.mp4", HighQualitySuffix);
        }

        private static void FormatCopyrightText(JToken data)
        {
            try
            {
                data["copyright_text"] = data["copyright_text"]?.ToString().Replace("&copy;", "©");
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Key not found");
            }
        }
        public static string GetLyrics(string id)
        {
            var url = $"https://www.jiosaavn.com/api.php?__call=lyrics.getLyrics&ctx=web6dot0&api_version=4&_format=json&_marker=0%3F_marker%3D0&lyrics_id={id}";
            var response = new HttpClient().GetStringAsync(url).Result;
            var lyricsJson = JObject.Parse(response);
            return lyricsJson["lyrics"].ToString();
        }
    }
}
