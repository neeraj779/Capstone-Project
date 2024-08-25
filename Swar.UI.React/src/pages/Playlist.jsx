import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Image,
  Dropdown,
  DropdownTrigger,
  DropdownMenu,
  DropdownItem,
  Button,
  cn,
} from "@nextui-org/react";
import { DeleteIcon } from "../components/Icons";

import useUserplaylistSongs from "../hooks/useUserPlaylistSongs";
import usePlaylistActionsBase from "../hooks/usePlaylistActionsBase";
import PlaylistsSkeleton from "../components/PlaylistsSkeleton";
import playlistSvg from "../assets/img/playlist.svg";
import ErrorMessage from "../components/Error/ErrorMessage";
import { IoMdMore, IoMdList, IoMdGlobe, IoMdCalendar } from "react-icons/io";
import toast from "react-hot-toast";

const Playlist = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { playlistInfo, playlistSongs, loading, error } =
    useUserplaylistSongs(id);
  const { handleRemoveFromPlaylist } = usePlaylistActionsBase();
  const [songs, setSongs] = useState(playlistSongs);

  useEffect(() => {
    setSongs(playlistSongs);
  }, [playlistSongs]);

  const handleSongClick = (songId) =>
    navigate("/player", { state: { songId } });

  const handleRemoveSong = async (songId) => {
    const updatedSongs = songs.filter((song) => song.id !== songId);
    setSongs(updatedSongs);
    playlistInfo.songsCount -= 1;
    toast.success("Song removed from playlist successfully.");

    try {
      await handleRemoveFromPlaylist(playlistInfo.playlistId, songId);
    } catch {
      setSongs(playlistSongs);
      playlistInfo.songsCount += 1;
    }
  };

  const iconClasses =
    "text-xl text-default-500 pointer-events-none flex-shrink-0";

  if (error) {
    return <ErrorMessage statusCode={error.status} />;
  }

  return (
    <div className="px-4 py-12 mx-auto sm:px-6 lg:px-8">
      {loading ? (
        <PlaylistsSkeleton />
      ) : (
        <>
          <div className="flex flex-row items-start p-6 mb-8 border border-gray-700 shadow-lg bg-gradient-to-br from-gray-800 to-gray-900 rounded-2xl">
            <div className="flex-shrink-0 mr-6">
              {playlistSongs.length >= 4 ? (
                <div className="grid w-32 h-32 grid-cols-2 grid-rows-2 overflow-hidden rounded-xl">
                  {playlistSongs.slice(0, 4).map((song, index) => (
                    <Image
                      key={index}
                      radius="none"
                      isBlurred
                      src={song?.image || playlistSvg}
                      alt={`Song ${index + 1}`}
                    />
                  ))}
                </div>
              ) : (
                <Image
                  isBlurred
                  src={playlistSongs[0]?.image || playlistSvg}
                  alt="playlist"
                  className="w-32 h-32"
                />
              )}
            </div>
            <div className="flex-grow overflow-hidden">
              <div className="mb-2">
                <p className="text-lg font-bold truncate">
                  {playlistInfo?.playlistName}
                </p>
                {id && (
                  <p className="text-lg text-gray-300 truncate">
                    {playlistInfo?.ownerName}{" "}
                    {`${
                      playlistInfo?.description
                        ? ` • ${playlistInfo?.description}`
                        : ""
                    }`}
                  </p>
                )}
              </div>
              <div className="flex flex-col mb-2 text-gray-300 sm:flex-row sm:items-center">
                <div className="flex items-center mb-2 sm:mb-0 sm:mr-6">
                  <IoMdGlobe className="mr-1 text-xl text-white" />
                  <p>{playlistInfo?.isPrivate ? "Private" : "Public"}</p>
                </div>
                {playlistInfo?.createdAt && (
                  <div className="flex items-center mb-2 sm:mb-0 sm:mr-6">
                    <IoMdCalendar className="mr-1 text-xl text-white" />
                    <p>
                      {new Date(playlistInfo?.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                )}
                <div className="flex items-center">
                  <IoMdList className="mr-1 text-xl text-white" />
                  <p>Total Tracks: {playlistInfo?.songsCount || "0"}</p>
                </div>
              </div>
            </div>
          </div>

          <div className="p-6 border border-gray-700 shadow-lg bg-gradient-to-br from-gray-800 to-gray-900 sm:p-8 rounded-2xl">
            <h2 className="mb-6 text-2xl font-bold text-white sm:text-3xl">
              Playlist Songs
            </h2>
            <div>
              {songs.length ? (
                songs.map((song) => (
                  <div
                    key={song.id}
                    className="flex items-start p-2"
                    onClick={() => handleSongClick(song.id)}
                  >
                    <div className="flex items-center flex-grow min-w-0">
                      <Image
                        isBlurred
                        radius="sm"
                        src={song.image || playlistSvg}
                        alt="Album Cover"
                        className="w-12 h-12 min-w-12"
                      />
                      <div className="flex flex-col justify-center flex-grow ml-4 mr-6 overflow-hidden">
                        <h3 className="mb-1 text-base font-semibold text-white truncate sm:text-lg">
                          {song.song}
                        </h3>
                        <p className="mb-1 text-xs text-gray-400 truncate sm:text-sm">
                          {song.primary_artists}
                        </p>
                      </div>
                    </div>
                    <Dropdown className="text-white bg-gray-800">
                      <DropdownTrigger>
                        <Button
                          isIconOnly
                          className="bg-transparent"
                          aria-label="More options"
                        >
                          <IoMdMore className="text-2xl text-white" />
                        </Button>
                      </DropdownTrigger>
                      <DropdownMenu
                        variant="faded"
                        aria-label="Playlist options"
                      >
                        <DropdownItem
                          key="delete"
                          className="text-danger"
                          color="danger"
                          startContent={
                            <DeleteIcon
                              className={cn(iconClasses, "text-danger")}
                            />
                          }
                          onClick={() => handleRemoveSong(song.id)}
                        >
                          Remove
                        </DropdownItem>
                      </DropdownMenu>
                    </Dropdown>
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
