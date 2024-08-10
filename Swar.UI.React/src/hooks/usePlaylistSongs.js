import { useState, useEffect, useCallback } from "react";
import useApiClient from "../hooks/useApiClient";

const ENDPOINT = "SongsData/GetPlaylistById?listId=1220338282&lyrics=false";

const usePlaylistSongs = () => {
  const songApiClient = useApiClient(true);
  const [playlistSongs, setPlaylistSongs] = useState({
    trending: [],
    relaxing: [],
    romance: [],
    lofi: [],
    history: [],
  });
  const [loading, setLoading] = useState(true);

  const cacheName = "playlist-cache";

  const fetchFromCache = useCallback(async () => {
    const cache = await caches.open(cacheName);
    const cachedResponse = await cache.match(ENDPOINT);
    if (cachedResponse) {
      return await cachedResponse.json();
    }
    return null;
  }, []);

  const fetchFromApi = useCallback(async () => {
    try {
      const { data: playlistData } = await songApiClient(ENDPOINT);
      const cache = await caches.open(cacheName);
      const clonedResponse = new Response(JSON.stringify(playlistData));
      await cache.put(ENDPOINT, clonedResponse);
      return playlistData;
    } catch (error) {
      console.error("Error fetching playlist songs:", error);
      throw error;
    }
  }, [songApiClient]);

  const processAndSetPlaylistSongs = useCallback((playlistData) => {
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
  }, []);

  useEffect(() => {
    const fetchPlaylistSongs = async () => {
      try {
        const cachedData = await fetchFromCache();
        if (cachedData) {
          processAndSetPlaylistSongs(cachedData);
        } else {
          const playlistData = await fetchFromApi();
          processAndSetPlaylistSongs(playlistData);
        }
      } catch {
        setPlaylistSongs({
          trending: [],
          relaxing: [],
          romance: [],
          lofi: [],
          history: [],
        });
      } finally {
        setLoading(false);
      }
    };

    fetchPlaylistSongs();
  }, [fetchFromCache, fetchFromApi, processAndSetPlaylistSongs]);

  return { playlistSongs, loading };
};

export default usePlaylistSongs;
