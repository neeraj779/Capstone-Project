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

        public LikedSongsService(IRepository<int, User> userRepository, IRepository<int, LikedSong> likedSongRepository)
        {
            _userRepository = userRepository;
            _likedSongRepository = likedSongRepository;
        }

        public async Task<LikedSongsReturnDTO> AddSongToLikedSongs(int userId, string songId)
        {
            var user = await _userRepository.GetById(userId) ?? throw new EntityNotFoundException("User not found.");

            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
            {
                throw new InactiveAccountException();
            }

            if (await IsSongLikedByUser(userId, songId))
            {
                throw new EntityAlreadyExistsException("You have already liked this song.");
            }

            var likedSong = new LikedSong
            {
                UserId = userId,
                SongId = songId,
                LikedDate = DateTime.UtcNow
            };

            await _likedSongRepository.Add(likedSong);

            return MapLikedSongToDTO(likedSong);
        }

        public async Task<LikedSongsReturnDTO> RemoveSongFromLikedSongs(int userId, string songId)
        {
            var likedSong = await GetLikedSong(userId, songId);

            if (likedSong == null)
            {
                throw new EntityNotFoundException("Liked song not found.");
            }

            await _likedSongRepository.Delete(likedSong.LikeId);

            return MapLikedSongToDTO(likedSong);
        }

        public async Task<SongsListDTO> GetAllLikedSongs(int userId)
        {
            var likedSongs = await _likedSongRepository.GetAll();
            var userLikedSongs = likedSongs.Where(ls => ls.UserId == userId).ToList();

            if (!userLikedSongs.Any())
            {
                throw new EntityNotFoundException("No liked songs found.");
            }

            var songIds = userLikedSongs.Select(ls => ls.SongId).ToList();

            return new SongsListDTO
            {
                UserId = userId,
                Songs = songIds
            };
        }

        public async Task<bool> IsSongLikedByUser(int userId, string songId)
        {
            var likedSong = await GetLikedSong(userId, songId);
            return likedSong != null;
        }

        private async Task<LikedSong> GetLikedSong(int userId, string songId)
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
