import { useState, useEffect, useCallback } from "react";
import PlaylistsSkeleton from "../components/PlaylistsSkeleton";
import PlaylistCard from "../components/PlaylistCard";
import useApiClient from "../hooks/useApiClient";

const Library = () => {
  const swarApiClient = useApiClient();
  const [data, setData] = useState({
    playlists: [],
    loading: true,
    error: null,
  });

  const fetchPlaylists = useCallback(async () => {
    try {
      const response = await swarApiClient.get(
        "Playlist/GetAllPlaylistsByUserId"
      );
      setData({
        playlists: response.data || [],
        loading: false,
        error: null,
      });
    } catch (error) {
      setData({
        playlists: [],
        loading: false,
        error: error.message || "An error occurred while fetching playlists.",
      });
    }
  }, [swarApiClient]);

  useEffect(() => {
    fetchPlaylists();
  }, [fetchPlaylists]);

  const { playlists, loading, error } = data;

  const defaultPlaylist = {
    playlistId: 0,
    playlistName: "Liked Songs",
    description: "Your favorite tracks all in one place.",
  };

  return (
    <div className="mx-auto px-6 py-12">
      <div className="flex flex-col gap-8">
        <div className="bg-gradient-to-br from-gray-800 to-gray-900 p-8 rounded-2xl shadow-lg border border-gray-700 mb-8">
          {loading ? (
            <PlaylistsSkeleton />
          ) : error ? (
            <div className="text-red-500">{`Error: ${error}`}</div>
          ) : (
            <>
              <h2 className="text-3xl font-bold text-white mb-6">
                Your Playlists
              </h2>
              <div className="space-y-4">
                <PlaylistCard
                  playlist={defaultPlaylist}
                  onUpdate={fetchPlaylists}
                />
                {playlists.length &&
                  playlists.map((playlist) => (
                    <PlaylistCard
                      key={playlist.playlistId}
                      playlist={playlist}
                      onUpdate={fetchPlaylists}
                    />
                  ))}
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default Library;
