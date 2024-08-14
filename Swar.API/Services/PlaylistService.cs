using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using Swar.API.Models.DTOs;
using Swar.API.Models.ENUMs;

namespace Swar.API.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IRepository<int, Playlist> _playlistRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly ILogger<PlaylistService> _logger;

        public PlaylistService(IRepository<int, Playlist> playlistRepository, IRepository<int, User> userRepository, ILogger<PlaylistService> logger)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ReturnPlaylistDTO> AddPlaylist(int userId, AddPlaylistDTO addPlaylistDTO)
        {
            User user = await _userRepository.GetById(userId);
            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
            {
                _logger.LogInformation($"User {userId} tried to add a playlist but the account is inactive.");
                throw new InactiveAccountException();
            }

            var userPlaylists = await _playlistRepository.GetAll();
            if (userPlaylists.Count(p => p.UserId == userId) >= 3)
            {
                _logger.LogInformation($"User {userId} tried to add more than 3 playlists.");
                throw new MaxLimitException();
            }

            Playlist playlist = new Playlist
            {
                UserId = userId,
                PlaylistName = addPlaylistDTO.PlaylistName,
                Description = addPlaylistDTO.Description,
                IsPrivate = addPlaylistDTO.IsPrivate,
                CreatedAt = DateTime.Now
            };

            await _playlistRepository.Add(playlist);

            _logger.LogInformation($"User {userId} added playlist {playlist.PlaylistId}");
            return MapPlaylistToReturnPlaylistDTO(playlist);

        }

        public async Task<ReturnPlaylistDTO> DeletePlaylist(int userId, int playlistId)
        {
            var playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
                throw new EntityNotFoundException();

            if (playlist.UserId != userId)
            {
                _logger.LogInformation($"User {userId} tried to delete playlist {playlistId} but is not authorized.");
                throw new UnauthorizedAccessException("You are not authorized to delete this playlist.");
            }

            await _playlistRepository.Delete(playlistId);

            _logger.LogInformation($"User {userId} deleted playlist {playlistId}");
            return MapPlaylistToReturnPlaylistDTO(playlist);
        }


        public async Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylists()
        {
            var playlists = await _playlistRepository.GetAll();
            if (!playlists.Any())
            {
                _logger.LogInformation("No playlists found during GetAllPlaylists.");
                throw new EntityNotFoundException("No playlists found.");
            }

            _logger.LogInformation("Returning all playlists.");
            return playlists.Select(MapPlaylistToReturnPlaylistDTO).ToList();
        }

        public async Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylistsByUserId(int userId)
        {
            var playlists = await _playlistRepository.GetAll();
            var userPlaylists = playlists.Where(p => p.UserId == userId);

            if (!userPlaylists.Any())
            {
                _logger.LogInformation($"No playlists found for user with ID {userId}.");
                throw new EntityNotFoundException($"No playlists found for user with ID {userId}.");
            }

            _logger.LogInformation($"Returning all playlists for user with ID {userId}.");
            return userPlaylists.Select(MapPlaylistToReturnPlaylistDTO).ToList();
        }

        public async Task<ReturnPlaylistDTO> GetPlaylistById(int userId, int playlistId)
        {
            Playlist playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
            {
                _logger.LogInformation($"Playlist with ID {playlistId} not found.");
                throw new EntityNotFoundException($"Playlist with ID {playlistId} not found.");
            }

            if (playlist.UserId != userId)
            {
                _logger.LogInformation($"User {userId} tried to access playlist {playlistId} but is not authorized.");
                throw new UnauthorizedAccessException("You are not authorized to view this playlist.");
            }

            _logger.LogInformation($"Returning playlist with ID {playlistId}.");
            return MapPlaylistToReturnPlaylistDTO(playlist);
        }

        public async Task<ReturnPlaylistDTO> UpdatePlaylist(int userId, int playlistId, UpdatePlaylistDTO updatePlaylistDTO)
        {
            Playlist playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
            {
                _logger.LogInformation($"Playlist with ID {playlistId} not found.");
                throw new EntityNotFoundException($"Playlist with ID {playlistId} not found.");
            }

            if (playlist.UserId != userId)
            {
                _logger.LogInformation($"User {userId} tried to update playlist {playlistId} but is not authorized.");
                throw new UnauthorizedAccessException("You are not authorized to update this playlist.");
            }

            playlist.PlaylistName = updatePlaylistDTO.PlaylistName;
            playlist.Description = updatePlaylistDTO.Description;

            await _playlistRepository.Update(playlist);

            _logger.LogInformation($"User {userId} updated playlist {playlistId}.");
            return MapPlaylistToReturnPlaylistDTO(playlist);
        }

        public async Task<ReturnPlaylistDTO> UpdatePlaylistPrivacy(int userId, int playlistId, bool IsPrivate)
        {
            Playlist playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
            {
                _logger.LogInformation($"Playlist with ID {playlistId} not found.");
                throw new EntityNotFoundException($"Playlist with ID {playlistId} not found.");
            }

            if (playlist.UserId != userId)
            {
                _logger.LogInformation($"User {userId} tried to update playlist {playlistId} but is not authorized.");
                throw new UnauthorizedAccessException("You are not authorized to update this playlist.");
            }

            playlist.IsPrivate = IsPrivate;
            await _playlistRepository.Update(playlist);

            _logger.LogInformation($"User {userId} updated privacy of playlist {playlistId} to {IsPrivate}.");
            return MapPlaylistToReturnPlaylistDTO(playlist);
        }

        public ReturnPlaylistDTO MapPlaylistToReturnPlaylistDTO(Playlist playlist)
        {
            return new ReturnPlaylistDTO
            {
                UserId = playlist.UserId,
                PublicId = playlist.PublicId,
                PlaylistId = playlist.PlaylistId,
                PlaylistName = playlist.PlaylistName,
                Description = playlist.Description,
                IsPrivate = playlist.IsPrivate,
                CreatedAt = playlist.CreatedAt
            };
        }
    }
}
