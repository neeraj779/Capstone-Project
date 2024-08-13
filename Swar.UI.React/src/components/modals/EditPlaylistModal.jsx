import {
  Input,
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
} from "@nextui-org/react";
import PropTypes from "prop-types";

const EditPlaylistModal = ({
  isOpen,
  onClose,
  playlistName,
  setPlaylistName,
  playlistDescription,
  setPlaylistDescription,
  handleEditPlaylist,
  isEditing,
}) => (
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
      <ModalHeader>Edit Playlist</ModalHeader>
      <ModalBody>
        <Input
          autoFocus
          value={playlistName}
          onChange={(e) => setPlaylistName(e.target.value)}
          label="Playlist Name"
          placeholder="Enter playlist name"
          variant="bordered"
          classNames={{
            inputWrapper: ["border-gray-600", "focus-within:!border-white"],
          }}
        />
        <Input
          value={playlistDescription}
          onChange={(e) => setPlaylistDescription(e.target.value)}
          label="Description"
          placeholder="Enter description"
          variant="bordered"
          classNames={{
            inputWrapper: ["border-gray-600", "focus-within:!border-white"],
          }}
        />
      </ModalBody>
      <ModalFooter>
        <Button color="danger" variant="flat" onPress={onClose}>
          Cancel
        </Button>
        <Button
          color="primary"
          isLoading={isEditing}
          onPress={handleEditPlaylist}
        >
          Save
        </Button>
      </ModalFooter>
    </ModalContent>
  </Modal>
);

EditPlaylistModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  playlistName: PropTypes.string.isRequired,
  setPlaylistName: PropTypes.func.isRequired,
  playlistDescription: PropTypes.string.isRequired,
  setPlaylistDescription: PropTypes.func.isRequired,
  handleEditPlaylist: PropTypes.func.isRequired,
  isEditing: PropTypes.bool.isRequired,
};

export default EditPlaylistModal;
