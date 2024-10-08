import { useState, useEffect, useCallback } from "react";
import useApiClient from "../hooks/useApiClient";

const useUserPlaylistSongs = (playlistId) => {
  const swarApiClient = useApiClient();
  const songApiClient = useApiClient(true);

  const [state, setState] = useState({
    playlistInfo: {},
    playlistSongs: [],
    loading: true,
    error: null,
  });

  const fetchSong = useCallback(
    async (songId) => {
      try {
        const cache = await caches.open("song-cache");
        const cachedResponse = await cache.match(songId);
        if (cachedResponse) {
          return await cachedResponse.json();
        }

        const { data: songData } = await songApiClient(
          `SongsData/GetSongById?id=${songId}&lyrics=false`
        );
        await cache.put(songId, new Response(JSON.stringify(songData)));
        return songData;
      } catch {
        return null;
      }
    },
    [songApiClient]
  );

  useEffect(() => {
    const fetchPlaylistSongs = async () => {
      try {
        const isLikedPlaylist = playlistId === "liked";
        const url = isLikedPlaylist
          ? "LikedSongs/GetLikedSongs"
          : `PlaylistSongs/GetAllSongsInUserPlaylist/${playlistId}`;

        const { data } = await swarApiClient.get(url);

        const playlistInfo = isLikedPlaylist
          ? {
              playlistId: 0,
              playlistName: "Liked Songs",
              songsCount: data?.songsCount || 0,
              isPrivate: true,
            }
          : data?.playlistInfo || {};

        const songs = data?.songs || [];

        const fetchedSongs =
          songs.length > 0 ? await Promise.all(songs.map(fetchSong)) : [];

        setState((prevState) => ({
          ...prevState,
          playlistInfo,
          playlistSongs: fetchedSongs.filter(Boolean),
        }));
      } catch (ex) {
        setState({
          playlistInfo: {},
          playlistSongs: [],
          error: {
            message: ex.response?.data?.message || ex.message,
            status: ex.response?.status || 500,
          },
        });
      } finally {
        setState((prev) => ({ ...prev, loading: false }));
      }
    };

    fetchPlaylistSongs();
  }, [swarApiClient, fetchSong, playlistId]);

  return {
    playlistInfo: state.playlistInfo,
    playlistSongs: state.playlistSongs,
    loading: state.loading,
    error: state.error,
  };
};

export default useUserPlaylistSongs;
