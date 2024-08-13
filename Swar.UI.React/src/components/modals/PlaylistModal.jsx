import {
  Input,
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  Checkbox,
} from "@nextui-org/react";
import PropTypes from "prop-types";
import { useState, useCallback } from "react";

const PlaylistModal = ({
  isOpen,
  onClose,
  isEditMode,
  modalTitle,
  buttonTitle,
  playlistName,
  setPlaylistName,
  playlistDescription,
  setPlaylistDescription,
  handlePlaylistOperation,
  isEditing,
}) => {
  const [isInvalid, setIsInvalid] = useState(false);

  const handleNameChange = useCallback(
    (e) => {
      const name = e.target.value.trim();
      setPlaylistName(name);
      if (isInvalid && name) {
        setIsInvalid(false);
      }
    },
    [isInvalid, setPlaylistName]
  );

  const handleSubmit = useCallback(() => {
    if (!playlistName.trim()) {
      setIsInvalid(true);
    } else {
      handlePlaylistOperation();
    }
  }, [playlistName, handlePlaylistOperation]);

  return (
    <Modal
      isOpen={isOpen}
      placement="center"
      onOpenChange={onClose}
      classNames={{
        body: "py-6",
        backdrop: "blur",
        base: "bg-gradient-to-br from-gray-800 to-gray-900 p-2 shadow-2xl text-white",
        header: "border-b-[1px] border-[#292f46]",
        footer: "border-t-[1px] border-[#292f46]",
        closeButton: "hover:bg-white/5 active:bg-white/10",
      }}
    >
      <ModalContent>
        <ModalHeader>{modalTitle}</ModalHeader>
        <ModalBody>
          <Input
            autoFocus
            isRequired
            value={playlistName}
            onChange={handleNameChange}
            label="Playlist Name"
            placeholder="Enter playlist name"
            variant="bordered"
            isInvalid={isInvalid}
            errorMessage={"Playlist name is required."}
            classNames={{
              inputWrapper: "border-gray-600 focus-within:!border-white",
            }}
          />
          <Input
            value={playlistDescription}
            onChange={(e) => setPlaylistDescription(e.target.value)}
            label="Description"
            placeholder="Enter description"
            variant="bordered"
            classNames={{
              inputWrapper: "border-gray-600 focus-within:!border-white",
            }}
          />
          {!isEditMode && (
            <Checkbox>
              <span className="text-white">Make Private</span>
            </Checkbox>
          )}
        </ModalBody>
        <ModalFooter>
          <Button color="danger" variant="flat" onPress={onClose}>
            Cancel
          </Button>
          <Button color="primary" isLoading={isEditing} onPress={handleSubmit}>
            {buttonTitle}
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
};

PlaylistModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  isEditMode: PropTypes.bool.isRequired,
  modalTitle: PropTypes.string.isRequired,
  buttonTitle: PropTypes.string.isRequired,
  playlistName: PropTypes.string.isRequired,
  setPlaylistName: PropTypes.func.isRequired,
  playlistDescription: PropTypes.string.isRequired,
  setPlaylistDescription: PropTypes.func.isRequired,
  handlePlaylistOperation: PropTypes.func.isRequired,
  isEditing: PropTypes.bool.isRequired,
};

export default PlaylistModal;
