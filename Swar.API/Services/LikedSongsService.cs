using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using Swar.API.Models.DTOs;
using Swar.API.Models.ENUMs;

namespace Swar.API.Services
{
    public class LikedSongsService : ILikedSongsService
    {
        private readonly IRepository<int, User> _userRepository;
        private readonly IRepository<int, LikedSong> _likedSongRepository;
        private readonly ILogger<LikedSongsService> _logger;

        public LikedSongsService(IRepository<int, User> userRepository, IRepository<int, LikedSong> likedSongRepository, ILogger<LikedSongsService> logger)
        {
            _userRepository = userRepository;
            _likedSongRepository = likedSongRepository;
            _logger = logger;
        }

        public async Task<LikedSongsReturnDTO> AddSongToLikedSongs(int userId, string songId)
        {
            var user = await _userRepository.GetById(userId) ?? throw new EntityNotFoundException("User not found.");

            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
            {
                _logger.LogInformation($"User {userId} tried to like a song but the account is inactive.");
                throw new InactiveAccountException();
            }

            if (await IsSongLikedByUser(userId, songId))
            {
                _logger.LogInformation($"User {userId} tried to like a song but the song is already liked.");
                throw new EntityAlreadyExistsException("You have already liked this song.");
            }

            var likedSong = new LikedSong
            {
                UserId = userId,
                SongId = songId,
                LikedDate = DateTime.UtcNow
            };

            await _likedSongRepository.Add(likedSong);

            _logger.LogInformation($"User {userId} liked song {songId}");
            return MapLikedSongToDTO(likedSong);
        }

        public async Task<LikedSongsReturnDTO> RemoveSongFromLikedSongs(int userId, string songId)
        {
            var likedSong = await GetLikedSong(userId, songId);

            if (likedSong == null)
            {
                _logger.LogInformation($"User {userId} tried to remove a song from liked songs but the song is not liked.");
                throw new EntityNotFoundException("Liked song not found.");
            }

            await _likedSongRepository.Delete(likedSong.LikeId);

            _logger.LogInformation($"User {userId} removed song {songId} from liked songs.");
            return MapLikedSongToDTO(likedSong);
        }

        public async Task<SongsListDTO> GetAllLikedSongs(int userId)
        {
            User user = await _userRepository.GetById(userId)
                ?? throw new EntityNotFoundException("User not found.");

            var likedSongs = await _likedSongRepository.GetAll();
            var userLikedSongs = likedSongs.Where(ls => ls.UserId == userId).ToList();

            var songIds = userLikedSongs.Select(ls => ls.SongId).ToList();

            _logger.LogInformation($"Returning all liked songs for user {userId}.");
            return new SongsListDTO
            {
                UserId = userId,
                songsCount = songIds.Count,
                Songs = songIds
            };
        }

        public async Task<bool> IsSongLikedByUser(int userId, string songId)
        {
            var likedSong = await GetLikedSong(userId, songId);
            return likedSong != null;
        }

        private async Task<LikedSong?> GetLikedSong(int userId, string songId)
        {
            var likedSongs = await _likedSongRepository.GetAll();
            return likedSongs.FirstOrDefault(ls => ls.UserId == userId && ls.SongId == songId);
        }

        private LikedSongsReturnDTO MapLikedSongToDTO(LikedSong likedSong)
        {
            return new LikedSongsReturnDTO
            {
                UserId = likedSong.UserId,
                SongId = likedSong.SongId,
                LikedDate = likedSong.LikedDate
            };
        }
    }
}
