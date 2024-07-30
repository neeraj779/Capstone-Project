using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using Swar.API.Models.DTOs;
using Swar.API.Models.ENUMs;

namespace Swar.API.Services
{
    public class PlayHistoryService : IPlayHistoryService
    {
        private readonly IRepository<int, PlayHistory> _playHistoryRepository;
        private readonly IRepository<int, User> _userRepository;

        public PlayHistoryService(IRepository<int, PlayHistory> playHistoryRepository, IRepository<int, User> userRepository)
        {
            _playHistoryRepository = playHistoryRepository;
            _userRepository = userRepository;
        }

        public async Task LogSongHistory(int userId, string songId)
        {
            User user = await _userRepository.GetById(userId);
            if (user == null)
                throw new EntityNotFoundException();

            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
                throw new InactiveAccountException();

            PlayHistory playHistory = new PlayHistory
            {
                UserId = userId,
                SongId = songId,
                PlayedAt = DateTime.Now
            };

            await _playHistoryRepository.Add(playHistory);
        }

        public async Task<SongsListDTO> GetSongHistory(int userId, bool unique)
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
                throw new EntityNotFoundException();

            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
                throw new InactiveAccountException();

            var playHistory = await _playHistoryRepository.GetAll();

            var userPlayHistory = playHistory
                .Where(p => p.UserId == userId)
                .ToList();

            if (unique)
            {
                userPlayHistory = userPlayHistory
                    .GroupBy(p => p.SongId)
                    .Select(g => g.OrderByDescending(p => p.PlayedAt).First())
                    .OrderByDescending(p => p.PlayedAt)
                    .ToList();
            }

            if (!userPlayHistory.Any())
                throw new EntityNotFoundException();

            return new SongsListDTO
            {
                UserId = userId,
                Songs = userPlayHistory.Select(p => p.SongId).ToList()
            };
        }

        public async Task<IEnumerable<SongsListDTO>> GetAllUserHistory()
        {
            var playHistory = await _playHistoryRepository.GetAll();
            if (!playHistory.Any())
                throw new EntityNotFoundException();

            List<SongsListDTO> songsListDTOs = new List<SongsListDTO>();
            var users = playHistory.Select(p => p.UserId).Distinct().ToList();
            foreach (var user in users)
            {
                var userPlayHistory = playHistory.Where(p => p.UserId == user).ToList();
                songsListDTOs.Add(new SongsListDTO
                {
                    UserId = user,
                    Songs = userPlayHistory.Select(p => p.SongId).ToList()
                });
            }

            return songsListDTOs;
        }
    }
}
