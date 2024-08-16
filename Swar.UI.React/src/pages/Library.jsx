import { useState, useEffect, useCallback } from "react";
import { Button, useDisclosure } from "@nextui-org/react";
import toast from "react-hot-toast";
import PlaylistModal from "../components/modals/PlaylistModal";
import PlaylistsSkeleton from "../components/PlaylistsSkeleton";
import PlaylistCard from "../components/PlaylistCard";
import useApiClient from "../hooks/useApiClient";
import { FaPlus } from "react-icons/fa";

const Library = () => {
  const swarApiClient = useApiClient();
  const [data, setData] = useState({
    playlists: [],
    loading: true,
    error: null,
  });
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [isAdding, setIsAdding] = useState(false);
  const [newPlaylist, setNewPlaylist] = useState({
    playlistName: "",
    description: "",
    isPrivate: false,
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
    publicId: "0",
    playlistName: "Liked Songs",
  };

  const handleCreate = async () => {
    setIsAdding(true);
    try {
      await swarApiClient.post("Playlist/CreatePlaylist", {
        playlistName: newPlaylist.playlistName,
        description: newPlaylist.description,
        isPrivate: newPlaylist.isPrivate,
      });
      fetchPlaylists();
      toast.success("Playlist created successfully.");
    } catch {
      toast.error("An error occurred while creating the playlist.");
    } finally {
      setIsAdding(false);
      onClose();
    }
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
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-3xl font-bold text-white">
                  Your Playlists
                </h2>
                <Button
                  isIconOnly
                  onClick={onOpen}
                  color="primary"
                  variant="shadow"
                >
                  <FaPlus className="text-white text-xl" />
                </Button>
              </div>
              <div className="space-y-4">
                <PlaylistCard
                  playlist={defaultPlaylist}
                  onUpdate={fetchPlaylists}
                />
                {playlists.length > 0 &&
                  playlists.map((playlist) => (
                    <PlaylistCard
                      key={playlist.playlistId}
                      playlist={playlist}
                      onUpdate={fetchPlaylists}
                    />
                  ))}
              </div>
              <PlaylistModal
                isOpen={isOpen}
                onClose={onClose}
                isEditMode={false}
                modalTitle="Create Playlist"
                buttonTitle="Add"
                playlistName={newPlaylist.playlistName}
                setPlaylistName={(name) =>
                  setNewPlaylist((prev) => ({ ...prev, playlistName: name }))
                }
                playlistDescription={newPlaylist.description}
                setPlaylistDescription={(desc) =>
                  setNewPlaylist((prev) => ({ ...prev, description: desc }))
                }
                isPrivate={newPlaylist.isPrivate}
                setIsPrivate={(isPrivate) =>
                  setNewPlaylist((prev) => ({ ...prev, isPrivate }))
                }
                handlePlaylistOperation={handleCreate}
                isEditing={isAdding}
              />
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default Library;
