import { useState, useEffect } from "react";
import { fetchAll } from "../services/CRUDService";

const CACHE_NAME = "playlist-cache";
const ENDPOINT = "SongsData/GetPlaylistById?listId=1220338282&lyrics=false";

const usePlaylistSongs = () => {
  const [playlistSongs, setPlaylistSongs] = useState({
    trending: [],
    relaxing: [],
    romance: [],
    lofi: [],
    history: [],
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPlaylistSongs = async () => {
      const cache = await caches.open(CACHE_NAME);

      const cachedResponse = await cache.match(ENDPOINT);
      if (cachedResponse) {
        const cachedData = await cachedResponse.json();
        processAndSetPlaylistSongs(cachedData);
        setLoading(false);
        return;
      }

      try {
        const playlistData = await fetchAll(ENDPOINT, true);

        const clonedResponse = new Response(JSON.stringify(playlistData));
        await cache.put(ENDPOINT, clonedResponse);

        processAndSetPlaylistSongs(playlistData);
      } catch (error) {
        console.error("Error fetching playlist songs:", error);
      } finally {
        setLoading(false);
      }
    };

    const processAndSetPlaylistSongs = (playlistData) => {
      const { content_list, songs } = playlistData;
      const songMap = new Map(songs.map((song) => [song.id, song]));

      const getCategorySongs = (start, end) =>
        content_list.slice(start, end).map((id) => songMap.get(id) || {});

      const categories = {
        trending: getCategorySongs(0, 5),
        relaxing: getCategorySongs(5, 10),
        romance: getCategorySongs(10, 15),
        lofi: getCategorySongs(15, 20),
      };

      setPlaylistSongs(categories);
    };

    fetchPlaylistSongs();
  }, []);

  return { playlistSongs, loading };
};

export default usePlaylistSongs;
