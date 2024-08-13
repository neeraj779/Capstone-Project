import { useCallback } from "react";
import toast from "react-hot-toast";
import useApiClient from "../hooks/useApiClient";

const usePlaylistActions = (playlist, onUpdate) => {
  const swarApiClient = useApiClient();

  const handleApiAction = useCallback(
    async (apiCall, successMessage, errorMessage) => {
      try {
        await apiCall();
        onUpdate();
        toast.success(successMessage);
      } catch (error) {
        console.error(errorMessage, error);
        toast.error(errorMessage);
      }
    },
    [onUpdate]
  );

  const handleDeletePlaylist = useCallback(() => {
    handleApiAction(
      () =>
        swarApiClient.delete(`Playlist/DeletePlaylist/${playlist.playlistId}`),
      `${playlist.playlistName} has been deleted.`,
      "Failed to delete playlist."
    );
  }, [handleApiAction, playlist, swarApiClient]);

  const handleTogglePrivacy = useCallback(() => {
    handleApiAction(
      () =>
        swarApiClient.put(
          `Playlist/UpdatePlaylistPrivacy/${playlist.playlistId}`,
          {
            isPrivate: !playlist.isPrivate,
          }
        ),
      `${playlist.playlistName} is now ${
        playlist.isPrivate ? "public" : "private"
      }.`,
      "Failed to update playlist privacy."
    );
  }, [handleApiAction, playlist, swarApiClient]);

  const handleCopyLink = useCallback(() => {
    const link = `${window.location.origin}/playlist/${playlist.playlistId}`;
    navigator.clipboard
      .writeText(link)
      .then(() => toast.success("Playlist link copied to clipboard!"))
      .catch(() => toast.error("Failed to copy playlist link."));
  }, [playlist.playlistId]);

  const handleEditPlaylist = useCallback(
    async (playlistName, playlistDescription) => {
      await handleApiAction(
        () =>
          swarApiClient.put(`Playlist/UpdatePlaylist/${playlist.playlistId}`, {
            playlistName,
            description: playlistDescription,
          }),
        "Playlist updated successfully.",
        "Failed to update playlist."
      );
    },
    [handleApiAction, playlist.playlistId, swarApiClient]
  );

  return {
    handleDeletePlaylist,
    handleTogglePrivacy,
    handleCopyLink,
    handleEditPlaylist,
  };
};

export default usePlaylistActions;
