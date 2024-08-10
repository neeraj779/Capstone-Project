import { useState, useEffect, useCallback } from "react";
import useApiClient from "../hooks/useApiClient";

const useSearchData = (query) => {
  const [searchResults, setSearchResults] = useState([]);
  const [artists, setArtists] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const songApiClient = useApiClient(true);

  const fetchSearchData = useCallback(async () => {
    if (!query) {
      setError({ message: "No search query provided.", status: 404 });
      return;
    }

    setLoading(true);

    try {
      const { data: searchData } = await songApiClient.get(
        `SongsData/GetSongsByQuery?query=${query}&lyrics=false&songData=true`
      );

      const artistSet = new Set(
        searchData.flatMap((song) => song.primary_artists)
      );
      setArtists(Array.from(artistSet));
      setSearchResults(searchData);
    } catch (ex) {
      setError({
        message:
          ex.response?.status === 404
            ? "No results found, try a different search term."
            : "Oops! Something went wrong.",
        status: ex.response?.status || 500,
      });
    } finally {
      setLoading(false);
    }
  }, [query, songApiClient]);

  useEffect(() => {
    fetchSearchData();
  }, [fetchSearchData]);

  return { searchResults, artists, loading, error };
};

export default useSearchData;
