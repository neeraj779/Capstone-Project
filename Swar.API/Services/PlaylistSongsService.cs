using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using Swar.API.Models.DTOs;
using Swar.API.Models.ENUMs;

namespace Swar.API.Services
{
    public class PlaylistSongsService : IPlaylistSongsService
    {
        private readonly IPlaylistSongsRepository _playlistSongRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly ILogger<PlaylistSongsService> _logger;

        public PlaylistSongsService(IPlaylistSongsRepository playlistSongRepository, IPlaylistRepository playlistRepository, IRepository<int, User> userRepository, ILogger<PlaylistSongsService> logger)
        {
            _playlistSongRepository = playlistSongRepository;
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _logger = logger;
        }


        public async Task<PlaylistSongsReturnDTO> AddSongToPlaylist(int userId, AddSongToPlaylistDTO addSongToPlaylistDTO)
        {
            await ValidateUserAndPlaylist(userId, addSongToPlaylistDTO.PlaylistId);

            var playlistSongExists = await _playlistSongRepository.GetByCompositeKey(addSongToPlaylistDTO.PlaylistId, addSongToPlaylistDTO.SongId);
            var playlistSongsCount = await _playlistSongRepository.GetAll();
            playlistSongsCount = playlistSongsCount.Where(ps => ps.PlaylistId == addSongToPlaylistDTO.PlaylistId);

            if (playlistSongExists != null)
            {
                _logger.LogInformation($"User {userId} tried to add song {addSongToPlaylistDTO.SongId} to playlist {addSongToPlaylistDTO.PlaylistId} but it already exists.");
                throw new EntityAlreadyExistsException("Song already exists in playlist.");
            }

            PlaylistSong playlistSong = new PlaylistSong
            {
                PlaylistId = addSongToPlaylistDTO.PlaylistId,
                SongId = addSongToPlaylistDTO.SongId
            };

            await _playlistSongRepository.Add(playlistSong);

            _logger.LogInformation($"User {userId} added song {addSongToPlaylistDTO.SongId} to playlist {addSongToPlaylistDTO.PlaylistId}");
            return MapPlaylistSongToReturnPlaylistSongDTO(playlistSong);
        }

        public async Task<IEnumerable<PlaylistSongsReturnDTO>> GetAllSongsInPlaylist()
        {
            var playlistSongs = await _playlistSongRepository.GetAll();
            if (!playlistSongs.Any())
            {
                _logger.LogInformation("No Playlist songs found.");
                throw new EntityNotFoundException("No Playlist songs found.");
            }

            _logger.LogInformation("Returning all Playlist songs.");
            return playlistSongs.Select(MapPlaylistSongToReturnPlaylistSongDTO);
        }

        public async Task<PlaylistSongsDTO> GetAllSongsInUserPlaylist(int userId, string publicId)
        {
            var (user, playlist) = await ValidateUserAndPlaylist(userId, publicId, true);

            var playlistSongs = await _playlistSongRepository.GetAll();
            var userPlaylistSongs = playlistSongs.Where(ps => ps.PlaylistId == playlist.PlaylistId).ToList();

            var songsIdList = userPlaylistSongs.Select(ps => ps.SongId).ToList();
            PlaylistInfoDTO playlistInfo = new PlaylistInfoDTO
            {
                UserId = user.UserId,
                PlaylistId = playlist.PlaylistId,
                PublicId = playlist.PublicId,
                OwnerName = user.Name,
                PlaylistName = playlist.PlaylistName,
                Description = playlist.Description,
                IsPrivate = playlist.IsPrivate,
                CreatedAt = playlist.CreatedAt,
                SongsCount = songsIdList.Count,
            };

            _logger.LogInformation($"Returning all Playlist songs for user with id {userId}.");
            return new PlaylistSongsDTO
            {
                PlaylistInfo = playlistInfo,
                Songs = songsIdList
            };
        }

        public async Task<PlaylistSongsReturnDTO> RemoveSongFromPlaylist(int userId, int playlistId, string songId)
        {
            await ValidateUserAndPlaylist(userId, playlistId);

            var playlistSong = await _playlistSongRepository.GetByCompositeKey(playlistId, songId);
            if (playlistSong == null)
            {
                _logger.LogInformation($"User {userId} tried to remove song {songId} from playlist {playlistId} but it does not exist.");
                throw new EntityNotFoundException("Playlist song not found.");
            }

            await _playlistSongRepository.DeletePlaylistSong(playlistId, songId);

            _logger.LogInformation($"User {userId} removed song {songId} from playlist {playlistId}");
            return MapPlaylistSongToReturnPlaylistSongDTO(playlistSong);
        }

        private async Task<(User user, Playlist playlist)> ValidateUserAndPlaylist(int userId, object playlistIdentifier, bool isGetRequest = false)
        {
            Playlist? playlist = playlistIdentifier switch
            {
                int playlistId => await _playlistRepository.GetById(playlistId),
                string publicId => await _playlistRepository.GetByPublicId(publicId),
                _ => throw new ArgumentException("Invalid playlist identifier type.")
            };

            if (playlist == null)
                throw new EntityNotFoundException("Playlist not found.");

            User user = await _userRepository.GetById(userId);

            if (userId != 0)
            {
                if (user == null)
                    throw new EntityNotFoundException("User not found.");

                if (user.UserStatus != UserStatusEnum.UserStatus.Active)
                {
                    throw new InactiveAccountException();
                }
            }

            if (isGetRequest)
            {
                if (playlist.IsPrivate && (userId == 0 || playlist.UserId != userId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to access this private playlist.");
                }
            }
            else
            {
                if (playlist.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this playlist.");
                }
            }

            return (user, playlist);
        }


        public PlaylistSongsReturnDTO MapPlaylistSongToReturnPlaylistSongDTO(PlaylistSong playlistSong)
        {
            return new PlaylistSongsReturnDTO
            {
                PlaylistId = playlistSong.PlaylistId,
                SongId = playlistSong.SongId
            };
        }
    }
}
