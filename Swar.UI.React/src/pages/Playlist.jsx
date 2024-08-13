import { useParams } from "react-router-dom";
import { Image } from "@nextui-org/react";
import useUserplaylistSongs from "../hooks/useUserPlaylistSongs";
import PlaylistsSkeleton from "../components/PlaylistsSkeleton";
import playlistSvg from "../assets/img/playlist.svg";
import ErrorMessage from "../components/Error/ErrorMessage";
import { Button } from "@nextui-org/react";
import { IoMdMore, IoMdList, IoMdGlobe, IoMdCalendar } from "react-icons/io";

const Playlist = () => {
  const { id } = useParams();
  const { playlistInfo, playlistSongs, loading, error } =
    useUserplaylistSongs(id);

  const firstSong = playlistSongs[0] || {};

  if (error) {
    return <ErrorMessage statusCode={error.status} />;
  }

  return (
    <div className="mx-auto px-4 sm:px-6 lg:px-8 py-12">
      {loading ? (
        <PlaylistsSkeleton />
      ) : (
        <>
          <div className="bg-gradient-to-br from-gray-800 to-gray-900 p-6 sm:p-8 rounded-2xl shadow-lg border border-gray-700 mb-8 flex flex-row items-start items-center">
            <div className="flex-shrink-0 mr-6">
              {firstSong.image ? (
                <Image
                  isBlurred
                  src={firstSong.image || playlistSvg}
                  alt="First Song"
                  className="w-32 h-32"
                />
              ) : (
                <div className="w-full h-auto max-w-xs bg-gray-700 rounded-md border border-gray-700" />
              )}
            </div>
            <div className="flex-grow">
              <h2 className="text-2xl sm:text-3xl font-bold text-white mb-2">
                {`${playlistInfo?.playlistName || "N/A"} ${
                  id ? `• ${playlistInfo?.ownerName || "N/A"}` : ""
                }`}
              </h2>
              <p className="text-gray-300 mb-2">
                {playlistInfo?.description || "N/A"}
              </p>
              <div className="text-gray-300 mb-2 flex flex-col sm:flex-row sm:items-center">
                <div className="flex items-center mb-2 sm:mb-0 sm:mr-6">
                  <IoMdGlobe className="text-white text-xl mr-1" />
                  <p>{playlistInfo?.isPrivate ? "Private" : "Public"}</p>
                </div>
                <div className="flex items-center mb-2 sm:mb-0 sm:mr-6">
                  <IoMdCalendar className="text-white text-xl mr-1" />
                  <p>
                    {playlistInfo?.createdAt
                      ? new Date(playlistInfo?.createdAt).toLocaleDateString()
                      : "N/A"}
                  </p>
                </div>
                <div className="flex items-center">
                  <IoMdList className="text-white text-xl mr-1" />
                  <p>Total Tracks: {playlistInfo?.songsCount || "0"}</p>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-gradient-to-br from-gray-800 to-gray-900 p-6 sm:p-8 rounded-2xl shadow-lg border border-gray-700">
            <h2 className="text-2xl sm:text-3xl font-bold text-white mb-6">
              Playlist Songs
            </h2>
            <div>
              {playlistSongs.length ? (
                playlistSongs.map((song) => (
                  <div
                    key={song.id}
                    className="p-2 flex items-start items-center"
                  >
                    <a
                      href={`songPlayer.html?id=${song.id}`}
                      className="flex-shrink-0"
                    >
                      <Image
                        isBlurred
                        radius="sm"
                        src={song.image || playlistSvg}
                        alt="Album Cover"
                        className="w-12 h-12"
                      />
                    </a>
                    <div className="ml-4 flex-grow flex flex-col justify-center overflow-hidden mr-6">
                      <a href={`songPlayer.html?id=${song.id}`}>
                        <h3 className="text-base sm:text-lg font-semibold text-white mb-1 truncate">
                          {song.song}
                        </h3>
                      </a>
                      <p className="text-xs sm:text-sm text-gray-400 truncate mb-1">
                        {song.primary_artists}
                      </p>
                    </div>
                    <Button
                      isIconOnly
                      className="bg-transparent"
                      aria-label="More options"
                    >
                      <IoMdMore className="text-white text-2xl" />
                    </Button>
                  </div>
                ))
              ) : (
                <p className="text-gray-400">
                  No songs found in this playlist.
                </p>
              )}
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default Playlist;
