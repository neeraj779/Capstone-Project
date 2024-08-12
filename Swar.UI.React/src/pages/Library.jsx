import PropTypes from "prop-types";
import { useState, useEffect } from "react";
import PlaylistsSkeleton from "../components/PlaylistsSkeleton";
import playlistSvg from "../assets/img/playlist.svg";
import heartSvg from "../assets/img/heart.svg";
import moreBtn from "../assets/img/lib-btn.svg";
import useApiClient from "../hooks/useApiClient";

const PlaylistCard = ({ playlist }) => (
  <div className="bg-gray-800 p-6 rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 flex items-center">
    <img
      src={playlist.playlistId == 0 ? heartSvg : playlistSvg}
      alt="Playlist Thumbnail"
      className="w-16 h-16 object-cover rounded-md"
    />
    <div className="flex-grow flex flex-col ml-4 overflow-hidden">
      <h3 className="text-xl font-semibold text-white mb-1 truncate">
        {playlist.playlistName}
      </h3>
      <p className="text-gray-300 truncate">{playlist.description}</p>
    </div>
    <button
      className="ml-4 text-gray-300 hover:text-gray-100 focus:outline-none flex-shrink-0"
      aria-haspopup="true"
      aria-expanded="false"
    >
      <img src={moreBtn} alt="More Options" className="w-8 h-8" />
    </button>
  </div>
);

PlaylistCard.propTypes = {
  playlist: PropTypes.shape({
    playlistId: PropTypes.number.isRequired,
    playlistName: PropTypes.string.isRequired,
    description: PropTypes.string.isRequired,
  }).isRequired,
};

const Library = () => {
  const swarApiClient = useApiClient();
  const [data, setData] = useState({
    playlists: [],
    loading: true,
    error: null,
  });

  useEffect(() => {
    const fetchPlaylists = async () => {
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
    };

    fetchPlaylists();
  }, [swarApiClient]);

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
                <PlaylistCard playlist={defaultPlaylist} />
                {playlists.length &&
                  playlists.map((playlist) => (
                    <PlaylistCard
                      key={playlist.playlistId}
                      playlist={playlist}
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
