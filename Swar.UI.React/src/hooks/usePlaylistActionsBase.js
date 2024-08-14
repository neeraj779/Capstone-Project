import { useCallback } from "react";
import toast from "react-hot-toast";
import useApiClient from "../hooks/useApiClient";

const usePlaylistActionsBase = () => {
  const swarApiClient = useApiClient();

  const handleApiAction = useCallback(
    async (apiCall, successMessage, errorMessage) => {
      try {
        await apiCall();
        toast.success(successMessage);
      } catch {
        toast.error(errorMessage);
      }
    },
    []
  );

  const handleAddToPlaylist = useCallback(
    async (playlistId, songId) => {
      await handleApiAction(
        () =>
          swarApiClient.post("PlaylistSongs/AddSongToPlaylist", {
            playlistId,
            songId,
          }),
        "Song added to playlist successfully.",
        "Failed to add song to playlist."
      );
    },
    [handleApiAction, swarApiClient]
  );

  return {
    handleAddToPlaylist,
  };
};

export default usePlaylistActionsBase;
