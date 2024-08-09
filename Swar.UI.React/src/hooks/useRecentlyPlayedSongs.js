import { useState, useEffect } from "react";
import { fetchAll } from "../services/CRUDService";

const useRecentlyPlayedSongs = () => {
  const [recentlyPlayed, setRecentlyPlayed] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchRecentlyPlayedSongId = async () => {
      try {
        const { songs } = await fetchAll(
          `PlayHistory/GetSongHistoryByUser?=true`,
          false
        );
        if (!songs || songs.length === 0) return [];

        const songsToFetch = songs.slice(0, 10);
        const fetchPromises = songsToFetch.map((songId) => fetchSong(songId));
        const data = await Promise.all(fetchPromises);
        setRecentlyPlayed(data.filter((songData) => songData != null));
      } catch (error) {
        console.error("Error fetching recently played songs:", error);
        setRecentlyPlayed([]);
      } finally {
        setLoading(false);
      }
    };

    const fetchSong = async (songId) => {
      const SONG_CACHE_NAME = "songCache";

      try {
        const cache = await caches.open(SONG_CACHE_NAME);
        const cachedResponse = await cache.match(songId);
        if (cachedResponse) {
          return await cachedResponse.json();
        }

        const songData = await fetchAll(
          `SongsData/GetSongById?id=${songId}&lyrics=false`,
          true
        );
        await cache.put(songId, new Response(JSON.stringify(songData)));
        return songData;
      } catch (error) {
        console.error("Error fetching song:", error);
        return null;
      }
    };

    fetchRecentlyPlayedSongId();
  }, []);

  return { recentlyPlayed, loading };
};

export default useRecentlyPlayedSongs;
