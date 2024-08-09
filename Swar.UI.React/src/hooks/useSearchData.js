import { useState, useEffect, useCallback } from "react";
import { fetchAll } from "../services/CRUDService";

const useSearchData = (query) => {
  const [searchResults, setSearchResults] = useState([]);
  const [artists, setArtists] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchSearchData = useCallback(async () => {
    if (!query) return;

    setLoading(true);

    try {
      const searchData = await fetchAll(
        `SongsData/GetSongsByQuery?query=${query}&lyrics=false&songData=true`,
        true
      );

      const artistSet = new Set(
        searchData.flatMap((song) => song.primary_artists)
      );
      setArtists(Array.from(artistSet));
      setSearchResults(searchData);
    } catch (ex) {
      console.error("Error fetching search data:", ex);
      setError({
        message:
          ex?.status === 404
            ? "No results found, try a different search term."
            : "Oops! Something went wrong.",
        status: ex?.status || 500,
      });
    } finally {
      setLoading(false);
    }
  }, [query]);

  useEffect(() => {
    fetchSearchData();
  }, [fetchSearchData]);

  return { searchResults, artists, loading, error };
};

export default useSearchData;
