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

        public PlaylistService(
            IRepository<int, Playlist> playlistRepository,
            IRepository<int, User> userRepository,
            ILogger<PlaylistService> logger)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ReturnPlaylistDTO> AddPlaylist(int userId, AddPlaylistDTO addPlaylistDTO)
        {
            var user = await ValidateUser(userId, true);
            var playlist = new Playlist
            {
                UserId = userId,
                PlaylistName = addPlaylistDTO.PlaylistName,
                Description = addPlaylistDTO.Description,
                IsPrivate = addPlaylistDTO.IsPrivate,
                CreatedAt = DateTime.UtcNow
            };

            await _playlistRepository.Add(playlist);
            _logger.LogInformation($"User {userId} added playlist {playlist.PlaylistId}");

            return MapPlaylistToReturnPlaylistDTO(playlist, user);
        }

        public async Task<ReturnPlaylistDTO> DeletePlaylist(int userId, int playlistId)
        {
            var (user, playlist) = await ValidateUserAndPlaylist(userId, playlistId);

            await _playlistRepository.Delete(playlistId);
            _logger.LogInformation($"User {userId} deleted playlist {playlistId}");

            return MapPlaylistToReturnPlaylistDTO(playlist, user);
        }

        public async Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylists()
        {
            var playlists = await _playlistRepository.GetAll();
            if (!playlists.Any())
            {
                _logger.LogInformation("No playlists found.");
                throw new EntityNotFoundException("No playlists found.");
            }

            var usersDict = await GetUsersDictionary(playlists.Select(p => p.UserId).Distinct());

            _logger.LogInformation("Returning all playlists.");
            return playlists.Select(p => MapPlaylistToReturnPlaylistDTO(p, usersDict[p.UserId])).ToList();
        }

        public async Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylistsByUserId(int userId)
        {
            User user = await ValidateUser(userId);

            var playlists = (await _playlistRepository.GetAll())
                .Where(p => p.UserId == userId)
                .ToList();

            _logger.LogInformation($"Returning all playlists for user with ID {userId}.");
            return playlists.Select(p => MapPlaylistToReturnPlaylistDTO(p, user)).ToList();
        }

        public async Task<ReturnPlaylistDTO> GetPlaylistById(int userId, int playlistId)
        {
            var (user, playlist) = await ValidateUserAndPlaylist(userId, playlistId);

            _logger.LogInformation($"Returning playlist with ID {playlistId}.");
            return MapPlaylistToReturnPlaylistDTO(playlist, user);
        }

        public async Task<ReturnPlaylistDTO> UpdatePlaylist(int userId, int playlistId, UpdatePlaylistDTO updatePlaylistDTO)
        {
            var (user, playlist) = await ValidateUserAndPlaylist(userId, playlistId);

            playlist.PlaylistName = updatePlaylistDTO.PlaylistName;
            playlist.Description = updatePlaylistDTO.Description;

            await _playlistRepository.Update(playlist);
            _logger.LogInformation($"User {userId} updated playlist {playlistId}");

            return MapPlaylistToReturnPlaylistDTO(playlist, user);
        }

        public async Task<ReturnPlaylistDTO> UpdatePlaylistPrivacy(int userId, int playlistId, bool isPrivate)
        {
            var (user, playlist) = await ValidateUserAndPlaylist(userId, playlistId);

            playlist.IsPrivate = isPrivate;
            await _playlistRepository.Update(playlist);

            _logger.LogInformation($"User {userId} updated privacy of playlist {playlistId} to {isPrivate}");

            return MapPlaylistToReturnPlaylistDTO(playlist, user);
        }

        private async Task<User> ValidateUser(int userId, bool validateStatus = false)
        {
            var user = await _userRepository.GetById(userId)
                ?? throw new EntityNotFoundException($"User with ID {userId} not found.");

            if (validateStatus && user.UserStatus != UserStatusEnum.UserStatus.Active)
                throw new InactiveAccountException();

            return user;
        }

        private async Task<(User user, Playlist playlist)> ValidateUserAndPlaylist(int userId, int playlistId)
        {
            var user = await _userRepository.GetById(userId)
                ?? throw new EntityNotFoundException("User not found.");
            var playlist = await _playlistRepository.GetById(playlistId)
                ?? throw new EntityNotFoundException("Playlist not found.");

            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
                throw new InactiveAccountException();

            if (playlist.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to modify this playlist.");

            return (user, playlist);
        }

        private async Task<Dictionary<int, User>> GetUsersDictionary(IEnumerable<int> userIds)
        {
            var userTasks = userIds.Select(userId => _userRepository.GetById(userId));
            return (await Task.WhenAll(userTasks)).ToDictionary(u => u.UserId);
        }

        private ReturnPlaylistDTO MapPlaylistToReturnPlaylistDTO(Playlist playlist, User user) => new ReturnPlaylistDTO
        {
            UserId = playlist.UserId,
            OwnerName = user.Name,
            PublicId = playlist.PublicId,
            PlaylistId = playlist.PlaylistId,
            PlaylistName = playlist.PlaylistName,
            Description = playlist.Description,
            IsPrivate = playlist.IsPrivate,
            CreatedAt = playlist.CreatedAt
        };
    }
}
