import { useState, useEffect, useCallback } from "react";
import useApiClient from "../hooks/useApiClient";
import toast from "react-hot-toast";

const useRecentlyPlayedSongs = () => {
  const swarApiClient = useApiClient();
  const songApiClient = useApiClient(true);
  const [recentlyPlayed, setRecentlyPlayed] = useState([]);
  const [loading, setLoading] = useState(true);

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
    const fetchRecentlyPlayedSongs = async () => {
      try {
        const { data } = await swarApiClient.get(
          "PlayHistory/GetSongHistoryByUser?=true"
        );

        const songs = data?.songs || [];
        if (songs.length === 0) return;

        const songsToFetch = songs.slice(0, 10);
        const fetchPromises = songsToFetch.map((songId) => fetchSong(songId));
        const result = await Promise.all(fetchPromises);
        setRecentlyPlayed(result.filter(Boolean));
      } catch {
        toast.error("Failed to fetch recently played songs");
        setRecentlyPlayed([]);
      } finally {
        setLoading(false);
      }
    };

    fetchRecentlyPlayedSongs();
  }, [swarApiClient, fetchSong]);

  return { recentlyPlayed, loading };
};

export default useRecentlyPlayedSongs;
