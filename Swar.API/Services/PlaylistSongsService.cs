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

        public PlaylistSongsService(IPlaylistSongsRepository playlistSongRepository, IRepository<int, Playlist> playlistRepository, IRepository<int, User> userRepository)
        {
            _playlistSongRepository = playlistSongRepository;
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
        }


        public async Task<PlaylistSongsReturnDTO> AddSongToPlaylist(int userId, AddSongToPlaylistDTO addSongToPlaylistDTO)
        {
            await ValidateUserAndPlaylist(userId, addSongToPlaylistDTO.PlaylistId);

            var playlistSongExists = await _playlistSongRepository.GetByCompositeKey(addSongToPlaylistDTO.PlaylistId, addSongToPlaylistDTO.SongId);

            if (playlistSongExists != null)
                throw new EntityAlreadyExistsException("Song already exists in playlist.");

            PlaylistSong playlistSong = new PlaylistSong
            {
                PlaylistId = addSongToPlaylistDTO.PlaylistId,
                SongId = addSongToPlaylistDTO.SongId
            };

            await _playlistSongRepository.Add(playlistSong);
            return MapPlaylistSongToReturnPlaylistSongDTO(playlistSong);
        }

        public async Task<IEnumerable<PlaylistSongsReturnDTO>> GetAllSongsInPlaylist()
        {
            var playlistSongs = await _playlistSongRepository.GetAll();
            if (!playlistSongs.Any())
                throw new EntityNotFoundException();

            return playlistSongs.Select(MapPlaylistSongToReturnPlaylistSongDTO);
        }

        public async Task<IEnumerable<PlaylistSongsReturnDTO>> GetAllSongsInUserPlaylist(int userId, int playlistId)
        {
            await ValidateUserAndPlaylist(userId, playlistId);

            var playlistSongs = await _playlistSongRepository.GetAll();
            if (!playlistSongs.Any())
                throw new EntityNotFoundException("No songs found in playlist.");

            var userPlaylistSongs = playlistSongs.Where(ps => ps.PlaylistId == playlistId);
            return userPlaylistSongs.Select(MapPlaylistSongToReturnPlaylistSongDTO);
        }

        public async Task<PlaylistSongsReturnDTO> RemoveSongFromPlaylist(int userId, int playlistId, string songId)
        {
            await ValidateUserAndPlaylist(userId, playlistId);

            var playlistSong = await _playlistSongRepository.GetByCompositeKey(playlistId, songId);
            if (playlistSong == null)
                throw new EntityNotFoundException("Playlist song not found.");

            await _playlistSongRepository.DeletePlaylistSong(playlistId, songId);
            return MapPlaylistSongToReturnPlaylistSongDTO(playlistSong);
        }

        private async Task ValidateUserAndPlaylist(int userId, int playlistId)
        {
            User user = await _userRepository.GetById(userId);
            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
            {
                throw new InactiveAccountException();
            }

            var playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
            {
                throw new EntityNotFoundException("Playlist not found.");
            }

            if (playlist.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }
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
