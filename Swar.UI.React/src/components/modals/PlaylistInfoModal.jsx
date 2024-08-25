import { useState, useCallback } from "react";
import PropTypes from "prop-types";
import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  useDisclosure,
  Select,
  SelectItem,
  Spinner,
} from "@nextui-org/react";
import usePlaylistInfo from "../../hooks/usePlaylistInfo";
import toast from "react-hot-toast";
import usePlaylistActionsBase from "../../hooks/usePlaylistActionsBase";
import { LuPlusCircle } from "react-icons/lu";

const PlaylistInfoModal = ({ songId }) => {
  const {
    playlistInfo,
    loading: playlistLoading,
    error: playlistError,
  } = usePlaylistInfo();
  const { handleAddToPlaylist } = usePlaylistActionsBase();
  const { isOpen, onOpen, onClose } = useDisclosure();

  const [selectedPlaylistId, setSelectedPlaylistId] = useState(null);
  const [isAdding, setIsAdding] = useState(false);

  const handleSubmit = useCallback(async () => {
    if (!selectedPlaylistId) {
      toast.error("Please select a playlist.");
      return;
    }

    setIsAdding(true);
    await handleAddToPlaylist(selectedPlaylistId, songId);
    toast.success("Song added to playlist successfully.");
    setIsAdding(false);
    onClose();
  }, [selectedPlaylistId, songId, handleAddToPlaylist, onClose]);

  return (
    <>
      <button onClick={onOpen}>
        <LuPlusCircle className="text-3xl text-white" />
      </button>
      <Modal isOpen={isOpen} onClose={onClose} placement="center">
        <ModalContent>
          <>
            <ModalHeader>Select Playlist</ModalHeader>
            <ModalBody>
              {playlistLoading ? (
                <div className="flex justify-center">
                  <Spinner size="lg" color="primary" />
                </div>
              ) : playlistError ? (
                <p className="text-red-500">
                  Failed to load playlists. Please try again later.
                </p>
              ) : playlistInfo.length === 0 ? (
                <p className="text-center text-gray-500">
                  You don&apos;t have any playlists. Please create one first.
                </p>
              ) : (
                <Select
                  isRequired
                  variant="filled"
                  color="success"
                  label="Select Playlist"
                  placeholder="Select a playlist"
                  description="Select a playlist to add the song to."
                  value={selectedPlaylistId}
                  onChange={(e) => setSelectedPlaylistId(e.target.value)}
                >
                  {playlistInfo.map((playlist) => (
                    <SelectItem
                      key={playlist.playlistId}
                      value={playlist.playlistId}
                    >
                      {playlist.playlistName}
                    </SelectItem>
                  ))}
                </Select>
              )}
            </ModalBody>
            <ModalFooter>
              <Button color="danger" variant="flat" onPress={onClose}>
                Close
              </Button>
              <Button
                color="primary"
                isDisabled={
                  !selectedPlaylistId || isAdding || playlistInfo.length === 0
                }
                isLoading={isAdding}
                onPress={handleSubmit}
              >
                Add
              </Button>
            </ModalFooter>
          </>
        </ModalContent>
      </Modal>
    </>
  );
};

PlaylistInfoModal.propTypes = {
  songId: PropTypes.string.isRequired,
};

export default PlaylistInfoModal;
