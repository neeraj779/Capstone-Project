import { useState, useEffect } from "react";
import useApiClient from "../hooks/useApiClient";

const usePlaylistInfo = () => {
  const swarApiClient = useApiClient();

  const [state, setState] = useState({
    playlistInfo: {},
    loading: true,
    error: null,
  });

  useEffect(() => {
    const fetchPlaylistInfo = async () => {
      try {
        const { data } = await swarApiClient.get(
          "Playlist/GetAllPlaylistsByUserId"
        );

        const playlistInfo = data || {};
        setState({
          playlistInfo,
          loading: false,
          error: null,
        });
      } catch (ex) {
        setState({
          playlistInfo: {},
          error: {
            message: ex.response?.data?.message || ex.message,
            status: ex.response?.status || 500,
          },
          loading: false,
        });
      }
    };

    fetchPlaylistInfo();
  }, [swarApiClient]);

  return {
    playlistInfo: state.playlistInfo,
    loading: state.loading,
    error: state.error,
  };
};

export default usePlaylistInfo;
