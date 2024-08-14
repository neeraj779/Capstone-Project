import { useState } from "react";
import { useNavigate } from "react-router-dom";
import PropTypes from "prop-types";
import {
  Dropdown,
  DropdownTrigger,
  DropdownMenu,
  DropdownItem,
  Button,
  useDisclosure,
  cn,
} from "@nextui-org/react";
import PlaylistModal from "./modals/PlaylistModal";
import { CopyIcon, EditIcon, DeleteIcon } from "../components/Icons";
import { IoMdMore, IoMdLock, IoMdUnlock } from "react-icons/io";
import playlistSvg from "../assets/img/playlist.svg";
import heartSvg from "../assets/img/heart.svg";
import usePlaylistActions from "../hooks/usePlaylistActions";

const PlaylistCard = ({ playlist, onUpdate }) => {
  const navigate = useNavigate();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [playlistName, setPlaylistName] = useState(playlist.playlistName);
  const [playlistDescription, setPlaylistDescription] = useState(
    playlist.description
  );
  const [isEditing, setIsEditing] = useState(false);
  const isDefaultPlaylist = playlist.publicId === "0";
  const playlistIcon = isDefaultPlaylist ? heartSvg : playlistSvg;
  const iconClasses =
    "text-xl text-default-500 pointer-events-none flex-shrink-0";

  const {
    handleDeletePlaylist,
    handleTogglePrivacy,
    handleCopyLink,
    handleEditPlaylist,
  } = usePlaylistActions(playlist, onUpdate);

  const handleEdit = async () => {
    setIsEditing(true);
    await handleEditPlaylist(playlistName, playlistDescription);
    setIsEditing(false);
    onClose();
  };

  const handleCardClick = () => {
    if (!isDefaultPlaylist) {
      navigate(`/playlist/${playlist.publicId}`);
    } else {
      navigate("/playlist/liked");
    }
  };

  return (
    <div className="bg-gray-800 p-6 rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 flex items-center">
      <div onClick={handleCardClick} className="flex items-center flex-grow">
        <img
          src={playlistIcon}
          alt="Playlist Thumbnail"
          className="w-16 h-16 object-cover rounded-md"
        />
        <div className="flex-grow flex flex-col ml-4 overflow-hidden">
          <h3 className="text-lg font-bold text-white mb-1 truncate">
            {playlist.playlistName}
          </h3>
          <p className="text-sm text-gray-300 truncate mb-1">
            {playlist.description}
          </p>
          <p className="text-xs text-gray-400 truncate">
            Playlist {!isDefaultPlaylist ? `• ${playlist.ownerName}` : ""}
          </p>
        </div>
      </div>
      {!isDefaultPlaylist && (
        <>
          <Dropdown className="bg-gray-800 text-white">
            <DropdownTrigger>
              <Button
                isIconOnly
                className="bg-transparent"
                aria-label="More options"
              >
                <IoMdMore className="text-white text-2xl" />
              </Button>
            </DropdownTrigger>
            <DropdownMenu variant="faded" aria-label="Playlist options">
              <DropdownItem
                key="copy"
                startContent={<CopyIcon className={iconClasses} />}
                onClick={handleCopyLink}
              >
                Copy link
              </DropdownItem>
              <DropdownItem
                key="edit"
                startContent={<EditIcon className={iconClasses} />}
                onClick={onOpen}
              >
                Edit Playlist
              </DropdownItem>
              <DropdownItem
                key="privacy"
                startContent={
                  playlist.isPrivate ? (
                    <IoMdUnlock className={iconClasses} />
                  ) : (
                    <IoMdLock className={iconClasses} />
                  )
                }
                onClick={handleTogglePrivacy}
              >
                {playlist.isPrivate ? "Make Public" : "Make Private"}
              </DropdownItem>
              <DropdownItem
                key="delete"
                className="text-danger"
                color="danger"
                startContent={
                  <DeleteIcon className={cn(iconClasses, "text-danger")} />
                }
                onClick={handleDeletePlaylist}
              >
                Delete Playlist
              </DropdownItem>
            </DropdownMenu>
          </Dropdown>

          <PlaylistModal
            isOpen={isOpen}
            onClose={onClose}
            isEditMode={true}
            modalTitle="Edit Playlist"
            buttonTitle="Update"
            playlistName={playlistName}
            setPlaylistName={setPlaylistName}
            playlistDescription={playlistDescription}
            setPlaylistDescription={setPlaylistDescription}
            handlePlaylistOperation={handleEdit}
            isEditing={isEditing}
          />
        </>
      )}
    </div>
  );
};

PlaylistCard.propTypes = {
  playlist: PropTypes.shape({
    publicId: PropTypes.string.isRequired,
    ownerName: PropTypes.string,
    playlistName: PropTypes.string.isRequired,
    description: PropTypes.string.isRequired,
    isPrivate: PropTypes.bool,
  }).isRequired,
  onUpdate: PropTypes.func.isRequired,
};

export default PlaylistCard;
