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
        private readonly IRepository<int, Playlist> _playlistRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly ILogger<PlaylistSongsService> _logger;

        public PlaylistSongsService(IPlaylistSongsRepository playlistSongRepository, IRepository<int, Playlist> playlistRepository, IRepository<int, User> userRepository, ILogger<PlaylistSongsService> logger)
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

            if (playlistSongsCount.Count() >= 5)
            {
                _logger.LogInformation($"User {userId} tried to add more than 5 songs to playlist {addSongToPlaylistDTO.PlaylistId}");
                throw new MaxLimitException("A playlist can only have 5 songs.");
            }


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

        public async Task<PlaylistSongsDTO> GetAllSongsInUserPlaylist(int userId, int playlistId)
        {
            var (user, playlist) = await ValidateUserAndPlaylist(userId, playlistId, true);

            var playlistSongs = await _playlistSongRepository.GetAll();
            if (!playlistSongs.Any())
                throw new EntityNotFoundException("No Playlist songs found.");

            var userPlaylistSongs = playlistSongs.Where(ps => ps.PlaylistId == playlistId).ToList();

            var songsIdList = userPlaylistSongs.Select(ps => ps.SongId).ToList();
            PlaylistInfoDTO playlistInfo = new PlaylistInfoDTO
            {
                PlaylistId = playlistId,
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

        private async Task<(User user, Playlist playlist)> ValidateUserAndPlaylist(int userId, int playlistId, bool isGetRequest = false)
        {
            Playlist playlist = await _playlistRepository.GetById(playlistId)
                ?? throw new EntityNotFoundException("Playlist not found.");

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
