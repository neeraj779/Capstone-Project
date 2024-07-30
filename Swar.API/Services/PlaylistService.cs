﻿using Swar.API.Exceptions;
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

        public PlaylistService(IRepository<int, Playlist> playlistRepository, IRepository<int, User> userRepository)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
        }

        public async Task<ReturnPlaylistDTO> AddPlaylist(int userId, AddPlaylistDTO addPlaylistDTO)
        {
            User user = await _userRepository.GetById(userId);
            if (user.UserStatus != UserStatusEnum.UserStatus.Active)
            {
                throw new InactiveAccountException();
            }

            var userPlaylists = await _playlistRepository.GetAll();
            if (userPlaylists.Count(p => p.UserId == userId) >= 3)
                throw new MaxPlaylistLimitException();

            Playlist playlist = new Playlist
            {
                UserId = userId,
                PlaylistName = addPlaylistDTO.PlaylistName,
                Description = addPlaylistDTO.Description,
                IsPrivate = addPlaylistDTO.IsPrivate,
                CreatedAt = DateTime.Now
            };

            await _playlistRepository.Add(playlist);

            return MapPlaylistToReturnPlaylistDTO(playlist);

        }

        public async Task<ReturnPlaylistDTO> DeletePlaylist(int userId, int playlistId)
        {
            var playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
                throw new EntityNotFoundException();

            if (playlist.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this playlist.");

            await _playlistRepository.Delete(playlistId);
            return MapPlaylistToReturnPlaylistDTO(playlist);
        }


        public async Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylists()
        {
            var playlists = await _playlistRepository.GetAll();
            if (!playlists.Any())
                throw new EntityNotFoundException("No playlists found.");

            return playlists.Select(MapPlaylistToReturnPlaylistDTO).ToList();
        }

        public async Task<IEnumerable<ReturnPlaylistDTO>> GetAllPlaylistsByUserId(int userId)
        {
            var playlists = await _playlistRepository.GetAll();
            var userPlaylists = playlists.Where(p => p.UserId == userId);

            if (!userPlaylists.Any())
                throw new EntityNotFoundException($"No playlists found for user with ID {userId}.");

            return userPlaylists.Select(MapPlaylistToReturnPlaylistDTO).ToList();
        }

        public async Task<ReturnPlaylistDTO> GetPlaylistById(int userId, int playlistId)
        {
            Playlist playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
                throw new EntityNotFoundException($"Playlist with ID {playlistId} not found.");

            if (playlist.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to view this playlist.");

            return MapPlaylistToReturnPlaylistDTO(playlist);
        }

        public async Task<ReturnPlaylistDTO> UpdatePlaylist(int userId, int playlistId, UpdatePlaylistDTO updatePlaylistDTO)
        {
            Playlist playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
                throw new EntityNotFoundException($"Playlist with ID {playlistId} not found.");

            if (playlist.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this playlist.");

            playlist.PlaylistName = updatePlaylistDTO.PlaylistName;
            playlist.Description = updatePlaylistDTO.Description;

            await _playlistRepository.Update(playlist);
            return MapPlaylistToReturnPlaylistDTO(playlist);
        }

        public async Task<ReturnPlaylistDTO> UpdatePlaylistPrivacy(int userId, int playlistId, bool IsPrivate)
        {
            Playlist playlist = await _playlistRepository.GetById(playlistId);
            if (playlist == null)
                throw new EntityNotFoundException($"Playlist with ID {playlistId} not found.");

            if (playlist.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this playlist.");

            playlist.IsPrivate = IsPrivate;
            await _playlistRepository.Update(playlist);
            return MapPlaylistToReturnPlaylistDTO(playlist);
        }

        public ReturnPlaylistDTO MapPlaylistToReturnPlaylistDTO(Playlist playlist)
        {
            return new ReturnPlaylistDTO
            {
                UserId = playlist.UserId,
                PlaylistId = playlist.PlaylistId,
                PlaylistName = playlist.PlaylistName,
                Description = playlist.Description,
                IsPrivate = playlist.IsPrivate,
                CreatedAt = playlist.CreatedAt
            };
        }
    }
}